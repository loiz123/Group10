using System;

namespace Library_Management.Models
{
    /// <summary>
    /// Phiếu phạt khi trả sách trễ.
    /// Chỉ lưu RecordId thay vì toàn bộ BorrowRecord để tránh trùng lặp dữ liệu.
    /// </summary>
    public class Fine
    {
        private string _fineId;
        private string _recordId;       
        private string _readerName;     
        private string _bookTitle;      
        private int _overdueDays;       
        private double _amount;
        private bool _isPaid;

        // Constructor rỗng cho JSON deserialization
        public Fine()
        {
            _fineId = Guid.NewGuid().ToString();
            _recordId = "";
            _readerName = "";
            _bookTitle = "";
            _overdueDays = 0;
            _amount = 0;
            _isPaid = false;
        }

        // Constructor dùng khi tạo phiếu phạt mới
        public Fine(BorrowRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            _fineId = Guid.NewGuid().ToString();
            _recordId = record.RecordId;
            _readerName = record.ReaderName;
            _bookTitle = record.BookTitle;
            _overdueDays = record.GetOverdueDays();
            _amount = 0;
            _isPaid = false;
        }

        // ===== PROPERTIES =====
        public string FineId
        {
            get { return _fineId; }
            set { _fineId = value; }
        }

        public string RecordId
        {
            get { return _recordId; }
            set { _recordId = value; }
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

        public int OverdueDays
        {
            get { return _overdueDays; }
            set { _overdueDays = value; }
        }

        public double Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public bool IsPaid
        {
            get { return _isPaid; }
            set { _isPaid = value; }
        }

        // ===== METHODS =====

        public void Calculate()
        {
            if (_overdueDays <= 0)
            {
                _amount = 0;
                return;
            }
            _amount = _overdueDays * 5000;
        }

        public void MarkAsPaid()
        {
            _isPaid = true;
        }

        
        public string GetInfo()
        {
            return $"FineId: {_fineId} | Sách: {_bookTitle} | Bạn đọc: {_readerName} " +
                   $"| Số ngày trễ: {_overdueDays} | Tiền phạt: {_amount} VNĐ | Đã thanh toán: {_isPaid}";
        }
    }
}