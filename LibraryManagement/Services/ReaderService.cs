using System;
using System.Collections.Generic;
using Library_Management.Models;
using Library_Management.Storage;

namespace Library_Management.Services
{
    /// <summary>
    /// Quản lý danh sách bạn đọc trong bộ nhớ.
    /// Nam Anh đã tích hợp FileStorage vào.
    /// </summary>
    public class ReaderService: IManageable<Reader>
    {
        private List<Reader> _readers;
        private FileStorage<Reader> _storage;

        public ReaderService()
        {
            _storage = new FileStorage<Reader>("data/readers.json");
            _readers = _storage.Load();
        }

        /// <summary>Thêm bạn đọc mới. Báo lỗi nếu ID đã tồn tại.</summary>
        public void Add(Reader reader)
        {
            if (FindById(reader.Id) != null)
            {
                Console.WriteLine($"Lỗi: ID '{reader.Id}' đã tồn tại.");
                return;
            }
            _readers.Add(reader);
            _storage.Save(_readers); // Nam Anh: Lưu file sau khi thêm
            Console.WriteLine($"Đã thêm bạn đọc: {reader.Name}");
        }

        /// <summary>Xóa bạn đọc theo ID.</summary>
        public void Remove(string id)
        {
            Reader? target = FindById(id);
            if (target == null)
            {
                Console.WriteLine($"Không tìm thấy bạn đọc với ID '{id}'.");
                return;
            }
            _readers.Remove(target);
            _storage.Save(_readers); // Nam Anh: Lưu file sau khi xóa
            Console.WriteLine($"Đã xóa bạn đọc: {target.Name}");
        }

        /// <summary>Tìm bạn đọc theo ID. Trả về null nếu không có.</summary>
        public Reader? FindById(string id)
        {
            for (int i = 0; i < _readers.Count; i++)
            {
                if (_readers[i].Id == id)
                    return _readers[i];
            }
            return null;
        }

        /// <summary>Trả về toàn bộ danh sách bạn đọc.</summary>
        public List<Reader> GetAll()
        {
            return _readers;
        }

        /// <summary>Cập nhật thông tin bạn đọc (tìm theo ID rồi thay thế).</summary>
        public void Update(Reader updated)
        {
            for (int i = 0; i < _readers.Count; i++)
            {
                if (_readers[i].Id == updated.Id)
                {
                    _readers[i] = updated;
                    _storage.Save(_readers); // Nam Anh: Lưu file sau khi cập nhật
                    Console.WriteLine($"Đã cập nhật bạn đọc: {updated.Name}");
                    return;
                }
            }
            Console.WriteLine($"Không tìm thấy bạn đọc với ID '{updated.Id}'.");
        }

        /// <summary>Tìm bạn đọc theo tên (không phân biệt hoa thường).</summary>
        public List<Reader> SearchByName(string keyword)
        {
            List<Reader> result = new List<Reader>();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return result;
            }

            for (int i = 0; i < _readers.Count; i++)
            {
                if (_readers[i].Name.ToLower().Contains(keyword.ToLower()))
                {
                    result.Add(_readers[i]);
                }
            }

            return result;
        }

        /// <summary>Trả về danh sách bạn đọc đang có sách mượn.</summary>
        public List<Reader> GetBorrowingReaders()
        {
            List<Reader> result = new List<Reader>();
            for (int i = 0; i < _readers.Count; i++)
            {
                if (_readers[i].BorrowedCount > 0)
                    result.Add(_readers[i]);
            }
            return result;
        }

        /// <summary>In toàn bộ danh sách bạn đọc ra console.</summary>
        public void PrintAll()
        {
            if (_readers.Count == 0)
            {
                Console.WriteLine("Danh sách bạn đọc trống.");
                return;
            }
            Console.WriteLine("===== DANH SÁCH BẠN ĐỌC =====");
            for (int i = 0; i < _readers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_readers[i].GetInfo()}");
            }
        }
        public void SaveData()
        {
            _storage.Save(_readers);
        }
    }
}