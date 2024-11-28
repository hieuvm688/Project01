//Tạo 1 đường link tới file bat
string linkfile = "C:\tmp\RunFile\{txt_sothaydoichinh.Text}.bat";
//thêm linkfile vào mail gửi
//Tạo file bat với nội dung: 
private void CreateFileRun(){
        string batContent = "@echo off\nset param=20241128_002\n\\\\apbivnfa11\\Common\\RD-EE\\10.APP\\App_TDTK.exe %param%"; 
        string filePath = "20241128_002.bat"; 
        try 
        { 
            File.WriteAllText(filePath, batContent); 
            Console.WriteLine($"Batch file '{filePath}' created successfully.");
         } 
         catch (Exception ex) 
         {
             Console.WriteLine($"An error occurred: {ex.Message}");
         }
}
//Sau khi Click vào file text thì xóa đi luôn, nghĩa là nếu app chạy từ đường link đấy ra thì sẽ xóa file bat
//Đọc tham số được truyền vào app thông qua App.xaml.cs
using System;
using System.Windows;

namespace YourNamespace
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                string param = e.Args[0];
                // Xử lý tham số dòng lệnh ở đây
                //Sau khi truyền vào, thì gọi tham số này qua PitX và chạy bình thường
                //Nếu có lỗi gây ra, thì nhớ kiểm tra xem output.txt có được đọc ra đúng cách không? vì có thể khi tạo 1 MainWindow mới sẽ không đọc được output
                MessageBox.Show($"Tham số truyền vào: {param}");
            }
            else
            {
                MessageBox.Show("Không có tham số nào được truyền vào.");
            }

            // Khởi chạy cửa sổ chính
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
