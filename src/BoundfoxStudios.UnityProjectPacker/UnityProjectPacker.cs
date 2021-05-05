using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace BoundfoxStudios.UnityProjectPacker
{
  public class UnityProjectPacker
  {
    public enum PackResult
    {
      Ok = 0,
      CouldNotDetermineProjectName = 2, // 1 is used for path determination failure
      NotAValidUnityProject = 3,
    }

    public event Action<string, bool> Log;

    private string[] _unityFolders = new[] { "Assets", "ProjectSettings", "Packages" };

    public async Task<PackResult> Pack(string basePath)
    {
      OnLog($"Using {basePath} for trying to pack a Unity project.");

      var projectName = GetProjectName(basePath);

      if (string.IsNullOrWhiteSpace(projectName))
      {
        OnLog("Could not determine project name. Aborting...");
        return PackResult.CouldNotDetermineProjectName;
      }

      if (!IsUnityProject(basePath))
      {
        OnLog("Current directory is not a Unity project.");
        OnLog("Please move this tool to a Unity project, containing the directories: Assets, ProjectSettings and Packages.");
        OnLog("Aborting...");

        return PackResult.NotAValidUnityProject;
      }

      OnLog($"Current directory looks like a Unity project named {projectName}. Starting to zip...");

      var finalFileName = Path.Combine(basePath, $"{projectName}.zip");

      var sourceFiles = GetSourceFiles(basePath);

      var totalBytes = sourceFiles.Sum(file => file.Length);
      var bytesWritten = 0L;

      await using (var zipArchiveStream = new FileStream(finalFileName, FileMode.Create))
      {
        using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Create))
        {
          foreach (var file in sourceFiles)
          {
            var entryName = file.FullName.Substring(basePath.Length);

            var zipEntry = zipArchive.CreateEntry(entryName);

            zipEntry.LastWriteTime = file.LastWriteTime;

            await using (var fileStream = new FileStream(file.FullName, FileMode.Open))
            {
              await using (var zipEntryStream = zipEntry.Open())
              {
                await fileStream.CopyToAsync(zipEntryStream);

                bytesWritten += file.Length;

                OnLog($"Bytes written: {bytesWritten}/{totalBytes}", true);
              }
            }
          }
        }
      }

      OnLog("");
      OnLog("Done! Please upload the following file:");
      OnLog(finalFileName);
      return PackResult.Ok;
    }

    private string GetProjectName(string basePath)
    {
      if (basePath[^1] == Path.PathSeparator)
      {
        basePath = basePath[..^1];
      }

      return basePath.Split(Path.PathSeparator).Last();
    }

    private bool IsUnityProject(string basePath)
    {
      return _unityFolders.All(folder => Directory.Exists(Path.Combine(basePath, folder)));
    }

    private FileInfo[] GetSourceFiles(string basePath) =>
      _unityFolders.SelectMany(folder => new DirectoryInfo(Path.Combine(basePath, folder)).GetFiles("*", SearchOption.AllDirectories)).ToArray();

    protected virtual void OnLog(string message, bool inplace = false)
    {
      Log?.Invoke(message, inplace);
    }
  }
}
