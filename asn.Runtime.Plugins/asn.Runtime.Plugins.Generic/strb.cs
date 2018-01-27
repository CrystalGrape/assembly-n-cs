using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    public class strb : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if (types[0] != 0)
                throw new VMException(VMFault.InvalidArgs);
            int data = Runtime.Read(args[0]);
            int address = Runtime.Read(args[1]);
            int oringnal= Runtime.Read(address);
            int shift = 0;
            if (types[2] == 0)
                shift = Runtime.Read(args[2]);
            else
                shift = args[2];
            int tmp = ~(0x000000ff << (shift * 8));
            oringnal &= tmp;
            data = data << (shift * 8);
            oringnal |= data;
            Runtime.Write(address, oringnal);
        }
    }
}
