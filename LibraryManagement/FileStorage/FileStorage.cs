using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Library_Management.Storage
{
    /// <summary>
    /// Generic class quản lý việc lưu và đọc dữ liệu ra file JSON.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của model (Reader, Book, BorrowRecord...)</typeparam>
    public class FileStorage<T>
    {
        private string _filePath;

        public FileStorage(string filePath)
        {
            _filePath = filePath;
            // Đảm bảo thư mục "data/" tồn tại trước khi thao tác file
            string directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>Kiểm tra file có tồn tại không.</summary>
        public bool FileExists()
        {
            return File.Exists(_filePath);
        }

        /// <summary>Tạo bản sao lưu (backup) trước khi ghi đè file mới.</summary>
        public void BackupFile()
        {
            if (FileExists())
            {
                try
                {
                    string backupPath = _filePath + ".bak";
                    File.Copy(_filePath, backupPath, true);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"[Cảnh báo] Lỗi backup file: {ex.Message}");
                }
            }
        }

        /// <summary>Lưu danh sách đối tượng ra file JSON.</summary>
        public void Save(List<T> data)
        {
            try
            {
                BackupFile();
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true; // Format file JSON dễ đọc

                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(_filePath, jsonString);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[Lỗi Hệ Thống] Không thể ghi file {_filePath}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi Hệ Thống] Lỗi không xác định khi lưu file: {ex.Message}");
            }
        }

        
        
        /// <summary>Đọc dữ liệu từ file JSON và trả về danh sách đối tượng.</summary>
        public List<T> Load()
        {
            if (!FileExists())
            {
                return new List<T>();
            }

            try
            {
                string jsonString = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return new List<T>();
                }

                List<T> data = JsonSerializer.Deserialize<List<T>>(jsonString);
                if (data != null)
                {
                    return data;
                }
                return new List<T>();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[Lỗi Hệ Thống] Không thể đọc file {_filePath}: {ex.Message}");
                return new List<T>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[Lỗi Dữ Liệu] Sai định dạng JSON ở file {_filePath}: {ex.Message}");
                return new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi Hệ Thống] Lỗi không xác định khi tải file: {ex.Message}");
                return new List<T>();
            }
        }
    }
}