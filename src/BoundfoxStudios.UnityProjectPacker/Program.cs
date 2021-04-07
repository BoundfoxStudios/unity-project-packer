using System;
using System.Threading.Tasks;

namespace BoundfoxStudios.UnityProjectPacker
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("BoundfoxStudios :: Unity Project Packer");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("\nThis little tool helps to create a zip archive of your Unity project for support purposes.");
            Console.WriteLine("After packing, please upload it either directly to our Discord Server,");
            Console.WriteLine("or, if the package is too big, upload it to WeTransfer.com");
            Console.WriteLine();

            var unityProjectPacker = new UnityProjectPacker();
            unityProjectPacker.Log += Console.WriteLine;

            var currentPath = args.Length > 0 ? args[0] : AppContext.BaseDirectory;

            if (string.IsNullOrWhiteSpace(currentPath))
            {
              Console.WriteLine("Could not determine current path.");
              return 1;
            }
            
            return (int) await unityProjectPacker.Pack(currentPath);
        }
    }
}
