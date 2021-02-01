using CefSharp;
using CefSharp.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace ChromiumWPFDemo
{
    /// <summary>
    /// 浏览器右键菜单
    /// </summary>
    public class MenuHandler : IContextMenuHandler
    {

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {

        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return true;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            //隐藏菜单栏
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            chromiumWebBrowser.Dispatcher.Invoke(() =>
            {
                chromiumWebBrowser.ContextMenu = null;
            });
        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            // return true;

            //绘制了一遍菜单栏  所以初始化的时候不必绘制菜单栏，再此处绘制即可
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            chromiumWebBrowser.Dispatcher.Invoke(() =>
            {
                var menu = new ContextMenu
                {
                    IsOpen = true
                };

                RoutedEventHandler handler = null;

                handler = (s, e) =>
              {
                  menu.Closed -= handler;

                  //If the callback has been disposed then it's already been executed
                  //so don't call Cancel
                  if (!callback.IsDisposed)
                  {
                      callback.Cancel();
                  }
              };

                menu.Closed += handler;

                menu.Items.Add(new MenuItem
                {
                    Header = "最小化",
                    // Command = new CustomCommand(MinWindow)
                });
                menu.Items.Add(new MenuItem
                {
                    Header = "关闭",
                    //  Command = new CustomCommand(CloseWindow)
                });
                chromiumWebBrowser.ContextMenu = menu;

            });

            return true;
        }



        /// <summary>
        /// 关闭窗体
        /// </summary>
        private void CloseWindow()
        {

        }

        /// <summary>
        /// 最小化窗体
        /// </summary>
        private void MinWindow()
        {

        }
    }
}
