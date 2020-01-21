using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic
{
    public class end : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            throw new VMException(VMFault.NormalExit, "正常退出");
        }
    }
}
