using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic.math
{
    public class mul : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if (types[0] != 0)
                throw new VMException(VMFault.InvalidArgs);
            int opdata1 = types[1] != 0 ? args[1] : Runtime.Read(args[1]);
            int opdata2 = types[2] != 0 ? args[2] : Runtime.Read(args[2]);
            Runtime.Write(args[0], opdata1 * opdata2);
        }
    }
}
