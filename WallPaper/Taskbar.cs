using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WallPaper
{
    public static class Taskbar
    {
        public static TaskbarIcon WindowsNotifyIcon { get; private set; }

        public static void Open()
        {
            if (WindowsNotifyIcon is null)
            {
                InitNotifyIcon();
            }
        }

        public static void Exit()
        {
            if (WindowsNotifyIcon is null) return;
            WindowsNotifyIcon.Visibility = System.Windows.Visibility.Collapsed;
            WindowsNotifyIcon.Dispose();
        }
        ///初始化托盘控件
        static void InitNotifyIcon()
        {
            WindowsNotifyIcon = new TaskbarIcon();
            WindowsNotifyIcon.Icon = new System.Drawing.Icon("Video.ico");
            ContextMenu context = new ContextMenu();
            MenuItem exit = new MenuItem();
            exit.Header = "退出";
            exit.Click += delegate (object sender, RoutedEventArgs e)
            {
                Environment.Exit(0);
            };
            context.Items.Add(exit);

            WindowsNotifyIcon.ContextMenu = context;
        }

       

    }
}
