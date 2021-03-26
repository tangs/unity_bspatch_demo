using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections;

public class AssetsDownloader : MonoBehaviour
{
    private static Regex versionRegex = new Regex(@"^assets\.(\w+)\.v(\d+)$");
    private static bool inited = false;

    public struct FileInfo
    {
        public string Url;
        public string Md5;
        public long FileSize;
    }

    /**************************需要设置的参数(Start)****************************/
    // 模块名
    public string ModleName;
    // 完整包信息
    public Dictionary<int, FileInfo> FullAssetInfos = new Dictionary<int, FileInfo>();
    // 升级补丁信息
    public Dictionary<int, FileInfo> UpPatchInfos = new Dictionary<int, FileInfo>();
    // 降级补丁信息
    public Dictionary<int, FileInfo> DownPatchInfos = new Dictionary<int, FileInfo>();

    // 目标版本号
    public int DestVersion;
    // 是否屏蔽补丁下载(直接下载完整包)
    public bool DisablePatchMode;
    /**************************需要设置的参数(End)******************************/

    /**************************使用者不需要关系的参数(Start)**********************/
    private List<DownloadTask> tasks = new List<DownloadTask>();
    private string rootPath;
    private bool patchMode;
    // 是否升级模式
    private bool isUpgrade;
    private int curVersion;
    private int sIdx;
    private int dIdx;
    /**************************使用者不需要关系的参数(End)8**********************/

    public enum Mode
    {
        Wait = 0,
        DownloadFile = 1,
        MergeFile = 2,
        CheckMd5 = 3,
        Finish = 4,
    }

    public enum RetCode
    {
        Success = 0,
        PatchFail = 1,
        // 未找到目标版本信息
        DestFileInfoNotFound = 2,
        // 目标文件md5校验失败
        DestMd5CheckFail = 3,
    }


    /**************************用户读取的下载信息(Start)*************************/
    // 下载信息
    public Mode mode = Mode.Wait;
    // 总下载字节数
    public long totalBytes;
    // 已经下载的字节数
    public long downloadedBytes;
    // 总共需要合并的文件
    public int totalMergeFiles;
    // 已经合并的文件数
    public int mergedFiles;
    // 返回值(0.为成功,否则有异常)
    public RetCode retCode;
    // 错误消息
    public string errMsg = "";
    /**************************用户读取的下载信息(End)***************************/

    private int getBestMatchVersion()
    {
        int ret = -1;
        if (Directory.Exists(rootPath))
        {
            foreach (var file in Directory.GetFiles(rootPath))
            {
                var name = file.Substring(file.Replace("\\", "/").LastIndexOf("/") + 1);
                Match match = versionRegex.Match(name);
                if (match.Success && match.Groups[1].Value.Equals(ModleName))
                {
                    int version = int.Parse(match.Groups[2].Value);
                    //Debug.Log(name + ":" + version);
                    if (ret == -1 || Mathf.Abs(version - DestVersion) < Mathf.Abs(ret - DestVersion))
                    {
                        ret = version;
                    }
                }
            }
        }
        return ret;
    }

    // 获取完整包本地文件名
    private string getFullAssetsFilePath(int version)
    {
        return string.Format("{0}/assets.{1}.v{2}",
            rootPath, ModleName, version);
    }

    private string getPatchFilePath(bool isUpgrade, int version)
    {
        return string.Format("{0}/patch_{1}{2}", rootPath,
            isUpgrade ? "up" : "down", version);
    }

    // 检查是否有所有补丁可下载
    private bool hasAllPatches()
    {
        for (int i = sIdx; i <= dIdx; ++i)
        {
            if (isUpgrade)
            {
                if (!UpPatchInfos.ContainsKey(i)) return false;
            }
            else
            {
                if (!DownPatchInfos.ContainsKey(i)) return false;
            }
        }
        return true;
    }

    private long getAllPatchesSize()
    {
        long ret = 0;
        for (int i = sIdx; i <= dIdx; ++i)
        {
            if (isUpgrade)
            {
                if (!UpPatchInfos.ContainsKey(i)) ret += UpPatchInfos[i].FileSize;
            }
            else
            {
                if (!DownPatchInfos.ContainsKey(i)) ret += UpPatchInfos[i].FileSize;
            }
        }
        return ret;
    }

    private bool checkFileMd5(string file, string destMd5)
    {
        var fileMd5 = Downloader.GetMd5(file).ToLower();
        return fileMd5 != null && fileMd5.ToLower().Equals(destMd5.ToLower());
    }

