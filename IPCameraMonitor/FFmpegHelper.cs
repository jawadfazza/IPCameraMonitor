using System;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

public static class FFmpegHelper
{
    private static bool _isInitialized = false;

    public static void Initialize()
    {
        if (_isInitialized) return;

        LoadFFmpegLibraries();

        //ffmpeg.avdevice_register_all();
        //ffmpeg.avformat_network_init();

        _isInitialized = true;
    }

    public static void LoadFFmpegLibraries()
    {
        string ffmpegRootPath = AppDomain.CurrentDomain.BaseDirectory;
        string ffmpegLibrariesPath = Path.Combine(ffmpegRootPath, "");

        if (!Directory.Exists(ffmpegLibrariesPath))
        {
            throw new DirectoryNotFoundException($"FFmpeg libraries not found in path: {ffmpegLibrariesPath}");
        }

        string[] libraries = {
            "avcodec-57.dll",
            "avdevice-57.dll",
            "avfilter-6.dll",
            "avformat-57.dll",
            "avutil-55.dll",
            "postproc-54.dll",
            "swresample-2.dll",
            "swscale-4.dll"
        };

        foreach (var library in libraries)
        {
            string libraryPath = Path.Combine(ffmpegLibrariesPath, library);
            if (!File.Exists(libraryPath))
            {
                throw new FileNotFoundException($"FFmpeg library not found: {libraryPath}");
            }

            LoadLibrary(libraryPath);
        }
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr LoadLibrary(string lpFileName);
}
