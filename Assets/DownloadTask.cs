using UnityEngine;
using System.IO;
using static Downloader;

public class DownloadTask
{
    // 下载地址
    public string Url;
    // 本地存放地址(相对可写路径Application.persistentDataPath)
    public string LocalPath;
    // 文件Md5
    public string Md5;
    // 文件大小
    public long FileSize;

    // 重试次数
    public int RetryTimes;

    public DownloadInfo Info = new DownloadInfo();
    // 本地绝对路径
    private string localFullPath;

    // 创建文件夹(可以创建任意深度的文件 如/a/b/c)
    private void mkDirs(string dir)
    {
        if (dir.Length == 0 || dir.Equals("~") || dir.Equals("/")) return;
        var parent = dir.Substring(0, dir.LastIndexOf("/"));
        if (!Directory.Exists(parent))
        {
            mkDirs(parent);
        }
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    // 检查并创建文件夹
    private void checkDirs()
    {
        Debug.Assert(localFullPath != null);
        Debug.Assert(localFullPath.Length > 0);
        Debug.Assert(localFullPath.IndexOf('/') != -1);

        var dir = localFullPath.Substring(0, localFullPath.LastIndexOf('/'));
        mkDirs(dir);
    }

    public void Start()
    {
        Debug.Assert(Url != null && Url.Length > 0);
        Debug.Assert(LocalPath != null && LocalPath.Length > 0);
        Debug.Assert(Md5 != null && Md5.Length > 0);

        if (LocalPath?.Length > 0 && LocalPath[0] != '/')
        {
            localFullPath = Application.streamingAssetsPath + "/" + LocalPath;
        }
        else
        {
            localFullPath = LocalPath;
        }
        localFullPath = localFullPath.Replace("\\", "/");
        checkDirs();

        int ret = Downloader.Downloader_Start(
            Url,
            localFullPath,
            Md5,
            FileSize
            );
        Debug.Log("start:" + ret);
        // 目标文件已经存在，且md5校验通过
        if (ret > 0)
        {
            Info.isStart = 1;
            Info.isEnd = 1;
            Info.result = 0;
        }
        else if (ret < 0)
        {
            // 创建下载任务失败
            Info.isStart = 1;
            Info.isEnd = 1;
            Info.result = ret;
        }
        Info.total = FileSize;
    }

    public int Update()
    {
        if (Downloader.Downloader_HasTask(Url))
        {
            Info = Downloader.Downloader_GetInfo(Url);
            if (Info.isEnd != 0)
            {
                Downloader.Downloader_RemoveTask(Url);
                if (Info.result != 0 && RetryTimes-- > 0)
                {
                    File.Delete(localFullPath);
                    Start();
                    return 0;
                }
                return 1;
            }
        }
        else
        {
            // 未找到任务
            return -1;
        }
        return 0;
    }
}