    private void clearOtherAssetsBundles()
    {
        // clear other version
        if (Directory.Exists(rootPath))
        {
            List<string> rmFiles = new List<string>();
            foreach (var file in Directory.GetFiles(rootPath))
            {
                var name = file.Substring(file.Replace("\\", "/").LastIndexOf("/") + 1);
                Match match = versionRegex.Match(name);
                if (match.Success && match.Groups[1].Equals(ModleName))
                {
                    int version = int.Parse(match.Groups[2].Value);
                    Debug.Log(name + ":" + version);
                    if (version != DestVersion)
                    {
                        rmFiles.Add(file);
                    }
                }
            }
            foreach (var file in rmFiles)
            {
                File.Delete(file);
            }
        }
    }

    private bool patchToVersion(int version)
    {
        //var src = string.Format("{0}/ab.v{1}", rootPath, isUpgrade ? version - 1 : version + 1);
        //var dest = string.Format("{0}/ab.v{1}", rootPath, version);
        //var src = string.Format("{0}/assets.{1}.v{2}", rootPath, ModleName, isUpgrade ? version - 1 : version + 1);
        //var dest = string.Format("{0}/assets.{1}.v{2}", rootPath, ModleName, version);
        //var patch = string.Format("{0}/patch_{1}{2}", rootPath, isUpgrade ? "up" : "down", version);
        var src = getFullAssetsFilePath(isUpgrade ? version - 1 : version + 1);
        var dest = getFullAssetsFilePath(version);
        var patch = getPatchFilePath(isUpgrade, version);
        Debug.Log(string.Format("patch:{0},{1},{2}", src, dest, patch));
        int ret = Downloader.BinPatch(src, patch, dest);
        Debug.Log(string.Format("bin patch ret:{0}", ret));
        if (ret == 0)
        {
            if (File.Exists(src))
            {
                File.Delete(src);
            }
            if (File.Exists(patch))
            {
                File.Delete(patch);
            }
            return true;
        }
        return false;
    }

    private IEnumerator mergeFiles()
    {
        yield return null;
        if (isUpgrade)
        {
            for (int i = sIdx; i <= dIdx; ++i)
            {
                if (!patchToVersion(i))
                {
                    //retCode = 100;
                    //errMsg = string.Format("patch fail({0},{1})", ModleName, i);
                    //mode = Mode.Finish;
                    finish(RetCode.PatchFail,
                        string.Format("patch fail({0},{1})", ModleName, i));
                    yield break;
                }
                mergedFiles++;
                yield return null;
            }
        }
        else
        {
            for (int i = dIdx; i >= sIdx; --i)
            {
                if (!patchToVersion(i))
                {
                    //retCode = 100;
                    //errMsg = string.Format("patch fail({0},{1})", ModleName, i);
                    //mode = Mode.Finish;
                    finish(RetCode.PatchFail,
                        string.Format("patch fail({0},{1})", ModleName, i));
                    yield break;
                }
                mergedFiles++;
                yield return null;
            }
        }

        mode = Mode.CheckMd5;
    }

    public string GetDestFilePath()
    {
        return getFullAssetsFilePath(DestVersion);
    }

    private void finish(RetCode retCode, string errMsg)
    {
        this.retCode = retCode;
        this.errMsg = errMsg;
        mode = Mode.Finish;
        //Debug.Log(string.Format("finsh, retcode={0}, errMsg={1}",
        //    retCode, errMsg));
        clearOtherAssetsBundles();
        //if (retCode == 0) loadAsset();
    }

