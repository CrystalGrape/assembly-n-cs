using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    public class mov : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if (types[0] != 0)
                throw new VMException(VMFault.InvalidArgs);
            int opdata;
            if (types[1] == 0)
                opdata = Runtime.Read(args[1]);
            else
                opdata = args[1];
            Runtime.Write(args[0], opdata);
        }
    }
}
