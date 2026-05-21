using System;
using System.Collections.Generic;
using Library_Management.Models;
using Library_Management.Storage;

namespace Library_Management.Services
{
    /// <summary>
    /// Service quản lý sách, implement IManageable<Book> (Polymorphism).
    /// Nam Anh sẽ tích hợp FileStorage<Book> vào sau.
    /// </summary>
    public class BookService : IManageable<Book>
    {
        private List<Book> _books;
       private FileStorage<Book> _storage;
        public BookService()
        {


            _storage = new FileStorage<Book>("data/books.json");
            _books = _storage.Load();
        }

        public void Add(Book item)
        {
            if (FindById(item.BookId) != null)
            {
                Console.WriteLine($"Lỗi: Sách với ID '{item.BookId}' đã tồn tại.");
                return;
            }
            _books.Add(item);
            _storage.Save(_books);
            Console.WriteLine($"Đã thêm sách: {item.Title}");
        }

        public void Remove(string id)
        {
            Book? target = FindById(id);
            if (target == null)
            {
                Console.WriteLine($"Không tìm thấy sách với ID '{id}'.");
                return;
            }
            _books.Remove(target);
            _storage.Save(_books);
            Console.WriteLine($"Đã xóa sách: {target.Title}");
        }

        public Book? FindById(string id)
        {
            for (int i = 0; i < _books.Count; i++)
            {
                if (_books[i].BookId == id)
                    return _books[i];
            }
            return null;
        }

        public List<Book> GetAll()
        {
            return _books;
        }

        public void Update(Book updated)
        {
            for (int i = 0; i < _books.Count; i++)
            {
                if (_books[i].BookId == updated.BookId)
                {
                    _books[i] = updated;
                    Console.WriteLine($"Đã cập nhật sách: {updated.Title}");
                    _storage.Save(_books);
                    return;
                }
            }
            Console.WriteLine($"Không tìm thấy sách với ID '{updated.BookId}'.");
        }

        public List<Book> SearchByTitle(string keyword)
        {
            List<Book> result = new List<Book>();
            for (int i = 0; i < _books.Count; i++)
            {
                if (_books[i].Title.ToLower().Contains(keyword.ToLower()))
                    result.Add(_books[i]);
            }
            return result;
        }

        public List<Book> SearchByAuthor(string keyword)
        {
            List<Book> result = new List<Book>();
            for (int i = 0; i < _books.Count; i++)
            {
                if (_books[i].Author.ToLower().Contains(keyword.ToLower()))
                    result.Add(_books[i]);
            }
            return result;
        }

        public List<Book> SearchByCategory(string keyword)
        {
            List<Book> result = new List<Book>();
            for (int i = 0; i < _books.Count; i++)
            {
                if (_books[i].Category.ToLower().Contains(keyword.ToLower()))
                    result.Add(_books[i]);
            }
            return result;
        }

        public List<Book> GetAvailableBooks()
        {
            List<Book> result = new List<Book>();
            for (int i = 0; i < _books.Count; i++)
            {
                if (_books[i].IsAvailable())
                    result.Add(_books[i]);
            }
            return result;
        }
    }
}
