using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asn.Runtime.Interface.Common
{
    public enum MemoryAttribute : int
    {
        /// <summary>
        /// 代码
        /// </summary>
        CODE,
        /// <summary>
        /// 指令参数 
        /// </summary>
        PARAM,
        /// <summary>
        /// 数据
        /// </summary>
        DATA,
    }
}
