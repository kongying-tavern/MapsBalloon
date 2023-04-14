using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Threading;
//using System.Windows.Forms;
using Drawing = System.Drawing;
using System.Windows.Threading;
using System.ComponentModel;
using NHotkey.Wpf;
using System.Windows.Interop;

namespace PopTips
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //byte[] MMRByte=new byte[1];
        private HookKeyWindow hookKeyWindow;
        public MainWindow()
        {
            //Start_UDPSender(22233,23333);
            //Start_UDPReceive(23233);
            hookKeyWindow = new HookKeyWindow(this);
            callback = ShowThis;
            EventCallback = Callback;
            OpenClose = openClose;
            x_offset = GetSystemMetrics(SM_CXDLGFRAME);
            y_offset = GetSystemMetrics(SM_CYDLGFRAME) + GetSystemMetrics(SM_CYCAPTION);
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("GenshinImpactCur.cur", UriKind.Relative));
            Cursor customCursor = new Cursor(sri.Stream);
            this.Cursor = customCursor;
            //Visibility = Visibility.Hidden;
            //InitialTray();
#if DEBUG
            this.Dispatcher.BeginInvoke(new Action(() => {
                //hwnd = FindWindow("UnityWndClass", "「空荧酒馆」原神地图");
                //Thread.Sleep(10000);
                //Trace.WriteLine("aaaa:"+GetWindowLong(hwnd, GWL_STYLE));
                //hwnd_this = new WindowInteropHelper(this).Handle;
                //this.Show();
                //this.Opacity = 1;
                //HotkeyManager.Current.AddOrReplace("OpenClose", Key.M, ModifierKeys.Alt, OpenClose);
                //SetHook();
            }));
#else
            this.Dispatcher.BeginInvoke(new Action(() => {
                this.Opacity = 1;
                //Thread.Sleep(1000);
                Hide();
                //Visibility = Visibility.Hidden;
                Key key = Key.M;
                ModifierKeys modifierKeys = ModifierKeys.Alt;
                LoadHookKey(ref key,ref modifierKeys);
                HotkeyManager.Current.AddOrReplace("OpenClose", key, modifierKeys, OpenClose);
            }));
