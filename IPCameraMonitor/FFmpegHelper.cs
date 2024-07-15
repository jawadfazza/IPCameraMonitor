using FFmpeg.AutoGen;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

public static class FFmpegHelper
{
    private static bool _isInitialized = false;

    public static unsafe void Initialize()
    {
        if (_isInitialized) return;

        ffmpeg.RootPath = AppDomain.CurrentDomain.BaseDirectory;
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            LoadLibrariesWindows();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            LoadLibrariesLinux();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            LoadLibrariesOSX();
        }

        ///ffmpeg.avdevice_register_all();

        //ffmpeg.avformat_network_init();
        _isInitialized = true;
    }

    private static void LoadLibrariesWindows()
    {
        const string dllPath = "libs/win";
        LoadLibrary($"{dllPath}/avcodec-58.dll");
        LoadLibrary($"{dllPath}/avdevice-58.dll");
        LoadLibrary($"{dllPath}/avfilter-7.dll");
        LoadLibrary($"{dllPath}/avformat-58.dll");
        LoadLibrary($"{dllPath}/avutil-56.dll");
        LoadLibrary($"{dllPath}/postproc-55.dll");
        LoadLibrary($"{dllPath}/swresample-3.dll");
        LoadLibrary($"{dllPath}/swscale-5.dll");
    }

    private static void LoadLibrariesLinux()
    {
        const string soPath = "libs/linux";
        ffmpeg.avdevice_register_all();
        LoadLibrary($"{soPath}/libavcodec.so.58");
        LoadLibrary($"{soPath}/libavdevice.so.58");
        LoadLibrary($"{soPath}/libavfilter.so.7");
        LoadLibrary($"{soPath}/libavformat.so.58");
        LoadLibrary($"{soPath}/libavutil.so.56");
        LoadLibrary($"{soPath}/libpostproc.so.55");
        LoadLibrary($"{soPath}/libswresample.so.3");
        LoadLibrary($"{soPath}/libswscale.so.5");
    }

    private static void LoadLibrariesOSX()
    {
        const string dylibPath = "libs/osx";
        LoadLibrary($"{dylibPath}/libavcodec.58.dylib");
        LoadLibrary($"{dylibPath}/libavdevice.58.dylib");
        LoadLibrary($"{dylibPath}/libavfilter.7.dylib");
        LoadLibrary($"{dylibPath}/libavformat.58.dylib");
        LoadLibrary($"{dylibPath}/libavutil.56.dylib");
        LoadLibrary($"{dylibPath}/libpostproc.55.dylib");
        LoadLibrary($"{dylibPath}/libswresample.3.dylib");
        LoadLibrary($"{dylibPath}/libswscale.5.dylib");
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
}
