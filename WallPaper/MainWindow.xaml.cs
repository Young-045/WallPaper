using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Numerics;
using System.IO;

namespace WallPaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoW _video;
        private MediaElement _mediaElement;
        private ICommand _iconCommand;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitIcon();
            _video = new VideoW();
            _mediaElement = _video.Media;
            _mediaElement.LoadedBehavior = MediaState.Manual;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            _video.Width = x1;
            _video.Height = y1;
            _mediaElement.Width = x1;
            _mediaElement.Height = y1;
            var prog = FindWindow("progman", "Program Manager");
            SendMessageTimeout(prog, 0x52C, 0, 0, 0, 100, 0);
            _video.Opacity = 0;
            _video.Show();
            var handle = new WindowInteropHelper(_video).Handle;
            SetParent(handle, GetBackground());
        }

        private void InitIcon()
        {
            _iconCommand = new CusCommand(ShowMainWindow);
            Taskbar.WindowsNotifyIcon.DoubleClickCommand = _iconCommand;
        }

        private void ShowMainWindow()
        {
            this.Show();
        }

        private unsafe static IntPtr GetBackground()
        {

            IntPtr background = IntPtr.Zero;
            IntPtr father = FindWindow("progman", "Program Manager");
            IntPtr workerW = IntPtr.Zero;
            do
            {
                workerW = FindWindowEx(IntPtr.Zero, workerW, "workerW", null);
                if (workerW != IntPtr.Zero)
                {
                    char[] buff = new char[200];
                    IntPtr b = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);
                    int ret = (int)GetClassName(workerW, b, 400);
                    if (ret == 0) throw new Exception("出错");
                }
                if (GetParent(workerW) == father)
                {
                    background = workerW;
                }
            } while (workerW != IntPtr.Zero);
            return background;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "媒体文件|*.mp4;*.mpeg;*.wma;*.wmv;*.wav;*.avi";
            if (dialog.ShowDialog() == true)
            {
                _video.Opacity = 1;
                _mediaElement.Source = new Uri(dialog.FileName);
                _mediaElement.Play();
            }
        }

        private void Button_Quit(object sender, RoutedEventArgs e)
        {
            _video.Hide();
            Refresh();
            _video.Close();
            this.Close();
            Taskbar.Exit();
            System.Environment.Exit(0);
        }

        private void VideoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(_mediaElement != null)
            {
                _mediaElement.Volume = (double)e.NewValue;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);

        public static bool Refresh()
        {
            StringBuilder wallpaper = new StringBuilder(200);
            SystemParametersInfo(0x73, 200, wallpaper, 0);
            int ret = SystemParametersInfo(20, 1, wallpaper, 3);
            if (ret != 0)
            {
                RegistryKey hk = Registry.CurrentUser;
                RegistryKey run = hk.CreateSubKey(@"Control Panel\Desktop\");
                run.SetValue("Wallpaper", wallpaper.ToString());
                return true;
            }
            return false;
        }

        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        private static extern IntPtr GetClassName(IntPtr hWnd, IntPtr lpClassName, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "GetParent")]
        private static extern IntPtr GetParent(IntPtr hWnd);


        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendMessageTimeout(IntPtr hWnd, uint uMsg, uint wParam, int lParam, uint fuFlags, uint uTimeout, int lpdwResult);


        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        
    }
}
