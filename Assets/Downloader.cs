using System;
using System.Runtime.InteropServices;
using UnityEngine;

// C++ 下载器接口类
public class Downloader
{
    //#if UNITY_IPHONE || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

#if (UNITY_IPHONE || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
    const string DLL_NAME = "__Internal";
#else
    const string DLL_NAME = "ts_downloader";
#endif

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Register_Log(Log_t f);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Log_t(IntPtr str);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ProgressCB_t(long downloaded, long total);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ResultCb_t(int ret);

    [AOT.MonoPInvokeCallback(typeof(Log_t))]
    public static void Log(IntPtr str)
    {
        Debug.Log(Marshal.PtrToStringAnsi(str));
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DownloadInfo
    {
        public int isStart;
        public int isEnd;
        // 结果(0.表示成功 -1表示已存在目标文件)
        public int result;
        // 当前下载的字节数
        public long downloaded;
        // 目标文件大小(字节)
        public long total;
    }

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Downloader_Init();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Downloader_Start(
        string url,
        string targetFilePath,
        string destMd5,
        long fileSise);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern DownloadInfo Downloader_GetInfo(string url);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool Downloader_HasTask(string url);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Downloader_RemoveTask(string url);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int BinPatch(
        string old_file,
        string patch_file,
        string dest_file);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CalcFileMd5(
        string file,
        byte[] file_md5);

    public static string GetMd5(string file)
    {
        byte[] buf = new byte[128];
        int ret = CalcFileMd5(file, buf);
        if (ret != 0) return null;
        var md5 = System.Text.Encoding.ASCII.GetString(buf).Substring(0, 32);
        return md5;
    }
}
