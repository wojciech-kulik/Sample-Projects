using System.IO;

namespace MemoryAppLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            File.Copy(@"..\..\..\SampleApp\bin\Debug\SampleDLL.dll", "SampleDLL.dll", true);
            var bytes = File.ReadAllBytes(@"..\..\..\SampleApp\bin\Debug\SampleApp.exe");
            MemoryUtils.RunFromMemory(bytes).Join();
        }
    }
}