#endif
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            hwnd_this = new WindowInteropHelper(this).Handle;
            hwnd = FindWindow("UnityWndClass", "「空荧酒馆」原神地图");
            hwnd_ys = FindWindow("UnityWndClass", "原神");
            if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "Genshin Impact");
            if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "원신");
            SetForegroundWindow(hwnd);
            ShowWindow(hwnd, 1);
            //if (Open("xfc", 64)==0)
            //{
            //    recvThread = new Thread(new ThreadStart(RecvThread1))
            //    {
            //        IsBackground = true
            //    };
            //    recvThread.Start();
            //}
            //HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            //if (source != null) source.AddHook(WndProc);
            //hwnd = FindWindow("UnityWndClass", "「空荧酒馆」原神地图");
            //hwnd_this = new WindowInteropHelper(this).Handle;
            recvThread = new Thread(new ThreadStart(RecvThread1))
            {
                IsBackground = true
            };
            recvThread.Start();
        }
        bool mapAndYs = false;
        string MMR = "";
        private void RecvThread1()
        {
            while (true)
            {
                try
                {
                    //if (Read(ref MMRByte) == 0)
                    //{
                    //    if (MMR != Encoding.Default.GetString(MMRByte))
                    //    {
                    //        MMR = Encoding.Default.GetString(MMRByte);
                    //        Dispatcher.Invoke(MMRFunction);
                    //    }
                    //}
                    if (isOn)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SwitchTopMost();
                        });
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Hide();
                            //Visibility = Visibility.Hidden;
                        });
                    }
                    //hwnd_ys = FindWindow("UnityWndClass", "原神");
                    //if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "Genshin Impact");
                    //if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "원신");
                    if (GetWindowLong(hwnd, GWL_STYLE) == 336265216 || GetWindowLong(hwnd, GWL_STYLE) == 67829760)
                    {
                        if (!mapAndYs)
                        {
                            mapAndYs = true;
                            isOn = true;
                            Dispatcher.Invoke(SetHook);
                        }
                    }
                    else if (mapAndYs)
                    {
                        mapAndYs = false;
                        isOn = false;
                        Dispatcher.Invoke(() =>
                        {
                            //Visibility = Visibility.Hidden;
                            Hide();
                            UnhookWinEvent(hWinEventHook);
                        });
                    }
                }
                catch { }
                Thread.Sleep(10);
            }
        }
        EventHandler<NHotkey.HotkeyEventArgs> OpenClose;
        private void openClose(object sender,NHotkey.HotkeyEventArgs e)
        {
            Trace.WriteLine("alt+m");
            hwnd = FindWindow("UnityWndClass", "「空荧酒馆」原神地图");
            hwnd_ys = FindWindow("UnityWndClass", "原神");
            if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "Genshin Impact");
            if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "원신");
            if (GetForegroundWindow()== hwnd_ys)
            {
                SetForegroundWindow(hwnd);
                ShowWindow(hwnd, 1);
            }
            else if (GetForegroundWindow() == hwnd)
            {
                SetForegroundWindow(hwnd_ys);
                ShowWindow(hwnd_ys, 1);
                Mouse.Synchronize();
            }
        }
        private bool SetHook()
        {
            hwnd_ys = FindWindow("UnityWndClass", "原神");
            if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "Genshin Impact");
            if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "원신");
            if (GetWindowLong(hwnd_ys, GWL_STYLE) == -1811939328 || GetWindowLong(hwnd_ys, GWL_STYLE) == -1778384896)
            {
                isNoBorder = true;
                Trace.WriteLine("Yes");
            }
            else
            {
                isNoBorder = false;
                Trace.WriteLine("No");
            }
            if (hwnd_this != IntPtr.Zero && hwnd_ys != IntPtr.Zero)
            {
                try
                {
                    NotifyIconContextContent.ShowBalloonTip("「空荧酒馆」原神地图", "已启动覆盖模式，将显示托盘图标，游戏内按Alt+M可呼出大地图~", HandyControl.Data.NotifyIconInfoType.Info);
                }
                catch { }
                //Visibility = Visibility.Hidden;
                hwnd_ys_ThreadId = GetWindowThreadProcessId(hwnd_ys, out hwnd_ys_ProcessId);
                //Trace.WriteLine((int)hwnd_ys_ProcessId);
                UnhookWinEvent(hWinEventHook);
                hWinEventHook = SetWinEventHook(EVENT_MIN, EVENT_MAX, IntPtr.Zero, EventCallback, (int)hwnd_ys_ProcessId, hwnd_ys_ThreadId, SetWinEventHookFlags.WINEVENT_OUTOFCONTEXT);
                GetWindowRect(hwnd_ys, out margins_xy);
                GetClientRect(hwnd_ys, out margins_size);
                SetWindowPos(hwnd_this, -1, margins_xy.cxLeftWidth + (isNoBorder ? 0 : x_offset), margins_xy.cxRightWidth + (isNoBorder ? 0 : y_offset) + margins_size.cyBottomHeight / 11, 75, 72, SWP_SHOWWINDOW| SWP_NOACTIVATE | SWP_NOSIZE | SWP_FRAMECHANGED | SWP_NOZORDER);
                //Trace.WriteLine(margins_size.cyBottomHeight);
                Height = (margins_size.cyBottomHeight / 1080f) * 72;
                Width = (margins_size.cyBottomHeight / 1080f) * 75;
                return true;
            }
            else
            {
                Trace.WriteLine(hwnd_this);
                Trace.WriteLine(hwnd_ys);
                return false;
            }
        }
        //#region 最小化系统托盘
        //private void InitialTray()
        //{
        //    //隐藏主窗体
        //    this.Visibility = Visibility.Hidden;
        //    //设置托盘的各个属性
        //    _notifyIcon = new NotifyIcon();
        //    _notifyIcon.BalloonTipText = "已启动覆盖模式，将显示托盘图标";//托盘气泡显示内容
        //    _notifyIcon.Text = "「空荧酒馆」原神地图悬浮窗";
        //    _notifyIcon.Visible = true;//托盘按钮是否可见
        //    _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);//托盘中显示的图标
        //    _notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;

        //    //窗体状态改变时触发
        //    this.StateChanged += MainWindow_StateChanged;
        //}
        //#endregion

        //#region 窗口状态改变
        //private void MainWindow_StateChanged(object sender, EventArgs e)
        //{
        //    if (this.WindowState == WindowState.Minimized)
        //    {
        //        this.Visibility = Visibility.Hidden;
        //    }
        //}
        //#endregion

        //#region 托盘图标鼠标单击事件
        //private void notifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        if (this.Visibility == Visibility.Visible)
        //        {
        //            this.Visibility = Visibility.Hidden;
        //        }
        //        else
        //        {
        //            this.Visibility = Visibility.Visible;
        //            this.Activate();
        //        }
        //    }
        //}
        //#endregion
        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
        [DllImport("User32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); //获得本窗体的句柄
        [DllImport("user32.dll")]
        static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out MARGINS margins);
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out MARGINS margins);
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int intnIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);
        internal delegate void WinEventProc(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);
        static WinEventProc EventCallback;//注册数据回调事件
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, SetWinEventHookFlags dwflags);
        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void Keybd_event(
byte bvk,//虚拟键值 ESC键对应的是27
byte bScan,//0
int dwFlags,//0为按下，1按住，2释放
int dwExtraInfo//0
);
        internal enum SetWinEventHookFlags
        {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        }
        private const int EVENT_MIN = 0x00000001;
        private const int EVENT_MAX = 0x7FFFFFFF;
        private const int SM_CYCAPTION = 4;
        private const int SM_CYBORDER = 6;
        private const int SM_CXDLGFRAME = 7;
        private const int SM_CYDLGFRAME = 8;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_NOREDRAW = 0x0008;
        const int SWP_NOACTIVATE = 0x0010;
        const int SWP_FRAMECHANGED = 0x0020;
        const int SWP_HIDEWINDOW = 0x0080;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_BORDER = 0x00800000;
        private const int WS_CAPTION = 0x00C00000;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int LWA_COLORKEY = 0x00000001;
        private const int LWA_ALPHA = 0x00000002;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_SIZEBOX = 0x00040000;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_STYLE = -16;
        IntPtr hwnd;
        IntPtr hwnd_this;
        IntPtr hwnd_ys;
        IntPtr hwnd_ys_ProcessId;
        int hwnd_ys_ThreadId;
        int x_offset;
        int y_offset;
        bool isNoBorder = false;
        MARGINS margins_xy;
        MARGINS margins_size;
        MARGINS margins_size2;
        IntPtr hWinEventHook;
        bool isYSMove = false;
        private void Callback(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime)
        {
            //callback function, called when message is intercepted
            //Trace.WriteLine(idChild);
            //Trace.WriteLine(iEvent);
            if (iEvent == 10)
            {
                isYSMove = true;
                ChangePOS();
                //Trace.WriteLine("开始变");
            }
            else if (iEvent == 11)
            {
                isYSMove = false;
                ChangePOS();
                //Trace.WriteLine("结束变");
            }
            else if(isYSMove)
            {
                ChangePOS();
                //Trace.WriteLine("正在变");
            }
            else if(iEvent == 9)
            {
                //Trace.WriteLine("鼠标松开");
                TheEnclosingMethod();
            }
            else if(iEvent== 32769)
            {
                CheckHwnd_ys();
            }
            //else if(iEvent == 3)
            //{
            //    //Show();
            //    SetWindowPos(hwnd_this, -1,0,0,0,0, SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED | SWP_NOZORDER);
            //    Topmost = true;
            //    Trace.WriteLine("3"+Topmost);
            //}
            //else if (iEvent == 32772)
            //{
            //    if(hwnd_ys!=GetForegroundWindow())
            //        Topmost = false;
            //    Trace.WriteLine("32772" + Topmost);
            //}
            //else if(iEvent == 32770)
            //{
            //    Topmost = false;
            //    Trace.WriteLine("32770" + Topmost);
            //}
            //else if (iEvent != 32779)
            //{
            //    Trace.WriteLine(iEvent);
            //}
        }
        public async void CheckHwnd_ys()
        {
            await Task.Delay(2000);
            hwnd_ys = FindWindow("UnityWndClass", "原神");
            if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "Genshin Impact");
            if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "원신");
            if (hwnd_ys == IntPtr.Zero)
            {
#if DEBUG
                Trace.WriteLine(hwnd_ys);
#else
                //Write_UDPSender("YSExit");
                hwnd = FindWindow("UnityWndClass", "「空荧酒馆」原神地图");
                SetForegroundWindow(hwnd);
                ShowWindow(hwnd, 1);
#endif
            }
        }
        private void ChangePOS()
        {
            GetWindowRect(hwnd_ys, out margins_xy);
            GetClientRect(hwnd_ys, out margins_size);
            SetWindowPos(hwnd_this, -1, margins_xy.cxLeftWidth + (isNoBorder ? 0 : x_offset), margins_xy.cxRightWidth + (isNoBorder ? 0 : y_offset) + margins_size.cyBottomHeight / 11, 75, 72, SWP_SHOWWINDOW | SWP_NOSIZE | SWP_FRAMECHANGED | SWP_NOZORDER);
            Height = ((margins_size.cyBottomHeight / 1080f) + (margins_size.cyTopHeight / 1920f)) * 36;
            Width = ((margins_size.cyBottomHeight / 1080f) + (margins_size.cyTopHeight / 1920f)) * 37.5;
            Trace.WriteLine("变");
        }
        bool DelayLock = false;
        int DelayLockMul = 0;
        public async void TheEnclosingMethod()
        {
            if (DelayLock)
            {
                DelayLockMul++;
            }
            else
            {
                DelayLock = true;
            }
            await Task.Delay(200);
            if (DelayLock && DelayLockMul==0)
            {
                DelayLock = false;
                GetClientRect(hwnd_ys, out margins_size2);
                if (margins_size2.cyTopHeight != margins_size.cyTopHeight || margins_size2.cyBottomHeight != margins_size.cyBottomHeight)
                    ChangePOS();
            }
            else if(DelayLockMul > 0)
            {
                await Task.Delay(100);
                DelayLockMul--;
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /// <summary>
        /// 最大化窗口，最小化窗口，正常大小窗口；
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetForegroundWindow(FindWindow("UnityWndClass", "「空荧酒馆」原神地图"));
            ShowWindow(FindWindow("UnityWndClass", "「空荧酒馆」原神地图"), 1);
            //Visibility = Visibility.Hidden;
            Hide();
        }
        UdpClient udpClient;
        IPEndPoint targetPoint;
        bool udp_send_flag = false;
        bool udp_recv_flag = false;
        public void Start_UDPSender(int self_port, int target_port)
        {
            if (udp_send_flag == true)
                return;
            udpClient = new UdpClient(self_port);

            targetPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), target_port);

            udp_send_flag = true;
        }
        private Thread recvThread;
        private UdpClient UDPrecv;
        private IPEndPoint endpoint;
        private byte[] recvBuf;
        public Transform transformself;
        public void Start_UDPReceive(int recv_port)
        {
            if (udp_recv_flag == true)
                return;
            UDPrecv = new UdpClient(new IPEndPoint(IPAddress.Any, recv_port));
            endpoint = new IPEndPoint(IPAddress.Any, 0);
            recvThread = new Thread(new ThreadStart(RecvThread))
            {
                IsBackground = true
            };
            recvThread.Start();
            udp_recv_flag = true;
        }
        public void Close_UDPSender()
        {
            if (udp_send_flag == false)
                return;
            udpClient.Close();
            udp_send_flag = false;
        }
        public void Close_UDPReceive()
        {
            if (udp_recv_flag == false)
                return;
            recvThread.Interrupt();
            recvThread.Abort();
            udp_recv_flag = false;
        }
        public void Write_UDPSender(string strdata)
        {
            if (udp_send_flag == false)
                return;
            Trace.WriteLine("Close");
            byte[] sendData = Encoding.Default.GetBytes(strdata);
            udpClient.Send(sendData, sendData.Length, targetPoint);
        }
        public string Read_UDPReceive()
        {
            returnstr = String.Copy(recvdata);
            if (old)
            {
                old = false;
                recvdata = "";
                return returnstr;
            }
            else
                return "";
        }
        bool old = false;
        string returnstr;
        string recvdata;
        private bool messageReceive;
        Action callback;
        private void ReceiveCallback(IAsyncResult ar)
        {
            recvBuf = UDPrecv.EndReceive(ar, ref endpoint);
            recvdata += Encoding.Default.GetString(recvBuf);
            old = true;
            messageReceive = true;
            Dispatcher.Invoke(callback);
        }
        private void ShowThis()
        {
            string temp = Read_UDPReceive();
            if (temp == "Init")
            {
                NotifyIconContextContent.ShowBalloonTip("「空荧酒馆」原神地图", "已启动覆盖模式，将显示托盘图标，游戏内按Alt+M可呼出大地图~", HandyControl.Data.NotifyIconInfoType.Info);
                SetHook();
                //Visibility = Visibility.Hidden;
                Hide();
                //_notifyIcon.ShowBalloonTip(200);//托盘气泡显示时间
            }
            else if (temp == "Hide")
            {
                Trace.WriteLine("Hide");
                //hwnd_ys = FindWindow("UnityWndClass", "原神");
                //if (GetWindowLong(hwnd_ys, GWL_STYLE) == -1811939328)
                //{
                //    isNoBorder = true;
                //    Trace.WriteLine("Yes");
                //}
                //else
                //{
                //    isNoBorder = false;
                //    Trace.WriteLine("No");
                //}
                //if (hwnd_this != IntPtr.Zero && hwnd_ys != IntPtr.Zero)
                //{
                //    GetWindowRect(hwnd_ys, out margins_xy);
                //    GetClientRect(hwnd_ys, out margins_size);
                //    SetWindowPos(hwnd_this, -1, margins_xy.cxLeftWidth + (isNoBorder ? 0 : x_offset) + 230, margins_xy.cxRightWidth + (isNoBorder ? 0 : y_offset) + 160, (margins_size.cyBottomHeight / 1080) * 75, (margins_size.cyBottomHeight / 1080) * 72, SWP_SHOWWINDOW | SWP_FRAMECHANGED);
                //}
                //NotifyIconContextContent.ShowBalloonTip("「空荧酒馆」原神地图", "已显示悬浮窗", HandyControl.Data.NotifyIconInfoType.Info);
                Visibility = Visibility.Visible;
            }
            else if (temp == "Show")
            {
                Trace.WriteLine("Show");
                //Visibility = Visibility.Hidden;
                Hide();
            }
            else if (temp == "Close")
            {
                Trace.WriteLine("Close");
                Close();
            }
        }
        private void RecvThread()
        {
            messageReceive = true;
            while (true)
            {
                try
                {
                    if (messageReceive)
                    {
                        UDPrecv.BeginReceive(new AsyncCallback(ReceiveCallback), null);
                        messageReceive = false;
                    }
                }
                catch { }
                Thread.Sleep(10);
            }
        }
        bool isClose = false;
        private void SwitchTopMost()
        {
            if (isOn)
            {
                hwnd_ys = FindWindow("UnityWndClass", "原神");
                if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "Genshin Impact");
                if (hwnd_ys == IntPtr.Zero) hwnd_ys = FindWindow("UnityWndClass", "원신");
                if(hwnd_ys == IntPtr.Zero)
                {
                    if (!isClose)
                    {
                        isClose = true;
                        SetForegroundWindow(hwnd);
                        ShowWindow(hwnd, 1);
                    }
                }
                else
                {
                    isClose = false;
                }
                //Trace.WriteLine(GetForegroundWindow());
                if (hwnd_ys == GetForegroundWindow())
                {
                    this.Show();
                    //Visibility = Visibility.Visible;
                    //this.Opacity = 1;
                }
                else if (hwnd_this == GetForegroundWindow())
                {
                    //this.Hide();
                    //this.Opacity = 1;
                }
                else
                {
                    //Visibility = Visibility.Hidden;
                    this.Hide();
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            btnSendMsg_Click();
            //Write_UDPSender("Close");
            hwnd = FindWindow("UnityWndClass", "「空荧酒馆」原神地图");
            SetForegroundWindow(hwnd);
            ShowWindow(hwnd, 1);
            Close();
        }
        private void MenuItem_Click2(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NotifyIconContextContent.Dispose();
            //e.Cancel = true;
        }
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        private const int WM_CLOSE = 0x0010;
        public void btnSendMsg_Click()
        {
            //IntPtr hWnd = new IntPtr();
            try
            {
                //hWnd = hwnd = ;
                int data = Convert.ToInt32("1");//未做数据校验
                SendMessage(FindWindow("UnityWndClass", "「空荧酒馆」原神地图"), WM_CLOSE, (IntPtr)data, (IntPtr)0);
            }
            catch
            {
            }
        }
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Trace.WriteLine(msg);
            if (msg == 1024)
            {
                NotifyIconContextContent.ShowBalloonTip("「空荧酒馆」原神地图", "已启动覆盖模式，将显示托盘图标，游戏内按Alt+M可呼出大地图~", HandyControl.Data.NotifyIconInfoType.Info);
                SetHook();
                //Visibility = Visibility.Hidden;
                Hide();
            }
            else if (msg == 1025)
            {
                Trace.WriteLine("Hide");
                //Visibility = Visibility.Visible;
                Show();
            }
            else if (msg == 1026)
            {
                Trace.WriteLine("Show");
                //Visibility = Visibility.Hidden;
                Hide();
            }
            else if (msg == 1027)
            {
                Trace.WriteLine("Close");
                Close();
            }
            Trace.WriteLine(GetForegroundWindow());
            return (new IntPtr(0));
        }
        bool isOn = false;
        string currentMMR = "";
        void MMRFunction()
        {
            if (currentMMR != MMR)
            {
                currentMMR = MMR;
                Trace.WriteLine(MMR);
                if (currentMMR == "0")
                {
                    UnhookWinEvent(hWinEventHook);
                    isOn = false;
                }
                else if (currentMMR == "1")
                {
                    SetHook();
                    isOn = true;
                }
                else if (currentMMR == "4")
                {
                    Trace.WriteLine("Close");
                    Close();
                }
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(int hFile, IntPtr lpAttributes, uint flProtect, uint dwMaxSizeHi, uint dwMaxSizeLow, string lpName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMapping, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnmapViewOfFile(IntPtr pvBaseAddress);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", EntryPoint = "GetLastError")]
        public static extern int GetLastError();

        const int ERROR_ALREADY_EXISTS = 183;

        const int INVALID_HANDLE_VALUE = -1;

        IntPtr m_hSharedMemoryFile = IntPtr.Zero;
        IntPtr m_pwData = IntPtr.Zero;
        bool m_bAlreadyExist = false;
        bool m_bInit = false;
        long m_MemSize = 0;

        //protected override void OnClosed(EventArgs e)
        //{
        //    NotifyIconContextContent.Visibility=Visibility.Hidden;
        //    base.OnClosed(e);
        //}

        const int PAGE_READWRITE = 0x40;
        const int FILE_MAP_WRITE = 0x0002;

        /// <summary>
        /// 初始化共享内存
        /// </summary>
        /// <param name="strName">共享内存名称</param>
        /// <param name="lngSize">共享内存大小</param>
        /// <returns></returns>
        public int Init(string strName, long lngSize)
        {
            if (lngSize <= 0 || lngSize > 0x00800000) lngSize = 0x00800000;
            m_MemSize = lngSize;
            if (strName.Length > 0)
            {
                //创建内存共享体(INVALID_HANDLE_VALUE)
                m_hSharedMemoryFile = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)lngSize, strName);
                if (m_hSharedMemoryFile == IntPtr.Zero)
                {
                    m_bAlreadyExist = false;
                    m_bInit = false;
                    return 2; //创建共享体失败
                }
                else
                {
                    if (GetLastError() == ERROR_ALREADY_EXISTS)  //已经创建
                    {
                        m_bAlreadyExist = true;
                    }
                    else                                         //新创建
                    {
                        m_bAlreadyExist = false;
                    }
                }
                //---------------------------------------
                //创建内存映射
                m_pwData = MapViewOfFile(m_hSharedMemoryFile, FILE_MAP_WRITE, 0, 0, (uint)lngSize);
                if (m_pwData == IntPtr.Zero)
                {
                    m_bInit = false;
                    CloseHandle(m_hSharedMemoryFile);
                    return 3; //创建内存映射失败
                }
                else
                {
                    m_bInit = true;
                    if (m_bAlreadyExist == false)
                    {
                        //初始化
                    }
                }
                //----------------------------------------
            }
            else
            {
                return 1; //参数错误    
            }

            return 0;     //创建成功
        }
        /// <summary>
        /// 读取共享内存
        /// </summary>
        /// <param name="strName">共享内存名称</param>
        /// <returns></returns>
        public int Open(string strName,int lngSize)
        {
            if (lngSize <= 0 || lngSize > 0x00800000) lngSize = 0x00800000;
            m_MemSize = lngSize;
            if (strName.Length > 0)
            {
                //创建内存共享体(INVALID_HANDLE_VALUE)
                m_hSharedMemoryFile = OpenFileMapping(FILE_MAP_WRITE, false, strName);
                if (m_hSharedMemoryFile == IntPtr.Zero)
                {
                    Trace.WriteLine(GetLastError());
                    m_bAlreadyExist = false;
                    m_bInit = false;
                    return 2; //创建共享体失败
                }
                else
                {
                    if (GetLastError() == ERROR_ALREADY_EXISTS)  //已经创建
                    {
                        m_bAlreadyExist = true;
                    }
                    else                                         //新创建
                    {
                        m_bAlreadyExist = false;
                    }
                }
                //---------------------------------------
                //创建内存映射
                m_pwData = MapViewOfFile(m_hSharedMemoryFile, FILE_MAP_WRITE, 0, 0, (uint)lngSize);
                if (m_pwData == IntPtr.Zero)
                {
                    m_bInit = false;
                    CloseHandle(m_hSharedMemoryFile);
                    return 3; //创建内存映射失败
                }
                else
                {
                    m_bInit = true;
                    if (m_bAlreadyExist == false)
                    {
                        //初始化
                    }
                }
                //----------------------------------------
            }
            else
            {
                return 1; //参数错误    
            }

            return 0;     //创建成功
        }
        /// <summary>
        /// 关闭共享内存
        /// </summary>
        public void CloseMMR()
        {
            if (m_bInit)
            {
                UnmapViewOfFile(m_pwData);
                CloseHandle(m_hSharedMemoryFile);
            }
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="bytData">数据</param>
        /// <param name="lngAddr">起始地址</param>
        /// <param name="lngSize">个数</param>
        /// <returns></returns>
        public int Read(ref byte[] bytData)
        {
            //if (lngAddr + lngSize > m_MemSize) 
            //    return 2; //超出数据区
            if (m_bInit)
            {
                Marshal.Copy(m_pwData, bytData, 0, 1);
            }
            else
            {
                return 1; //共享内存未初始化
            }
            return 0;     //读成功
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="bytData">数据</param>
        /// <param name="lngAddr">起始地址</param>
        /// <param name="lngSize">个数</param>
        /// <returns></returns>
        public int Write(byte[] bytData, int lngAddr, int lngSize)
        {
            if (lngAddr + lngSize > m_MemSize) return 2; //超出数据区
            if (m_bInit)
            {
                Marshal.Copy(bytData, lngAddr, m_pwData, lngSize);
            }
            else
            {
                return 1; //共享内存未初始化
            }
            return 0;     //写成功
        }

        private void Window_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            hwnd = FindWindow("UnityWndClass", "「空荧酒馆」原神地图");
            SetForegroundWindow(hwnd);
            ShowWindow(hwnd, 1);
            //Visibility = Visibility.Hidden;
            Hide();
        }

        private void MenuItem_Click_SetHookKey(object sender, RoutedEventArgs e)
        {
            if(hookKeyWindow == null)
            {
                hookKeyWindow = new HookKeyWindow(this);

            }
            hookKeyWindow.Show();
            hookKeyWindow.isCanSet = true;

            // TODO: Wait Windoe return set Hook key
        }
        /// <summary>
        /// Load HookKey Setting From PC
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifierKeys"></param>
        private void LoadHookKey(ref Key key,ref ModifierKeys modifierKeys)
        {
            key = (Key)MapsBalloon.Properties.Settings.Default.HookKey;
            modifierKeys = (ModifierKeys)MapsBalloon.Properties.Settings.Default.HookConKey;
        }
        /// <summary>
        /// Save HookKey To Loaction
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifierKeys"></param>
        private void SaveHookKey(Key key, ModifierKeys modifierKeys)
        {
            MapsBalloon.Properties.Settings.Default.HookKey =(char) key;
            MapsBalloon.Properties.Settings.Default.HookConKey = (char)modifierKeys;
            MapsBalloon.Properties.Settings.Default.Save();
        }

        public void Slot_HookKey(Key setKey, ModifierKeys setModifierKeys)
        {
            // MessageBox.Show("This need set Hook key ["+setKey.ToString()+"+"+ setModifierKeys.ToString()+"]");

            // TODO: Regions HookKey On Here
            HotkeyManager.Current.AddOrReplace("OpenClose", setKey, setModifierKeys, openClose);
            // Save HookKey
            SaveHookKey(setKey, setModifierKeys);

            NotifyIconContextContent.ShowBalloonTip("「空荧酒馆」原神地图",
                "已注册覆盖模式快捷键 ["+ setModifierKeys.ToString() + "+"+ setKey.ToString() +  "]", HandyControl.Data.NotifyIconInfoType.Info);
        }
    }
}
