using System;
using System.Collections.Generic;
using Library_Management.Models;
using Library_Management.Services;

namespace Library_Management
{
    /// <summary>
    /// Controller điều hướng menu chính của hệ thống.
    /// Giữ toàn bộ logic điều hướng bên trong (Encapsulation).
    /// </summary>
    public class MenuController
    {
        private ReaderService _readerService;
        private BookService _bookService;
        private BorrowService _borrowService;

        // Thủ thư mặc định cho phiên làm việc (dùng cho BorrowService)
        private Librarian _currentLibrarian;

        public MenuController()
        {
            _readerService = new ReaderService();
            _bookService = new BookService();
            _borrowService = new BorrowService(_readerService, _bookService);
            _currentLibrarian = new Librarian("L001", "Admin", "0900000000",
                "admin@library.vn", "Thư viện", "NV001", "Quản lý", new DateTime(2020, 1, 1));
        }

        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                try
                {
                    ShowMainMenu();
                    Console.Write("Chọn chức năng (0-4): ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            HandleReaderMenu();
                            break;
                        case "2":
                            HandleBookMenu();
                            break;
                        case "3":
                            HandleBorrowMenu();
                            break;
                        case "4":
                            HandleReportMenu();
                            break;
                        case "0":
                            isRunning = false;
                            Console.WriteLine("Đang đóng chương trình. Tạm biệt!");
                            break;
                        default:
                            Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng nhập lại.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[LỖI HỆ THỐNG PHÁT HIỆN Ở MENU]: {ex.Message}");
                    Console.WriteLine("Chương trình đã ngăn chặn lỗi crash. Nhấn phím bất kỳ để quay lại menu...");
                    Console.ReadKey();
                }
            }
        }

        // =====================================================================
        // MAIN MENU
        // =====================================================================

        private void ShowMainMenu()
        {
            Console.WriteLine("\n===== HỆ THỐNG QUẢN LÝ THƯ VIỆN OOP =====");
            Console.WriteLine("1. Quản lý Bạn Đọc");
            Console.WriteLine("2. Quản lý Sách");
            Console.WriteLine("3. Quản lý Mượn / Trả (Nghiệp vụ)");
            Console.WriteLine("4. Báo cáo & Thống kê");
            Console.WriteLine("0. Thoát chương trình");
            Console.WriteLine("=========================================");
        }

        // =====================================================================
        // 1. QUẢN LÝ BẠN ĐỌC
        // =====================================================================

