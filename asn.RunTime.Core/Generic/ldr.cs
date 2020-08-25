using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    public class ldr : IOperator
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

            int offset = args[2];
            if (types[2] == 0)
                offset = Runtime.Read(args[2]);

            Runtime.Write(args[0], Runtime.Read(memoryAddr + offset));
        }
    }
}
