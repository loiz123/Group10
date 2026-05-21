using System;

namespace Library_Management.Models
{
    /// <summary>
    /// Đại diện thủ thư. Kế thừa từ Person (Inheritance).
    /// </summary>
    public class Librarian : Person
    {
        private string _staffCode;
        private string _department;
        private DateTime _hireDate;

        public Librarian(string id, string name, string phone, string email,
                         string address, string staffCode, string department, DateTime hireDate)
            : base(id, name, phone, email, address)
        {
            _staffCode = staffCode;
            _department = department;
            _hireDate = hireDate;
        }

        public string StaffCode
        {
            get { return _staffCode; }
            set { _staffCode = value; }
        }

        public string Department
        {
            get { return _department; }
            set { _department = value; }
        }

        public DateTime HireDate
        {
            get { return _hireDate; }
            set { _hireDate = value; }
        }

        /// <summary>Duyệt yêu cầu mượn sách — trả về true nếu hợp lệ.</summary>
        public bool ApproveBorrow(Reader reader)
        {
            if (!reader.CanBorrow())
            {
                Console.WriteLine($"Từ chối: {reader.Name} đã đạt giới hạn mượn.");
                return false;
            }
            Console.WriteLine($"Thủ thư {Name} đã duyệt mượn sách cho {reader.Name}.");
            return true;
        }

        /// <summary>Duyệt yêu cầu trả sách.</summary>
        public void ApproveReturn(Reader reader)
        {
            Console.WriteLine($"Thủ thư {Name} đã xác nhận trả sách từ {reader.Name}.");
        }

        // Polymorphism: override khác nội dung so với Reader
        public override string GetInfo()
        {
            return $"[Thủ thư] ID: {Id} | Tên: {Name} | Mã NV: {_staffCode} " +
                   $"| Phòng ban: {_department} | Ngày vào làm: {_hireDate:dd/MM/yyyy}";
        }

        public override string GetRole()
        {
            return "Librarian";
        }
    }
}