using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class TestDownload : MonoBehaviour
{
    public Image image;
    public GameObject panle;
    public Slider slider;
    public Text totalText;
    public Text titleText;
    public Toggle isFullToggle;

    private AssetsDownloader downloader;

    public void StartDownload(int destVersion)
    {
        if (downloader != null) return;
        downloader = gameObject.AddComponent<AssetsDownloader>();
        downloader.ModleName = "login";
        downloader.DestVersion = destVersion;
        downloader.DisablePatchMode = isFullToggle.isOn;

        //downloader.UpPatchInfos.Add(2, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_up2",
        //    Md5 = "dcc55177a32fa4b1e989458cdc881fe0", FileSize = 839 });
        //downloader.UpPatchInfos.Add(3, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_up3",
        //    Md5 = "fb6f34cca14bdaf47cbd66f056dfdcea", FileSize = 3418 });
        //downloader.UpPatchInfos.Add(4, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_up4",
        //    Md5 = "3bb6859ab7aee5147c4d41c02087eef8", FileSize = 28796055 });
        //downloader.UpPatchInfos.Add(5, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_up5",
        //    Md5 = "5f43143e64f30f7087d5cc2b4724cd08", FileSize = 3754 });
        //downloader.UpPatchInfos.Add(6, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_up6",
        //    Md5 = "29227dccd9f95b56adebf244bdffe2c0", FileSize = 3593 });
        //downloader.UpPatchInfos.Add(7, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_up7",
        //    Md5 = "ab13aa546c3d698be3e39577beebc94a", FileSize = 3450 });

        //downloader.DownPatchInfos.Add(1, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_down1",
        //    Md5 = "5ef1cff80f7a880979b98ab07388c990", FileSize = 862 });
        //downloader.DownPatchInfos.Add(2, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_down2",
        //    Md5 = "00a75e104199073ef3b9380c6430c579", FileSize = 3076 });
        //downloader.DownPatchInfos.Add(3, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_down3",
        //    Md5 = "42486499b61422487181490c3ced8de8", FileSize = 18149923 });
        //downloader.DownPatchInfos.Add(4, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_down4",
        //    Md5 = "e891a58bb7398e445f78b948096d7607", FileSize = 3693 });
        //downloader.DownPatchInfos.Add(5, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_down5",
        //    Md5 = "f3f12b18a72b09b58410df3e63a2f792", FileSize = 3877 });
        //downloader.DownPatchInfos.Add(6, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/patch_down6",
        //    Md5 = "08bfd164b46dd28bc23db4ae468f6049", FileSize = 3238 });

        //downloader.FullAssetInfos.Add(1, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/assets.login.v1",
        //    Md5 = "c91a232a688b4028b3b78b6c59833053", FileSize = 61434110 });
        //downloader.FullAssetInfos.Add(2, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/assets.login.v2",
        //    Md5 = "90c9c8842ea73f40bfce8b89eda0fc5f", FileSize = 61434120 });
        //downloader.FullAssetInfos.Add(3, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/assets.login.v3",
        //    Md5 = "768c917d5fa3dcaf6d9ea55873cf553e", FileSize = 61433495 });
        //downloader.FullAssetInfos.Add(4, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/assets.login.v4",
        //    Md5 = "652fc5eea06b731ca0e351454e7fd3ef", FileSize = 74901684 });
        //downloader.FullAssetInfos.Add(5, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/assets.login.v5",
        //    Md5 = "a4c78754bcf80628f940544a63291564", FileSize = 74901709 });
        //downloader.FullAssetInfos.Add(6, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/assets.login.v6",
        //    Md5 = "c066f11508c8e22105a8e65168a6b213", FileSize = 74901509 });
        //downloader.FullAssetInfos.Add(7, new AssetsDownloader.FileInfo() {
        //    Url = "http://192.168.1.143:8080/unity_assets/login/assets.login.v7",
        //    Md5 = "1910a7b79b7bb43821d4868a0f0ed360", FileSize = 74901357 });

        downloader.UpPatchInfos.Add(2, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/patch_up2",
            Md5 = "f0f586e44f1f26ffb6582929a630ea4e",
            FileSize = 305635
        });
        downloader.UpPatchInfos.Add(3, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/patch_up3",
            Md5 = "a59c9e85d6c592b375044de85ccce2fe",
            FileSize = 182081
        });
        downloader.UpPatchInfos.Add(4, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/patch_up4",
            Md5 = "69612d72a2a3e4d6cdf5d68f098fe838",
            FileSize = 220114
        });

        downloader.DownPatchInfos.Add(1, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/patch_down1",
            Md5 = "7376a7893779ebf013a9b74af72d8f72",
            FileSize = 316669
        });
        downloader.DownPatchInfos.Add(2, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/patch_down2",
            Md5 = "75e58344dc5a49d330fd7b7a9d84c906",
            FileSize = 174661
        });
        downloader.DownPatchInfos.Add(3, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/patch_down3",
            Md5 = "200983ee80d0b733c499142e89b0f49d",
            FileSize = 224892
        });

        downloader.FullAssetInfos.Add(1, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/assets.login.v1",
            Md5 = "c5236c4fa34505a9d549778760d3e71b",
            FileSize = 5376893
        });
        downloader.FullAssetInfos.Add(2, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/assets.login.v2",
            Md5 = "e91db0510976848590974ab30bcbab95",
            FileSize = 5304902
        });
        downloader.FullAssetInfos.Add(3, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/assets.login.v3",
            Md5 = "ffd8f49525c4024afcb5503fc06607ec",
            FileSize = 5310724
        });
        downloader.FullAssetInfos.Add(4, new AssetsDownloader.FileInfo()
        {
            Url = "http://192.168.1.131/com.xvsoft.demo(0.1.0-1)/region/assets.login/assets.login.v4",
            Md5 = "a41a1083a6e2441ca805d81c5cb5dab4",
            FileSize = 5307757
        });
    }

    // test
    private void loadAsset()
    {
        //var dest = string.Format("{0}/assets.{1}.v{2}", rootPath, ModleName, DestVersion);
        var dest = downloader.GetDestFilePath();
        AssetBundle.UnloadAllAssetBundles(true);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(dest);
        //var img = assetBundle.LoadAsset<Sprite>("Assets/demo/res/hall/hall/texture/lobbycards/cn/by.png");
        var img = assetBundle.LoadAsset<Sprite>("Assets/demo/res/login/textures/dating_zalo.png");
        Debug.Log("load ab.");
        image.sprite = img;
        image.SetNativeSize();
    }

    public void Update()
    {
        bool disable = downloader == null ||
            downloader.mode == AssetsDownloader.Mode.Wait;
        panle.SetActive(!disable);
        if (disable) return;

        switch (downloader.mode)
        {
            case AssetsDownloader.Mode.DownloadFile:
                {
                    titleText.text = "下载中...";
                    slider.value = downloader.downloadedBytes / (float)downloader.totalBytes;
                    totalText.text = downloader.downloadedBytes + "/" + downloader.totalBytes;
                }
                break;
            case AssetsDownloader.Mode.MergeFile:
                {
                    titleText.text = "升级中(升级不消耗流量)...";
                    totalText.text = downloader.mergedFiles + "/" + downloader.totalMergeFiles;
                    slider.value = downloader.mergedFiles / (float)downloader.totalMergeFiles;
                }
                break;
            case AssetsDownloader.Mode.CheckMd5:
                {
                    titleText.text = "校验文件...";
                    totalText.text = "";
                    slider.value = 1;
                }
                break;
            case AssetsDownloader.Mode.Finish:
                {
                    // retCode为0表示成功
                    Debug.Log(string.Format("download finsh, retcode={0}, errMsg={1}",
                        downloader.retCode, downloader.errMsg));
                    loadAsset();
                    Destroy(downloader);
                    downloader = null;
                }
                break;
        }
    }
}
