using asn.Runtime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asn.Ide
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //加载伪指令插件
            DummyInsCompiler dummyInsCompiler = new DummyInsCompiler();
            dummyInsCompiler.Load("dummys/");

            //加载指令插件
            Plugins optLoader = new Plugins();
            optLoader.Load("plugins/");
            Application.Run(new AsnForm());
        }
    }
}
