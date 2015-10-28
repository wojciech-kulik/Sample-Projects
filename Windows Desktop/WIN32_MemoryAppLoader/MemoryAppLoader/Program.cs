using System;
using System.IO;

namespace MemoryAppLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes(@"..\..\..\..\Debug\SampleApp.exe");
            CMemoryExecute.Run(bytes, @"C:\Windows\Microsoft.NET\Framework\v2.0.50727\vbc.exe");
            Console.ReadKey();
        }
    }
}
