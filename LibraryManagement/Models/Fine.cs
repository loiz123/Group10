using System;
using Library_Management.Models;

namespace Library_Management.Models
{
    /// <summary>
    /// Phiếu phạt khi trả sách trễ
    /// </summary>
    public class Fine
    {
        private string _fineId;
        private BorrowRecord _borrowRecord;
        private double _amount;
        private bool _isPaid;

        public Fine(BorrowRecord borrowRecord)
        {
            _fineId = Guid.NewGuid().ToString();

            _borrowRecord = borrowRecord
                ?? throw new ArgumentNullException(nameof(borrowRecord));

            _amount = 0;
            _isPaid = false;
        }

        // ===== Properties =====
        public string FineId
        {
            get { return _fineId; }
            set { _fineId = value; }
        }

        public BorrowRecord BorrowRecord
        {
            get { return _borrowRecord; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(BorrowRecord));
                _borrowRecord = value;
            }
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
        // ===== Logic =====
        public void Calculate()
        {
            if (_borrowRecord == null)
                throw new InvalidOperationException("BorrowRecord không tồn tại.");

            int daysLate = _borrowRecord.GetOverdueDays();

            if (daysLate <= 0)
            {
                _amount = 0;
                return;
            }

            _amount = daysLate * 5000;

            // chống tiền âm (safe guard)
            if (_amount < 0)
            {
                _amount = 0;
            }
        }

        public void MarkAsPaid()
        {
            _isPaid = true;
        }

        public string GetInfo()
        {
            return $"FineId: {_fineId} | Amount: {_amount} | Paid: {_isPaid}";
        }
    }
}