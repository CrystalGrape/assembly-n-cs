using asn.Runtime.Core.Common;
using asn.Runtime.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace asn.Runtime.Core
{
    public class Plugins
    {
        public Plugins() { }
        public Plugins(Plugins original)
        {
            foreach(var kv in original.OperatorMap)
            {
                OperatorMap.Add(kv.Key, kv.Value);
            }
            foreach (var kv in original.RunMap)
            {
                RunMap.Add(kv.Key, kv.Value);
            }
        }
        private Dictionary<string, int> OperatorMap = new Dictionary<string, int>();
        private Dictionary<int, IOperator> RunMap = new Dictionary<int, IOperator>();
        public void Load(string pluginsDir)
        {
            DirectoryInfo pluginsDirInfo = new DirectoryInfo(pluginsDir);
            List<FileInfo> pluginList = pluginsDirInfo.GetFiles("*.dll").ToList();
            int pluginIndex = 0;
            pluginList.ForEach(x =>
            {
                Assembly assembly = Assembly.LoadFile(x.FullName);
                Type[] types = assembly.GetTypes();
                List<Type> pluginTypes = types.Where(p => typeof(IOperator).IsAssignableFrom(p) && !p.IsInterface && p.IsClass && !p.IsAbstract).ToList();
                foreach (Type t in pluginTypes)
                {
                    IOperator plugin = (IOperator)assembly.CreateInstance(t.FullName);
                    OperatorMap.Add(t.Name, pluginIndex);
                    RunMap.Add(pluginIndex, plugin);
                    pluginIndex++;
                }
            });
        }

        private IVirtualMachine Runtime = null;
        /// <summary>
        /// 设置运行时
        /// </summary>
        /// <param name="Runtime"></param>
        public void SetRuntime(IVirtualMachine Runtime)
        {
            this.Runtime = Runtime;
        }

        public int GetOptCode(string code)
        {
            return OperatorMap[code];
        }

        public void Run(OperatorLine opt)
        {
            RunMap[opt.opt].Run(Runtime, opt.args, opt.argTypes);
        }

        public void ForEach(Action<KeyValuePair<string, int>, bool> action)
        {
            int count = 0;
            foreach (KeyValuePair<string, int> kv in OperatorMap)
            {
                count++;
                action(kv, count == OperatorMap.Count);
            }
        }
    }
}
