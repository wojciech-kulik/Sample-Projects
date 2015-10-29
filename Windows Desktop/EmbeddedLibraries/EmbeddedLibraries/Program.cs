using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace EmbeddedLibraries
{
    static class Program
    {
        private static Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        private static string[] EmbeddedLibraries = ExecutingAssembly.GetManifestResourceNames().Where(x => x.EndsWith(".dll")).ToArray();

        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Get assembly name
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";

            // Get resource name
            var resourceName = EmbeddedLibraries.FirstOrDefault(x => x.EndsWith(assemblyName));
            if (resourceName == null)
            {
                return null;
            }

            // Load assembly from resource
            using (var stream = ExecutingAssembly.GetManifestResourceStream(resourceName))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return Assembly.Load(bytes);
            }
        }
    }
}
