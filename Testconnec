using System;
using System.Windows.Forms;
using System.Timers;

class Program
{
    private static bool[] physicalButtonStates;  // Trạng thái nút vật lý
    private static Button[] softwareButtons;     // Danh sách các nút phần mềm

    static void Main()
    {
        var screens = Screen.AllScreens;
        int screenCount = screens.Length;

        physicalButtonStates = new bool[screenCount];
        softwareButtons = new Button[screenCount];

        for (int i = 0; i < screenCount; i++)
        {
            Form form = new Form
            {
                Text = "Window " + (i + 1),
                Size = new System.Drawing.Size(300, 200),
                StartPosition = FormStartPosition.Manual,
                Location = screens[i].Bounds.Location
            };

            Button btn = new Button
            {
                Text = "Button " + (i + 1),
                Location = new System.Drawing.Point(100, 80)
            };
            form.Controls.Add(btn);
            softwareButtons[i] = btn;

            form.Show();
        }

        System.Timers.Timer timer = new System.Timers.Timer(1000); // Kiểm tra mỗi giây
        timer.Elapsed += CheckPhysicalButtons;
        timer.Start();

        Application.Run();  // Bắt đầu ứng dụng
    }

    private static void CheckPhysicalButtons(object sender, ElapsedEventArgs e)
    {
        for (int i = 0; i < physicalButtonStates.Length; i++)
        {
            if (physicalButtonStates[i])
            {
                ActivateSoftwareButton(i);
            }
        }
    }

    private static void ActivateSoftwareButton(int index)
    {
        if (softwareButtons[index] != null)
        {
            softwareButtons[index].PerformClick();
        }
    }
}
