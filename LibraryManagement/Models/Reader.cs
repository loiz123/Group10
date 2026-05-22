using System.Text.Json.Serialization;
using System;

namespace Library_Management.Models
{
    /// <summary>
    /// Đại diện bạn đọc thư viện. Kế thừa từ Person (Inheritance).
    /// </summary>
    public class Reader : Person
    {
        private int _maxBorrow;       // Số sách tối đa được mượn cùng lúc
        private int _borrowedCount;   // Số sách đang mượn hiện tại
        private string _readerType;   // Ví dụ: "Student", "Teacher"

        public Reader(string id, string name, string phone, string email,
                      string address, string readerType, int maxBorrow = 3)
            : base(id, name, phone, email, address)
        {
            _readerType = readerType;
            _maxBorrow = maxBorrow;
            _borrowedCount = 0;
        }
        public Reader() : base()
        {
            _readerType = "";
            _maxBorrow = 3;
            _borrowedCount = 0;
        }
        public int MaxBorrow
        {
            get { return _maxBorrow; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Giới hạn mượn phải lớn hơn 0.");
                _maxBorrow = value;
            }
        }

        [JsonInclude]
        public int BorrowedCount
        {
            get { return _borrowedCount; }
            private set { _borrowedCount = value; }
        }

        public string ReaderType
        {
            get { return _readerType; }
            set { _readerType = value; }
        }

        /// <summary>Kiểm tra xem bạn đọc có thể mượn thêm sách không.</summary>
        public bool CanBorrow()
        {
            return _borrowedCount < _maxBorrow;
        }

        /// <summary>Tăng số sách đang mượn lên 1 khi mượn sách.</summary>
        public void IncreaseBorrowCount()
        {
            if (_borrowedCount >= _maxBorrow)
                throw new InvalidOperationException("Đã đạt giới hạn mượn sách.");
            _borrowedCount++;
        }

        /// <summary>Giảm số sách đang mượn xuống 1 khi trả sách.</summary>
        public void DecreaseBorrowCount()
        {
            if (_borrowedCount <= 0)
                throw new InvalidOperationException("Số sách đang mượn đã là 0.");
            _borrowedCount--;
        }

        // Polymorphism: override khác nội dung so với Librarian
        public override string GetInfo()
        {
            return $"[Bạn đọc] ID: {Id} | Tên: {Name} | Loại: {_readerType} " +
                   $"| Đang mượn: {_borrowedCount}/{_maxBorrow}";
        }

        public override string GetRole()
        {
            return "Reader";
        }
        
        
    }
}