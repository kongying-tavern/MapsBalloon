using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PopTips
{
    /// <summary>
    /// HookKeyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HookKeyWindow : Window
    {
        public Key key;
        public ModifierKeys modifierKeys;

        private Key key_old;
        private ModifierKeys modifierKeys_old;
        
        private bool isCanSetHookKey = false;
        public bool isCanSet {get;set;}

        private MainWindow Main;

        public HookKeyWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            isCanSet = false;
            Main = mainWindow;
            // 获取按键输入
            this.KeyDown += new System.Windows.Input.KeyEventHandler(HookKeyWindow_KeyDown);
        }

        private void HookKeyWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (isCanSetHookKey)
            {
                // 设置按键
                if (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl && e.Key != Key.LeftAlt && e.Key != Key.RightAlt && e.Key != Key.LeftShift && e.Key != Key.RightShift)
                {
                    key = e.Key;
                    //modifierKeys = Keyboard.Modifiers;
                    HookKeyText.Text = key.ToString();
                    isCanSetHookKey = false;
                    // 设置颜色为淡灰色 #FFD8D8D8
                    HookKeyText.Background = new SolidColorBrush(Color.FromArgb(255, 216, 216, 216));
                }
            }
        }


        public int CheckHookKeyLow()
        {
            // TODO: Check Key is Right  && is Not Regin

            return 1;
        }

        public void SingleSaveHookKey()
        {
            // TODO: 确定hookkey实现 shuangpin
            key_old = key;
            modifierKeys_old = modifierKeys;
            Main.Slot_HookKey(key, modifierKeys);
        }

        private void Button_ReDefault_event(object sender, RoutedEventArgs e)
        {
            key = Key.M;
            modifierKeys = ModifierKeys.Alt;
            comboBox.SelectedIndex = 0;
            HookKeyText.Text =  key.ToString();
            //SingleSaveHookKey();
        }

        private void Button_Apply_event(object sender, RoutedEventArgs e)
        {
            if(CheckHookKeyLow()==1)
            {
                SingleSaveHookKey();
            }
            else
            {
                // TODO: Not Right Hook Key Catch
                System.Windows.MessageBox.Show("Error Hook Key, Please Try Argin!");
            }
        }

        private void Button_Cancle_event(object sender, RoutedEventArgs e)
        {
            key = key_old;
            modifierKeys = modifierKeys_old;
            SingleSaveHookKey();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isCanSetHookKey = true;
            // 设置颜色为淡黄色 #FFF9FFC0
            HookKeyText.Background = new SolidColorBrush(Color.FromArgb(255, 249, 255, 192));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 获取选中项内容
            string content = ((TextBlock)comboBox.SelectedItem).Text;
            if(content == "Alt")
            {
                modifierKeys = ModifierKeys.Alt;
            }
            else if (content == "Ctrl")
            {
                modifierKeys = ModifierKeys.Control;
            }
            else if (content == "Shift")
            {
                modifierKeys = ModifierKeys.Shift;
            }
            else if (content == "Win")
            {
                modifierKeys = ModifierKeys.Windows;
            }
        }
    }
}
