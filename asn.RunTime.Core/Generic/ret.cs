using asn.Runtime.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    public class ret : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            Runtime.pc = Runtime.lr;
        }
    }
}
