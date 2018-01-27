using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    public class ldrb : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if (types[0] != 0)
                throw new VMException(VMFault.InvalidArgs);
            int oringnal = Runtime.Read(args[1]);
            int data;
            int shift = 0;
            if (types[2] == 0)
                shift = Runtime.Read(args[2]);
            else
                shift = args[2];
            int tmp = 0x000000ff << (shift * 8);
            oringnal &= tmp;
            data = oringnal >> (shift * 8);
            Runtime.Write(args[1], oringnal);
        }
    }
}
