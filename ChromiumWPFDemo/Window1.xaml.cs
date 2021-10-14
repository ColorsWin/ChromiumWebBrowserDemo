using System;
using System.IO;
using System.Windows;


namespace ChromiumWPFDemo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            string url = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/"), "pages/transparent.html");
            chromiumWebBrowser.Address = "file:///" + url;
        }

    }
}
