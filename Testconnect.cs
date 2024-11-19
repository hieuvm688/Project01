using System;
using System.Collections.Generic;
using System.Management;
using System.Windows;

namespace MonitorUSBChecker
{
    public partial class MainWindow : Window
    {
        // Lưu thông tin cổng USB đúng của mỗi màn hình
        Dictionary<string, string> monitorToUSBPort = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
//Rxf9BeNq6p0kFirRAwRujA==
            // Đặt thông tin kết nối đúng của các màn hình và cổng USB
            monitorToUSBPort["Màn hình 1"] = "USB\\VID_1234&PID_5678";  // Cổng USB 1
            monitorToUSBPort["Màn hình 2"] = "USB\\VID_2345&PID_6789";  // Cổng USB 2
            monitorToUSBPort["Màn hình 3"] = "USB\\VID_3456&PID_7890";  // Cổng USB 3
            monitorToUSBPort["Màn hình 4"] = "USB\\VID_4567&PID_8901";  // Cổng USB 4
            monitorToUSBPort["Màn hình 5"] = "USB\\VID_5678&PID_9012";  // Cổng USB 5
            monitorToUSBPort["Màn hình 6"] = "USB\\VID_6789&PID_0123";  // Cổng USB 6

            // Bắt đầu giám sát sự kiện kết nối và ngắt kết nối USB
            StartMonitoringUSBEvents();
        }

        private void StartMonitoringUSBEvents()
        {
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += new EventArrivedEventHandler(USBChangedEvent);
            watcher.Start();
        }

        private void USBChangedEvent(object sender, EventArrivedEventArgs e)
        {
            string eventType = e.NewEvent.ClassPath.ClassName;

            if (eventType == "__InstanceCreationEvent")
            {
                // Khi có thiết bị USB được kết nối
                CheckUSBConnection();
            }
            else if (eventType == "__InstanceDeletionEvent")
            {
                // Khi có thiết bị USB bị ngắt kết nối
                CheckUSBConnection();
            }
        }

        private void CheckUSBConnection()
        {
            // Duyệt qua tất cả các thiết bị USB hiện đang kết nối
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub");
            var connectedDevices = searcher.Get();

            // Kiểm tra từng thiết bị USB kết nối
            foreach (ManagementObject usbDevice in connectedDevices)
            {
                string deviceId = usbDevice["DeviceID"].ToString();

                // Kiểm tra xem thiết bị này có tương ứng với màn hình đã gán hay không
                foreach (var monitor in monitorToUSBPort)
                {
                    if (deviceId.Contains(monitor.Value))
                    {
                        // Thiết bị đã kết nối đúng
                        MessageBox.Show($"Màn hình {monitor.Key} đã kết nối đúng vào cổng USB {monitor.Value}");
                    }
                    else
                    {
                        // Thiết bị không kết nối đúng
                        MessageBox.Show($"Lỗi: Màn hình {monitor.Key} không được kết nối vào cổng đúng. Hãy kết nối lại vào cổng {monitor.Value}.");
                    }
                }
            }
        }
    }
}
