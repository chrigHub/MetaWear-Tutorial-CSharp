using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreExamples {
    class Program {
        static void Main(string[] args) {
            string[] abc = { "NetCoreExamples.OwnDataProcessor", "F5:8B:76:C8:4E:C8" };
            MainAsync(abc).Wait();
        }

        private static async Task MainAsync(string[] args) {
            var type = Type.GetType(args[0]);
            await (Task) type.GetMethod("RunAsync", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { args.TakeLast(args.Length - 1).ToArray() });
        }
    }
}