    public void Start()
    {
        if (!inited)
        {
            Downloader.Register_Log(Downloader.Log);
            Downloader.Downloader_Init();
            inited = true;
        }
        //DisablePatchMode = isFullToggle.isOn;
        //DestVersion = destVersion;

        // init
        mode = Mode.DownloadFile;
        downloadedBytes = 0L;
        totalBytes = 0L;
        mergedFiles = 0;
        totalMergeFiles = 0;
        retCode = 0;
        errMsg = "";
        var destVersion = DestVersion;

        //rootPath = Application.persistentDataPath + "/abs/" + ModleName;
        rootPath = Application.persistentDataPath + "/" + ModleName;

        if (!FullAssetInfos.ContainsKey(DestVersion))
        {
            finish(RetCode.DestFileInfoNotFound,
                    string.Format("未找到指定版本的完整包信息.({0},{1})",
                    ModleName, destVersion));
            return;
        }
        var destFileInfo = FullAssetInfos[destVersion];

        // 校验本地版本信息
        do
        {
            curVersion = getBestMatchVersion();
            Debug.Log("best version:" + curVersion);
            if (curVersion == -1 || (FullAssetInfos.ContainsKey(curVersion) &&
                checkFileMd5(getFullAssetsFilePath(curVersion), FullAssetInfos[curVersion].Md5)))
            {
                break;
            }
            var path = getFullAssetsFilePath(curVersion);
            Debug.Log(string.Format("local and remote dest file({0}) md5 check fail.", path));
            File.Delete(path);
        } while (true);

        if (curVersion == DestVersion)
        {
            var path = getFullAssetsFilePath(DestVersion);
            var md5 = Downloader.GetMd5(path).ToLower();
            Debug.Log("Has dest version assets bundle.");
            finish(RetCode.Success, "");
            return;
        }

        isUpgrade = curVersion < DestVersion;
        if (isUpgrade)
        {
            sIdx = curVersion + 1;
            dIdx = DestVersion;
        }
        else
        {
            sIdx = DestVersion;
            dIdx = curVersion - 1;
        }

        // 判断是否走纯补丁模式
        patchMode = !DisablePatchMode && curVersion != -1 && hasAllPatches() &&
            getAllPatchesSize() < destFileInfo.FileSize;

        tasks.Clear();
        if (patchMode)
        {
            totalMergeFiles = Mathf.Abs(dIdx - sIdx + 1);
            for (int i = sIdx; i <= dIdx; ++i)
            {
                var info = isUpgrade ? UpPatchInfos[i] : DownPatchInfos[i];
                var task = new DownloadTask();
                task.Url = info.Url;
                task.Md5 = info.Md5;
                task.FileSize = info.FileSize;
                //task.LocalPath = rootPath + "/patch_" + (isUpgrade ? "up" : "down") + i;
                task.LocalPath = getPatchFilePath(isUpgrade, i);
                task.RetryTimes = 3;
                task.Start();
                tasks.Add(task);
                totalBytes += task.FileSize;
            }
        }
        else
        {
            //var info = FullAssetInfos[destVersion];
            var task = new DownloadTask();
            task.Url = destFileInfo.Url;
            task.Md5 = destFileInfo.Md5;
            task.FileSize = destFileInfo.FileSize;
            //task.LocalPath = string.Format("{0}/ab.v{1}", rootPath, DestVersion);
            //task.LocalPath = string.Format("{0}/assets.{1}.v{2}", rootPath, ModleName, DestVersion);
            task.LocalPath = getFullAssetsFilePath(DestVersion);
            task.RetryTimes = 3;
            task.Start();
            tasks.Add(task);
            totalBytes += task.FileSize;
        }
    }

    public void Update()
    {
        bool disable = mode == Mode.Wait || mode == Mode.Finish;
        //panle.SetActive(!disable);
        if (disable) return;

        switch(mode)
        {
            case Mode.DownloadFile:
                {
                    //titleText.text = "下载中...";
                    //slider.value = downloadedBytes / (float)totalBytes;
                    bool hasWorkTask = false;
                    downloadedBytes = 0;
                    foreach (var task in tasks)
                    {
                        if (task.Info.isEnd == 0)
                        {
                            hasWorkTask = true;
                            task.Update();
                        }
                        downloadedBytes += task.Info.downloaded;
                    }
                    if (!hasWorkTask)
                    {
                        if (patchMode)
                        {
                            mode = Mode.MergeFile;
                            StartCoroutine(mergeFiles());
                        }
                        else
                        {
                            clearOtherAssetsBundles();
                            mode = Mode.CheckMd5;
                        }
                    }
                    //totalText.text = downloadedBytes + "/" + totalBytes;
                }
                break;
            case Mode.CheckMd5:
                {
                    //titleText.text = "校验文件...";
                    //totalText.text = "";
                    //slider.value = 1;
                    //var destFile = string.Format("{0}/ab.v{1}",
                    //    rootPath, DestVersion);
                    //var destFile = string.Format("{0}/assets.{1}.v{2}", rootPath, ModleName, DestVersion);
                    var destFile = getFullAssetsFilePath(DestVersion);
                    if (!FullAssetInfos.ContainsKey(DestVersion))
                    {
                        finish(RetCode.DestFileInfoNotFound,
                            string.Format("未找到指定版本的完整包信息.({0},{1})",
                            ModleName, DestVersion));
                    }
                    else
                    {
                        var info = FullAssetInfos[DestVersion];
                        var md5 = Downloader.GetMd5(destFile).ToLower();
                        if (md5 != null && md5.Equals(info.Md5.ToLower()))
                        {
                            finish(0, "");
                        }
                        else
                        {
                            finish(RetCode.DestMd5CheckFail,
                                string.Format("文件校验失败({0},{1},{2},{3})",
                                ModleName, destFile, md5, info.Md5));
                        }
                    }
                }
                break;
        }
    }

    public void OnDestroy()
    {
        Debug.Log("Destroy AssetsDownloader.");
    }
}
