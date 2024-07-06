using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using System.IO;
using Button = System.Windows.Controls.Button;
using System.Diagnostics;
using System.Linq;
using USB_RDPE_LIB;

namespace KanbanSystemShow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection("Data Source=APBIVNDB20;Initial Catalog=KanBanSystemDB;User ID=sa;Password=Pa$$w0rd ");
        List<string> listModel = new List<string>();
        List<string> valueModel = new List<string>();
        List<string> ValuesMK = new List<string>();
        DataTable dt3 = new DataTable();
        string path = Directory.GetCurrentDirectory();
        string item;
        string LotSx;
        string MaMay;
        string CongDoan;
        string NamePC = Environment.MachineName;
        string folderPath = Directory.GetCurrentDirectory();
        public static DataTable dtM = new DataTable();
        DataTable dtMT = new DataTable();
        string NameWindow = Environment.MachineName;
        Screen[] screens = Screen.AllScreens;

        Devices usb_device = new Devices();

        public MainWindow()
        {
            InitializeComponent();
            using(SqlConnection con = new SqlConnection("Data Source=APBIVNDB20;Initial Catalog=KanBanSystemDB;User ID=sa;Password=Pa$$w0rd "))
            {
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * from [KanBanSystemDB].[dbo].[MayTinh]", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
            }

            using (SqlConnection con = new SqlConnection("Data Source=APBIVNDB20;Initial Catalog=KanBanSystemDB;User ID=sa;Password=Pa$$w0rd"))
            {
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(" SELECT * from [KanBanSystemDB].[dbo].[MayTinh] where Name = '" + NamePC + "'", con))
                {
                    da.Fill(dtMT);
                }
            }

            //end
            //int count = dtMT.AsEnumerable().Count(row => row.Field<Int32>("NameScreen") != 0);
            usb_device.Connect();
        }

        private void DataInImage()
        {
            try
            {
                con.Open();
                //Đọc tên máy tính để check số lượng màn cắm vào máy tính đó
                using (SqlDataAdapter da = new SqlDataAdapter(" SELECT * from [KanBanSystemDB].[dbo].[MayTinh] where Name = '" + NamePC + "'", con))
                {
                    dtMT.Clear();
                    da.Fill(dtMT);
                }
                int count = dtMT.Rows.Count;
                int ScreenCount = Convert.ToInt32(dtMT.Rows[0][2]);
                if (screens.Length < 0 || count < screens.Length)
                {
                    MessageBox.Show("Data isn't unable");
                }
                else
                {

                    for (int i = 0; i < screens.Length; i++)
                    {
                        using (SqlCommand cmd = new SqlCommand("select Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read() && reader != null)
                                {
                                    item = "[" + reader["Material"].ToString() + "]";
                                }
                            }
                        }
                        dtM.Clear();
                        int nameScreen = i + ScreenCount;
                        var da = new SqlDataAdapter("SELECT mt.NameScreen, m.VitriChuyen, m.MaLK from [KanBanSystemDB].[dbo].[MayTinh] mt  JOIN [KanBanSystemDB].[dbo].[ListMasterKanban]" +
                        " m on mt.CongDoan = m.VitriChuyen where mt.[NameScreen]='" + nameScreen + "' AND " + item + " != '' ", con);
                        da.Fill(dtM);
                        Screen screen = screens[nameScreen];
                        //Check nếu model hiện tại đang không  có sản phẩm chạy trên chuyền, thì chuyển sang model mới để khỏi bị lãng phí công đoạn
                        // =========================================================HIỂN THỊ MÀN HÌNH 1===============================================================

                        if (dtM != null && dtM.Rows.Count == 0)
                        {
                            MessageBox.Show($"Không có lot để sản xuất ở công đoạn {nameScreen}!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            using (SqlCommand command = new SqlCommand("SELECT CongDoan from [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + nameScreen + "' ", con))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        CongDoan = reader["CongDoan"].ToString();
                                    }
                                }
                            }
                            using (SqlCommand command = new SqlCommand(" Select ProductOrderBarcode, Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read() && reader != null)
                                    {
                                        LotSx = reader["ProductOrderBarcode"].ToString();
                                        MaMay = reader["Material"].ToString();
                                    }
                                }
                            }
                            mainWindow = new Window();
                            mainWindow.Title = "Hiển thị kanban ảnh";
                            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                            mainWindow.Left = screen.WorkingArea.Left;
                            mainWindow.Top = screen.WorkingArea.Top;
                            mainWindow.Width = 1920;
                            mainWindow.Height = 1080;
                            //mainWindow.WindowStyle = WindowStyle.None;
                            mainWindow.ShowInTaskbar = true;
                            mainWindow.ShowActivated = true;
                            
                            Grid myGrid = new Grid();
                            myGrid.Width = 1900;
                            myGrid.Height = 1060;
                            myGrid.Background = Brushes.PowderBlue;
                            myGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            myGrid.VerticalAlignment = VerticalAlignment.Center;
                            myGrid.ShowGridLines = true;
                            
                            ColumnDefinition colDef1 = new ColumnDefinition();
                            ColumnDefinition colDef2 = new ColumnDefinition();
                            ColumnDefinition colDef3 = new ColumnDefinition();
                            myGrid.ColumnDefinitions.Add(colDef1);
                            myGrid.ColumnDefinitions.Add(colDef2);
                            myGrid.ColumnDefinitions.Add(colDef3);
                            
                            RowDefinition rowDef1 = new RowDefinition();
                            RowDefinition rowDef2 = new RowDefinition();
                            RowDefinition rowDef3 = new RowDefinition();
                            myGrid.RowDefinitions.Add(rowDef1);
                            myGrid.RowDefinitions.Add(rowDef2);
                            myGrid.RowDefinitions.Add(rowDef3);
                            rowDef1.Height = new GridLength(80);
                            TextBlock txt1 = new TextBlock();
                            txt1.Text = $"Công đoạn: {CongDoan}";
                            txt1.FontSize = 20;
                            txt1.FontWeight = FontWeights.Bold;
                            txt1.VerticalAlignment = VerticalAlignment.Center;
                            txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            
                            Grid.SetColumn(txt1, 0);
                            Grid.SetRow(txt1, 0);
                            TextBlock txt2 = new TextBlock();
                            txt2.Text = $"Production Order: {LotSx} ";
                            txt2.FontSize = 20;
                            txt2.FontWeight = FontWeights.Bold;
                            txt2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            txt2.VerticalAlignment = VerticalAlignment.Center;
                            Grid.SetColumn(txt2, 1);
                            Grid.SetRow(txt1, 0);
                            
                            TextBlock txt3 = new TextBlock();
                            txt3.Text = $"Mã máy: {MaMay} ";
                            txt3.FontSize = 20;
                            txt3.FontWeight = FontWeights.Bold;
                            txt3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            txt3.VerticalAlignment = VerticalAlignment.Center;
                            Grid.SetColumn(txt3, 2);
                            Grid.SetRow(txt1, 0);
                            //End search

                            var button = new Button
                            {
                                Width = 100,
                                Height = 40,
                                Content = "Hoàn thành",
                                Background = Brushes.DeepSkyBlue,
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                FontSize = 14,
                                Tag = $"{i}"
                            };
                            button.Click += btnTrangThai_Click;
                            Grid.SetColumn(button, 0);
                            Grid.SetRow(button, 0);
                            
                            var button1 = new Button
                            {
                                Width = 100,
                                Height = 40,
                                Content = "Close",
                                Background = Brushes.OrangeRed,
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                                VerticalAlignment = VerticalAlignment.Center,
                                FontSize = 14
                            };
                            button1.Click += btnClose_Click;
                            Grid.SetColumn(button1, 2);
                            Grid.SetRow(button1, 0);
                            
                            myGrid.Children.Add(txt1);
                            myGrid.Children.Add(txt2);
                            myGrid.Children.Add(txt3);
                            myGrid.Children.Add(button);
                            mainWindow.Content = myGrid;
                            mainWindow.Show();
                        }

                        /*============================================================HIỂN THỊ Ở MÀN HÌNH 2============================================================= */
                        else
                        {
                            using (SqlCommand command = new SqlCommand("SELECT CongDoan from [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + nameScreen + "' ", con))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        CongDoan = reader["CongDoan"].ToString();
                                    }
                                }
                            }
                            using (SqlCommand command = new SqlCommand(" Select ProductOrderBarcode, Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read() && reader != null)
                                    {
                                        LotSx = reader["ProductOrderBarcode"].ToString();
                                        MaMay = reader["Material"].ToString();
                                    }
                                }
                            }
                            mainWindow = new Window();
                            mainWindow.Title = "Hiển thị kanban ảnh";
                            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                            mainWindow.Left = screen.WorkingArea.Left;
                            mainWindow.Top = screen.WorkingArea.Top;
                            mainWindow.Width = 1920;
                            mainWindow.Height = 1080;
                            //mainWindow.WindowStyle = WindowStyle.None;
                            mainWindow.ShowInTaskbar = true;
                            mainWindow.ShowActivated = true;
                            
                            Grid myGrid = new Grid();
                            myGrid.Width = 1900;
                            myGrid.Height = 1060;
                            myGrid.Background = Brushes.PowderBlue;
                            myGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            myGrid.VerticalAlignment = VerticalAlignment.Center;
                            myGrid.ShowGridLines = true;

                            RowDefinition rowDef1 = new RowDefinition();
                            RowDefinition rowDef2 = new RowDefinition();
                            myGrid.RowDefinitions.Add(rowDef1);
                            myGrid.RowDefinitions.Add(rowDef2);
                            rowDef1.Height = new GridLength(80);

                            TextBlock txt1 = new TextBlock();
                            txt1.Text = $"Công đoạn: {CongDoan}";
                            txt1.FontSize = 20;
                            txt1.FontWeight = FontWeights.Bold;
                            txt1.VerticalAlignment = VerticalAlignment.Center;
                            txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                            Grid.SetColumn(txt1, 0);
                            Grid.SetRow(txt1, 0);

                            TextBlock txt2 = new TextBlock();
                            txt2.Text = $"Production Order: {LotSx} ";
                            txt2.FontSize = 20;
                            txt2.FontWeight = FontWeights.Bold;
                            txt2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            txt2.VerticalAlignment = VerticalAlignment.Center;
                            Grid.SetColumn(txt2, 1);
                            Grid.SetRow(txt1, 0);

                            TextBlock txt3 = new TextBlock();
                            txt3.Text = $"Mã máy: {MaMay} ";
                            txt3.FontSize = 20;
                            txt3.FontWeight = FontWeights.Bold;
                            txt3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            txt3.VerticalAlignment = VerticalAlignment.Center;
                            Grid.SetColumn(txt3, 2);
                            Grid.SetRow(txt1, 0);
                            //End search

                            Grid GridR1 = new Grid();
                            GridR1.Width = 1900;
                            GridR1.Height = 80;
                            GridR1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            GridR1.VerticalAlignment = VerticalAlignment.Center;
                            Grid.SetColumn(GridR1, 0);
                            Grid.SetRow(GridR1, 0);
                            ColumnDefinition colDef1 = new ColumnDefinition();
                            ColumnDefinition colDef2 = new ColumnDefinition();
                            ColumnDefinition colDef3 = new ColumnDefinition();
                            GridR1.ColumnDefinitions.Add(colDef1);
                            GridR1.ColumnDefinitions.Add(colDef2);
                            GridR1.ColumnDefinitions.Add(colDef3);
                            GridR1.ShowGridLines = true;

                            var button = new Button
                            {
                                Width = 100,
                                Height = 40,
                                Content = "Hoàn thành",
                                Background = Brushes.DeepSkyBlue,
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                FontSize = 14,
                                Tag = $"{i}"
                            };
                            button.Click += btnTrangThai_Click;
                            Grid.SetColumn(button, 0);
                            Grid.SetRow(button, 0);

                            //Button Close
                            var button1 = new Button
                            {
                                Width = 100,
                                Height = 40,
                                Content = "Close",
                                Background = Brushes.OrangeRed,
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                                VerticalAlignment = VerticalAlignment.Center,
                                FontSize = 14
                            };
                            button1.Click += btnClose_Click;
                            Grid.SetColumn(button1, 2);
                            Grid.SetRow(button1, 0);

                            Grid GridR2 = new Grid();
                            GridR2.Width = 1900;
                            GridR2.Height = 980;
                            GridR2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            GridR2.VerticalAlignment = VerticalAlignment.Center;
                            Grid.SetRow(GridR2, 1);
                            Grid.SetColumn(GridR2, 0);
                            int counT = dtM.Rows.Count;
                            switch (counT)
                            {
                                case 0:
                                    MessageBox.Show("Không có dữ liệu!");
                                    break;
                                case 1:
                                    var image_ = new Image();
                                    image_.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtM.Rows[0][2]}.jpg", UriKind.RelativeOrAbsolute));
                                    Grid.SetRow(image_, 0);
                                    Grid.SetColumn(image_, 0);
                                    GridR2.Children.Add(image_);
                                    break;
                                case 2:
                                    for (int j = 0; j < 2; j++)
                                    {
                                        ColumnDefinition colDef_2 = new ColumnDefinition();
                                        GridR2.ColumnDefinitions.Add(colDef_2);
                                        GridR2.ShowGridLines = true;
                                        test test2 = new test(j);
                                        Grid.SetRow(test2, 0);
                                        Grid.SetColumn(test2, j);
                                        GridR2.Children.Add(test2);
                                    }
                                    break;
                                case 3:
                                    for (int j = 0; j < counT - 1; j++)
                                    {
                                        ColumnDefinition colDef_2 = new ColumnDefinition();
                                        GridR2.ColumnDefinitions.Add(colDef_2);
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 1)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 1)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 2);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 4:
                                    for (int j = 0; j < counT - 2; j++)
                                    {
                                        ColumnDefinition colDef_2 = new ColumnDefinition();
                                        GridR2.ColumnDefinitions.Add(colDef_2);
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 1)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 1)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 2);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 5:
                                    for (int j = 0; j < 2; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 3; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 2)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 2)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 3);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 6:
                                    for (int j = 0; j < 2; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 3; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 2)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 2)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 3);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 7:
                                    for (int j = 0; j < 2; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 4; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 3)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 3)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 4);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 8:
                                    for (int j = 0; j < 2; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 4; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 3)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 3)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 4);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 9:
                                    for (int j = 0; j < 2; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 5; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 4)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 4)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 5);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 10:
                                    for (int j = 0; j < 2; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 5; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 4)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 4)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 5);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 11:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 4; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 3)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 3 && j <= 7)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 4);
                                                GridR2.Children.Add(test2);
                                            }
                                            else
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 2);
                                                Grid.SetColumn(test2, j - 8);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 12:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 4; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 3)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 3 && j <= 7)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 4);
                                                GridR2.Children.Add(test2);
                                            }
                                            else
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 2);
                                                Grid.SetColumn(test2, j - 8);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 13:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 5; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 4)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 4 && j <= 9)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 5);
                                                GridR2.Children.Add(test2);
                                            }
                                            else
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 2);
                                                Grid.SetColumn(test2, j - 10);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 14:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 5; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 4)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 4 && j <= 9)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 5);
                                                GridR2.Children.Add(test2);
                                            }
                                            else
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 2);
                                                Grid.SetColumn(test2, j - 10);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                case 15:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        RowDefinition rowDef_2 = new RowDefinition();
                                        GridR2.RowDefinitions.Add(rowDef_2);
                                        for (int z = 0; z < 5; z++)
                                        {
                                            ColumnDefinition colDef_2 = new ColumnDefinition();
                                            GridR2.ColumnDefinitions.Add(colDef_2);
                                        }
                                        GridR2.ShowGridLines = true;
                                    }
                                    for (int j = 0; j < counT; j++)
                                    {
                                        if (j <= 4)
                                        {
                                            test test2 = new test(j);
                                            Grid.SetRow(test2, 0);
                                            Grid.SetColumn(test2, j);
                                            GridR2.Children.Add(test2);
                                        }
                                        else
                                        {
                                            if (j > 4 && j <= 9)
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 1);
                                                Grid.SetColumn(test2, j - 5);
                                                GridR2.Children.Add(test2);
                                            }
                                            else
                                            {
                                                test test2 = new test(j);
                                                Grid.SetRow(test2, 2);
                                                Grid.SetColumn(test2, j - 10);
                                                GridR2.Children.Add(test2);
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    MessageBox.Show("Số lượng ảnh vượt quá phạm vi.");
                                    break;
                            }
                            //this.Close();
                            GridR1.Children.Add(txt1);
                            GridR1.Children.Add(txt2);
                            GridR1.Children.Add(txt3);
                            myGrid.Children.Add(GridR1);
                            myGrid.Children.Add(GridR2);
                            myGrid.Children.Add(button);
                            mainWindow.Content = myGrid;
                            mainWindow.Show();
                        }
                    }
                }
                //end mail
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hhehehe" + ex);
            }
            finally
            {
                con.Close();
            }
        }

        //Check window có đang mở?
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name) ? System.Windows.Application.Current.Windows.OfType<T>().Any() : System.Windows.Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        //End
        private Window mainWindow;
        
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnTrangThai_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=APBIVNDB20;Initial Catalog=KanBanSystemDB;User ID=sa;Password=Pa$$w0rd");
                con.Open();

                //========================================================================Hiển thị ở màn hình=========================================================
                string screenNameTag = "";
                string salesOrder = "";
                var clicked = sender as Button;
                if (clicked != null)
                {
                    screenNameTag = clicked.Tag.ToString();
                }
                string folder = Directory.GetCurrentDirectory().Replace("HienThiKanban", "");
                string[] readtext = File.ReadAllLines($"{folder}/Setting/DT_GridMall.txt"); //Sau khi publish lên cần sửa lại vì đã bị replace

                //Tạo 1 DataTable để tìm ra hàng có SalesOrder có trạng thái = "Dang san xuat"
                DataTable dt = new DataTable();
                dt.Columns.Add("SalesOrder");
                dt.Columns.Add("TrangThai");
                for (int i = 0; i < readtext.Length; i++)
                {
                    var values = readtext[i].Split(',');
                    dt.Rows.Add(values[0], values[1]);
                }

                if (dt.Rows.Count >= 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string trangThai = row.Field<string>("TrangThai");
                        if (trangThai == "Dang san xuat")
                        {
                            salesOrder = row.Field<string>("SalesOrder");
                            //Debug.WriteLine("values =======================" + salesOrder);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Dữ liệu trong dt.Rows đang trống.");
                }

                if (screenNameTag == "0")
                {
                    string SalesValue = "";
                    int rowIndex = 0;
                    //Update hàng "Dang san xuat" thành hoàn thành, và hàng dưới thành "Dang san xuat" bên hệ thống 1.
                    DataRow[] filteredRows = dt.Select("TrangThai = 'Dang san xuat'");
                    if (filteredRows.Length > 0)
                    {
                        rowIndex = dt.Rows.IndexOf(filteredRows[0]) + 1;
                        Console.WriteLine("Vị trí hàng: " + rowIndex);
                    }
                    else
                    {
                        Console.WriteLine("Không tìm thấy hàng có TrangThai = 'Dang san xuat'.");
                    }
                    SalesValue = dt.Rows[rowIndex]["SalesOrder"].ToString();
                    using (SqlCommand command = new SqlCommand(" Update [KanBanSystemDB].[dbo].[UpLot] Set TrangThai = 'Da hoan thanh' where TrangThai = 'Dang san xuat' ", con))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(" Update TOP(1) [KanBanSystemDB].[dbo].[UpLot] Set TrangThai = 'Dang san xuat' where SalesOrder = '" + SalesValue + "' ", con))
                    {
                        command.ExecuteNonQuery();
                    }
                    //lưu từ db vào file text
                    DataRow[] foundRows = dt.Select("SalesOrder = '" + salesOrder + "'");
                    if (foundRows.Length > 0)
                    {
                        foundRows[0]["TrangThai"] = "Da hoan thanh";
                    }
                    DataRow[] findRows = dt.Select("SalesOrder = '" + SalesValue + "'");
                    if (findRows.Length > 0)
                    {
                        findRows[0]["TrangThai"] = "Dang san xuat";
                    }
                    using (StreamWriter sw = File.CreateText($"{folder}/Setting/DT_GridMall.txt"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            sw.WriteLine(string.Join(",", row.ItemArray));
                        }
                    }

                }
                string Item = "";
                using (SqlCommand cmd = new SqlCommand("select Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && reader != null)
                        {
                            Item = "[" + reader["Material"].ToString() + "]";
                        }
                    }
                }
                DataTable dtM = new DataTable();
                var da1 = new SqlDataAdapter("SELECT mt.NameScreen, m.VitriChuyen, m.MaLK from [KanBanSystemDB].[dbo].[MayTinh] mt  JOIN [KanBanSystemDB].[dbo].[ListMasterKanban]" +
                " m on mt.CongDoan = m.VitriChuyen where mt.[NameScreen]='" + screenNameTag + "' AND " + Item + " != '' ", con);
                da1.Fill(dtM);
                //Check nếu model hiện tại đang không  có sản phẩm chạy trên chuyền, thì chuyển sang model mới để khỏi bị lãng phí công đoạn
                // =========================================================HIỂN THỊ MÀN HÌNH 1===============================================================
                string cd1 = "";
                string lotsx1 = "";
                string mamay1 = "";
                if (dtM != null && dtM.Rows.Count == 0)
                {
                    MessageBox.Show($"Không có lot để sản xuất ở công đoạn {Convert.ToInt32(screenNameTag)}!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    using (SqlCommand command = new SqlCommand("SELECT CongDoan from [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + screenNameTag + "' ", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cd1 = reader["CongDoan"].ToString();
                            }
                        }
                    }
                    // Create the application's main window
                    using (SqlCommand command = new SqlCommand(" Select ProductOrderBarcode, Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() && reader != null)
                            {
                                lotsx1 = reader["ProductOrderBarcode"].ToString();
                                mamay1 = reader["Material"].ToString();
                            }
                        }
                    }
                    
                    Screen screen1 = screens[Convert.ToInt32(screenNameTag)];
                    mainWindow = new Window();
                    mainWindow.Title = "Hiển thị kanban ảnh";
                    mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    mainWindow.Left = screen1.WorkingArea.Left;
                    mainWindow.Top = screen1.WorkingArea.Top;
                    mainWindow.Width = 1920;
                    mainWindow.Height = 1080;
                    mainWindow.WindowStyle = WindowStyle.None;
                    mainWindow.ShowInTaskbar = false;
                    mainWindow.ShowActivated = true;

                    // Create the Grid
                    Grid myGrid = new Grid();
                    myGrid.Width = 1900;
                    myGrid.Height = 1060;
                    myGrid.Background = Brushes.PowderBlue;
                    myGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    myGrid.VerticalAlignment = VerticalAlignment.Center;
                    myGrid.ShowGridLines = true;

                    // Define the Columns
                    ColumnDefinition colDef1 = new ColumnDefinition();
                    ColumnDefinition colDef2 = new ColumnDefinition();
                    ColumnDefinition colDef3 = new ColumnDefinition();
                    myGrid.ColumnDefinitions.Add(colDef1);
                    myGrid.ColumnDefinitions.Add(colDef2);
                    myGrid.ColumnDefinitions.Add(colDef3);

                    // Define the Rows
                    RowDefinition rowDef1 = new RowDefinition();
                    RowDefinition rowDef2 = new RowDefinition();
                    RowDefinition rowDef3 = new RowDefinition();
                    myGrid.RowDefinitions.Add(rowDef1);
                    myGrid.RowDefinitions.Add(rowDef2);
                    myGrid.RowDefinitions.Add(rowDef3);
                    rowDef1.Height = new GridLength(80);

                    // Add the first text cell to the Grid
                    TextBlock txt1 = new TextBlock();
                    txt1.Text = $"Công đoạn: {cd1}";
                    txt1.FontSize = 20;
                    txt1.FontWeight = FontWeights.Bold;
                    txt1.VerticalAlignment = VerticalAlignment.Center;
                    txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    Grid.SetColumn(txt1, 0);
                    Grid.SetRow(txt1, 0);

                    // Add the first text cell to the Grid
                    TextBlock txt2 = new TextBlock();
                    txt2.Text = $"Production Order: {lotsx1} ";
                    txt2.FontSize = 20;
                    txt2.FontWeight = FontWeights.Bold;
                    txt2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt2.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetColumn(txt2, 1);
                    Grid.SetRow(txt1, 0);

                    // Add the first text cell to the Grid
                    TextBlock txt3 = new TextBlock();
                    txt3.Text = $"Mã máy: {mamay1} ";
                    txt3.FontSize = 20;
                    txt3.FontWeight = FontWeights.Bold;
                    txt3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt3.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetColumn(txt3, 2);
                    Grid.SetRow(txt1, 0);
                    //End search

                    var button = new Button
                    {
                        Width = 100,
                        Height = 40,
                        Content = "Hoàn thành",
                        Background = Brushes.DeepSkyBlue,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 14,
                        Tag = $"{Convert.ToInt32(screenNameTag)}"
                    };
                    button.Click += btnTrangThai_Click;
                    Grid.SetColumn(button, 0);
                    Grid.SetRow(button, 0);

                    //Button Close
                    var button1 = new Button
                    {
                        Width = 100,
                        Height = 40,
                        Content = "Close",
                        Background = Brushes.OrangeRed,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 14
                        //Tag = $"{i}"
                    };
                    button1.Click += btnClose_Click;
                    Grid.SetColumn(button1, 2);
                    Grid.SetRow(button1, 0);

                    myGrid.Children.Add(txt1);
                    myGrid.Children.Add(txt2);
                    myGrid.Children.Add(txt3);
                    myGrid.Children.Add(button);
                    //myGrid.Children.Add(button1);
                    mainWindow.Content = myGrid;
                    mainWindow.Show();

                }
                else
                {
                    string congdoan = "";
                    string itemM = "";
                    using (SqlCommand command = new SqlCommand("SELECT CongDoan from [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + screenNameTag + "' ", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                congdoan = reader["CongDoan"].ToString();
                            }
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("select Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader != null)
                            {
                                itemM = "[" + reader["Material"].ToString() + "]";
                            }
                        }
                    }
                    //===================================================================Giống với khi hiển thị, có thể sẽ lấy đoạn trên DataInImage==================================

                    DataTable dt1 = new DataTable();
                    var da = new SqlDataAdapter("SELECT mt.NameScreen, m.VitriChuyen, m.MaLK from [KanBanSystemDB].[dbo].[MayTinh] mt  JOIN [KanBanSystemDB].[dbo].[ListMasterKanban]" +
                    " m on mt.CongDoan = m.VitriChuyen where mt.[NameScreen]='" + screenNameTag + "' AND " + itemM + " != '' ", con);
                    da.Fill(dt1);
                    string mamay = "";
                    string lotsx = "";
                    using (SqlCommand command = new SqlCommand(" Select ProductOrderBarcode, Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() && reader != null)
                            {
                                lotsx = reader["ProductOrderBarcode"].ToString();
                                mamay = reader["Material"].ToString();
                            }
                        }
                    }
                    Screen screen = screens[Convert.ToInt32(screenNameTag)];
                    // Create the application's main window
                    mainWindow = new Window();
                    mainWindow.Title = "Hiển thị kanban ảnh";
                    mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    mainWindow.Left = screen.WorkingArea.Left;
                    mainWindow.Top = screen.WorkingArea.Top;
                    mainWindow.Width = 1920;
                    mainWindow.Height = 1080;
                    mainWindow.WindowStyle = WindowStyle.None;
                    mainWindow.ShowInTaskbar = false;
                    mainWindow.ShowActivated = true;

                    // Create the Grid
                    Grid myGrid = new Grid();
                    myGrid.Width = 1900;
                    myGrid.Height = 1060;
                    myGrid.Background = Brushes.PowderBlue;
                    myGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    myGrid.VerticalAlignment = VerticalAlignment.Center;
                    myGrid.ShowGridLines = true;

                    // Define the Columns
                    ColumnDefinition colDef1 = new ColumnDefinition();
                    ColumnDefinition colDef2 = new ColumnDefinition();
                    ColumnDefinition colDef3 = new ColumnDefinition();
                    myGrid.ColumnDefinitions.Add(colDef1);
                    myGrid.ColumnDefinitions.Add(colDef2);
                    myGrid.ColumnDefinitions.Add(colDef3);
                    //colDef1.Width = 100;

                    // Define the Rows
                    RowDefinition rowDef1 = new RowDefinition();
                    RowDefinition rowDef2 = new RowDefinition();
                    RowDefinition rowDef3 = new RowDefinition();
                    //RowDefinition rowDef4 = new RowDefinition();
                    myGrid.RowDefinitions.Add(rowDef1);
                    myGrid.RowDefinitions.Add(rowDef2);
                    myGrid.RowDefinitions.Add(rowDef3);
                    rowDef1.Height = new GridLength(80);
                    //myGrid.RowDefinitions.Add(rowDef4);

                    // Add the first text cell to the Grid
                    TextBlock txt1 = new TextBlock();
                    txt1.Text = $"Công đoạn: {congdoan}";
                    txt1.FontSize = 20;
                    txt1.FontWeight = FontWeights.Bold;
                    txt1.VerticalAlignment = VerticalAlignment.Center;
                    txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    Grid.SetColumn(txt1, 0);
                    Grid.SetRow(txt1, 0);

                    // Add the first text cell to the Grid
                    TextBlock txt2 = new TextBlock();
                    txt2.Text = $"Production Order: {lotsx} ";
                    txt2.FontSize = 20;
                    txt2.FontWeight = FontWeights.Bold;
                    txt2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt2.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetColumn(txt2, 1);
                    Grid.SetRow(txt1, 0);

                    // Add the first text cell to the Grid
                    TextBlock txt3 = new TextBlock();
                    txt3.Text = $"Mã máy: {mamay} ";
                    txt3.FontSize = 20;
                    txt3.FontWeight = FontWeights.Bold;
                    txt3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt3.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetColumn(txt3, 2);
                    Grid.SetRow(txt1, 0);
                    //End search

                    var button = new Button
                    {
                        Width = 100,
                        Height = 40,
                        Content = "Hoàn thành",
                        Background = Brushes.DeepSkyBlue,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 14,
                        Tag = $"{screenNameTag}"
                    };
                    button.Click += btnTrangThai_Click;
                    Grid.SetColumn(button, 0);
                    Grid.SetRow(button, 0);

                    var button1 = new Button
                    {
                        Width = 100,
                        Height = 40,
                        Content = "Close",
                        Background = Brushes.OrangeRed,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 14
                    };
                    button1.Click += btnClose_Click;
                    Grid.SetColumn(button1, 2);
                    Grid.SetRow(button1, 0);

                    var image01 = new Image();
                    image01.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dt1.Rows[0][2]}.jpg", UriKind.RelativeOrAbsolute));

                    Grid.SetRow(image01, 1);
                    Grid.SetColumn(image01, 0);

                    var image02 = new Image();
                    if (dt1.Rows.Count > 1)
                    {
                        image02.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[1][2]}.jpg", UriKind.RelativeOrAbsolute));
                    }
                    Grid.SetRow(image02, 1);
                    Grid.SetColumn(image02, 1);

                    var image03 = new Image();
                    if (dt1.Rows.Count > 2)
                    {
                        image03.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[2][2]}.jpg", UriKind.RelativeOrAbsolute));
                    }
                    Grid.SetRow(image03, 1);
                    Grid.SetColumn(image03, 2);

                    var image04 = new Image();
                    if (dt1.Rows.Count > 3)
                    {
                        image04.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[3][2]}.jpg", UriKind.RelativeOrAbsolute));
                    }
                    Grid.SetRow(image04, 2);
                    Grid.SetColumn(image04, 0);

                    var image05 = new Image();
                    if (dt1.Rows.Count > 4)
                    {
                        image05.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[4][2]}.jpg", UriKind.RelativeOrAbsolute));
                    }
                    Grid.SetRow(image05, 2);
                    Grid.SetColumn(image05, 1);

                    var image06 = new Image();
                    if (dt1.Rows.Count > 5)
                    {
                        image06.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[5][2]}.jpg", UriKind.RelativeOrAbsolute));
                    }
                    Grid.SetRow(image06, 2);
                    Grid.SetColumn(image06, 2);

                    myGrid.Children.Add(txt1);
                    myGrid.Children.Add(txt2);
                    myGrid.Children.Add(txt3);
                    myGrid.Children.Add(button);
                    myGrid.Children.Add(image01);
                    myGrid.Children.Add(image02);
                    myGrid.Children.Add(image03);
                    myGrid.Children.Add(image04);
                    myGrid.Children.Add(image05);
                    myGrid.Children.Add(image06);
                    mainWindow.Content = myGrid;
                    mainWindow.Show();
                }
                
                
                //========================================================================End hiển  thị ảnh===========================================================
            }
            catch { }
            finally
            {
                con.Close();
            }
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mainWindow.Close();
            }
            catch
            { }
        }

        
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            DataInImage();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("Insert into [KanBanSystemDB].[dbo].[MayTinh](Name, CongDoan, NameScreen, Chuyen) values ('" + txtModel.Text + "','" + txtCountKanban.Text + "','" + KanbanCount.Text + "','" + txtMode.Text + "')", con))
                {
                    command.ExecuteNonQuery();
                }
                using(SqlDataAdapter da = new SqlDataAdapter("SELECT * from [KanBanSystemDB].[dbo].[MayTinh]", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
            }
            finally
            {
                con.Close();
            }
        }

        private void dgvSaveEditData(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedValue = ((System.Windows.Controls.TextBox)e.EditingElement).Text;
            var rowIndex = e.Row.GetIndex();
            var columnIndex = e.Column.DisplayIndex;
            dtMT.Rows[rowIndex][columnIndex] = editedValue;
        }

        private void btnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            Updategrid();
        }

        private void Updategrid()
        {
            try
            {
                con.Open();
                string CmdString1 = $"DELETE [KanBanSystemDB].[dbo].[MayTinh]";
                SqlCommand cmd1 = new SqlCommand(CmdString1, con);
                cmd1.ExecuteNonQuery();
                string Name;
                string CongDoan;
                string NameScreen;
                string Chuyen;
                for (int i = 0; i < dtMT.Rows.Count; i++)
                {
                    Name = Convert.ToString(dtMT.Rows[i][0]);
                    CongDoan = Convert.ToString(dtMT.Rows[i][1]);
                    NameScreen = Convert.ToString(dtMT.Rows[i][2]);
                    Chuyen = Convert.ToString(dtMT.Rows[i][3]);

                    string CmdString = $"INSERT INTO tbl_GHINHAPYEUCAU ([STT], [NgayNhap], [MaDon], [DuToanThang], [LoaiChiPhi] ) VALUES ( '{Name}', '{CongDoan}', N'{NameScreen}', N'{Chuyen}')";

                    using (SqlCommand cmd = new SqlCommand(CmdString, con))
                    {
                        cmd.Parameters.AddWithValue("@NgayNhap", Name);
                        cmd.Parameters.AddWithValue("@MaDon", CongDoan);
                        cmd.Parameters.AddWithValue("@DuToanThang", NameScreen);
                        cmd.Parameters.AddWithValue("@LoaiChiPhi", Chuyen);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
            finally
            {
                con.Close();
            }
            MessageBox.Show("Cập nhật file thành công!", "Thông báo");

        }

    }
   
}

/* Đoạn này là code hiển thị ở btn hoàn thành vị trí " //Hiển thị sau khi click Hoàn thành "
 *  
 *  string screenNameTag = "";
            string salesOrder="";
            var clicked = sender as Button;
            if(clicked != null)
            {
                screenNameTag = clicked.Tag.ToString();
            }
            Debug.WriteLine("Values " + screenNameTag);
            string folder = Directory.GetCurrentDirectory().Replace("HienThiKanban", "");
            string[] readtext = File.ReadAllLines($"{folder}/Setting/DT_GridMall.txt"); //Sau khi publish lên cần sửa lại vì đã bị replace

            
            //Tạo 1 DataTable để tìm ra hàng có SalesOrder có trạng thái = "Dang san xuat"
            DataTable dt = new DataTable();
            dt.Columns.Add("SalesOrder");
            dt.Columns.Add("TrangThai");
            for(int i = 1; i< readtext.Length; i++)
            {
                var values = readtext[i].Split(',');
                dt.Rows.Add(values[0], values[1]);
            }
            foreach (DataRow row in dt.Rows)
            {
                string trangThai = row.Field<string>("TrangThai");
                if (trangThai == "Dang san xuat")
                {
                    salesOrder = row.Field<string>("SalesOrder");
                    //Debug.WriteLine("vales =======================" + salesOrder);
                    break;
                }
            }
            if(screenNameTag == "0")
            {
                string SalesValue = "";
                using (SqlConnection con = new SqlConnection("Data Source=APBIVNDB20;Initial Catalog=KanBanSystemDB;User ID=sa;Password=Pa$$w0rd"))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(" Update [KanBanSystemDB].[dbo].[UpLot] Set TrangThai = 'Da hoan thanh' where TrangThai = 'Dang san xuat' ", con))
                    {
                        command.ExecuteNonQuery();
                        //MessageBox.Show("Sản phẩm đã hoàn thành!");
                    }

                    //Update hàng "Dang san xuat" thành hoàn thành, và hàng dưới thành "Dang san xuat" bên hệ thống 1.
                    DataRow[] filteredRow = dt.Select("TrangThai = 'Dang san xuat'"); //Lấy ra trạng thái dang san xuat trong datatable
                    int rowIndex = dt.Rows.IndexOf(filteredRow[0]) + 1; //Lấy ra vị trí hàng dưới hàng có TrangThai = 'Dang san xuat'
                    SalesValue = dt.Rows[rowIndex]["SalesOrder"].ToString();
                    using (SqlCommand command = new SqlCommand(" Update TOP(1) [KanBanSystemDB].[dbo].[UpLot] Set TrangThai = 'Dang san xuat' where SalesOrder = '" + SalesValue + "' ", con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                //Đang bị bug ở db không có "Dang hoan thanh". Nhưng đói quá không còn sức để sửa. Cần sửa lại ở chỗ lấy từ db
                //ra đổ vào dt, sau đó cập nhật từ dt để so sánh với file text, nếu trong dt hảng nào có trạng thái bị thay đổi, thì cập nhật vào text (file .txt)
                //So sánh giữa db và các hàng thông qua SalesOrder để cập nhật lại các hàng.
                
//DataTable dtB = new DataTable();
DataTable dtA = new DataTable();
                using(SqlDataAdapter daa = new SqlDataAdapter("Select SalesOrder, TrangThai from [KanBanSystemDB].[dbo].[UpLot] ", con))
                {
                    daa.Fill(dtA);
                }
                foreach (DataRow rowB in dt.Rows)
                {
                    DataRow rowA = dtA.AsEnumerable().FirstOrDefault(r => r.Field<string>("SalesOrder") == rowB.Field<string>("SalesOrder"));
                    if (rowA != null)
                    {
                        // Cập nhật giá trị từ dtA vào dtB
                        rowB.SetField("TrangThai", rowA.Field<string>("TrangThai"));
                        // Thêm các cột khác tương tự
                    }
                }

                //lưu từ db vào file text
                using (StreamWriter sw = File.CreateText($"{folder}HienThiAnh/HienThiAnh/bin/Debug/Setting/DT_GridMall.txt"))
                {
                //using(SqlDataAdapter da = new SqlDataAdapter("", con))
                    foreach (DataRow row in dt.Rows)
                    {
                        sw.WriteLine(string.Join(",", row.ItemArray));
                    }
                }
            }


 *  using (SqlConnection con = new SqlConnection("Data Source=APBIVNDB20;Initial Catalog=KanBanSystemDB;User ID=sa;Password=Pa$$w0rd"))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("SELECT CongDoan from [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + screenNameTag + "' ", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CongDoan = reader["CongDoan"].ToString();
                            }
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("select Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader != null)
                            {
                                item = "[" + reader["Material"].ToString() + "]";
                            }
                        }
                    }
                }
                DataTable dt1 = new DataTable();
                //dt1.Clear();
                var da = new SqlDataAdapter("SELECT mt.NameScreen, m.VitriChuyen, m.MaLK from [KanBanSystemDB].[dbo].[MayTinh] mt  JOIN [KanBanSystemDB].[dbo].[ListMasterKanban]" +
                " m on mt.CongDoan = m.VitriChuyen where mt.[NameScreen]='" + screenNameTag + "' AND " + item + " != '' ", con);
                da.Fill(dt1);
                //dataGrid.ItemsSource = dt1.DefaultView;
                Screen screen = screens[Convert.ToInt32(screenNameTag)];
                // Create the application's main window
                mainWindow = new Window();
                mainWindow.Title = "Hiển thị kanban ảnh";
                mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                mainWindow.Left = screen.WorkingArea.Left;
                mainWindow.Top = screen.WorkingArea.Top;
                mainWindow.Width = 1920;
                mainWindow.Height = 1080;
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.ShowInTaskbar = false;
                mainWindow.ShowActivated = true;

                // Create the Grid
                Grid myGrid = new Grid();
                myGrid.Width = 1900;
                myGrid.Height = 1060;
                myGrid.Background = Brushes.PowderBlue;
                myGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                myGrid.VerticalAlignment = VerticalAlignment.Center;
                myGrid.ShowGridLines = true;

                // Define the Columns
                ColumnDefinition colDef1 = new ColumnDefinition();
                ColumnDefinition colDef2 = new ColumnDefinition();
                ColumnDefinition colDef3 = new ColumnDefinition();
                myGrid.ColumnDefinitions.Add(colDef1);
                myGrid.ColumnDefinitions.Add(colDef2);
                myGrid.ColumnDefinitions.Add(colDef3);
                //colDef1.Width = 100;

                // Define the Rows
                RowDefinition rowDef1 = new RowDefinition();
                RowDefinition rowDef2 = new RowDefinition();
                RowDefinition rowDef3 = new RowDefinition();
                //RowDefinition rowDef4 = new RowDefinition();
                myGrid.RowDefinitions.Add(rowDef1);
                myGrid.RowDefinitions.Add(rowDef2);
                myGrid.RowDefinitions.Add(rowDef3);
                rowDef1.Height = new GridLength(80);
                //myGrid.RowDefinitions.Add(rowDef4);

                // Add the first text cell to the Grid
                TextBlock txt1 = new TextBlock();
                txt1.Text = $"Công đoạn: {CongDoan}";
                txt1.FontSize = 20;
                txt1.FontWeight = FontWeights.Bold;
                txt1.VerticalAlignment = VerticalAlignment.Center;
                txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                //Border border = new Border();
                //border.VerticalAlignment = VerticalAlignment.Center; // Center vertically
                //border.Child = txt1;

                Grid.SetColumn(txt1, 0);
                Grid.SetRow(txt1, 0);

                // Add the first text cell to the Grid
                TextBlock txt2 = new TextBlock();
                txt2.Text = $"Lot đang sx: {LotSx} ";
                txt2.FontSize = 20;
                txt2.FontWeight = FontWeights.Bold;
                txt2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txt2.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(txt2, 1);
                Grid.SetRow(txt1, 0);

                // Add the first text cell to the Grid
                TextBlock txt3 = new TextBlock();
                txt3.Text = $"Mã máy: {MaMay} ";
                txt3.FontSize = 20;
                txt3.FontWeight = FontWeights.Bold;
                txt3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txt3.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(txt3, 2);
                Grid.SetRow(txt1, 0);
                //End search

                var button = new Button
                {
                    Width = 100,
                    Height = 40,
                    Content = "Hoàn thành",
                    Background = Brushes.DeepSkyBlue,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                    Tag = $"{screenNameTag}"
                };
                button.Click += btnTrangThai_Click;
                Grid.SetColumn(button, 0);
                Grid.SetRow(button, 0);

                var button1 = new Button
                {
                    Width = 100,
                    Height = 40,
                    Content = "Close",
                    Background = Brushes.OrangeRed,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                    //Tag = $"{i}"
                };
                button1.Click += btnClose_Click;
                Grid.SetColumn(button1, 2);
                Grid.SetRow(button1, 0);

                var image01 = new Image();
                Debug.WriteLine($"ANH CUA DT1 LA: { dt1.Rows[0][2]}");
                image01.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dt1.Rows[0][2]}.jpg", UriKind.RelativeOrAbsolute));
                
                Grid.SetRow(image01, 1);
                Grid.SetColumn(image01, 0);

                var image02 = new Image();
                if (dt1.Rows.Count > 1)
                {
                    image02.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[1][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image02, 1);
                Grid.SetColumn(image02, 1);

                var image03 = new Image();
                if (dt1.Rows.Count > 2)
                {
                    image03.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[2][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image03, 1);
                Grid.SetColumn(image03, 2);

                var image04 = new Image();
                if (dt1.Rows.Count > 3)
                {
                    image04.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[3][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image04, 2);
                Grid.SetColumn(image04, 0);

                var image05 = new Image();
                if (dt1.Rows.Count > 4)
                {
                    image05.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[4][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image05, 2);
                Grid.SetColumn(image05, 1);

                var image06 = new Image();
                if (dt1.Rows.Count > 5)
                {
                    image06.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dt1.Rows[5][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image06, 2);
                Grid.SetColumn(image06, 2);
                //Debug.WriteLine("VALUE OF IMAGE: " + a);

                myGrid.Children.Add(txt1);
                myGrid.Children.Add(txt2);
                myGrid.Children.Add(txt3);
                myGrid.Children.Add(button);
                myGrid.Children.Add(button1);
                myGrid.Children.Add(image01);
                myGrid.Children.Add(image02);
                myGrid.Children.Add(image03);
                myGrid.Children.Add(image04);
                myGrid.Children.Add(image05);
                myGrid.Children.Add(image06);
                mainWindow.Content = myGrid;
                mainWindow.Show();

 */

/*
 * if (slChuyen == Values.Length)
            {
                DataTable dtb = new DataTable();
                if (dtb.Rows.Count == 0)
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter("SELECT SalesOrder, TrangThai FROM [KanBanSystemDB].[dbo].[UpLot] ORDER BY CASE TrangThai WHEN 'Da hoan thanh' THEN 1 WHEN 'Dang san xuat' THEN 2 WHEN '' THEN 3 ELSE 4 END;", con))
                    {
                        sda.Fill(dtb);
                    }
                }
                DataRow[] filteredRow = dtb.Select("TrangThai = 'Dang san xuat'");
                int rowIndex = dtb.Rows.IndexOf(filteredRow[0]) + 1; //Lấy ra vị trí hàng dưới hàng có TrangThai = 'Dang san xuat'
                string SalesValue = dtb.Rows[rowIndex]["SalesOrder"].ToString();
            }

            using (SqlCommand cmd = new SqlCommand("select Material from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && reader != null)
                    {
                        item = "[" + reader["Material"].ToString() + "]";
                    }
                }
            }

            DataTable dtm = new DataTable();
            int adv = Convert.ToInt32(clicked.Tag);
            var da = new SqlDataAdapter("SELECT mt.NameScreen, m.VitriChuyen, m.MaLK from [KanBanSystemDB].[dbo].[MayTinh] mt  JOIN [KanBanSystemDB].[dbo].[ListMasterKanban]" +
            " m on mt.CongDoan = m.VitriChuyen where mt.[NameScreen]='" + screenNameTag + "' AND " + item + " != '' ", con);
            da.Fill(dtm);
            Screen screen = screens[adv];

            //Check nếu model hiện tại đang không  có sản phẩm chạy trên chuyền, thì chuyển sang model mới để khỏi bị lãng phí công đoạn
            // =========================================================HIỂN THỊ MÀN HÌNH 1===============================================================

            if (dtm != null && dtm.Rows.Count == 0)
            {
                //dtm.Clear();

                string folder = Directory.GetCurrentDirectory().Replace("KanbanSystemShow\\KanbanSystemShow\\bin\\Debug", "");
                string[] readtext = File.ReadAllLines($"{folder}HienThiAnh/HienThiAnh/bin/Debug/Setting/DT_GridMall.txt");
                DataTable dt3 = new DataTable();
                //dt3.Clear();
                dt3.Columns.Add("SalesOrder");
                dt3.Columns.Add("TrangThai");
                foreach (string value in readtext)
                {
                    var values = value.Split(',');
                    dt3.Rows.Add(values[0], values[1]); // Thêm giá trị vào DataTable
                }
                string nameModel = "";
                string SalesValue = "";
                DataRow[] filteredRow = dt3.Select("TrangThai = 'Dang san xuat'"); //Lấy ra trạng thái dang san xuat trong datatable
                for (int z = 0; z < dt3.Rows.Count; z++)
                {
                    int rowIndex = dt3.Rows.IndexOf(filteredRow[0]) + z; //Lấy ra vị trí hàng dưới hàng có TrangThai = 'Dang san xuat'
                    SalesValue = dt3.Rows[rowIndex]["SalesOrder"].ToString();
                    using (SqlCommand command = new SqlCommand("SELECT Material from [KanBanSystemDB].[dbo].[UpLot] where SalesOrder = '" + SalesValue + "' ", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nameModel = "[" + reader["Material"].ToString() + "]";
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand("SELECT CongDoan from [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + screenNameTag + "' ", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CongDoan = reader["CongDoan"].ToString();
                            }
                        }
                    }
                    var dataA = new SqlDataAdapter("SELECT mt.NameScreen, m.VitriChuyen, m.MaLK from [KanBanSystemDB].[dbo].[MayTinh] mt  JOIN [KanBanSystemDB].[dbo].[ListMasterKanban]" +
                    " m on mt.CongDoan = m.VitriChuyen where mt.[NameScreen]='" + screenNameTag + "' AND " + nameModel + " != '' ", con);
                    dataA.Fill(dtm);
                    if (dtm.Rows.Count > 0)
                    {
                        break;
                    }
                    //dataGrid.ItemsSource = dtm.DefaultView;
                    //end
                }

                //Open check Lot , mã máy
                //MaMay = SalesValue;
                using (SqlCommand command = new SqlCommand(" Select ProductOrderBarcode from [KanBanSystemDB].[dbo].[UpLot] where SalesOrder = '" + MaMay + "'", con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read() && reader != null)
                        {
                            LotSx = reader["ProductOrderBarcode"].ToString();
                        }
                    }
                }
                //End check

                //Đoạn này thêm hiển thị ảnh của model tiếp theo để sản xuất nếu model không có ảnh sản xuất đc ở TD này
                mainWindow = new Window();
                mainWindow.Title = "Hiển thị kanban ảnh";
                mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                mainWindow.Left = screen.WorkingArea.Left;
                mainWindow.Top = screen.WorkingArea.Top;
                mainWindow.Width = 1920;
                mainWindow.Height = 1080;
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.ShowInTaskbar = false;
                mainWindow.ShowActivated = true;

                Grid Grid = new Grid();
                Grid.Width = 1900;
                Grid.Height = 1060;
                Grid.Background = Brushes.PowderBlue;
                Grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.VerticalAlignment = VerticalAlignment.Center;
                Grid.ShowGridLines = true;

                ColumnDefinition colDefinition1 = new ColumnDefinition();
                ColumnDefinition colDefinition2 = new ColumnDefinition();
                ColumnDefinition colDefinition3 = new ColumnDefinition();
                Grid.ColumnDefinitions.Add(colDefinition1);
                Grid.ColumnDefinitions.Add(colDefinition2);
                Grid.ColumnDefinitions.Add(colDefinition3);

                RowDefinition rowDefinition1 = new RowDefinition();
                RowDefinition rowDefinition2 = new RowDefinition();
                RowDefinition rowDefinition3 = new RowDefinition();
                Grid.RowDefinitions.Add(rowDefinition1);
                Grid.RowDefinitions.Add(rowDefinition2);
                Grid.RowDefinitions.Add(rowDefinition3);
                rowDefinition1.Height = new GridLength(80);

                TextBlock text1 = new TextBlock();
                text1.Text = $"Công đoạn: {CongDoan}";
                text1.FontSize = 20;
                text1.FontWeight = FontWeights.Bold;
                text1.VerticalAlignment = VerticalAlignment.Center;
                text1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                Grid.SetColumn(text1, 0);
                Grid.SetRow(text1, 0);

                TextBlock text2 = new TextBlock();
                text2.Text = $"Lot đang sx: {LotSx} ";
                text2.FontSize = 20;
                text2.FontWeight = FontWeights.Bold;
                text2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                text2.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(text2, 1);
                Grid.SetRow(text1, 0);

                TextBlock text3 = new TextBlock();
                text3.Text = $"Mã máy: {MaMay} ";
                text3.FontSize = 20;
                text3.FontWeight = FontWeights.Bold;
                text3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                text3.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(text3, 2);
                Grid.SetRow(text1, 0);

                var btn = new Button
                {
                    Width = 100,
                    Height = 40,
                    Content = "Hoàn thành",
                    Background = Brushes.DeepSkyBlue,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                    Tag = $"{screenNameTag}"
                };
                btn.Click += btnTrangThai_Click;
                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);

                var btn1 = new Button
                {
                    Width = 100,
                    Height = 40,
                    Content = "Close",
                    Background = Brushes.OrangeRed,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                };
                btn1.Click += btnClose_Click;
                Grid.SetColumn(btn1, 2);
                Grid.SetRow(btn1, 0);

                var img01 = new Image();
                var img02 = new Image();
                var img03 = new Image();
                var img04 = new Image();
                var img05 = new Image();
                var img06 = new Image();

                img01.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtm.Rows[0][2]}.jpg", UriKind.RelativeOrAbsolute));
                Grid.SetRow(img01, 1);
                Grid.SetColumn(img01, 0);

                if (dtm.Rows.Count > 1)
                {
                    img02.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtm.Rows[1][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(img02, 1);
                Grid.SetColumn(img02, 1);

                if (dtm.Rows.Count > 2)
                {
                    img03.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtm.Rows[2][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(img03, 1);
                Grid.SetColumn(img03, 2);

                if (dtm.Rows.Count > 3)
                {
                    img04.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtm.Rows[3][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(img04, 2);
                Grid.SetColumn(img04, 0);

                if (dtm.Rows.Count > 4)
                {
                    img05.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtm.Rows[4][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(img05, 2);
                Grid.SetColumn(img05, 1);

                if (dtm.Rows.Count > 5)
                {
                    img06.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtm.Rows[5][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(img06, 2);
                Grid.SetColumn(img06, 2);

                Grid.Children.Add(text1);
                Grid.Children.Add(text2);
                Grid.Children.Add(text3);
                Grid.Children.Add(btn);
                Grid.Children.Add(btn1);
                Grid.Children.Add(img01);
                Grid.Children.Add(img02);
                Grid.Children.Add(img03);
                Grid.Children.Add(img04);
                Grid.Children.Add(img05);
                Grid.Children.Add(img06);

                mainWindow.Content = Grid;
                mainWindow.Show();
                //continue;
            }

            //============================================================HIỂN THỊ Ở MÀN HÌNH 2============================================================= 
            else
            {
                using (SqlCommand command = new SqlCommand(" Select ProductOrderBarcode, SalesOrder from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read() && reader != null)
                        {
                            LotSx = reader["ProductOrderBarcode"].ToString();
MaMay = reader["SalesOrder"].ToString();
                        }
                    }
                }
                using (SqlCommand command = new SqlCommand("SELECT CongDoan from [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + screenNameTag + "' ", con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CongDoan = reader["CongDoan"].ToString();
                        }
                    }
                }


                // Create the application's main window
                using (SqlCommand command = new SqlCommand(" Select ProductOrderBarcode, SalesOrder from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read() && reader != null)
                        {
                            LotSx = reader["ProductOrderBarcode"].ToString();
MaMay = reader["SalesOrder"].ToString();
                        }
                    }
                }
                mainWindow = new Window();
mainWindow.Title = "Hiển thị kanban ảnh";
                mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                mainWindow.Left = screen.WorkingArea.Left;
                mainWindow.Top = screen.WorkingArea.Top;
                mainWindow.Width = 1920;
                mainWindow.Height = 1080;
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.ShowInTaskbar = false;
                mainWindow.ShowActivated = true;

                // Create the Grid
                Grid myGrid = new Grid();
myGrid.Width = 1900;
                myGrid.Height = 1060;
                myGrid.Background = Brushes.PowderBlue;
                myGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                myGrid.VerticalAlignment = VerticalAlignment.Center;
                myGrid.ShowGridLines = true;

                // Define the Columns
                ColumnDefinition colDef1 = new ColumnDefinition();
ColumnDefinition colDef2 = new ColumnDefinition();
ColumnDefinition colDef3 = new ColumnDefinition();
myGrid.ColumnDefinitions.Add(colDef1);
                myGrid.ColumnDefinitions.Add(colDef2);
                myGrid.ColumnDefinitions.Add(colDef3);

                // Define the Rows
                RowDefinition rowDef1 = new RowDefinition();
RowDefinition rowDef2 = new RowDefinition();
RowDefinition rowDef3 = new RowDefinition();
//RowDefinition rowDef4 = new RowDefinition();
myGrid.RowDefinitions.Add(rowDef1);
                myGrid.RowDefinitions.Add(rowDef2);
                myGrid.RowDefinitions.Add(rowDef3);
                rowDef1.Height = new GridLength(80);
//myGrid.RowDefinitions.Add(rowDef4);

// Add the first text cell to the Grid
TextBlock txt1 = new TextBlock();
txt1.Text = $"Công đoạn: {CongDoan}";
                txt1.FontSize = 20;
                txt1.FontWeight = FontWeights.Bold;
                txt1.VerticalAlignment = VerticalAlignment.Center;
                txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                Grid.SetColumn(txt1, 0);
                Grid.SetRow(txt1, 0);

                // Add the first text cell to the Grid
                TextBlock txt2 = new TextBlock();
txt2.Text = $"Lot đang sx: {LotSx} ";
                txt2.FontSize = 20;
                txt2.FontWeight = FontWeights.Bold;
                txt2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txt2.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(txt2, 1);
                Grid.SetRow(txt1, 0);

                // Add the first text cell to the Grid
                TextBlock txt3 = new TextBlock();
txt3.Text = $"Mã máy: {MaMay} ";
                txt3.FontSize = 20;
                txt3.FontWeight = FontWeights.Bold;
                txt3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txt3.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(txt3, 2);
                Grid.SetRow(txt1, 0);
                //End search

                var button = new Button
                {
                    Width = 100,
                    Height = 40,
                    Content = "Hoàn thành",
                    Background = Brushes.DeepSkyBlue,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                    Tag = $"{screenNameTag}"
                };
button.Click += btnTrangThai_Click;
                Grid.SetColumn(button, 0);
                Grid.SetRow(button, 0);

                //Button Close
                var button1 = new Button
                {
                    Width = 100,
                    Height = 40,
                    Content = "Close",
                    Background = Brushes.OrangeRed,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                    //Tag = $"{i}"
                };
button1.Click += btnClose_Click;
                Grid.SetColumn(button1, 2);
                Grid.SetRow(button1, 0);

                var image01 = new Image();
var image02 = new Image();
var image03 = new Image();
var image04 = new Image();
var image05 = new Image();
var image06 = new Image();

image01.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtm.Rows[0][2]}.jpg", UriKind.RelativeOrAbsolute));
                Grid.SetRow(image01, 1);
                Grid.SetColumn(image01, 0);

                //var image02 = new Image();
                if (dtm.Rows.Count > 1)
                {
                    image02.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dtm.Rows[1][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image02, 1);
                Grid.SetColumn(image02, 1);

                //var image03 = new Image();
                if (dtm.Rows.Count > 2)
                {
                    image03.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dtm.Rows[2][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image03, 1);
                Grid.SetColumn(image03, 2);

                //var image04 = new Image();
                if (dtm.Rows.Count > 3)
                {
                    image04.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dtm.Rows[3][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image04, 2);
                Grid.SetColumn(image04, 0);

                //var image05 = new Image();
                if (dtm.Rows.Count > 4)
                {
                    image05.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dtm.Rows[4][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image05, 2);
                Grid.SetColumn(image05, 1);

                //var image06 = new Image();
                if (dtm.Rows.Count > 5)
                {
                    image06.Source = new BitmapImage(new Uri($"{path}/ImagesSave//5VJ21A09005/{dtm.Rows[5][2]}.jpg", UriKind.RelativeOrAbsolute));
                }
                Grid.SetRow(image06, 2);
                Grid.SetColumn(image06, 2);
                //Debug.WriteLine("VALUE OF IMAGE: " + a);

                myGrid.Children.Add(txt1);
                myGrid.Children.Add(txt2);
                myGrid.Children.Add(txt3);
                myGrid.Children.Add(button);
                myGrid.Children.Add(button1);
                myGrid.Children.Add(image01);
                myGrid.Children.Add(image02);
                myGrid.Children.Add(image03);
                myGrid.Children.Add(image04);
                myGrid.Children.Add(image05);
                myGrid.Children.Add(image06);
                mainWindow.Content = myGrid;
                mainWindow.Show();
                //openWindows.Add(mainWindow);

            }
 */

/* Đoạn này để tìm ra vị trí của ảnh tiếp theo
 *  using(SqlConnection con = new SqlConnection("Data Source=APBIVNDB20;Initial Catalog=KanBanSystemDB;User ID=sa;Password=Pa$$w0rd "))
        {
            con.Open();
            string screenNameTag = "";
            var clicked = sender as Button;
            if (clicked != null)
            {
                screenNameTag = clicked.Tag.ToString();
            }
            //Debug.WriteLine("Values " + screenNameTag);
            string NameScreen = Environment.MachineName;
            string NameFile = "";
            string checkOrder = "";
            string checkvalue = "";
            string checkModel = "";
            foreach (DataRow value in dt3.Rows)
            {
                checkOrder = value["SalesOrder"].ToString();
                using (SqlCommand command = new SqlCommand(" select Material FROM [KanBanSystemDB].[dbo].[UpLot] where SalesOrder = '"+ checkOrder + "'", con))
                {
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            checkModel = reader["Material"].ToString();
                        }
                    }
                }
                string query = $"SELECT mt.NameScreen, m.VitriChuyen, m.MaLK, m.[{checkModel}]  from [KanBanSystemDB].[dbo].[MayTinh] mt " +
                        "JOIN [KanBanSystemDB].[dbo].[ListMasterKanban] m on mt.CongDoan = m.VitriChuyen where mt.[NameScreen]='" + screenNameTag + "'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            checkvalue = reader[$"{checkModel}"].ToString();
                        }
                    }
                }
                if (checkvalue != "")
                    MessageBox.Show("Có sản phẩm!");
                    dt3.Rows.Remove(value);
                    break;
            }

            using (SqlCommand command = new SqlCommand("SELECT Material FROM [KanBanSystemDB].[dbo].[UpLot] where SalesOrder = '" + checkOrder + "'", con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NameFile = reader["Material"].ToString();
                    }
                }
            }

            //string SalesValue1 = "";
            ////string SalesValue2 = "";
            //DataRow[] filteredRow1 = dt3.Select("TrangThai = 'Dang san xuat'"); //Lấy ra trạng thái dang san xuat trong datatable
            //for (int z = 0; z < dt3.Rows.Count; z++)
            //{
            //    int rowIndex = dt3.Rows.IndexOf(filteredRow1[0]) + z; //Lấy ra vị trí hàng dưới hàng có TrangThai = 'Dang san xuat'
            //    SalesValue1 = dt3.Rows[rowIndex]["SalesOrder"].ToString();

            //}

            //note 

            string[] Values = File.ReadAllLines($"{folderPath}/FileSaveData/{NameFile}.txt");
            using (SqlDataAdapter sda = new SqlDataAdapter("  SELECT Name, CongDoan FROM [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + screenNameTag + "'", con))
            {

                DataSet dataSet = new DataSet();
                sda.Fill(dataSet);
                string valueClick = string.Join(",", dataSet.Tables[0].Rows[0].ItemArray);
                bool foundValueClick = false;
                if (Values.Length == 0)
                {
                    using (StreamWriter writer = new StreamWriter($"{folderPath}/FileSaveData/{NameFile}.txt", true))
                    {
                        foreach (DataRow row1 in dataSet.Tables[0].Rows)
                        {
                            writer.WriteLine(string.Join(",", row1.ItemArray));
                            break;
                        }
                    }
                    MessageBox.Show("Đã hoàn thành!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    //write code show next image

                }
                //Debug.WriteLine("VALUES " + valueClick);
                else
                {
                    foreach (string value in Values)
                    {
                        if (value == valueClick)
                        {
                            MessageBox.Show("Đã hoàn thành lot này rồi!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            foundValueClick = true;
                            break;
                        }
                    }
                    if (!foundValueClick)
                    {
                        using (StreamWriter writer = new StreamWriter($"{folderPath}/FileSaveData/{NameFile}.txt", true))
                        {
                            foreach (DataRow row1 in dataSet.Tables[0].Rows)
                            {
                                writer.WriteLine(string.Join(",", row1.ItemArray));
                                break;
                            }
                        }
                        MessageBox.Show("Đã hoàn thành!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    }
                }
                //Hoặc viết ở đây. Sau khi hoàn thành thì hiển thị lại view của window

            }
            //write code here
            //Check 
            int slChuyen = 0;
            using (SqlCommand command = new SqlCommand($" select Count(distinct VitriChuyen) as SLChuyen FROM [KanBanSystemDB].[dbo].[ListMasterKanban] where [{NameFile}] <> ''", con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        slChuyen = (int)reader["SLChuyen"];
                    }
                }
            }
            if (slChuyen == Values.Length)
            {
                DataTable dtb = new DataTable();
                if (dtb.Rows.Count == 0)
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter("SELECT SalesOrder, TrangThai FROM [KanBanSystemDB].[dbo].[UpLot] ORDER BY CASE TrangThai WHEN 'Da hoan thanh' THEN 1 WHEN 'Dang san xuat' THEN 2 WHEN '' THEN 3 ELSE 4 END;", con))
                    {
                        sda.Fill(dtb);
                        //using (StreamWriter sw = File.CreateText($"{folderPath.Replace("KanbanSystemShow\\KanbanSystemShow\\bin\\Debug", "")}HienThiAnh/HienThiAnh/bin/Debug/Setting/DT_GridMall.txt"))
                        //{
                        //    foreach (DataRow row in dtb.Rows)
                        //    {
                        //        sw.WriteLine(string.Join(",", row.ItemArray));
                        //    }
                        //}
                    }
                }
                DataRow[] filteredRow = dtb.Select("TrangThai = 'Dang san xuat'");
                using (SqlCommand command = new SqlCommand(" Update [KanBanSystemDB].[dbo].[UpLot] Set TrangThai = 'Da hoan thanh' where TrangThai = 'Dang san xuat' ", con))
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Sản phẩm đã hoàn thành!");
                }
                int rowIndex = dtb.Rows.IndexOf(filteredRow[0]) + 1; //Lấy ra vị trí hàng dưới hàng có TrangThai = 'Dang san xuat'
                string SalesValue = dtb.Rows[rowIndex]["SalesOrder"].ToString();
                using (SqlCommand command = new SqlCommand(" Update TOP(1) [KanBanSystemDB].[dbo].[UpLot] Set TrangThai = 'Dang san xuat' where SalesOrder = '" + SalesValue + "' ", con))
                {
                    command.ExecuteNonQuery();
                }
                dtb.Clear();
                using (SqlDataAdapter xda = new SqlDataAdapter("SELECT SalesOrder, TrangThai FROM [KanBanSystemDB].[dbo].[UpLot] ORDER BY CASE TrangThai WHEN 'Da hoan thanh' THEN 1 WHEN 'Dang san xuat' THEN 2 WHEN '' THEN 3 ELSE 4 END;", con))
                {
                    xda.Fill(dtb);
                    using (StreamWriter sw = File.CreateText($"{folderPath.Replace("KanbanSystemShow\\KanbanSystemShow\\bin\\Debug", "")}HienThiAnh/HienThiAnh/bin/Debug/Setting/DT_GridMall.txt"))
                    {
                        foreach (DataRow row in dtb.Rows)
                        {
                            sw.WriteLine(string.Join(",", row.ItemArray));
                        }
                    }
                }
            }

            //Write code here


            //End code
        }

 */

/*
 * 
//Đang bị bug ở db không có "Dang hoan thanh". Nhưng đói quá không còn sức để sửa. Cần sửa lại ở chỗ lấy từ db
//ra đổ vào dt, sau đó cập nhật từ dt để so sánh với file text, nếu trong dt hảng nào có trạng thái bị thay đổi, thì cập nhật vào text (file .txt)
//So sánh giữa db và các hàng thông qua SalesOrder để cập nhật lại các hàng.

//DataTable dtB = new DataTable(); //Chả hiểu viết để làm gì luôn
//DataTable dtA = new DataTable();
//using (SqlDataAdapter daa = new SqlDataAdapter("Select SalesOrder, TrangThai from [KanBanSystemDB].[dbo].[UpLot] ", con))
//{
//    daa.Fill(dtA);
//}
//foreach (DataRow rowB in dt.Rows)
//{
//    DataRow rowA = dtA.AsEnumerable().FirstOrDefault(r => r.Field<string>("SalesOrder") == rowB.Field<string>("SalesOrder"));
//    if (rowA != null)
//    {
//        // Cập nhật giá trị từ dtA vào dtB
//        rowB.SetField("TrangThai", rowA.Field<string>("TrangThai"));

//    }
//}

*/

/*
 * //using(SqlConnection con = new SqlConnection("Data Source=APBIVNDB20;Initial Catalog=KanBanSystemDB;User ID=sa;Password=Pa$$w0rd "))
            //{
            //    con.Open();
            //    using(SqlCommand command = new SqlCommand(" Select ProductOrderBarcode, SalesOrder from [KanBanSystemDB].[dbo].[UpLot] where TrangThai = 'Dang san xuat'", con))
            //    {
            //        using(SqlDataReader reader = command.ExecuteReader())
            //        {
            //            while(reader.Read() && reader != null)
            //            {
            //                LotSx = reader["ProductOrderBarcode"].ToString();
            //                MaMay = reader["SalesOrder"].ToString();
            //            }
            //        }
            //    }
            //}

    
            //dt3.Clear();
            //string folder = Directory.GetCurrentDirectory().Replace("KanbanSystemShow\\KanbanSystemShow\\bin\\Debug", "");
            //string[] readtext = File.ReadAllLines($"{folder}HienThiAnh/HienThiAnh/bin/Debug/Setting/DT_GridMall.txt");
            //if(dt3.Columns.Contains("SalesOrder") || dt3.Columns.Contains("TrangThai"))
            //{
            //    foreach (string value in readtext)
            //    {
            //        var values = value.Split(',');
            //        dt3.Rows.Add(values[0], values[1]);
            //    }
            //}
            //else
            //{
            //    dt3.Columns.Add("SalesOrder");
            //    dt3.Columns.Add("TrangThai");
            //    foreach (string value in readtext)
            //    {
            //        var values = value.Split(',');
            //        dt3.Rows.Add(values[0], values[1]);
            //    }
            //}
 */

/* ĐOẠN NÀY RẤT QUAN TRỌNG
 * dtM.Clear();

                        string folder = Directory.GetCurrentDirectory().Replace("KanbanSystemShow\\KanbanSystemShow\\bin\\Debug", "");
                        string[] readtext = File.ReadAllLines($"{folder}HienThiAnh/HienThiAnh/bin/Debug/Setting/DT_GridMall.txt");
                        DataTable dt3 = new DataTable();
                        //dt3.Clear();
                        dt3.Columns.Add("SalesOrder");
                        dt3.Columns.Add("TrangThai");
                        foreach (string value in readtext)
                        {
                            var values = value.Split(',');
                            dt3.Rows.Add(values[0], values[1]); // Thêm giá trị vào DataTable
                        }
                        string nameModel = "";
                        string SalesValue = "";
                        DataRow[] filteredRow = dt3.Select("TrangThai = 'Dang san xuat'"); //Lấy ra trạng thái dang san xuat trong datatable
                        for(int z = 0; z < dt3.Rows.Count; z++)
                        {
                            int rowIndex = dt3.Rows.IndexOf(filteredRow[0]) + z; //Lấy ra vị trí hàng dưới hàng có TrangThai = 'Dang san xuat'
                            SalesValue = dt3.Rows[rowIndex]["SalesOrder"].ToString();
                            using (SqlCommand command = new SqlCommand("SELECT Material from [KanBanSystemDB].[dbo].[UpLot] where SalesOrder = '" + SalesValue + "' ", con))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        nameModel = "[" + reader["Material"].ToString() + "]";
                                    }
                                }
                            }

                            using (SqlCommand command = new SqlCommand("SELECT CongDoan from [KanBanSystemDB].[dbo].[MayTinh] where NameScreen = '" + i + "' ", con))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        CongDoan = reader["CongDoan"].ToString();
                                    }
                                }
                            }
                            var dataA = new SqlDataAdapter("SELECT mt.NameScreen, m.VitriChuyen, m.MaLK from [KanBanSystemDB].[dbo].[MayTinh] mt  JOIN [KanBanSystemDB].[dbo].[ListMasterKanban]" +
                            " m on mt.CongDoan = m.VitriChuyen where mt.[NameScreen]='" + i + "' AND " + nameModel + " != '' ", con);
                            dataA.Fill(dtM);
                            if (dtM.Rows.Count > 0)
                            {
                                break;
                            }
                            //dataGrid.ItemsSource = dtM.DefaultView;
                            //end
                        }

                        //Open check Lot , mã máy
                        MaMay = SalesValue;
                        using (SqlCommand command = new SqlCommand(" Select ProductOrderBarcode from [KanBanSystemDB].[dbo].[UpLot] where SalesOrder = '" + SalesValue + "'", con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read() && reader != null)
                                {
                                    LotSx = reader["ProductOrderBarcode"].ToString();
                                }
                            }
                        }
                        //End check

                        //Đoạn này thêm hiển thị ảnh của model tiếp theo để sản xuất nếu model không có ảnh sản xuất đc ở TD này
                        mainWindow = new Window();
                        mainWindow.Title = "Hiển thị kanban ảnh";
                        mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                        mainWindow.Left = screen.WorkingArea.Left;
                        mainWindow.Top = screen.WorkingArea.Top;
                        mainWindow.Width = 1920;
                        mainWindow.Height = 1080;
                        mainWindow.WindowStyle = WindowStyle.None;
                        mainWindow.ShowInTaskbar = false;
                        mainWindow.ShowActivated = true;

                        Grid Grid = new Grid();
                        Grid.Width = 1900;
                        Grid.Height = 1060;
                        Grid.Background = Brushes.PowderBlue;
                        Grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        Grid.VerticalAlignment = VerticalAlignment.Center;
                        Grid.ShowGridLines = true;

                        ColumnDefinition colDefinition1 = new ColumnDefinition();
                        ColumnDefinition colDefinition2 = new ColumnDefinition();
                        ColumnDefinition colDefinition3 = new ColumnDefinition();
                        Grid.ColumnDefinitions.Add(colDefinition1);
                        Grid.ColumnDefinitions.Add(colDefinition2);
                        Grid.ColumnDefinitions.Add(colDefinition3);

                        RowDefinition rowDefinition1 = new RowDefinition();
                        RowDefinition rowDefinition2 = new RowDefinition();
                        RowDefinition rowDefinition3 = new RowDefinition();
                        Grid.RowDefinitions.Add(rowDefinition1);
                        Grid.RowDefinitions.Add(rowDefinition2);
                        Grid.RowDefinitions.Add(rowDefinition3);
                        rowDefinition1.Height = new GridLength(80);

                        TextBlock text1 = new TextBlock();
                        text1.Text = $"Công đoạn: {CongDoan}";
                        text1.FontSize = 20;
                        text1.FontWeight = FontWeights.Bold;
                        text1.VerticalAlignment = VerticalAlignment.Center;
                        text1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                        Grid.SetColumn(text1, 0);
                        Grid.SetRow(text1, 0);

                        TextBlock text2 = new TextBlock();
                        text2.Text = $"Lot đang sx: {LotSx} ";
                        text2.FontSize = 20;
                        text2.FontWeight = FontWeights.Bold;
                        text2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        text2.VerticalAlignment = VerticalAlignment.Center;
                        Grid.SetColumn(text2, 1);
                        Grid.SetRow(text1, 0);

                        TextBlock text3 = new TextBlock();
                        text3.Text = $"Mã máy: {MaMay} ";
                        text3.FontSize = 20;
                        text3.FontWeight = FontWeights.Bold;
                        text3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        text3.VerticalAlignment = VerticalAlignment.Center;
                        Grid.SetColumn(text3, 2);
                        Grid.SetRow(text1, 0);

                        var btn = new Button
                        {
                            Width = 100,
                            Height = 40,
                            Content = "Hoàn thành",
                            Background = Brushes.DeepSkyBlue,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 14,
                            Tag = $"{i}"
                        };
                        btn.Click += btnTrangThai_Click;
                        Grid.SetColumn(btn, 0);
                        Grid.SetRow(btn, 0);

                        var btn1 = new Button
                        {
                            Width = 100,
                            Height = 40,
                            Content = "Close",
                            Background = Brushes.OrangeRed,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 14
                        };
                        btn1.Click += btnClose_Click;
                        Grid.SetColumn(btn1, 2);
                        Grid.SetRow(btn1, 0);

                        var img01 = new Image();
                        var img02 = new Image();
                        var img03 = new Image();
                        var img04 = new Image();
                        var img05 = new Image();
                        var img06 = new Image();

                        img01.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtM.Rows[0][2]}.jpg", UriKind.RelativeOrAbsolute));
                        Grid.SetRow(img01, 1);
                        Grid.SetColumn(img01, 0);

                        if (dtM.Rows.Count > 1)
                        {
                            img02.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtM.Rows[1][2]}.jpg", UriKind.RelativeOrAbsolute));
                        }
                        Grid.SetRow(img02, 1);
                        Grid.SetColumn(img02, 1);

                        if (dtM.Rows.Count > 2)
                        {
                            img03.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtM.Rows[2][2]}.jpg", UriKind.RelativeOrAbsolute));
                        }
                        Grid.SetRow(img03, 1);
                        Grid.SetColumn(img03, 2);

                        if (dtM.Rows.Count > 3)
                        {
                            img04.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtM.Rows[3][2]}.jpg", UriKind.RelativeOrAbsolute));
                        }
                        Grid.SetRow(img04, 2);
                        Grid.SetColumn(img04, 0);

                        if (dtM.Rows.Count > 4)
                        {
                            img05.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtM.Rows[4][2]}.jpg", UriKind.RelativeOrAbsolute));
                        }
                        Grid.SetRow(img05, 2);
                        Grid.SetColumn(img05, 1);

                        if (dtM.Rows.Count > 5)
                        {
                            img06.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{dtM.Rows[5][2]}.jpg", UriKind.RelativeOrAbsolute));
                        }
                        Grid.SetRow(img06, 2);
                        Grid.SetColumn(img06, 2);

                        Grid.Children.Add(text1);
                        Grid.Children.Add(text2);
                        Grid.Children.Add(text3);
                        Grid.Children.Add(btn);
                        Grid.Children.Add(btn1);
                        Grid.Children.Add(img01);
                        Grid.Children.Add(img02);
                        Grid.Children.Add(img03);
                        Grid.Children.Add(img04);
                        Grid.Children.Add(img05);
                        Grid.Children.Add(img06);

                        mainWindow.Content = Grid;
                        mainWindow.Show();
 */
