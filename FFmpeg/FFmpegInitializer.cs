using System;
using System.IO;
using System.Reflection;
using FFMpegCore;

public class FFmpegInitializer
{
    public static void InitializeFFmpeg()
    {
        string tempPath = Path.Combine(Path.GetTempPath(), "FFmpeg");

        // Ensure the directory exists
        if (!Directory.Exists(tempPath))
        {
            Directory.CreateDirectory(tempPath);
        }

        // Extract ffmpeg.exe and ffprobe.exe from the embedded resources
        ExtractEmbeddedResource("YourNamespace.ffmpeg.exe", Path.Combine(tempPath, "ffmpeg.exe"));
        ExtractEmbeddedResource("YourNamespace.ffprobe.exe", Path.Combine(tempPath, "ffprobe.exe"));

        // Set the binary folder for FFMpegCore to the temp directory
        GlobalFFOptions.Configure(options => options.BinaryFolder = tempPath);
    }

    private static void ExtractEmbeddedResource(string resourceName, string outputPath)
    {
        // Check if the file already exists
        if (!File.Exists(outputPath))
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new Exception($"Resource {resourceName} not found.");
                }

                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
        }
    }
}
