﻿using asn.Runtime.Core;
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"assembly-n virtual machine[{VirtualMachine.vmVersion}]");
            Console.WriteLine($"(c) 2017 Jay Ni。保留所有权利。");
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

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
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
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"<dir>   {x.Name}");
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
                Console.ForegroundColor = ConsoleColor.White;
                if (cmd.StartsWith("run"))
                {
                    codes = new List<string>();
                    compiler = new Compiler(optLoader, dummyInsCompiler);
                    vm = new VirtualMachine(optLoader);
                    string arg = cmd.Split(' ')[1];
                    compiler.LoadModule(codes, $"{currentDir.FullName}\\{arg}");
                    vm.Programing(compiler.Compile(codes));
                    vm.Run();
                }
                if(cmd.StartsWith("pwd"))
                    Console.WriteLine(currentDir.FullName);
                if (cmd.StartsWith("quit"))
                    break;
                if(cmd.StartsWith("show opt"))
                    PrintOperators(optLoader);
            }
        }
    }
}
