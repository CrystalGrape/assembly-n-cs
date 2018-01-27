using asn.Runtime.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Plugins.Generic.asnDebug
{
    public class showstring : IOperator
    {
        public void Run(IVirtualMachine Runtime, int[] args, char[] types)
        {
            int addr = 0;
            if (types[0] == 0)
                addr = Runtime.Read(args[0]);
            else
                addr = args[0];
            string result = "";
            while (true)
            {
                for (int shift = 0; shift < 4; shift++)
                {
                    int oringnal = Runtime.Read(addr);
                    int tmp = 0x000000ff << (shift * 8);
                    oringnal &= tmp;
                    char data = (char)(oringnal >> (shift * 8));
                    if (data == '\0')
                        goto end;
                    result += data;
                }
                addr++;
            }
            end: Console.WriteLine(result);
        }
    }
}