        private void HandleReaderMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- QUẢN LÝ BẠN ĐỌC ---");
                Console.WriteLine("1. Thêm bạn đọc mới");
                Console.WriteLine("2. Xem danh sách bạn đọc");
                Console.WriteLine("3. Tìm bạn đọc theo tên");
                Console.WriteLine("4. Cập nhật thông tin bạn đọc");
                Console.WriteLine("5. Xóa bạn đọc");
                Console.WriteLine("0. Quay lại");
                Console.Write("Lựa chọn của bạn: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddReader();
                        break;
                    case "2":
                        _readerService.PrintAll();
                        break;
                    case "3":
                        SearchReader();
                        break;
                    case "4":
                        UpdateReader();
                        break;
                    case "5":
                        DeleteReader();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }
        }

        private void AddReader()
        {
            Console.WriteLine("\n[THÊM BẠN ĐỌC MỚI]");
            Console.Write("Nhập ID: ");
            string id = Console.ReadLine();
            Console.Write("Nhập Tên: ");
            string name = Console.ReadLine();
            Console.Write("Nhập SDT: ");
            string phone = Console.ReadLine();
            Console.Write("Nhập Email (Enter để bỏ qua): ");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email)) email = "N/A";
            Console.Write("Nhập Địa chỉ (Enter để bỏ qua): ");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address)) address = "N/A";
            Console.Write("Loại bạn đọc (SinhVien/GiangVien): ");
            string type = Console.ReadLine();

            Reader newReader = new Reader(id, name, phone, email, address, type, 3);
            _readerService.Add(newReader);
        }

        private void SearchReader()
        {
            Console.Write("Nhập từ khóa tìm kiếm (tên): ");
            string keyword = Console.ReadLine();
            List<Reader> results = _readerService.SearchByName(keyword);
            if (results.Count == 0)
            {
                Console.WriteLine("Không tìm thấy bạn đọc nào.");
                return;
            }
            Console.WriteLine($"Tìm thấy {results.Count} kết quả:");
            for (int i = 0; i < results.Count; i++)
                Console.WriteLine($"{i + 1}. {results[i].GetInfo()}");
        }

        private void UpdateReader()
        {
            Console.Write("Nhập ID bạn đọc cần cập nhật: ");
            string id = Console.ReadLine();
            Reader? existing = _readerService.FindById(id);
            if (existing == null)
            {
                Console.WriteLine($"Không tìm thấy bạn đọc với ID '{id}'.");
                return;
            }
            Console.WriteLine($"Thông tin hiện tại: {existing.GetInfo()}");
            Console.Write("Tên mới (Enter để giữ nguyên): ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = existing.Name;
            Console.Write("SDT mới (Enter để giữ nguyên): ");
            string phone = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phone)) phone = existing.Phone;
            Console.Write("Loại bạn đọc mới (Enter để   giữ nguyên): ");
            string type = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(type)) type = existing.ReaderType;

            existing.Name = name;
            existing.Phone = phone;
            existing.ReaderType = type;

            _readerService.Update(existing);
        }

        private void DeleteReader()
        {
            Console.Write("Nhập ID bạn đọc cần xóa: ");
            string id = Console.ReadLine();
            if (_borrowService.IsReaderBorrowing(id))
            {
                Console.WriteLine("Không thể xóa bạn đọc vì đang có sách chưa trả.");
                return;
            }
            _readerService.Remove(id);
        }

        // =====================================================================
        // 2. QUẢN LÝ SÁCH
        // =====================================================================

        private void HandleBookMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- QUẢN LÝ SÁCH ---");
                Console.WriteLine("1. Xem danh sách toàn bộ sách");
                Console.WriteLine("2. Thêm sách mới");
                Console.WriteLine("3. Tìm sách theo tên");
                Console.WriteLine("4. Tìm sách theo tác giả");
                Console.WriteLine("5. Tìm sách theo thể loại");
                Console.WriteLine("6. Xem sách còn có thể mượn");
                Console.WriteLine("7. Xóa sách");
                Console.WriteLine("8. Cập nhật sách");
                Console.WriteLine("0. Quay lại");
                Console.Write("Lựa chọn của bạn: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PrintBooks(_bookService.GetAll());
                        break;
                    case "2":
                        AddBook();
                        break;
                    case "3":
                        SearchBookByTitle();
                        break;
                    case "4":
                        SearchBookByAuthor();
                        break;
                    case "5":
                        SearchBookByCategory();
                        break;
                    case "6":
                        PrintBooks(_bookService.GetAvailableBooks());
                        break;
                    case "7":
                        DeleteBook();
                        break;
                    case "8":
                        UpdateBook();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }
        }

        private void PrintBooks(List<Book> books)
        {
            if (books.Count == 0)
            {
                Console.WriteLine("Không có sách nào.");
                return;
            }
            Console.WriteLine($"Tổng: {books.Count} cuốn");
            for (int i = 0; i < books.Count; i++)
                Console.WriteLine($"{i + 1}. {books[i].GetInfo()}");
        }

        private void AddBook()
        {
            Console.WriteLine("\n[THÊM SÁCH MỚI]");
            Console.Write("Nhập ID sách: ");
            string id = Console.ReadLine();
            Console.Write("Nhập Tựa đề: ");
            string title = Console.ReadLine();
            Console.Write("Nhập Tác giả: ");
            string author = Console.ReadLine();
            Console.Write("Nhập Thể loại: ");
            string category = Console.ReadLine();
            Console.Write("Nhập Nhà xuất bản: ");
            string publisher = Console.ReadLine();
            Console.Write("Nhập Số lượng: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
            {
                Console.WriteLine("Số lượng không hợp lệ.");
                return;
            }

            Book newBook = new Book(id, title, author, category, publisher, qty);
            _bookService.Add(newBook);
        }

        private void SearchBookByTitle()
        {
            Console.Write("Nhập tên sách cần tìm: ");
            string keyword = Console.ReadLine();
            PrintBooks(_bookService.SearchByTitle(keyword));
        }

        private void SearchBookByAuthor()
        {
            Console.Write("Nhập tên tác giả cần tìm: ");
            string keyword = Console.ReadLine();
            PrintBooks(_bookService.SearchByAuthor(keyword));
        }

        private void SearchBookByCategory()
        {
            Console.Write("Nhập thể loại cần tìm: ");
            string keyword = Console.ReadLine();
            PrintBooks(_bookService.SearchByCategory(keyword));
        }

        private void DeleteBook()
        {
            Console.Write("Nhập ID sách cần xóa: ");
            string id = Console.ReadLine();
            if (_borrowService.IsBookBorrowing(id))
            {
                Console.WriteLine("Không thể xóa sách vì đang có phiếu mượn.");
                return;
            }
            _bookService.Remove(id);
        }
        private void UpdateBook()
        {
            Console.Write("Nhập ID sách cần cập nhật: ");
            string id = Console.ReadLine();

            Book? existing = _bookService.FindById(id);

            if (existing == null)
            {
                Console.WriteLine("Không tìm thấy sách với ID '" + id + "'.");
                return;
            }

            Console.WriteLine("Thông tin hiện tại: " + existing.GetInfo());

            Console.Write("Tựa đề mới (Enter để giữ nguyên): ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                title = existing.Title;
            }

            Console.Write("Tác giả mới (Enter để giữ nguyên): ");
            string author = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(author))
            {
                author = existing.Author;
            }

            Console.Write("Thể loại mới (Enter để giữ nguyên): ");
            string category = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(category))
            {
                category = existing.Category;
            }

            Console.Write("Nhà xuất bản mới (Enter để giữ nguyên): ");
            string publisher = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(publisher))
            {
                publisher = existing.Publisher;
            }

            Console.Write("Tổng số lượng mới (Enter để giữ nguyên): ");
            string totalInput = Console.ReadLine();

            int newTotalQuantity = existing.TotalQuantity;
            int borrowedCount = existing.TotalQuantity - existing.AvailableQuantity;

            if (!string.IsNullOrWhiteSpace(totalInput))
            {
                if (!int.TryParse(totalInput, out newTotalQuantity) || newTotalQuantity <= 0)
                {
                    Console.WriteLine("Tổng số lượng không hợp lệ.");
                    return;
                }

                if (newTotalQuantity < borrowedCount)
                {
                    Console.WriteLine("Không thể cập nhật tổng số lượng nhỏ hơn số sách đang được mượn.");
                    return;
                }
            }

            existing.Title = title;
            existing.Author = author;
            existing.Category = category;
            existing.Publisher = publisher;
            existing.TotalQuantity = newTotalQuantity;
            existing.AvailableQuantity = newTotalQuantity - borrowedCount;

            _bookService.Update(existing);
        }

        // =====================================================================
        // 3. QUẢN LÝ MƯỢN / TRẢ
        // =====================================================================

        private void HandleBorrowMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- QUẢN LÝ MƯỢN TRẢ ---");
                Console.WriteLine("1. Mượn sách");
                Console.WriteLine("2. Trả sách");
                Console.WriteLine("3. Xem tất cả phiếu mượn");
                Console.WriteLine("4. Xem phiếu mượn theo bạn đọc");
                Console.WriteLine("5. Xem sách quá hạn");
                Console.WriteLine("6. Xem phiếu phạt chưa thanh toán");
                Console.WriteLine("0. Quay lại");
                Console.Write("Lựa chọn của bạn: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        BorrowBook();
                        break;
                    case "2":
                        ReturnBook();
                        break;
                    case "3":
                        PrintBorrowRecords(_borrowService.GetAll());
                        break;
                    case "4":
                        ViewRecordsByReader();
                        break;
                    case "5":
                        PrintBorrowRecords(_borrowService.GetOverdueRecords());
                        break;
                    case "6":
                        PrintFines(_borrowService.GetUnpaidFines());
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }
        }

        private void BorrowBook()
        {
            Console.WriteLine("\n[MƯỢN SÁCH]");
            Console.Write("Nhập ID bạn đọc: ");
            string readerId = Console.ReadLine();
            Console.Write("Nhập ID sách: ");
            string bookId = Console.ReadLine();
            _borrowService.BorrowBook(readerId, bookId, _currentLibrarian);
        }

        private void ReturnBook()
        {
            Console.WriteLine("\n[TRẢ SÁCH]");
            Console.Write("Nhập ID bạn đọc: ");
            string readerId = Console.ReadLine();

            List<BorrowRecord> borrowing = _borrowService.GetBorrowingRecordsByReader(readerId);

            if (borrowing.Count == 0)
            {
                Console.WriteLine("Bạn đọc này không có sách nào đang mượn.");
                return;
            }

            Console.WriteLine("Danh sách sách đang mượn:");
            for (int i = 0; i < borrowing.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {borrowing[i].GetInfo()}");
            }

            Console.Write("Chọn số thứ tự sách cần trả: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > borrowing.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }

            string recordId = borrowing[choice - 1].RecordId;
            _borrowService.ReturnBook(recordId);
        }


        private void ViewRecordsByReader()
        {
            Console.Write("Nhập ID bạn đọc: ");
            string readerId = Console.ReadLine();
            List<BorrowRecord> records = _borrowService.GetRecordsByReader(readerId);
            PrintBorrowRecords(records);
        }

        private void PrintBorrowRecords(List<BorrowRecord> records)
        {
            if (records.Count == 0)
            {
                Console.WriteLine("Không có phiếu mượn nào.");
                return;
            }
            Console.WriteLine($"Tổng: {records.Count} phiếu");
            for (int i = 0; i < records.Count; i++)
                Console.WriteLine($"{i + 1}. {records[i].GetInfo()}");
        }

        private void PrintFines(List<Fine> fines)
        {
            if (fines.Count == 0)
            {
                Console.WriteLine("Không có phiếu phạt chưa thanh toán.");
                return;
            }
            Console.WriteLine($"Tổng: {fines.Count} phiếu phạt");
            for (int i = 0; i < fines.Count; i++)
                Console.WriteLine($"{i + 1}. {fines[i].GetInfo()}");
        }

        // =====================================================================
        // 4. BÁO CÁO & THỐNG KÊ
        // =====================================================================

        private void HandleReportMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- BÁO CÁO THỐNG KÊ ---");
                Console.WriteLine("1. Danh sách bạn đọc đang mượn sách");
                Console.WriteLine("2. Tổng số sách trong thư viện");
                Console.WriteLine("3. Tổng số phiếu mượn");
                Console.WriteLine("4. Số phiếu mượn quá hạn");
                Console.WriteLine("5. Số phiếu phạt chưa thanh toán");
                Console.WriteLine("0. Quay lại");
                Console.Write("Lựa chọn của bạn: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        List<Reader> borrowing = _readerService.GetBorrowingReaders();
                        if (borrowing.Count == 0)
                            Console.WriteLine("Không có ai đang mượn sách.");
                        else
                            for (int i = 0; i < borrowing.Count; i++)
                                Console.WriteLine($"{i + 1}. {borrowing[i].GetInfo()}");
                        break;
                    case "2":
                        List<Book> allBooks = _bookService.GetAll();
                        Console.WriteLine($"Tổng số đầu sách: {allBooks.Count}");
                        int totalCopies = 0;
                        int availCopies = 0;
                        for (int i = 0; i < allBooks.Count; i++)
                        {
                            totalCopies += allBooks[i].TotalQuantity;
                            availCopies += allBooks[i].AvailableQuantity;
                        }
                        Console.WriteLine($"Tổng số bản: {totalCopies} | Còn có thể mượn: {availCopies} | Đang được mượn: {totalCopies - availCopies}");
                        break;
                    case "3":
                        Console.WriteLine($"Tổng số phiếu mượn: {_borrowService.GetAll().Count}");
                        break;
                    case "4":
                        Console.WriteLine($"Số phiếu quá hạn: {_borrowService.GetOverdueRecords().Count}");
                        break;
                    case "5":
                        Console.WriteLine($"Số phiếu phạt chưa thanh toán: {_borrowService.GetUnpaidFines().Count}");
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }
        }
    }
}