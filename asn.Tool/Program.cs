using asn.Runtime.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asn.Tool
{
    class Program
    {
        static void PrintOperators(Plugins optLoader)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("┌");
            Console.Write("─");
            Console.Write("─┬");
            Console.Write("─");
            Console.Write("─┐");
            Console.WriteLine();
            Console.Write("│");
            Console.Write("代码");
            Console.Write("│");
            Console.Write("指令│");
            Console.WriteLine();
            Console.Write("├");
            Console.Write("──┼──");
            Console.Write("┤");
            Console.WriteLine();
            optLoader.ForEach((kv, isEnd) =>
            {
                if (isEnd)
                {
                    Console.Write("│");
                    Console.Write($"{string.Format("{0:D4}", kv.Value)}");
                    Console.Write("│");
                    Console.Write($"{kv.Key.PadLeft(4, ' ')}│");
                    Console.WriteLine();
                    Console.Write("└");
                    Console.Write("──┴──");
                    Console.Write("┘");
                    Console.WriteLine();
                }
                else
                {
                    Console.Write("│");
                    Console.Write($"{string.Format("{0:D4}", kv.Value)}");
                    Console.Write("│");
                    Console.Write($"{kv.Key.PadLeft(4, ' ')}│");
                    Console.WriteLine();
                    Console.Write("├");
                    Console.Write("──┼──");
                    Console.Write("┤");
                    Console.WriteLine();
                }
            });
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void Main(string[] args)
        {
            Console.WriteLine($"assembly-n virtual machine {VirtualMachine.vmVersion}");
            Console.WriteLine($"(C) 2017 Jay Ni。保留所有权利。\r\n");
            DirectoryInfo currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            //加载伪指令插件
            DummyInsCompiler dummyInsCompiler = new DummyInsCompiler();
            dummyInsCompiler.Load("dummys/");
            //加载指令插件
            Plugins optLoader = new Plugins();
            optLoader.Load("plugins/");
            List<string> codes;
            Compiler compiler;
            VirtualMachine vm;
            ConsoleColor DefaultColor = Console.ForegroundColor;
            while (true)
            {
                Console.ForegroundColor = DefaultColor;
                Console.Write("asn>:");
                string cmd = Console.ReadLine();
                cmd = cmd.Trim(new char[] { '\n', ' ' });
                if (cmd.StartsWith("ls"))
                {
                    Console.WriteLine($"<类型>  名称");
                    currentDir.GetFileSystemInfos().ToList().ForEach(x =>
                    {
                        if (x is DirectoryInfo)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"<dir>   {x.Name}");
                        }
                        else if (x.FullName.ToLower().EndsWith(".asn"))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"<assemblyn>  {x.Name}");
                        }
                        else if (x.FullName.ToLower().EndsWith(".abin"))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"<binary>  {x.Name}");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"<file>  {x.Name}");
                        }
                    });
                }
                if (cmd.StartsWith("cd"))
                {
                    string arg = cmd.Split(' ')[1];
                    DirectoryInfo tmpDir;
                    if (arg.Contains(":"))
                        tmpDir = new DirectoryInfo(arg);
                    else
                        tmpDir = new DirectoryInfo($"{currentDir.FullName}\\{arg}");
                    if (!tmpDir.Exists)
                        Console.WriteLine("目录不存在");
                    else
                        currentDir = tmpDir;
                }
                Console.ForegroundColor = DefaultColor;
                if (cmd.StartsWith("run"))
                {
                    try
                    {
                        string arg = cmd.Split(' ')[1];
                        if (arg.EndsWith(".abin"))
                        {
                            vm = new VirtualMachine(optLoader);
                            string binfile = Path.Combine(currentDir.FullName, arg);
                            using(FileStream fs = File.Open(binfile, FileMode.Open))
                            {
                                vm.Burn(fs);
                            }
                            vm.Run();
                        }
                        else
                        {
                            codes = new List<string>();
                            compiler = new Compiler(optLoader, dummyInsCompiler);
                            vm = new VirtualMachine(optLoader);
                            compiler.LoadModule(codes, arg, currentDir.FullName);
                            vm.Programing(compiler.Compile(codes));
                            vm.Run();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                if (cmd.StartsWith("compile"))
                {
                    try
                    {
                        codes = new List<string>();
                        compiler = new Compiler(optLoader, dummyInsCompiler);
                        vm = new VirtualMachine(optLoader);
                        var parameters = cmd.Split(' ');
                        string arg = parameters[1];
                        string dest = "out.abin";
                        if (parameters.Length == 3)
                        {
                            dest = parameters[2];
                        }
                        
                        compiler.LoadModule(codes, arg, currentDir.FullName);
                        vm.Programing(compiler.Compile(codes));
                        string destFile = Path.Combine(currentDir.FullName, dest);
                        if (File.Exists(destFile))
                            File.Delete(destFile);
                        using(FileStream fs = File.Open(destFile, FileMode.CreateNew))
                        {
                            vm.Dump(fs);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                if (cmd.StartsWith("pwd"))
                    Console.WriteLine(currentDir.FullName);
                if (cmd.StartsWith("quit"))
                    break;
                if (cmd.StartsWith("show opt"))
                    PrintOperators(optLoader);
            }
        }
    }
}
