using System.Collections.Generic;

namespace Library_Management.Services
{
    /// <summary>
    /// Contract dùng chung cho các Service quản lý đối tượng.
    /// Che giấu chi tiết cài đặt, chỉ lộ ra các thao tác cơ bản (Abstraction).
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của đối tượng cần quản lý (Book, BorrowRecord,...)</typeparam>
    public interface IManageable<T>
    {
        /// <summary>Thêm một đối tượng mới vào danh sách.</summary>
        void Add(T item);

        /// <summary>Xóa đối tượng khỏi danh sách dựa vào ID.</summary>
        void Remove(string id);

        /// <summary>
        /// Tìm kiếm đối tượng dựa vào ID.
        /// Trả về null nếu không tìm thấy.
        /// </summary>
        T? FindById(string id);

        /// <summary>Lấy toàn bộ danh sách đối tượng.</summary>
        List<T> GetAll();

        /// <summary>Cập nhật thông tin đối tượng đã tồn tại.</summary>
        void Update(T item);
    }
}