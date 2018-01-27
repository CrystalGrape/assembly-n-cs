using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic.math
{
    public class gte : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if (types[0] != 0 || types[1] != 0)
                throw new VMException(VMFault.InvalidArgs);
            int opdata1 = Runtime.Read(args[0]);
            int opdata2 = Runtime.Read(args[1]);
            if (opdata1 >= opdata2)
            {
                Runtime.cpsr |= 0x00000001;
            }
            else
            {
                Runtime.cpsr &= ~0x00000001;
            }
        }
    }
}
