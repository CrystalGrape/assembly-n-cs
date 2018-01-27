using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asn.Runtime.Interface
{
    public interface IVirtualMachine
    {
        int pc { get; set; }
        int lr { get; set; }
        int sp { get; set; }
        int cpsr { get; set; }
        int debug { get; set; }
        int icon { get; set; }
        int VirtualMachineId { get; }
        int Read(int Address);
        void Write(int Address, int Value, MemoryAttribute Attr = MemoryAttribute.DATA);
        IVirtualMachine CopyVM();
        void Continue(int pcAddr);
    }
}
