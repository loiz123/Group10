using System;
using System.Collections.Generic;
using Library_Management.Models;
using Library_Management.Storage;

namespace Library_Management.Services
{
    public class BorrowService : IManageable<BorrowRecord>
    {
        private List<BorrowRecord> _records = new List<BorrowRecord>();
        private List<Fine> _fines = new List<Fine>();
        private FileStorage<BorrowRecord> _storage;
        private ReaderService _readerService;
        private BookService _bookService;

        public BorrowService(ReaderService readerService, BookService bookService)
        {
            _readerService = readerService;
            _bookService = bookService;
            _storage = new FileStorage<BorrowRecord>("data/borrowrecords.json");
            _records = _storage.Load();
        }

        public void Add(BorrowRecord item)
        {
            _records.Add(item);
            _storage.Save(_records);
        }

        public void Remove(string id)
        {
            BorrowRecord? record = FindById(id);
            if (record == null) return;
            _records.Remove(record);
            _storage.Save(_records);
        }

        public BorrowRecord? FindById(string id)
        {
            for (int i = 0; i < _records.Count; i++)
                if (_records[i].RecordId == id) return _records[i];
            return null;
        }

        public List<BorrowRecord> GetAll() => _records;

        public void Update(BorrowRecord item)
        {
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].RecordId == item.RecordId)
                {
                    _records[i] = item;
                    _storage.Save(_records);
                    return;
                }
            }
        }

        public void BorrowBook(string readerId, string bookId, Librarian librarian)
        {
            Reader? reader = _readerService.FindById(readerId);
            Book? book = _bookService.FindById(bookId);

            if (reader == null || book == null) { Console.WriteLine("Không tìm thấy Reader hoặc Book"); return; }
            if (!reader.CanBorrow()) { Console.WriteLine("Reader đã đạt giới hạn mượn"); return; }
            if (!book.IsAvailable()) { Console.WriteLine("Sách đã hết"); return; }

            BorrowRecord record = new BorrowRecord
            {
                Reader = reader,
                Book = book,
                Librarian = librarian,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7)
            };

            book.Checkout();
            reader.IncreaseBorrowCount();
            _records.Add(record);
            _storage.Save(_records);
            Console.WriteLine("Mượn sách thành công");
        }

        public void ReturnBook(string recordId)
        {
            BorrowRecord? record = FindById(recordId);
            if (record == null) { Console.WriteLine("Không tìm thấy record"); return; }

            bool wasOverdue = record.IsOverdue();
            record.CompleteReturn();
            record.Book.Return();
            record.Reader.DecreaseBorrowCount();

            if (wasOverdue)
            {
                Fine fine = new Fine(record);
                fine.Calculate();
                _fines.Add(fine);
                record.Status = BorrowStatus.Overdue;
            }

            _storage.Save(_records);
            Console.WriteLine("Trả sách thành công");
        }

        public List<BorrowRecord> GetOverdueRecords()
        {
            List<BorrowRecord> result = new List<BorrowRecord>();
            for (int i = 0; i < _records.Count; i++)
                if (_records[i].IsOverdue()) result.Add(_records[i]);
            return result;
        }

        public List<Fine> GetUnpaidFines()
        {
            List<Fine> result = new List<Fine>();
            for (int i = 0; i < _fines.Count; i++)
                if (!_fines[i].IsPaid) result.Add(_fines[i]);
            return result;
        }

        public List<BorrowRecord> GetRecordsByReader(string readerId)
        {
            if (string.IsNullOrWhiteSpace(readerId))
                throw new ArgumentException("readerId không hợp lệ");
            List<BorrowRecord> result = new List<BorrowRecord>();
            for (int i = 0; i < _records.Count; i++)
                if (_records[i].Reader.Id == readerId) result.Add(_records[i]);
            return result;
        }
    }
}