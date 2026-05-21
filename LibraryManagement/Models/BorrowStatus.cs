namespace Library_Management.Models
{
    /// <summary>
    /// Trạng thái của phiếu mượn sách
    /// </summary>
    public enum BorrowStatus
    {
        Borrowing,
        Returned,
        Overdue,
        Lost
    }
}