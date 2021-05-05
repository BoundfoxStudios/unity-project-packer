using System;
using System.Diagnostics;
using System.IO;
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

      var currentPath = DetermineDirectory(args);

      if (string.IsNullOrWhiteSpace(currentPath))
      {
        Console.WriteLine("Could not determine current path.");
        Console.ReadLine();
        return 1;
      }

      var exitCode = (int) await unityProjectPacker.Pack(currentPath);
      
      Console.ReadLine();

      return exitCode;
    }

    private static string DetermineDirectory(string[] args)
    {
      if (args.Length > 0)
      {
        return args[0];
      }

      var mainModule = Process.GetCurrentProcess().MainModule;

      if (mainModule?.FileName != null)
      {
        return Path.GetDirectoryName(mainModule.FileName);
      }

      return AppContext.BaseDirectory;
    }
  }
}
