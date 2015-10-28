using System.Reflection;
using System.Threading;

namespace MemoryAppLoader
{
    public static class MemoryUtils
    {
        public static Thread RunFromMemory(byte[] bytes)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                var assembly = Assembly.Load(bytes);
                MethodInfo method = assembly.EntryPoint;
                if (method != null)
                {
                    method.Invoke(null, null);
                }
            }));

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return thread;
        }
    }
}
