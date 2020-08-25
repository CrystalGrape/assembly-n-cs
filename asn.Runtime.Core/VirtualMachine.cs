using asn.Runtime.Core.Common;
using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace asn.Runtime.Core
{
    public class VirtualMachine : IVirtualMachine
    {
        public const string vmVersion = "V0.1.2";
        private static int virtualMachineIdIndex = 0;
        public int VirtualMachineId { get; private set; } 
        /// <summary>
        /// 内存
        /// </summary>
        private Memory Memory;
        #region 寄存器定义
        public const int RegisterBaseAddr = 0x56000000;
        private int[] Register = new int[17];
        public int pc { get { return Register[10]; } set { Register[10] = value; } }
        public int lr { get { return Register[11]; } set { Register[11] = value; } }
        public int sp { get { return Register[12]; } set { Register[12] = value; } }
        public int cpsr { get { return Register[13]; } set { Register[13] = value; } }
        public int debug { get { return Register[14]; } set { Register[14] = value; } }
        public int icon { get { return Register[15]; } set { Register[15] = value; } }
        public int offset { get { return Register[16]; } set { Register[16] = value; } }  //偏移地址，由于低地址内存用于存储指令集，所以访问时必须偏移一个地址
        #endregion

        private Plugins optLoader = null;
        public VirtualMachine(Plugins optLoader)
        {
            this.optLoader = optLoader;
            optLoader.SetRuntime(this);
            Memory = new Memory();
            VirtualMachineId = virtualMachineIdIndex++;
        }
        public VirtualMachine(VirtualMachine vm)
        {
            this.optLoader = new Plugins(vm.optLoader);
            optLoader.SetRuntime(this);
            this.offset = vm.offset;
            Memory = new Memory(vm.Memory);
            for (int i = 0; i < 17; i++)
                Register[i] = vm.Register[i];
            VirtualMachineId = virtualMachineIdIndex++;
        }

        public int Read(int Address)
        {
            if (Address >= RegisterBaseAddr && Address <= RegisterBaseAddr + 14)
                return Register[Address - RegisterBaseAddr];
            else
            {
                Address += offset;
                return Memory.Read(Address);
            }
        }
        public void Write(int Address, int Value, MemoryAttribute Attr = MemoryAttribute.DATA)
        {
            if (Address >= RegisterBaseAddr && Address <= RegisterBaseAddr + 15)
                Register[Address - RegisterBaseAddr] = Value;
            else
            {
                Address += offset;
                Memory.Write(Address, Value, Attr);
            }
        }

        /// <summary>
        /// 烧录程序
        /// </summary>
        public void Programing(List<OperatorLine> Codes)
        {
            int BaseAddress = 0;
            for (int i = 0; i < Codes.Count; i++)
            {
                Write(BaseAddress + i, Codes[i].opt, MemoryAttribute.CODE);
                Write(BaseAddress + Codes.Count + i * 4, Codes[i].args[0], MemoryAttribute.PARAM);
                Write(BaseAddress + Codes.Count + i * 4 + 1, Codes[i].args[1], MemoryAttribute.PARAM);
                Write(BaseAddress + Codes.Count + i * 4 + 2, Codes[i].args[2], MemoryAttribute.PARAM);
                int data = 0;
                for(int j = 0; j < 3; j++)
                {
                    if (Codes[i].argTypes[j] == 'd')    //数据
                        data |= 0x01 << j;
                    else
                        data &= ~(0x01 << j);
                }
                Write(BaseAddress + Codes.Count + i * 4 + 3, data, MemoryAttribute.PARAM);
            }
            offset = Codes.Count * 5; ;
            sp = offset;            
        }

        private int operatorCount = 0;
        public void Run()
        {
            pc = 0;
            lr = 0;
            while (true)
            {
                try
                {
                    //不在中断处理中，使能中断，指令数达到
                    if ((icon & 0x02) == 0 && (icon & 0x01) != 0 && operatorCount == 40)
                    {
                        operatorCount = 0;
                        icon |= 0x02;
                        lr = pc;
                        pc = 1;
                    }
                    //没有在中断中
                    else if ((icon & 0x02) == 0 && (icon & 0x01) != 0)
                        operatorCount++;
                    OperatorLine code = Memory.ReadOperator(pc++);
                    optLoader.Run(code);
                }
                catch(VMException e)
                {
                    if (e.VMFaultException == VMFault.NormalExit)
                        break;
                    else
                        throw e;
                }
            }
        }

        public void Continue(int pcAddr)
        {
            pc = pcAddr;
            while (true)
            {
                try
                {
                    //不在中断处理中，使能中断，指令数达到
                    if ((icon & 0x02) == 0 && (icon & 0x01) != 0 && operatorCount == 40)
                    {
                        operatorCount = 0;
                        icon |= 0x02;
                        lr = pc;
                        pc = 1;
                    }
                    //没有在中断中
                    else if ((icon & 0x02) == 0 && (icon & 0x01) != 0)
                        operatorCount++;
                    OperatorLine code = Memory.ReadOperator(pc++);
                    optLoader.Run(code);
                }
                catch (VMException e)
                {
                    if (e.VMFaultException == VMFault.NormalExit)
                        break;
                    throw e;
                }
            }
        }

        /// <summary>
        /// 从当前虚拟机拷贝一个新的虚拟机
        /// </summary>
        /// <returns></returns>
        public IVirtualMachine CopyVM()
        {
            VirtualMachine vm = new VirtualMachine(this);
            return vm;
        }

        /// <summary>
        /// 转储虚拟机
        /// </summary>
        /// <param name="stream"></param>
        public void Dump(Stream stream)
        {
            //转储寄存器
            for(var i = 0; i < 17; i++)
            {
                byte[] registerBytes = BitConverter.GetBytes(Register[i]);
                stream.Write(registerBytes, 0, registerBytes.Length);
            }
            //转储内存
            Memory.Dump(stream);
            stream.Flush();
        }

        public void Burn(Stream stream)
        {
            //转储寄存器
            for (var i = 0; i < 17; i++)
            {
                byte[] registerBytes = new byte[4];
                stream.Read(registerBytes, 0, 4);
                Register[i] = BitConverter.ToInt32(registerBytes, 0);
            }
            //转储内存
            Memory.Burn(stream);
        }
    }
}
