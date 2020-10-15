using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    /// <summary>
    /// 存储一个byte到内存
    /// 参数1：数据（寄存器）
    /// 参数2：目的地址
    /// 参数3：偏移量（相对目的地址的偏移量）
    /// </summary>
    public class strb : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if (types[0] != 0)
                throw new VMException(VMFault.InvalidArgs, $"strb，参数错误");
            int data = Runtime.Read(args[0]);

            int address = args[1];
            if (types[1] == 0)
                address = Runtime.Read(args[1]);

            int offset = args[2];
            if (types[2] == 0)
                offset = Runtime.Read(args[2]);

            address += offset / 4;
            int oringnal = Runtime.Read(address);
            int shift = offset % 4;

            int tmp = ~(0x000000ff << (shift * 8));
            oringnal &= tmp;
            data = data << (shift * 8);
            oringnal |= data;
            Runtime.Write(address, oringnal);
        }
    }
}
