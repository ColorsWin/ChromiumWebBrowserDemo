using CefSharp;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ChromiumWPFDemo
{
    /// <summary>
    /// 网页调用C#方法
    /// </summary>
    public class ScriptCallbackObject
    {
        public void ShowMessage()
        {
            MessageBox.Show("网页调用C#");
        }

        public void ShowMessageArg(string arg)
        {
            MessageBox.Show("【网页调用C#】:" + arg);
        }

        public string GetData(string arg)
        {
            return "【网页调用C#获取数据】;" + arg;
        }

        /// <summary>
        /// 通过回调方式返回数据【该种方法咱不知道如何传参数】
        /// </summary>
        /// <param name="javascriptCallback"></param>
        public void GetDataByCallback(IJavascriptCallback javascriptCallback)
        {
            Task.Factory.StartNew(async () =>
            {
                using (javascriptCallback)
                {
                    await javascriptCallback.ExecuteAsync("【网页调用C#返回数据】:" + Guid.NewGuid().ToString());
                }
            });
        }
    }
}
