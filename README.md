#  Library Management System

Hệ thống quản lý mượn/trả sách thư viện viết bằng **C#**, áp dụng 4 tính chất OOP: Encapsulation, Abstraction, Inheritance, Polymorphism.

> Môn học: Lập trình Hướng đối tượng (OOP)

---

##  Thành viên nhóm

| Thành viên | Phụ trách | Branch |
|---|---|---|
| **Đại Phát** | Person, Reader, Librarian, ReaderService | `feature/loiz123` |
| **Ngọc Thiện** | IManageable\<T\>, Book, BookService | `feature/HNTRHunter` |
| **Định Quốc** | BorrowStatus, BorrowRecord, Fine, BorrowService | `feature/dinhquoc03032626` |
| **Nam Anh** | FileStorage\<T\>, MenuController, Program.cs | `feature/nvm4nh` |

---

##  Cấu trúc dự án

```
LibraryManagement/
├── Models/
│   ├── Person.cs           # Abstract base class
│   ├── Reader.cs           # Kế thừa Person
│   ├── Librarian.cs        # Kế thừa Person
│   ├── Book.cs
│   ├── BorrowRecord.cs
│   ├── Fine.cs
│   └── BorrowStatus.cs     # Enum
├── Services/
│   ├── IManageable.cs      # Interface dùng chung
│   ├── ReaderService.cs
│   ├── BookService.cs
│   └── BorrowService.cs
├── Storage/
│   └── FileStorage.cs      # Generic JSON storage
├── data/
│   ├── books.json
│   ├── readers.json
│   ├── borrowrecords.json
│   └── fines.json
├── Program.cs
└── MenuController.cs
```

---

##  Yêu cầu

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)
- Không dùng thư viện ngoài — chỉ `System.Text.Json` (built-in)

---

##  Chạy chương trình

```bash
# Clone repo
git clone <repo-url>
cd LibraryManagement

# Build và chạy
dotnet run
```

---

##  Các tính chất OOP trong dự án

### Abstraction
- `Person` là abstract class — không tạo object trực tiếp
- `IManageable<T>` là interface dùng chung cho tất cả Services

### Inheritance
- `Reader` và `Librarian` đều kế thừa từ `Person`

### Polymorphism
- `GetInfo()` và `GetRole()` được override khác nhau ở `Reader` và `Librarian`
- `BookService`, `ReaderService`, `BorrowService` đều implement `IManageable<T>`

### Encapsulation
- Tất cả fields là `private`/`protected`, truy cập qua property
- `BorrowRecord` tự quản lý trạng thái bên trong
- `MenuController` ẩn toàn bộ logic điều hướng

---

##  Lưu trữ dữ liệu

Dữ liệu được lưu ra file JSON trong thư mục `data/` bằng `System.Text.Json`.  
`FileStorage<T>` là generic class xử lý Save/Load cho mọi kiểu dữ liệu.

```
data/books.json          ← danh sách sách
data/readers.json        ← danh sách bạn đọc
data/borrowrecords.json  ← phiếu mượn
data/fines.json          ← phiếu phạt
```

---

## 📋 Quy tắc code

- Không dùng `var` — khai báo rõ kiểu
- Không dùng LINQ/Lambda — dùng `for`/`foreach`
- Mọi thao tác I/O file đều bọc `try-catch IOException`
- Comment `/// <summary>` cho mọi class và method quan trọng

---

##  Git workflow

```bash
# Tạo branch trước khi làm
git checkout -b feature/<tên-thành-viên>

# Commit thường xuyên
git commit -m "feat: thêm class Reader với CanBorrow()"

# Merge vào main sau khi có review
git checkout main
git merge feature/<tên-thành-viên>
```

>  **ReaderService.cs:** Đại Phát commit trước. Nam Anh tạo branch riêng để tích hợp FileStorage, sau đó mới merge — không viết lại từ đầu.

---

##  Definition of Done

Một task được coi là hoàn thành khi:
- [ ] Code chạy được, không lỗi build
- [ ] Đã review bởi ít nhất 1 thành viên khác
- [ ] Đã merge vào `main` và không còn conflict
- [ ] Có `/// <summary>` comment đầy đủ

---

*Dự án môn Lập trình Hướng đối tượng — nhóm 4 thành viên.*
