using System;

namespace Library_Management.Models
{
    /// <summary>
    /// Phiếu mượn sách - liên kết Reader, Book, Librarian
    /// </summary>
    public class BorrowRecord
    {
       // ===== PRIVATE FIELDS =====
private string _recordId;

private Reader _reader = null!;
private Book _book = null!;
private Librarian _librarian = null!;

private DateTime _borrowDate;
private DateTime _dueDate;
private DateTime? _returnDate;

private BorrowStatus _status;
        // ===== CONSTRUCTOR (giữ để không lỗi BorrowService) =====
        public BorrowRecord()
        {
            _recordId = Guid.NewGuid().ToString();
            _status = BorrowStatus.Borrowing;
        }

        // ===== PROPERTIES =====
        public string RecordId
        {
            get { return _recordId; }
            set { _recordId = value; }
        }

        public Reader Reader
        {
            get { return _reader; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Reader));
                _reader = value;
            }
        }

        public Book Book
        {
            get { return _book; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Book));
                _book = value;
            }
        }

        public Librarian Librarian
        {
            get { return _librarian; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Librarian));
                _librarian = value;
            }
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
            if (!IsOverdue()) return 0;
            return (DateTime.Now - _dueDate).Days;
        }

        public void CompleteReturn()
        {
            _returnDate = DateTime.Now;
            _status = BorrowStatus.Returned;
        }

        public string GetInfo()
        {
            return $"RecordId: {_recordId} | Book: {_book?.Title} | Reader: {_reader?.Name} | Status: {_status}";
        }
    }
}