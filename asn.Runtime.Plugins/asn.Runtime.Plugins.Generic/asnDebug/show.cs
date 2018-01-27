using asn.Runtime.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic.asnDebug
{
    public class show : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            if (types[0] == 0)
                Console.WriteLine(Runtime.Read(args[0]));
            else
                Console.WriteLine(args[0]);
        }
    }
}
