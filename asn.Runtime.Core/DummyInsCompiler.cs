using asn.Runtime.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace asn.Runtime.Core
{
    /// <summary>
    /// 伪指令翻译器
    /// </summary>
    public class DummyInsCompiler
    {
        public DummyInsCompiler() { }
        private Dictionary<string, IDummyInstructions> DummyInsMap = new Dictionary<string, IDummyInstructions>();

        public void Load(string pluginsDir)
        {
            DirectoryInfo pluginsDirInfo = new DirectoryInfo(pluginsDir);
            List<FileInfo> pluginList = pluginsDirInfo.GetFiles("*.dll").ToList();
            int pluginIndex = 0;
            pluginList.ForEach(x =>
            {
                Assembly assembly = Assembly.LoadFile(x.FullName);
                Type[] types = assembly.GetTypes();
                List<Type> pluginTypes = types.Where(p => typeof(IDummyInstructions).IsAssignableFrom(p) && !p.IsInterface && p.IsClass && !p.IsAbstract).ToList();
                foreach (Type t in pluginTypes)
                {
                    IDummyInstructions plugin = (IDummyInstructions)assembly.CreateInstance(t.FullName);
                    DummyInsMap.Add(t.Name, plugin);
                    pluginIndex++;
                }
            });
        }

        public List<string> Compiler(List<string> Codes)
        {
            List<string> destCodes = new List<string>();

            for (int i = 0; i < Codes.Count; i++)
            {
                string[] FirstParse = Codes[i].ToLower().Split(' ');
                IDummyInstructions di = null;
                if (DummyInsMap.TryGetValue(FirstParse[0], out di))
                {
                    di.Input(Codes[i], Codes, i);
                    destCodes.AddRange(di.Result());
                }
                else
                {
                    destCodes.Add(Codes[i].ToLower());
                }
            }
            return destCodes;
        }
    }
}
