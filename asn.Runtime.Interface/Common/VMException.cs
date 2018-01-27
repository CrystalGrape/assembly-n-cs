using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asn.Runtime.Interface.Common
{
    public enum VMFault
    {
        //编译过程错误
        ArgsError,
        //运行错误
        InvalidAddr,
        ReadOnly,
        InvalidArgs,
        //退出
        NormalExit,
    }
    public class VMException : Exception
    {
        public VMFault InternelException;
        private string message;
        public override string Message => message;
        public VMException(VMFault fault, string message = "")
        {
            InternelException = fault;
            this.message = message;
        }
    }
}
