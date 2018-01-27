using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asn.Runtime.Core.Common
{
    public class OperatorLine
    {
        public int opt { get; set; }
        public int[] args { get; set; } = new int[3];
        public char[] argTypes { get; set; } = new char[3];
    }
}
