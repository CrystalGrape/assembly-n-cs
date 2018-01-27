using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Interface
{
    /// <summary>
    /// 伪指令插件接口
    /// </summary>
    public interface IDummyInstructions
    {
        /// <summary>
        /// 初始化伪指令
        /// </summary>
        /// <param name="line">命令行</param>
        /// <param name="codes">所有代码行</param>
        /// <param name="index">当前所在行索引</param>
        void Input(string line, List<string> codes, int index);
        //指令名
        string DummyInsName { get; }
        //参数
        string[] Args { get; }
        //获取编译结果
        List<string> Result();
    }
}
