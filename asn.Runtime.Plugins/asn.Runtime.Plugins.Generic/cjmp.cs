using asn.Runtime.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    public class cjmp : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if ((Runtime.cpsr & 0x00000001) == 0)
                return;
            int pcAddr = args[0];
            if (types[0] == 0)
                pcAddr = Runtime.Read(args[0]);
            Runtime.pc = pcAddr;
            Runtime.cpsr &= (~0x00000001);
        }
    }
}
