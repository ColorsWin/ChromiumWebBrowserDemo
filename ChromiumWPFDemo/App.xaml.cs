using CefSharp;
using CefSharp.Wpf;
using ChromiumBrowser.Helpers;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ChromiumWPFDemo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ChromiumHelper.Init(); 
        } 
    }
}
