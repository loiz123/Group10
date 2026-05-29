using System;

namespace Library_Management.Models
{
    /// <summary>
    /// Thể loại sách trong thư viện.
    /// Tách khỏi Book để quản lý danh mục chuẩn, tránh nhập sai tên thể loại.
    /// </summary>
    public class Category
    {
        private string _categoryId;
        private string _categoryName;
        private string _description;

        // Constructor rỗng cho JSON deserialization
        public Category() { }

        public Category(string categoryId, string categoryName, string description)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentException("Mã thể loại không được để trống.");
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Tên thể loại không được để trống.");

            _categoryId = categoryId;
            _categoryName = categoryName;
            _description = description ?? string.Empty;
        }

        public string CategoryId
        {
            get { return _categoryId; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã thể loại không được để trống.");
                _categoryId = value;
            }
        }

        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên thể loại không được để trống.");
                _categoryName = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value ?? string.Empty; }
        }

        public string GetInfo()
        {
            return $"[Thể loại] ID: {_categoryId} | Tên: {_categoryName} | Mô tả: {_description}";
        }
    }
}
