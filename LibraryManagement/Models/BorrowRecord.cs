using System;

namespace Library_Management.Models
{
    /// <summary>
    /// Phiếu mượn sách - chỉ lưu ID của Reader, Book, Librarian
    /// để tránh trùng lặp dữ liệu khi serialize ra file JSON.
    /// </summary>
    public class BorrowRecord
    {
        // ===== PRIVATE FIELDS =====
        private string _recordId;
        private string _readerId;
        private string _bookId;
        private string _librarianId;

        
        private string _readerName;
        private string _bookTitle;

        private DateTime _borrowDate;
        private DateTime _dueDate;
        private DateTime? _returnDate;
        private BorrowStatus _status;

        // ===== CONSTRUCTOR =====
        public BorrowRecord()
        {
            _recordId = Guid.NewGuid().ToString();
            _readerId = "";
            _bookId = "";
            _librarianId = "";
            _readerName = "";
            _bookTitle = "";
            _status = BorrowStatus.Borrowing;
        }

        // ===== PROPERTIES =====
        public string RecordId
        {
            get { return _recordId; }
            set { _recordId = value; }
        }

        public string ReaderId
        {
            get { return _readerId; }
            set { _readerId = value; }
        }

        public string BookId
        {
            get { return _bookId; }
            set { _bookId = value; }
        }

        public string LibrarianId
        {
            get { return _librarianId; }
            set { _librarianId = value; }
        }

    
        public string ReaderName
        {
            get { return _readerName; }
            set { _readerName = value; }
        }

        public string BookTitle
        {
            get { return _bookTitle; }
            set { _bookTitle = value; }
        }

        public DateTime BorrowDate
        {
            get { return _borrowDate; }
            set { _borrowDate = value; }
        }

        public DateTime DueDate
        {
            get { return _dueDate; }
            set { _dueDate = value; }
        }

        public DateTime? ReturnDate
        {
            get { return _returnDate; }
            set { _returnDate = value; }
        }

        public BorrowStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        // ===== METHODS =====

        public bool IsOverdue()
        {
            return _status == BorrowStatus.Borrowing && DateTime.Now > _dueDate;
        }

        public int GetOverdueDays()
        {
            DateTime checkDate = _returnDate.HasValue ? _returnDate.Value : DateTime.Now;
            if (checkDate <= _dueDate) return 0;
            return (checkDate.Date - _dueDate.Date).Days;
        }

        public void CompleteReturn()
        {
            _returnDate = DateTime.Now;
            _status = BorrowStatus.Returned;
        }

        
        public string GetInfo()
        {
            return $"RecordId: {_recordId} | Sách: {_bookTitle} | Bạn đọc: {_readerName} " +
                   $"| Mượn: {_borrowDate:dd/MM/yyyy} | Hạn: {_dueDate:dd/MM/yyyy} | Trạng thái: {_status}";
        }
    }
}