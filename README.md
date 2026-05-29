# Library Management System

Hệ thống quản lý mượn/trả sách thư viện viết bằng **C# .NET 8 Console Application**.  
Dự án được xây dựng cho môn **Lập trình hướng đối tượng (OOP)**, tập trung thể hiện 4 tính chất chính của OOP: **Encapsulation, Abstraction, Inheritance, Polymorphism**.

---

## Thông tin dự án

- **Đề tài:** Xây dựng chương trình quản lý mượn/trả sách thư viện
- **Ngôn ngữ:** C#
- **Framework:** .NET 8.0
- **Giao diện:** Console
- **Lưu trữ:** File JSON bằng `System.Text.Json`
- **Quy tắc:** Không sử dụng LINQ, Lambda và từ khóa `var`

---

## Thành viên nhóm

| Thành viên | Tài khoản GitHub | Phụ trách chính |
|---|---|---|
| **Trần Đại Phát** | `loiz123` | `Person`, `Reader`, `Librarian`, `ReaderService`, `BorrowPolicy` |
| **Hà Ngọc Thiện** | `HNTRHunter` | `IManageable<T>`, `Book`, `BookService` |
| **Tiêu Lâm Định Quốc** | `dinhquoc03032626` | `BorrowStatus`, `BorrowRecord`, `Fine`, `BorrowService` |
| **Khiếu Hoàng Nam Anh** | `nvm4nh` | `FileStorage<T>`, `MenuController`, `Program`, `Category`, `Notification` |

---

## Chức năng chính

- Quản lý bạn đọc:
  - Thêm, xem, tìm kiếm, cập nhật và xóa bạn đọc.
  - Kiểm tra giới hạn số sách đang mượn.
- Quản lý sách:
  - Thêm, xem, tìm kiếm, cập nhật và xóa sách.
  - Quản lý số lượng sách còn lại.
  - Hỗ trợ chọn thể loại từ danh sách `Category`.
- Quản lý mượn/trả:
  - Tạo phiếu mượn sách.
  - Trả sách theo bạn đọc.
  - Kiểm tra sách quá hạn.
  - Tự động tạo phiếu phạt khi trả quá hạn.
- Báo cáo và thông báo:
  - Xem phiếu mượn, sách quá hạn, phiếu phạt chưa thanh toán.
  - Tạo và lưu thông báo cho bạn đọc.
- Cài đặt hệ thống:
  - Cập nhật số ngày mượn tối đa.
  - Cập nhật mức phạt mỗi ngày trễ.
  - Cập nhật số sách tối đa theo loại bạn đọc.
  - Quản lý danh mục thể loại sách.

---

## Cấu trúc dự án

```text
LibraryManagement/
├── Models/
│   ├── Person.cs              # Abstract base class cho Reader và Librarian
│   ├── Reader.cs              # Lớp bạn đọc
│   ├── Librarian.cs           # Lớp thủ thư
│   ├── Book.cs                # Lớp sách
│   ├── Category.cs            # Lớp thể loại sách
│   ├── BorrowRecord.cs        # Lớp phiếu mượn/trả
│   ├── BorrowStatus.cs        # Enum trạng thái phiếu mượn
│   ├── Fine.cs                # Lớp phiếu phạt
│   ├── Notification.cs        # Lớp thông báo
│   └── BorrowPolicy.cs        # Chính sách mượn sách
├── Services/
│   ├── IManageable.cs         # Generic interface quản lý dữ liệu
│   ├── ReaderService.cs       # Xử lý nghiệp vụ bạn đọc
│   ├── BookService.cs         # Xử lý nghiệp vụ sách
│   └── BorrowService.cs       # Xử lý nghiệp vụ mượn/trả và tiền phạt
├── FileStorage/
│   └── FileStorage.cs         # Generic class đọc/ghi JSON
├── Program.cs                 # Điểm khởi động chương trình
├── MenuController.cs          # Điều hướng menu Console
├── LibraryManagement.csproj
└── LibraryManagement.sln
```

Thư mục `data/` được tạo khi chương trình chạy và dùng để lưu dữ liệu JSON.

---

## Dữ liệu lưu trữ

Dữ liệu được lưu vào các file JSON trong thư mục `data/`.

```text
data/books.json            # Danh sách sách
data/readers.json          # Danh sách bạn đọc
data/borrowrecords.json    # Danh sách phiếu mượn/trả
data/fines.json            # Danh sách phiếu phạt
data/categories.json       # Danh sách thể loại sách
data/notifications.json    # Danh sách thông báo
data/borrowpolicy.json     # Chính sách mượn sách
```

