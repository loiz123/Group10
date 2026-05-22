using System;

namespace Library_Management.Models
{
    /// <summary>
    /// Abstract base class đại diện cho một người dùng hệ thống thư viện.
    /// Không thể tạo object trực tiếp từ class này (Abstraction).
    /// </summary>
    public abstract class Person
    {
        // Encapsulation: fields private, chỉ truy cập qua property
        private string _id;
        private string _name;
        private string _phone;
        private string _email;
        private string _address;

        // Constructor
        public Person(string id, string name, string phone, string email, string address)
        {
            _id = id;
            _name = name;
            _phone = phone;
            _email = email;
            _address = address;
        }
        protected Person()
        {
            _id = ""; _name = ""; _phone = ""; _email = ""; _address = "";
        }

        // Properties với validation cơ bản
        public string Id
        {
            get { return _id; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Id không được để trống.");
                _id = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên không được để trống.");
                _name = value;
            }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        // Abstract methods — buộc lớp con phải override (Polymorphism)
        /// <summary>Trả về thông tin chi tiết của người dùng.</summary>
        public abstract string GetInfo();

        /// <summary>Trả về vai trò trong hệ thống (Reader / Librarian).</summary>
        public abstract string GetRole();
    }
}