using System;
using System.Text.Json;

namespace Library_Management.Models
{
    /// <summary>
    /// Chính sách mượn sách của thư viện.
    /// Tập trung các thông số nghiệp vụ vào một chỗ thay vì hardcode rải rác trong code.
    /// Ví dụ: số ngày mượn tối đa, tiền phạt mỗi ngày trễ.
    /// </summary>
    public class BorrowPolicy
    {
        private int _maxBorrowDays;
        private double _finePerDay;
        private int _defaultBooksPerStudent;
        private int _defaultBooksPerTeacher;

        // Singleton — toàn hệ thống dùng chung một chính sách
        private static BorrowPolicy _instance;

        public static BorrowPolicy Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BorrowPolicy();
                return _instance;
            }
        }

        // Constructor rỗng — dùng giá trị mặc định
        public BorrowPolicy()
        {
            _maxBorrowDays = 14;
            _finePerDay = 5000;
            _defaultBooksPerStudent = 3;
            _defaultBooksPerTeacher = 5;
        }

        /// <summary>Số ngày được mượn tối đa trước khi tính là quá hạn.</summary>
        public int MaxBorrowDays
        {
            get { return _maxBorrowDays; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Số ngày mượn tối đa phải lớn hơn 0.");
                _maxBorrowDays = value;
            }
        }

        /// <summary>Tiền phạt mỗi ngày trả trễ (VNĐ).</summary>
        public double FinePerDay
        {
            get { return _finePerDay; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Tiền phạt không được âm.");
                _finePerDay = value;
            }
        }

        /// <summary>Số sách tối đa sinh viên được mượn cùng lúc.</summary>
        public int MaxBooksPerStudent
        {
            get { return _defaultBooksPerStudent; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Số sách tối đa phải lớn hơn 0.");
                _defaultBooksPerStudent = value;
            }
        }

        /// <summary>Số sách tối đa giáo viên được mượn cùng lúc.</summary>
        public int MaxBooksPerTeacher
        {
            get { return _defaultBooksPerTeacher; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Số sách tối đa phải lớn hơn 0.");
                _defaultBooksPerTeacher = value;
            }
        }

        /// <summary>
        /// Tính tiền phạt dựa trên số ngày trễ theo chính sách hiện tại.
        /// </summary>
        public double CalculateFine(int overdueDays)
        {
            if (overdueDays <= 0) return 0;
            return overdueDays * _finePerDay;
        }

        public string GetInfo()
        {
            return $"[Chính sách] Số ngày mượn: {_maxBorrowDays} ngày | " +
                   $"Phạt: {_finePerDay:N0} VNĐ/ngày | " +
                   $"Sinh viên mặc định: {_defaultBooksPerStudent} quyển | " +
                   $"Giáo viên mặc định: {_defaultBooksPerTeacher} quyển";
        }
        private static readonly string _policyPath = "data/borrowpolicy.json";

        public void Save()
        {
            try
            {
                Directory.CreateDirectory("data");
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_policyPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cảnh báo] Không thể lưu chính sách: {ex.Message}");
            }
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(_policyPath)) return;
                string json = File.ReadAllText(_policyPath);
                BorrowPolicy? loaded = JsonSerializer.Deserialize<BorrowPolicy>(json);
                if (loaded == null) return;

                _instance = loaded; // thay thẳng Singleton bằng dữ liệu từ file
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cảnh báo] Không thể tải chính sách, dùng mặc định: {ex.Message}");
            }
        }
    }
}
