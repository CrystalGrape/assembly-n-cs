﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asn.Runtime.Interface.Common
{
    public enum VMFault
    {
        //编译过程错误
        ArgsError,
        FileNotFound,
        //无效的指令
        InvalidInstructions,
        DuplicateDefinition,
        //运行错误
        InvalidAddr,
        ReadOnly,
        InvalidArgs,
        //退出
        NormalExit,
    }
    public class VMException : Exception
    {
        public VMFault VMFaultException { get; private set; }
        private string message;
        public override string Message => message;
        public VMException(VMFault fault, string message = "")
        {
            VMFaultException = fault;
            this.message = message;
        }
        public override string ToString()
        {
            return $"{VMFaultException}:{Message}";
        }
    }
}
