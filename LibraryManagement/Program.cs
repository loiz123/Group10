using System;

namespace Library_Management
{
    class Program
    {
        static void Main(string[] args)
        {
            // Hỗ trợ in tiếng Việt có dấu trên Console
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // Khởi tạo và chạy giao diện
            MenuController menu = new MenuController();
            menu.Run();
        }
    }
}