Các danh sách dữ liệu được đọc/ghi thông qua generic class `FileStorage<T>`.  
Riêng `BorrowPolicy` tự quản lý việc đọc/ghi `borrowpolicy.json` thông qua `BorrowPolicy.Load()` và `BorrowPolicy.Save()`.

---

## Các tính chất OOP được áp dụng

### 1. Encapsulation - Đóng gói

- Các field được khai báo `private`.
- Dữ liệu được truy cập thông qua property hoặc phương thức nghiệp vụ.
- `Book.Checkout()` và `Book.Return()` kiểm soát số lượng sách, không cho số lượng âm hoặc vượt quá tổng số lượng.
- `Reader.IncreaseBorrowCount()` và `Reader.DecreaseBorrowCount()` kiểm soát số sách đang mượn.

### 2. Abstraction - Trừu tượng

- `Person` là abstract class, không tạo object trực tiếp.
- `IManageable<T>` là generic interface quy định các thao tác chung như `Add`, `Remove`, `FindById`, `GetAll`, `Update`.

### 3. Inheritance - Kế thừa

- `Reader` kế thừa từ `Person`.
- `Librarian` kế thừa từ `Person`.

### 4. Polymorphism - Đa hình

- `Reader` và `Librarian` override `GetInfo()` và `GetRole()` theo cách riêng.
- `ReaderService`, `BookService`, `BorrowService` cùng implement `IManageable<T>` với kiểu dữ liệu khác nhau.

---

## Kỹ thuật OOP và kỹ thuật hỗ trợ

- **Generic:** `IManageable<T>`, `FileStorage<T>`
- **Enum:** `BorrowStatus`
- **Serialization/Deserialization:** `System.Text.Json`
- **Exception Handling:** `try-catch` khi đọc/ghi file và kiểm tra nghiệp vụ
- **Nullable Type:** `DateTime? ReturnDate`, `T? FindById()`
- **Constructor Chaining:** `Reader`, `Librarian` gọi constructor của `Person`
- **Dependency Injection thủ công:** `BorrowService` nhận `ReaderService` và `BookService` qua constructor
- **Singleton thông qua Instance:** `BorrowPolicy.Instance`

---

## Yêu cầu cài đặt

- .NET SDK 8.0 trở lên
- Visual Studio 2022 hoặc Visual Studio Code
- Không cần cài thêm thư viện ngoài

---

## Cách chạy chương trình

### Cách 1: Chạy bằng Visual Studio

1. Mở file `LibraryManagement.sln`.
2. Chọn project `LibraryManagement`.
3. Nhấn `Ctrl + F5` để chạy chương trình.

### Cách 2: Chạy bằng terminal

```bash
cd LibraryManagement
dotnet restore
dotnet run
```

---

## Quy tắc code của nhóm

- Không dùng `var`, khai báo kiểu dữ liệu tường minh.
- Không dùng LINQ và Lambda.
- Dùng `for` hoặc `foreach` để duyệt danh sách.
- Các lớp và phương thức quan trọng có comment `/// <summary>`.
- Tách code theo nhóm `Models`, `Services`, `Storage`, `Controller`.
- Dữ liệu được lưu ra file JSON, không hardcode dữ liệu lâu dài trong chương trình.

---

## Git workflow

```bash
# Tạo branch chức năng
git checkout -b feature/<ten-thanh-vien-hoac-chuc-nang>

# Commit sau mỗi phần hoàn thành
git add .
git commit -m "feat: mo ta ngan gon chuc nang"

# Merge vào main sau khi kiểm tra
git checkout main
git merge feature/<ten-branch>
```

Nhóm sử dụng branch, commit và pull request để tích hợp từng phần chức năng như quản lý sách, quản lý bạn đọc, mượn/trả sách, lưu trữ JSON và điều hướng menu.

---

## Definition of Done

Một chức năng được xem là hoàn thành khi:

- [ ] Code build được và chạy được.
- [ ] Không sử dụng LINQ, Lambda hoặc `var`.
- [ ] Có xử lý lỗi nhập liệu cơ bản.
- [ ] Dữ liệu được lưu/đọc đúng từ file JSON nếu chức năng có thay đổi dữ liệu.
- [ ] Đã được kiểm tra bằng menu Console.
- [ ] Đã commit và merge vào branch chính.

---

## Ghi chú

Dự án được xây dựng nhằm phục vụ môn học Lập trình hướng đối tượng.  
Mục tiêu chính là thể hiện cách áp dụng các nguyên lý OOP vào một bài toán quản lý thư viện đơn giản.
