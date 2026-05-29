using System;

namespace Library_Management.Models
{
    /// <summary>
    /// Thông báo gửi đến bạn đọc về các sự kiện liên quan đến tài khoản mượn sách.
    /// Ví dụ: sách sắp đến hạn trả, có phiếu phạt chưa thanh toán.
    /// </summary>
    public class Notification
    {
        private string _notificationId;
        private string _readerId;
        private string _message;
        private DateTime _createdDate;
        private bool _isRead;

        // Constructor rỗng cho JSON deserialization
        public Notification() { }

        public Notification(string notificationId, string readerId, string message)
        {
            if (string.IsNullOrWhiteSpace(notificationId))
                throw new ArgumentException("Mã thông báo không được để trống.");
            if (string.IsNullOrWhiteSpace(readerId))
                throw new ArgumentException("Mã bạn đọc không được để trống.");
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Nội dung thông báo không được để trống.");

            _notificationId = notificationId;
            _readerId = readerId;
            _message = message;
            _createdDate = DateTime.Now;
            _isRead = false;
        }

        public string NotificationId
        {
            get { return _notificationId; }
            set { _notificationId = value; }
        }

        public string ReaderId
        {
            get { return _readerId; }
            set { _readerId = value; }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Nội dung thông báo không được để trống.");
                _message = value;
            }
        }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

        public bool IsRead
        {
            get { return _isRead; }
            set { _isRead = value; }
        }

        /// <summary>
        /// Đánh dấu thông báo đã được đọc.
        /// </summary>
        public void MarkAsRead()
        {
            _isRead = true;
        }

        public string GetInfo()
        {
            string trangThai = _isRead ? "Đã đọc" : "Chưa đọc";
            return $"[Thông báo] ID: {_notificationId} | Bạn đọc: {_readerId} " +
                   $"| {_createdDate:dd/MM/yyyy HH:mm} | {trangThai}\n  >> {_message}";
        }
    }
}
