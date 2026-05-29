using System;
using System.Collections.Generic;
using Library_Management.Models;
using Library_Management.Services;
using Library_Management.Storage;

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

        // Danh sách thể loại và thông báo quản lý nội bộ
        private List<Category> _categories;
        private FileStorage<Category> _categoryStorage;
        private List<Notification> _notifications;
        private FileStorage<Notification> _notificationStorage;

        // Thủ thư mặc định cho phiên làm việc
        private Librarian _currentLibrarian;

        public MenuController()
        {
            _readerService = new ReaderService();
            _bookService = new BookService();
            _borrowService = new BorrowService(_readerService, _bookService);
            _currentLibrarian = new Librarian("L001", "Admin", "0900000000",
                "admin@library.vn", "Thư viện", "NV001", "Quản lý", new DateTime(2020, 1, 1));

            // Khởi tạo danh sách thể loại từ file, seed mặc định nếu chưa có
            _categoryStorage = new FileStorage<Category>("data/categories.json");
            _categories = _categoryStorage.Load();
            if (_categories.Count == 0)
            {
                _categories = new List<Category>
                {
                    new Category("TL001", "Công nghệ", "Sách lập trình, CNTT, kỹ thuật số"),
                    new Category("TL002", "Văn học", "Tiểu thuyết, truyện ngắn, thơ"),
                    new Category("TL003", "Kỹ năng sống", "Phát triển bản thân, tâm lý học"),
                    new Category("TL004", "Lịch sử", "Lịch sử Việt Nam và thế giới"),
                    new Category("TL005", "Khoa học", "Vật lý, hóa học, sinh học")
                };
                _categoryStorage.Save(_categories);
            }

            _notificationStorage = new FileStorage<Notification>("data/notifications.json");
            _notifications = _notificationStorage.Load();
            BorrowPolicy.Load();
            CheckUpcomingDueDates();
        }

        /// <summary>
        /// Quét toàn bộ phiếu đang mượn khi khởi động.
        /// Nếu còn <= 3 ngày đến hạn trả và chưa có thông báo hôm nay,
        /// tự động tạo thông báo nhắc nhở cho bạn đọc.
        /// </summary>
        private void CheckUpcomingDueDates()
        {
            List<BorrowRecord> records = _borrowService.GetAll();
            bool hasNew = false;

            for (int i = 0; i < records.Count; i++)
            {
                BorrowRecord r = records[i];
                if (r.Status != BorrowStatus.Borrowing) continue;

                int daysLeft = (r.DueDate.Date - DateTime.Now.Date).Days;
                if (daysLeft < 0 || daysLeft > 3) continue;

                // Kiểm tra đã có thông báo nhắc hôm nay cho phiếu này chưa để tránh trùng
                bool alreadyNotified = false;
                for (int j = 0; j < _notifications.Count; j++)
                {
                    Notification n = _notifications[j];
                    if (n.ReaderId == r.ReaderId
                        && n.CreatedDate.Date == DateTime.Now.Date
                        && n.Message.Contains(r.BookTitle)
                        && (n.Message.Contains("sắp đến hạn") || n.Message.Contains("HÔM NAY")))
                    {
                        alreadyNotified = true;
                        break;
                    }
                }
                if (alreadyNotified) continue;

                string msg;
                if (daysLeft == 0)
                    msg = $"Sách \"{r.BookTitle}\" đến hạn trả HÔM NAY. Vui lòng trả sách để tránh phát sinh phiếu phạt.";
                else
                    msg = $"Sách \"{r.BookTitle}\" sắp đến hạn trả sau {daysLeft} ngày ({r.DueDate:dd/MM/yyyy}). Vui lòng chuẩn bị trả đúng hạn.";

                _notifications.Add(new Notification("N" + DateTime.Now.Ticks + i, r.ReaderId, msg));
                hasNew = true;
            }

            if (hasNew)
                _notificationStorage.Save(_notifications);
        }

        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                try
                {
                    ShowMainMenu();
                    Console.Write("Chọn chức năng (0-5): ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": HandleReaderMenu(); break;
                        case "2": HandleBookMenu(); break;
                        case "3": HandleBorrowMenu(); break;
                        case "4": HandleReportMenu(); break;
                        case "5": HandleSystemMenu(); break;
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
                    Console.WriteLine($"\n[LỖI HỆ THỐNG PHÁT HIỆN Ở MENU]: Lỗi: {ex.Message}");
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
            Console.WriteLine("5. Cài đặt hệ thống");
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
                    case "1": AddReader(); break;
                    case "2": _readerService.PrintAll(); break;
                    case "3": SearchReader(); break;
                    case "4": UpdateReader(); break;
                    case "5": DeleteReader(); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("Lựa chọn không hợp lệ."); break;
                }
            }
        }

        private void AddReader()
        {
            Console.WriteLine("\n[THÊM BẠN ĐỌC MỚI]");

            Console.Write("Nhập ID: ");
            string id = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(id)) { Console.WriteLine("ID không được để trống."); return; }
            if (_readerService.FindById(id) != null) { Console.WriteLine($"ID '{id}' đã tồn tại. Vui lòng chọn ID khác."); return; }
            Console.Write("Nhập Tên: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Tên không được để trống."); return; }

            Console.Write("Nhập SDT: ");
            string phone = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phone)) { Console.WriteLine("Số điện thoại không được để trống."); return; }

            Console.Write("Nhập Email (Enter để bỏ qua): ");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email)) email = "N/A";

            Console.Write("Nhập Địa chỉ (Enter để bỏ qua): ");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address)) address = "N/A";

            Console.Write("Loại bạn đọc (1.Student / 2.Teacher): ");
            string type = Console.ReadLine();
            if (type == "1") type = "Student";
            else if (type == "2") type = "Teacher";
            else { Console.WriteLine("Loại bạn đọc không hợp lệ. Vui lòng chọn 1 hoặc 2."); return; }

            int defaultMax = type == "Teacher"
                ? BorrowPolicy.Instance.MaxBooksPerTeacher
                : BorrowPolicy.Instance.MaxBooksPerStudent;

            Console.Write($"Số sách được mượn tối đa (Enter để dùng mặc định {defaultMax}): ");
            string maxBorrowInput = Console.ReadLine();
            int maxBorrow = defaultMax;
            if (!string.IsNullOrWhiteSpace(maxBorrowInput))
            {
                if (!int.TryParse(maxBorrowInput, out maxBorrow) || maxBorrow <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, dùng mặc định.");
                    maxBorrow = defaultMax;
                }
            }

            Reader newReader = new Reader(id, name, phone, email, address, type, maxBorrow);
            _readerService.Add(newReader);

            // Tạo thông báo chào mừng
            Notification welcome = new Notification(
                "N" + DateTime.Now.Ticks,
                id,
                $"Chào mừng {name} đến với thư viện! Bạn có thể mượn tối đa {maxBorrow} quyển sách."
            );
            _notifications.Add(welcome);
            _notificationStorage.Save(_notifications);
        }

        private void SearchReader()
        {
            Console.Write("Nhập từ khóa tìm kiếm (tên): ");
            string keyword = Console.ReadLine();
            List<Reader> results = _readerService.SearchByName(keyword);
            if (results.Count == 0) { Console.WriteLine("Không tìm thấy bạn đọc nào."); return; }
            Console.WriteLine($"Tìm thấy {results.Count} kết quả:");
            for (int i = 0; i < results.Count; i++)
                Console.WriteLine($"{i + 1}. {results[i].GetInfo()}");
        }

        private void UpdateReader()
        {
            Console.Write("Nhập ID bạn đọc cần cập nhật: ");
            string id = Console.ReadLine();
            Reader existing = _readerService.FindById(id);
            if (existing == null) { Console.WriteLine($"Không tìm thấy bạn đọc với ID '{id}'."); return; }

            Console.WriteLine($"Thông tin hiện tại: {existing.GetInfo()}");
            Console.Write("Tên mới (Enter để giữ nguyên): ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = existing.Name;

            Console.Write("SDT mới (Enter để giữ nguyên): ");
            string phone = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phone)) phone = existing.Phone;

            Console.Write("Loại bạn đọc mới (Enter để giữ nguyên): ");
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
                    case "1": PrintBooks(_bookService.GetAll()); break;
                    case "2": AddBook(); break;
                    case "3": SearchBookByTitle(); break;
                    case "4": SearchBookByAuthor(); break;
                    case "5": SearchBookByCategory(); break;
                    case "6": PrintBooks(_bookService.GetAvailableBooks()); break;
                    case "7": DeleteBook(); break;
                    case "8": UpdateBook(); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("Lựa chọn không hợp lệ."); break;
                }
            }
        }

        private void PrintBooks(List<Book> books)
        {
            if (books.Count == 0) { Console.WriteLine("Không có sách nào."); return; }
            Console.WriteLine($"Tổng: {books.Count} cuốn");
            for (int i = 0; i < books.Count; i++)
                Console.WriteLine($"{i + 1}. {books[i].GetInfo()}");
        }

        private void AddBook()
        {
            Console.WriteLine("\n[THÊM SÁCH MỚI]");

            Console.Write("Nhập ID sách: ");
            string id = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(id)) { Console.WriteLine("ID không được để trống."); return; }
            if (_bookService.FindById(id) != null) { Console.WriteLine($"ID '{id}' đã tồn tại. Vui lòng chọn ID khác."); return; }

            Console.Write("Nhập Tựa đề: ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("Tựa đề không được để trống."); return; }

            Console.Write("Nhập Tác giả: ");
            string author = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(author)) { Console.WriteLine("Tác giả không được để trống."); return; }

            // Hiển thị danh sách thể loại để chọn
            Console.WriteLine("Danh sách thể loại:");
            for (int i = 0; i < _categories.Count; i++)
                Console.WriteLine($"  {i + 1}. {_categories[i].CategoryName}");
            Console.Write("Chọn thể loại (nhập số hoặc gõ tên thủ công): ");
            string catInput = Console.ReadLine().Trim();
            string category;
            int catIndex;
            if (int.TryParse(catInput, out catIndex) && catIndex >= 1 && catIndex <= _categories.Count)
                category = _categories[catIndex - 1].CategoryName;
            else if (!string.IsNullOrWhiteSpace(catInput))
                category = catInput;
            else { Console.WriteLine("Thể loại không được để trống."); return; }

            Console.Write("Nhập Nhà xuất bản (Enter để bỏ qua): ");
            string publisher = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(publisher)) publisher = "N/A";

            Console.Write("Nhập Số lượng: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
            {
                Console.WriteLine("Số lượng không hợp lệ, phải là số nguyên dương.");
                return;
            }

            Book newBook = new Book(id, title, author, category, publisher, qty);
            _bookService.Add(newBook);
        }

        private void SearchBookByTitle()
        {
            Console.Write("Nhập tên sách cần tìm: ");
            PrintBooks(_bookService.SearchByTitle(Console.ReadLine()));
        }

        private void SearchBookByAuthor()
        {
            Console.Write("Nhập tên tác giả cần tìm: ");
            PrintBooks(_bookService.SearchByAuthor(Console.ReadLine()));
        }

        private void SearchBookByCategory()
        {
            Console.Write("Nhập thể loại cần tìm: ");
            PrintBooks(_bookService.SearchByCategory(Console.ReadLine()));
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
            Book existing = _bookService.FindById(id);
            if (existing == null) { Console.WriteLine($"Không tìm thấy sách với ID '{id}'."); return; }

            Console.WriteLine("Thông tin hiện tại: " + existing.GetInfo());

            Console.Write("Tựa đề mới (Enter để giữ nguyên): ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title)) title = existing.Title;

            Console.Write("Tác giả mới (Enter để giữ nguyên): ");
            string author = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(author)) author = existing.Author;

            Console.Write("Thể loại mới (Enter để giữ nguyên): ");
            string category = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(category)) category = existing.Category;

            Console.Write("Nhà xuất bản mới (Enter để giữ nguyên): ");
            string publisher = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(publisher)) publisher = existing.Publisher;

            Console.Write("Tổng số lượng mới (Enter để giữ nguyên): ");
            string totalInput = Console.ReadLine();
            int newTotal = existing.TotalQuantity;
            int borrowedCount = existing.TotalQuantity - existing.AvailableQuantity;

            if (!string.IsNullOrWhiteSpace(totalInput))
            {
                if (!int.TryParse(totalInput, out newTotal) || newTotal <= 0)
                {
                    Console.WriteLine("Tổng số lượng không hợp lệ.");
                    return;
                }
                if (newTotal < borrowedCount)
                {
                    Console.WriteLine("Không thể cập nhật tổng số lượng nhỏ hơn số sách đang được mượn.");
                    return;
                }
            }

            existing.Title = title;
            existing.Author = author;
            existing.Category = category;
            existing.Publisher = publisher;
            existing.TotalQuantity = newTotal;
            existing.AvailableQuantity = newTotal - borrowedCount;
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
                    case "1": BorrowBook(); break;
                    case "2": ReturnBook(); break;
                    case "3": PrintBorrowRecords(_borrowService.GetAll()); break;
                    case "4": ViewRecordsByReader(); break;
                    case "5": PrintBorrowRecords(_borrowService.GetOverdueRecords()); break;
                    case "6": PrintFines(_borrowService.GetUnpaidFines()); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("Lựa chọn không hợp lệ."); break;
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

            bool success = _borrowService.BorrowBook(readerId, bookId, _currentLibrarian);
            if (!success) return;

            // Chỉ tạo thông báo khi mượn thành công
            Reader r = _readerService.FindById(readerId);
            Book b = _bookService.FindById(bookId);
            if (r != null && b != null)
            {
                string dueDate = DateTime.Now.AddDays(BorrowPolicy.Instance.MaxBorrowDays).ToString("dd/MM/yyyy");
                Notification n = new Notification(
                    "N" + DateTime.Now.Ticks,
                    readerId,
                    $"Bạn đã mượn sách \"{b.Title}\". Hạn trả: {dueDate}."
                );
                _notifications.Add(n);
                _notificationStorage.Save(_notifications);
            }
        }

        private void ReturnBook()
        {
            Console.WriteLine("\n[TRẢ SÁCH]");
            Console.Write("Nhập ID bạn đọc: ");
            string readerId = Console.ReadLine();

            List<BorrowRecord> borrowing = _borrowService.GetBorrowingRecordsByReader(readerId);
            if (borrowing.Count == 0) { Console.WriteLine("Bạn đọc này không có sách nào đang mượn."); return; }

            Console.WriteLine("Danh sách sách đang mượn:");
            for (int i = 0; i < borrowing.Count; i++)
                Console.WriteLine($"{i + 1}. {borrowing[i].GetInfo()}");

            Console.Write("Chọn số thứ tự sách cần trả: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > borrowing.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }

            string recordId = borrowing[choice - 1].RecordId;
            BorrowRecord record = borrowing[choice - 1];
            bool wasOverdue = record.IsOverdue();

            bool success = _borrowService.ReturnBook(recordId);
            if (!success) return;

            // Chỉ tạo thông báo khi trả thành công
            if (wasOverdue)
            {
                Notification n = new Notification(
                    "N" + DateTime.Now.Ticks,
                    readerId,
                    $"Bạn đã trả trễ sách \"{record.BookTitle}\" {record.GetOverdueDays()} ngày. Vui lòng thanh toán phiếu phạt."
                );
                _notifications.Add(n);
            }
            else
            {
                Notification n = new Notification(
                    "N" + DateTime.Now.Ticks,
                    readerId,
                    $"Bạn đã trả sách \"{record.BookTitle}\" thành công. Cảm ơn bạn đã trả đúng hạn!"
                );
                _notifications.Add(n);
            }
            _notificationStorage.Save(_notifications);
        }

        private void ViewRecordsByReader()
        {
            Console.Write("Nhập ID bạn đọc: ");
            PrintBorrowRecords(_borrowService.GetRecordsByReader(Console.ReadLine()));
        }

        private void PrintBorrowRecords(List<BorrowRecord> records)
        {
            if (records.Count == 0) { Console.WriteLine("Không có phiếu mượn nào."); return; }
            Console.WriteLine($"Tổng: {records.Count} phiếu");
            for (int i = 0; i < records.Count; i++)
                Console.WriteLine($"{i + 1}. {records[i].GetInfo()}");
        }

        private void PrintFines(List<Fine> fines)
        {
            if (fines.Count == 0) { Console.WriteLine("Không có phiếu phạt chưa thanh toán."); return; }
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
                Console.WriteLine("6. Xem thông báo của bạn đọc");
                Console.WriteLine("0. Quay lại");
                Console.Write("Lựa chọn của bạn: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        List<Reader> borrowing = _readerService.GetBorrowingReaders();
                        if (borrowing.Count == 0) Console.WriteLine("Không có ai đang mượn sách.");
                        else for (int i = 0; i < borrowing.Count; i++)
                                Console.WriteLine($"{i + 1}. {borrowing[i].GetInfo()}");
                        break;
                    case "2":
                        List<Book> allBooks = _bookService.GetAll();
                        Console.WriteLine($"Tổng số đầu sách: {allBooks.Count}");
                        int totalCopies = 0, availCopies = 0;
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
                    case "6":
                        ViewNotifications();
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

        private void ViewNotifications()
        {
            Console.Write("Nhập ID bạn đọc: ");
            string readerId = Console.ReadLine();
            List<Notification> result = new List<Notification>();
            for (int i = 0; i < _notifications.Count; i++)
                if (_notifications[i].ReaderId == readerId)
                    result.Add(_notifications[i]);

            if (result.Count == 0) { Console.WriteLine("Không có thông báo nào."); return; }
            Console.WriteLine($"Có {result.Count} thông báo:");
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {result[i].GetInfo()}");
                result[i].MarkAsRead();
                _notificationStorage.Save(_notifications);
            }
        }

        // =====================================================================
        // 5. CÀI ĐẶT HỆ THỐNG
        // =====================================================================

        private void HandleSystemMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- CÀI ĐẶT HỆ THỐNG ---");
                Console.WriteLine("1. Xem chính sách mượn hiện tại");
                Console.WriteLine("2. Cập nhật chính sách mượn");
                Console.WriteLine("3. Xem danh sách thể loại sách");
                Console.WriteLine("4. Thêm thể loại mới");
                Console.WriteLine("5. Xóa thể loại");
                Console.WriteLine("6. Sửa thể loại");
                Console.WriteLine("0. Quay lại");
                Console.Write("Lựa chọn của bạn: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": Console.WriteLine(BorrowPolicy.Instance.GetInfo()); break;
                    case "2": UpdateBorrowPolicy(); break;
                    case "3": PrintCategories(); break;
                    case "4": AddCategory(); break;
                    case "5": DeleteCategory(); break;
                    case "6": EditCategory(); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("Lựa chọn không hợp lệ."); break;
                }
            }
        }


        private void UpdateBorrowPolicy()
        {
            Console.WriteLine("\n[CẬP NHẬT CHÍNH SÁCH MƯỢN]");
            Console.WriteLine("Hiện tại: " + BorrowPolicy.Instance.GetInfo());

            Console.Write($"Số ngày mượn tối đa (Enter để giữ {BorrowPolicy.Instance.MaxBorrowDays}): ");
            string daysInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(daysInput))
            {
                if (int.TryParse(daysInput, out int days) && days > 0)
                    BorrowPolicy.Instance.MaxBorrowDays = days;
                else
                    Console.WriteLine("Giá trị không hợp lệ, giữ nguyên.");
            }

            Console.Write($"Tiền phạt mỗi ngày trễ (Enter để giữ {BorrowPolicy.Instance.FinePerDay:N0}): ");
            string fineInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(fineInput))
            {
                if (double.TryParse(fineInput, out double fine) && fine >= 0)
                    BorrowPolicy.Instance.FinePerDay = fine;
                else
                    Console.WriteLine("Giá trị không hợp lệ, giữ nguyên.");
            }

            Console.Write($"Số sách tối đa sinh viên được mượn (Enter để giữ {BorrowPolicy.Instance.MaxBooksPerStudent}): ");
            string studentInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(studentInput))
            {
                if (int.TryParse(studentInput, out int studentMax) && studentMax > 0)
                    BorrowPolicy.Instance.MaxBooksPerStudent = studentMax;
                else
                    Console.WriteLine("Giá trị không hợp lệ, giữ nguyên.");
            }

            Console.Write($"Số sách tối đa giáo viên được mượn (Enter để giữ {BorrowPolicy.Instance.MaxBooksPerTeacher}): ");
            string teacherInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(teacherInput))
            {
                if (int.TryParse(teacherInput, out int teacherMax) && teacherMax > 0)
                    BorrowPolicy.Instance.MaxBooksPerTeacher = teacherMax;
                else
                    Console.WriteLine("Giá trị không hợp lệ, giữ nguyên.");
            }

            BorrowPolicy.Instance.Save();
            Console.WriteLine("Đã cập nhật: " + BorrowPolicy.Instance.GetInfo());
        }

        private void PrintCategories()
        {
            if (_categories.Count == 0) { Console.WriteLine("Chưa có thể loại nào."); return; }
            Console.WriteLine($"Danh sách {_categories.Count} thể loại:");
            for (int i = 0; i < _categories.Count; i++)
                Console.WriteLine($"{i + 1}. {_categories[i].GetInfo()}");
        }

        private void AddCategory()
        {
            Console.WriteLine("\n[THÊM THỂ LOẠI MỚI]");
            Console.Write("Nhập ID thể loại: ");
            string id = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(id)) { Console.WriteLine("ID không được để trống."); return; }

            Console.Write("Nhập tên thể loại: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Tên không được để trống."); return; }

            Console.Write("Nhập mô tả (Enter để bỏ qua): ");
            string desc = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(desc)) desc = "";

            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].CategoryId == id)
                {
                    Console.WriteLine($"Mã thể loại '{id}' đã tồn tại.");
                    return;
                }
                if (_categories[i].CategoryName.ToLower() == name.ToLower())
                {
                    Console.WriteLine($"Tên thể loại '{name}' đã tồn tại.");
                    return;
                }
            }

            _categories.Add(new Category(id, name, desc));
            _categoryStorage.Save(_categories);
            Console.WriteLine($"Đã thêm thể loại \"{name}\".");
        }

        private void DeleteCategory()
        {
            PrintCategories();
            Console.Write("Nhập số thứ tự thể loại cần xóa: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > _categories.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }
            string name = _categories[idx - 1].CategoryName;
            _categories.RemoveAt(idx - 1);
            _categoryStorage.Save(_categories);
            Console.WriteLine($"Đã xóa thể loại \"{name}\".");
        }
        private void EditCategory()
        {
            PrintCategories();
            Console.Write("Nhập số thứ tự thể loại cần sửa: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > _categories.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }
            Category target = _categories[idx - 1];

            Console.Write($"Tên mới (Enter để giữ \"{target.CategoryName}\"): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
                target.CategoryName = newName;

            Console.Write($"Mô tả mới (Enter để giữ \"{target.Description}\"): ");
            string newDesc = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newDesc))
                target.Description = newDesc;

            _categoryStorage.Save(_categories);
            Console.WriteLine($"Đã cập nhật thể loại \"{target.CategoryName}\".");
        }
    }
}