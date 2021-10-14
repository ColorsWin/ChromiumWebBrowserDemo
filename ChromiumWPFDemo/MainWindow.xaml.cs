using CefSharp;
using System;
using System.IO;
using System.Windows;

namespace ChromiumWPFDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            //CefSharpSettings.WcfEnabled = true;
            var bindingOptions = new BindingOptions()
            {
                CamelCaseJavascriptNames = false
            };
            //  bindingOptions.CamelCaseJavascriptNames = false;
            chromiumWebBrowser.JavascriptObjectRepository.Register("webBrowserObj",
                new ScriptCallbackObject(), false, bindingOptions);

            chromiumWebBrowser.FrameLoadEnd += ChromiumWebBrowser_FrameLoadEnd;
            chromiumWebBrowser.MenuHandler = new MenuHandler();

            LoadTest();
        }

        private void ChromiumWebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            chromiumWebBrowser.ExecuteScriptAsync("CefSharp.BindObjectAsync('webBrowserObj')");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            chromiumWebBrowser.Address = txtUrl.Text;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window1();
            window.Loaded += delegate
            {
                window.Owner = this;
            };
            window.Show();

            //this.Owner = window;

            //window.Owner = this;

            LoadTest();
        }

        private void LoadTest()
        {
            //注入脚本 这里执行 有时候成功 有时候不成功 放到FrameLoadEnd事件里面
            // chromiumWebBrowser.ExecuteScriptAsync("CefSharp.BindObjectAsync('chromiumWebBrowser')");
            string url = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/"), "pages/testPage.html");
            chromiumWebBrowser.Address = "file:///" + url;

            //txtUrl.Text = chromiumWebBrowser.Address;

        }



        private void callJS_Click(object sender, RoutedEventArgs e)
        {
            chromiumWebBrowser.ExecuteScriptAsync("ShowMessage()");

            return;

            #region 其他功能
            //chromiumWebBrowser.ExecuteScriptAsync("Hello()");
            //// 为 js 的 变量jsVar赋值 'abc'
            //chromiumWebBrowser.ExecuteScriptAsync("jsVar='---改变值abc'");
            //chromiumWebBrowser.ExecuteScriptAsync("Hello()");
            #endregion
        }

        private void callJSArg_Click(object sender, RoutedEventArgs e)
        {
            chromiumWebBrowser.ExecuteScriptAsync($"ShowMessageArg('{txtArg.Text}')");
        }

        private void callJSGetData_Click(object sender, RoutedEventArgs e)
        {
            var jsResult = chromiumWebBrowser.EvaluateScriptAsync($"GetData('{txtArg.Text}')");
            jsResult.Wait();
            if (jsResult.Result.Result != null)
            {
                MessageBox.Show(jsResult.Result.Result.ToString());
            }
        }
    }
}
