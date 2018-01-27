﻿using asn.Runtime.Core.Common;
using asn.Runtime.Interface;
using asn.Runtime.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asn.Runtime.Core
{
    /// <summary>
    /// 内存
    /// </summary>
    public class Memory
    {
        private int[] memoryPool;
        private int memorySize;
        private MemoryAttribute[] memoryAttribute;
        private int ParamBaseAddress = 0;
        /// <summary>
        /// 内存大小，每个内存占据32位（4byte）
        /// </summary>
        /// <param name="MemorySize"></param>
        public Memory(int MemorySize)
        {
            memoryPool = new int[MemorySize];
            memoryAttribute = new MemoryAttribute[MemorySize];
            for (int i = 0; i < MemorySize; i++)
            {
                memoryAttribute[i] = MemoryAttribute.DATA;
            }
            this.memorySize = MemorySize;
        }
        public Memory()
        {
            memoryPool = new int[1024];
            memoryAttribute = new MemoryAttribute[1024];
            for (int i = 0; i < 1024; i++)
            {
                memoryAttribute[i] = MemoryAttribute.DATA;
            }
            this.memorySize = 1024;
        }

        public Memory(Memory memory)
        {
            memoryPool = new int[memory.memorySize];
            memoryAttribute = new MemoryAttribute[memory.memorySize];
            for (int i = 0; i < 1024; i++)
            {
                memoryAttribute[i] = memory.memoryAttribute[i];
                memoryPool[i] = memory.memoryPool[i];
            }
            ParamBaseAddress = memory.ParamBaseAddress;
            this.memorySize = 1024;
        }

        /// <summary>
        /// 从内存中获取某个地址的数据
        /// </summary>
        /// <param name="Address">地址</param>
        /// <returns>数据</returns>
        public int Read(int Address)
        {
            if (Address < 0 || Address > 1024)
                throw new VMException(VMFault.InvalidAddr);
            return memoryPool[Address];
        }

        /// <summary>
        /// 写数据到某个地址
        /// </summary>
        /// <param name="Address">地址</param>
        /// <param name="Value">值</param>
        /// <param name="Attr">属性</param>
        public void Write(int Address, int Value, MemoryAttribute Attr = MemoryAttribute.DATA)
        {
            if (Address < 0 || Address > 1024)
                throw new VMException(VMFault.InvalidAddr);
            if (memoryAttribute[Address] != MemoryAttribute.DATA)
                throw new VMException(VMFault.ReadOnly);
            memoryPool[Address] = Value;
            memoryAttribute[Address] = Attr;
            if (Attr == MemoryAttribute.CODE)
                ParamBaseAddress = Address + 1;
        }

        /// <summary>
        /// 从内存中读取一个指令
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public OperatorLine ReadOperator(int Address)
        {
            if (Address < 0 || Address > 1024)
                throw new VMException(VMFault.InvalidAddr);
            if (memoryAttribute[Address] != MemoryAttribute.CODE)
                throw new VMException(VMFault.InvalidAddr);

            OperatorLine opt = new OperatorLine();
            opt.opt = memoryPool[Address];
            opt.args[0] = memoryPool[Address * 4 + ParamBaseAddress];
            opt.args[1] = memoryPool[Address * 4 + ParamBaseAddress + 1];
            opt.args[2] = memoryPool[Address * 4 + ParamBaseAddress + 2];
            int dataType = memoryPool[Address * 4 + ParamBaseAddress + 3];
            // 1-数据 0-寄存器
            opt.argTypes[2] = (char)(dataType & 0x04);
            opt.argTypes[1] = (char)(dataType & 0x02);
            opt.argTypes[0] = (char)(dataType & 0x01);
            return opt;
        }
    }
}
