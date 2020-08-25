using asn.Runtime.Core.Common;
using asn.Runtime.Interface.Common;
using System.Collections.Generic;
using System.IO;

namespace asn.Runtime.Core
{
    public class Compiler
    {
        private Plugins optLoader = null;
        private DummyInsCompiler dummyInsCompiler = null;
        public Compiler(Plugins optLoader,DummyInsCompiler dummyInsCompiler)
        {
            this.optLoader = optLoader;
            this.dummyInsCompiler = dummyInsCompiler;
            RegisterMap["r0"] = 0x56000000;
            RegisterMap["r1"] = 0x56000001;
            RegisterMap["r2"] = 0x56000002;
            RegisterMap["r3"] = 0x56000003;
            RegisterMap["r4"] = 0x56000004;
            RegisterMap["r5"] = 0x56000005;
            RegisterMap["r6"] = 0x56000006;
            RegisterMap["r7"] = 0x56000007;
            RegisterMap["r8"] = 0x56000008;
            RegisterMap["r9"] = 0x56000009;
            RegisterMap["r10"] = 0x5600000A;
            RegisterMap["r11"] = 0x5600000B;
            RegisterMap["r12"] = 0x5600000C;
            RegisterMap["r13"] = 0x5600000D;
            RegisterMap["r14"] = 0x5600000E;
            RegisterMap["r15"] = 0x5600000F;
            RegisterMap["pc"] = 0x5600000A;
            RegisterMap["lr"] = 0x5600000B;
            RegisterMap["sp"] = 0x5600000C;
            RegisterMap["cpsr"] = 0x5600000D;
            RegisterMap["debug"] = 0x5600000E;
            RegisterMap["icon"] = 0x5600000F;
        }
        /// <summary>
        /// 段映射
        /// </summary>
        private Dictionary<string, int> SectionMap = new Dictionary<string, int>();
        /// <summary>
        /// 寄存器映射
        /// </summary>
        private Dictionary<string, int> RegisterMap = new Dictionary<string, int>();

        /// <summary>
        /// 预编译
        /// </summary>
        /// <param name="Codes">代码行</param>
        /// <returns>代码集</returns>
        public List<string> PreCompile(List<string> Codes)
        {
            List<string> destCodes = new List<string>();
            int pc = 0;

            for (int i = 0; i < Codes.Count; i++)
            {
                if (Codes[i].ToLower().StartsWith("section"))
                {
                    string sectionName = Codes[i].Substring(8, Codes[i].Length - 8);
                    sectionName = sectionName.Substring(0, sectionName.Length - 1);
                    if (SectionMap.ContainsKey(sectionName))
                        throw new VMException(VMFault.DuplicateDefinition, $"重复定义的section,{sectionName}");
                    SectionMap[sectionName] = pc;
                    continue;
                }

                if (Codes[i].ToLower().StartsWith(";"))
                    continue;
                destCodes.Add(Codes[i]);
                pc = destCodes.Count;
            }
            return destCodes;
        }

        /// <summary>
        /// 解析行
        /// </summary>
        /// <param name="Line">代码行</param>
        /// <returns>一条指令</returns>
        public OperatorLine ParseLine(string Line)
        {
            string OpCode = "";
            string[] Args = { "", "", "" };
            //string[] FirstParse = Line.Split(' ');
            int index = 0;
            for (int i = 0; i < (int)Line.Length; i++)
            {
                if (index == 0 && Line.ToCharArray()[i] == ' ')
                {
                    index++;
                    continue;
                }
                if (Line.ToCharArray()[i] == ',')
                {
                    index++;
                    continue;
                }

                if (index == 0)
                    OpCode += Line.ToCharArray()[i];
                else if (index > 3)
                    throw new VMException(VMFault.ArgsError, $"指令的参数最多三个:{Line}");
                else
                    Args[index - 1] += Line.ToCharArray()[i];
            }
            OperatorLine CodeLine = new OperatorLine();
            CodeLine.opt = optLoader.GetOptCode(OpCode);
            if (CodeLine.opt == -1)
                throw new VMException(VMFault.InvalidInstructions, $"无效的指令:{Line}");

            for (int i = 0; i < index; i++)
            {

                if (SectionMap.ContainsKey(Args[i]))
                {
                    CodeLine.args[i] = SectionMap[Args[i]];
                    CodeLine.argTypes[i] = 'd';
                }
                else if (RegisterMap.ContainsKey((Args[i])))
                {
                    CodeLine.args[i] = RegisterMap[Args[i]];
                    CodeLine.argTypes[i] = 'r';
                }
                else
                {
                    if (int.TryParse(Args[i], out CodeLine.args[i]))
                        CodeLine.argTypes[i] = 'd';
                    else
                        throw new VMException(VMFault.ArgsError, $"参数错误：{Line}->参数{i + 1}");
                }
            }
            return CodeLine;
        }

        private List<string> loadModules = new List<string>();
        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="OriginalCode"></param>
        /// <param name="moduleName"></param>
        public void LoadModule(List<string> OriginalCode, string moduleName, string envPath)
        {
            //不重复加载模块
            if (loadModules.Contains(moduleName))
                return;
            loadModules.Add(moduleName);

            string modulePath = string.Copy(moduleName);
            int envPathPos = moduleName.LastIndexOf('.');
            string subDir = "";
            string mn = modulePath;
            if (envPathPos != -1)
            {
                subDir = modulePath.Substring(0, envPathPos);
                mn = modulePath.Substring(envPathPos + 1, modulePath.Length - envPathPos - 1);
            }
            subDir = subDir.Replace('.', '\\');

            envPath = Path.Combine(envPath, subDir);
            modulePath = Path.Combine(envPath, $"{mn}.asn");

            if (!File.Exists(modulePath))
                throw new VMException(VMFault.FileNotFound, $"未找到模块:{moduleName}");
            FileStream fs = File.OpenRead(modulePath);
            StreamReader sr = new StreamReader(fs);
            try
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;
                    line = line.Trim();
                    line = line.Trim('\t');
                    line = line.Trim('\n');
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (line.ToLower().StartsWith("import"))
                    {
                        string newModule = line;
                        newModule = line.Substring(7, line.Length - 7);
                        LoadModule(OriginalCode, newModule, envPath);
                        continue;
                    }
                    OriginalCode.Add(line);
                }
            }
            finally
            {
                sr.Dispose();
                fs.Dispose();
            }
        }

        /// <summary>
        /// 编译
        /// </summary>
        /// <param name="OriginalCode"></param>
        public List<OperatorLine> Compile(List<string> OriginalCode)
        {
            List<OperatorLine> Codes = new List<OperatorLine>();
            OriginalCode = dummyInsCompiler.Compiler(OriginalCode);
            OriginalCode = PreCompile(OriginalCode);
            for (int i = 0; i < (int)OriginalCode.Count; i++)
            {
                Codes.Add(ParseLine(OriginalCode[i].ToLower()));
            }
            return Codes;
        }
    }
}
