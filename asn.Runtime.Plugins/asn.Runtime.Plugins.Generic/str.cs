using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    public class str : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if (types[0] != 0)
                throw new VMException(VMFault.InvalidArgs);
            int memoryAddr;
            if (types[1] == 0)
                memoryAddr = Runtime.Read(args[1]);
            else
                memoryAddr = args[1];
            Runtime.Write(memoryAddr, Runtime.Read(args[0]));
        }
    }
}
