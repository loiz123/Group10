using System;
using System.Collections.Generic;
using Library_Management.Models;
using Library_Management.Storage;

namespace Library_Management.Services
{
    public class BorrowService : IManageable<BorrowRecord>
    {
        private List<BorrowRecord> _records;
        private List<Fine> _fines;
        private FileStorage<BorrowRecord> _storage;
        private FileStorage<Fine> _fineStorage;
        private ReaderService _readerService;
        private BookService _bookService;

        public BorrowService(ReaderService readerService, BookService bookService)
        {
            _readerService = readerService;
            _bookService = bookService;
            _storage = new FileStorage<BorrowRecord>("data/borrowrecords.json");
            _fineStorage = new FileStorage<Fine>("data/fines.json");
            _records = _storage.Load();
            _fines = _fineStorage.Load();
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
            {
                if (_records[i].RecordId == id)
                    return _records[i];
            }
            return null;
        }

        public List<BorrowRecord> GetAll()
        {
            return _records;
        }

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

            if (reader == null || book == null)
            {
                Console.WriteLine("Không tìm thấy bạn đọc hoặc sách.");
                return;
            }

            if (librarian == null)
            {
                Console.WriteLine("Không tìm thấy thủ thư xử lý.");
                return;
            }

            if (!book.IsAvailable())
            {
                Console.WriteLine("Sách hiện không còn để mượn.");
                return;
            }

            if (!librarian.ApproveBorrow(reader))
            {
                return;
            }

            // Tạo phiếu mượn chỉ với ID + tên để hiển thị
            BorrowRecord record = new BorrowRecord();
            record.ReaderId = reader.Id;
            record.BookId = book.BookId;
            record.LibrarianId = librarian.Id;
            record.ReaderName = reader.Name;
            record.BookTitle = book.Title;
            record.BorrowDate = DateTime.Now;
            record.DueDate = DateTime.Now.AddDays(7);

            book.Checkout();
            reader.IncreaseBorrowCount();

            _records.Add(record);
            _bookService.SaveData();
            _readerService.SaveData();
            _storage.Save(_records);

            Console.WriteLine("Mượn sách thành công.");
        }

        
        public void ReturnBook(string recordId)
        {
            BorrowRecord? record = FindById(recordId);
            if (record == null)
            {
                Console.WriteLine("Không tìm thấy phiếu mượn.");
                return;
            }

            if (record.Status != BorrowStatus.Borrowing)
            {
                Console.WriteLine("Phiếu mượn này đã được xử lý, không thể trả lại lần nữa.");
                return;
            }

            // Lookup qua Service để lấy đúng instance đang dùng
            Reader? reader = _readerService.FindById(record.ReaderId);
            Book? book = _bookService.FindById(record.BookId);

            if (reader == null || book == null)
            {
                Console.WriteLine("Không tìm thấy sách hoặc bạn đọc trong dữ liệu hiện tại.");
                return;
            }

            bool wasOverdue = record.IsOverdue();

            record.CompleteReturn();
            book.Return();
            reader.DecreaseBorrowCount();

            if (wasOverdue)
            {
                Fine fine = new Fine(record);
                fine.Calculate();
                _fines.Add(fine);
                _fineStorage.Save(_fines);
                Console.WriteLine("Sách trả quá hạn. Tiền phạt: " + fine.Amount + " VNĐ");
            }

            _bookService.SaveData();
            _readerService.SaveData();
            _storage.Save(_records);

            Console.WriteLine("Trả sách thành công.");
        }

        public List<BorrowRecord> GetOverdueRecords()
        {
            List<BorrowRecord> result = new List<BorrowRecord>();
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].IsOverdue())
                    result.Add(_records[i]);
            }
            return result;
        }

        public List<Fine> GetUnpaidFines()
        {
            List<Fine> result = new List<Fine>();
            for (int i = 0; i < _fines.Count; i++)
            {
                if (!_fines[i].IsPaid)
                    result.Add(_fines[i]);
            }
            return result;
        }

        public List<BorrowRecord> GetRecordsByReader(string readerId)
        {
            if (string.IsNullOrWhiteSpace(readerId))
                throw new ArgumentException("readerId không hợp lệ.");
            List<BorrowRecord> result = new List<BorrowRecord>();
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].ReaderId == readerId)
                    result.Add(_records[i]);
            }
            return result;
        }

        // Lấy phiếu đang mượn của một bạn đọc (dùng cho UX trả sách)
        public List<BorrowRecord> GetBorrowingRecordsByReader(string readerId)
        {
            List<BorrowRecord> result = new List<BorrowRecord>();
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].ReaderId == readerId &&
                    _records[i].Status == BorrowStatus.Borrowing)
                    result.Add(_records[i]);
            }
            return result;
        }

        public bool IsBookBorrowing(string bookId)
        {
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].BookId == bookId &&
                    _records[i].Status == BorrowStatus.Borrowing)
                    return true;
            }
            return false;
        }

        public bool IsReaderBorrowing(string readerId)
        {
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].ReaderId == readerId &&
                    _records[i].Status == BorrowStatus.Borrowing)
                    return true;
            }
            return false;
        }
    }
}