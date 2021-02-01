/************************************************************************************* 
* 类 名 称：ChromiumHelper 
* 文 件 名：ChromiumHelper
* 创建时间：2019/9/16 15:04:09     
* 作  者：hueEnergy     
* 说   明：     
* 修改时间：     
* 修 改 人：
*************************************************************************************/

using CefSharp.Wpf;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ChromiumBrowser.Helpers
{
    /// <summary>
    /// 浏览器帮助类
    /// </summary>
    public class ChromiumHelper
    {
        public static void Init()
        {
            //支持 any Cpu   1、<CefSharpAnyCpuSupport>true</CefSharpAnyCpuSupport> 2、执行下面代码 

            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            //Any CefSharp references have to be in another method with NonInlining
            // attribute so the assembly rolver has time to do it's thing.
            InitializeCefSharp();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            //Perform dependency check to make sure all relevant resources are in our output directory.
            var settings = new CefSettings();
           // settings.CefCommandLineArgs.Add("touch-events", "1");
            settings.CefCommandLineArgs.Add("touch-events", "enabled");
            // settings.CefCommandLineArgs.Add("--disable-web-security", "1");//关闭同源策略,允许跨域
            CefSharp.Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }
            return null;
        }
    }
}