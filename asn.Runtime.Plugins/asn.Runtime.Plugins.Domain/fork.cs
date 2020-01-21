using asn.Runtime.Interface;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Domain
{
    public class fork : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            int pcAddr = args[0];
            if (types[0] == 0)
                pcAddr = Runtime.Read(args[0]);
            IVirtualMachine vm = Runtime.CopyVM();
            Task.Run(() =>
            {
                vm.Continue(pcAddr);
            });
        }
    }
}
