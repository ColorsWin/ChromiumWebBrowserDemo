/************************************************************************************* 
* 类 名 称：CefHelper 
* 文 件 名：CefHelper
* 创建时间：2020/10/20 9:42:58     
* 作  者  ：ColorsWin     
* 说  明  ：     
* 修改时间：     
* 修 改 人：
*************************************************************************************/
using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ChromiumWPFDemo
{
    /// <summary>
    /// 触摸 帮助  在控件里面使用 local:CefTouch.Scrolling="True" 
    /// 47版本自带支持触摸，中间版本不支持比如71版本， 新版本有支持比如81版本
    /// </summary>
    public class CefTouchHelper : DependencyObject
    {

        #region Property.Attached - Scrolling

        public static bool GetScrolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollingProperty);
        }

        public static void SetScrolling(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollingProperty, value);
        }

        /// <summary> G/S:滚动功能 </summary>
        public bool Scrolling
        {
            get { return (bool)GetValue(ScrollingProperty); }
            set { SetValue(ScrollingProperty, value); }
        }

        public static readonly DependencyProperty ScrollingProperty =
            DependencyProperty.RegisterAttached("Scrolling", typeof(bool), typeof(CefTouchHelper), new UIPropertyMetadata(false, OnScrollingChanged));

        static void OnScrollingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement _target)) { return; }

            if ((bool)e.NewValue)
            {
                _target.Loaded += OnLoaded;
            }
            else
            {
                OnUnloaded(_target, new RoutedEventArgs());
            }
        }

        #endregion

        #region Functions.Scrolling

        /// <summary> G/S:捕获的触屏数据 </summary>
        private static readonly Dictionary<object, CefTouchCapture> _Captures = new Dictionary<object, CefTouchCapture>();

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Scrolling Touched Loaded");

            if (!(sender is FrameworkElement _target)) { return; }

            _target.Unloaded += OnUnloaded;
            //_target.PreviewTouchDown += _OnDown;

            _target.TouchDown += OnDown;
            _target.TouchMove += OnMove;
            _target.TouchUp += OnUp;
            _target.TouchLeave += OnUp;
        }



        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Scrolling Touched Unloaded");

            if (!(sender is FrameworkElement _target)) { return; }

            _Captures.Remove(sender);

            _target.Loaded -= OnLoaded;
            _target.Unloaded -= OnUnloaded;

            _target.TouchDown -= OnDown;
            _target.TouchMove -= OnMove;
            _target.TouchUp -= OnUp;
            _target.TouchLeave -= OnUp;
        }

        private static void OnDown(object sender, TouchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Scrolling Touch Down");

            e.Handled = true;

            IWebBrowser webBrowser = null;
            CefTouchCapture _capture = null;
            if (!_Captures.ContainsKey(sender))
            {
                webBrowser = (sender as IWebBrowser);
                if (webBrowser == null)
                {
                    webBrowser = VisualTreeHelperEx.FindChildOfType<ChromiumWebBrowser>(sender as DependencyObject);
                }
            }
            else
            {
                _capture = _Captures[sender];
                if (_capture == null) { return; }
                webBrowser = _capture.Browser;
            }

            if (webBrowser == null) { return; }
            if (_capture == null)
            {
                _capture = new CefTouchCapture();
            }
            _capture.IsMouseDown = true;
            _capture.Point = null;
            _capture.MoveTime = DateTime.Now;
            _capture.Browser = webBrowser;
            _capture.Host = webBrowser.GetBrowser().GetHost();
            _Captures[sender] = _capture;
        }

        private static void OnUp(object sender, TouchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Scrolling Touch Up");
            if (!_Captures.ContainsKey(sender)) { return; }

            var _capture = _Captures[sender];
            if (_capture == null) { return; }

            _capture.IsMouseDown = false;
            _capture.Point = null;
        }

        private static void OnMove(object sender, TouchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Scrolling Touch Move");
            if (!_Captures.ContainsKey(sender)) { return; }

            var _capture = _Captures[sender];
            if (_capture == null) { return; }

            //if ((DateTime.Now - _capture.MoveTime).TotalMilliseconds < 150) { return; }

            if (!(sender is IInputElement _ielement)) { return; }

            if (_capture.Host != null && _capture.IsMouseDown)
            {
                System.Diagnostics.Debug.WriteLine("Scrolling Touch Move - Moved");
                _capture.MoveTime = DateTime.Now;

                TouchPoint _tPoint = e.GetTouchPoint(_ielement);
                Point _newPoint = _tPoint.Position;

                if (!_capture.Point.HasValue)
                {
                    _capture.Point = _newPoint;
                }

                int _x = (int)_newPoint.X;
                int _y = (int)_newPoint.Y;
                int _oldX = (int)_capture.Point.Value.X;
                int _oldy = (int)_capture.Point.Value.Y;

                _capture.Host.SendMouseWheelEvent(_x, _y, 0, _y - _oldy, CefEventFlags.MiddleMouseButton);
                _capture.Point = _newPoint;
            }
        }

        #endregion

        #region Class

        /// <summary>
        /// Class:CEF的触屏捕获
        /// </summary>
        internal class CefTouchCapture
        {

            public DateTime MoveTime { get; set; }

            public IBrowserHost Host { get; set; }
            public IWebBrowser Browser { get; set; }
            public bool IsMouseDown { get; set; }
            public Point? Point { get; set; }

        }
        #endregion
    }

    public class VisualTreeHelperEx
    {
        public static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }
    }

}

