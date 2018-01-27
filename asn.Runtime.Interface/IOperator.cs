using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asn.Runtime.Interface
{
    public interface IOperator
    {
        void Run(IVirtualMachine Runtime, int[] args, char[] types);
    }
}
