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
                throw new VMException(VMFault.InvalidArgs, $"ldrb，参数错误");
            int data;
            
            int address = args[1];
            if (types[1] == 0)
                address = Runtime.Read(args[1]);

            int offset = args[2]; 
            if (types[2] == 0)
                offset = Runtime.Read(args[2]);

            //偏移后的基地址
            address += offset / 4;
            int oringnal = Runtime.Read(address);
            int shift = offset % 4;

            int tmp = 0x000000ff << (shift * 8);
            oringnal &= tmp;
            data = oringnal >> (shift * 8);
            Runtime.Write(args[0], data);
        }
    }
}
