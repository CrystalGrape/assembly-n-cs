using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.DummyInstructions.Plugins.Generic
{
    public class di_strstring : IDummyInstructions
    {
        public string DummyInsName { get; private set; }

        public string[] Args { get; private set; }

        private void parseLine(string line)
        {
            int index = 0;
            //这个字符是否是字符串内
            bool isString = false;
            for (int i = 0; i < (int)line.Length; i++)
            {
                if (index == 0 && line.ToCharArray()[i] == ' ')
                {
                    index++;
                    continue;
                }
                if (!isString && line.ToCharArray()[i] == ',')
                {
                    index++;
                    continue;
                }
                if (line.ToCharArray()[i] == '"')
                {
                    isString = !isString;
                    continue;
                }

                if (index == 0)
                    DummyInsName += line.ToCharArray()[i];
                else if (index >= 3)
                    throw new VMException(VMFault.ArgsError, "伪指令di_strstring，只有三个参数");
                else
                    Args[index - 1] += line.ToCharArray()[i];
            }
            Args[0] = Args[0].ToLower();
        }
        public void Input(string line, List<string> codes, int index)
        {
            DummyInsName = "";
            Args = new string[2];
            parseLine(line);
        }

        public List<string> Result()
        {
            List<string> resultLines = new List<string>();
            if (Args[0].StartsWith("r"))
            {
                resultLines.Add("push debug");
                int i;
                for (i = 0; i < Args[1].Length; i++)
                {
                    resultLines.Add($"mov debug,{(int)Args[1][i]}");
                    resultLines.Add($"strb debug,{Args[0]},{i}");
                }
                resultLines.Add($"mov debug,0");
                resultLines.Add($"strb debug,{Args[0]},{i}");
                resultLines.Add("pop debug");
            }
            else
                throw new VMException(VMFault.InvalidArgs, "伪指令di_strstring，参数1必须为寄存器");

            return resultLines;
        }
    }
}
