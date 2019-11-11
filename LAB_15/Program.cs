﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection.Emit;
using System.IO;

namespace LAB_15
{
    class Program
    {
        static AutoResetEvent waitHandler = new AutoResetEvent(true), waitHandler2 = new AutoResetEvent(true);
        static Thread myThreadEven, myThreadNotEven;

        private static void Main()
        {
            WriteColoredLine(("").PadLeft(54, '-') + " First Task " + ("").PadLeft(54, '-'), ConsoleColor.Green);

            foreach (Process item in Process.GetProcesses())
            {
                try
                {
                    WriteColoredLine("*", ConsoleColor.Blue);
                    Console.WriteLine($"ID: {item.Id}\nName: {item.ProcessName}\nStart time: {item.StartTime}\nCPU usage time: {item.TotalProcessorTime}");
                }
                catch (Exception e)
                {
                    WriteColoredLine($"{e.Message}: {item.ProcessName}", ConsoleColor.Red);
                }
            }

            WriteColoredLine(("").PadLeft(54, '-') + " Fifth Task " + ("").PadLeft(54, '-'), ConsoleColor.Green);

            TimerCallback tm = new TimerCallback(timerT);
            Timer timer = new Timer(tm, 5, 1000, 2000);

            WriteColoredLine(("").PadLeft(53, '-') + " Second Task " + ("").PadLeft(54, '-'), ConsoleColor.Green);

            AppDomain domain = AppDomain.CurrentDomain;
            Console.WriteLine($"Name: {domain.FriendlyName}\nID: {domain.Id}\nConfiguration:\n\tConfigurationFile: {domain.SetupInformation.ConfigurationFile}\n" +
                $"\tAplication Base: {domain.SetupInformation.ApplicationBase}");
            Console.WriteLine("All assemblies:");
            var Assemblies = from asembl in domain.GetAssemblies()
                             orderby asembl.GetName().Name
                             select asembl;
            foreach (var asembl in Assemblies)
            {
                Console.WriteLine($"\tAssembly name: {asembl.GetName().Name}\tVersion: {asembl.GetName().Version}");
            }

            WriteColoredLine(("").PadLeft(52, '-') + " Created Domain " + ("").PadLeft(52, '-'), ConsoleColor.Green);

            AppDomain newD = AppDomain.CreateDomain("NeW");
            
            newD.Load("ClassLibraryFor15LAB");
            Console.WriteLine($"Name: {newD.FriendlyName}");
            AppDomain.Unload(newD);

            WriteColoredLine(("").PadLeft(54, '-') + " Third Task " + ("").PadLeft(54, '-'), ConsoleColor.Green);

            Thread myThread = new Thread(new ParameterizedThreadStart(Count));
            Console.WriteLine("Enter count: ");
            myThread.Start(Convert.ToInt32(Console.ReadLine()));
            Thread.Sleep(4000);
            myThread.Resume();
            myThread.Join();

            WriteColoredLine(("").PadLeft(54, '-') + " Fourth Task " + ("").PadLeft(53, '-'), ConsoleColor.Green);

            StreamWriter st = new StreamWriter("EvenNotEven.txt");
            myThreadEven = new Thread(new ParameterizedThreadStart(Even));
            myThreadNotEven = new Thread(new ParameterizedThreadStart(NotEven));
            myThreadEven.Start(st);
            myThreadNotEven.Priority = ThreadPriority.Lowest;
            myThreadNotEven.Start(st);
            myThreadNotEven.Join();
            Console.WriteLine();

            Thread first = new Thread(new ThreadStart(EvenC));
            Thread second = new Thread(new ThreadStart(NotEvenC));
            first.Start();
            second.Start();


            

            //waitHandler.WaitOne();
            //if (myThreadEven.IsAlive && myThreadNotEven.IsAlive)
            //  waitHandler.Set();

            st.Close();
        }

        private static void WriteColoredLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Count(object n)
        {
            Thread.CurrentThread.Suspend();
            StreamWriter stream = new StreamWriter("count.txt");
            for (int i = 1; i <= (int)n; i++)
            {
                Console.WriteLine($"Thread: {Thread.CurrentThread}");
                Console.WriteLine($"Thread State: {Thread.CurrentThread.ThreadState}");
                Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("Второй поток:");
                //Thread.CurrentThread.Interrupt();
                stream.WriteLine(i);
                Console.WriteLine(i);
                //Thread.CurrentThread.Resume();
                Thread.Sleep(400);
            }
            stream.Close();
        }

        public static void Even(object st)
        {
            
            var n = 40;
            //Console.WriteLine("Четные числа: ");
            //((StreamWriter)st).WriteLine("Четные числа: ");
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                {
                    Console.Write(i + ", ");
                    ((StreamWriter)st).Write(i + ", ");
                }
            }
            Console.WriteLine();
            ((StreamWriter)st).WriteLine();
            Thread.CurrentThread.Abort();
            
        }

        public static void NotEven(object st)
        {
            myThreadEven.Join();
            var n = 40;
            //Console.WriteLine("Нечетные числа: ");
            //((StreamWriter)st).WriteLine("Нечетные числа: ");
            for (int i = 0; i < n; i++)
            {
                if (i % 2 != 0)
                {
                    Console.Write(i + ", ");
                    ((StreamWriter)st).Write(i + ", ");
                }
            }
            Console.WriteLine();
            ((StreamWriter)st).WriteLine();
            Thread.CurrentThread.Abort();
            
        }

        public static void EvenC()
        {

            var n = 40;
            for (int i = 0; i < n; i++)
            {
                waitHandler2.WaitOne();
                if (i % 2 == 0)
                    Console.Write(i + ", ");
                waitHandler.Set();
            }

        }

        public static void NotEvenC()
        {
            var n = 40;
            for (int i = 0; i < n; i++)
            {
                waitHandler.WaitOne();
                if (i % 2 != 0)
                    Console.Write(i + ", ");
                waitHandler2.Set();
            }

        }

        public static void timerT(object n)
        {
            int x = (int)n;
            for (int i = 1; i < 9; i++, x++)
            {
                Console.WriteLine($"{x * i}");
            }
        }
    }
}
