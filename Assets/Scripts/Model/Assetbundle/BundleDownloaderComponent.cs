using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


//请求资源服务器的版本文件,然后缓存起来,
//获取客户端的版本文件-存在或者是不存在
//不存在的话 就直接下载服务器所有的AB文件
//存在的话 获取后 进行json反序列化  
//进行遍历->如果

/// <summary>
/// 用来对比web端的资源，比较md5，对比下载资源
/// </summary>
public class BundleDownloaderComponent : MonoBehaviour
{
    private VersionConfig remoteVersionConfig;

    //等待下载的队列 
    public Queue<string> bundles;

    public long TotalSize;

    public HashSet<string> downloadedBundles;

    //正在请求的url
    public string downloadingBundle;

    //正在请求的对象
    public UnityWebRequest webRequest;

    Transform canvas;
    Image progressBar;
    Text size;
    Text state;
    GameObject go;
    public void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        var obj = Resources.Load("prefab/HotfixPanel");
      go = (GameObject)GameObject.Instantiate(obj);
        go.transform.SetParent(canvas, false);

        progressBar = go.transform.Find("Image/progressBar").GetComponent<Image>();
        size=go.transform.Find("Image/size").GetComponent<Text>();
        state = go.transform.Find("Image/state").GetComponent<Text>();

        bundles = new Queue<string>();
        downloadedBundles = new HashSet<string>();
        downloadingBundle = "";

        //开始下载
        StartCoroutine(DownloadAsync(null));
        //StartCoroutine(DownloadAsync(LoadComplete));

    }

    int gameState = 0;
    public void Update()
    {
        if (Progress >=1 && gameState == 0)
        {
            state.text = "请点击任意位置,进入游戏!";
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                LoadComplete();
                gameState = 1;
                Destroy(go);
            }
        }
    }

    void LoadComplete() {
        //进入到资源加载
        GameObject go = new GameObject("ResourcesComponent");
     
        go.AddComponent<Main>();


        //加载热更dll
      
    }
  

    public static string AssetBundleServerUrl = "http://localhost:8000/Desktop/Release/";// "http://193.112.44.199:8080/";

    public static string GetUrl()
    {
        string url = AssetBundleServerUrl;
#if UNITY_ANDROID
			url += "Android/";
#elif UNITY_IOS
			url += "IOS/";
#elif UNITY_WEBGL
			url += "WebGL/";
#elif UNITY_STANDALONE_OSX
			url += "MacOS/";
#else
        url += "StandaloneWindows/";
#endif
        return url;
    }

    byte[] versionData;
    //获取本地的版本文件
    VersionConfig localVersionConfig;
    private IEnumerator StartAsync()
    {
        // 获取远程的Version.txt
        string versionUrl = "";
        
        versionUrl = GetUrl()+ "StreamingAssets/" + "Version.txt";
        Debug.Log(versionUrl);
        //url编码中的空格用%20表示
        versionUrl = versionUrl.Replace(" ", "%20");
        using (UnityWebRequest webRequestAsync = UnityWebRequest.Get(versionUrl))
        {
           //获取资源服务器上的版本文件
            yield return webRequestAsync.SendWebRequest();
            Debug.Log("版本信息:\n" + webRequestAsync.downloadHandler.text);
            versionData = webRequestAsync.downloadHandler.data;
            //反序列化成VersionConfig
            remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(webRequestAsync.downloadHandler.text);
            
        }

      
        string versionPath = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");
        using (UnityWebRequest request = UnityWebRequest.Get(versionPath))
        {
            yield return request.SendWebRequest();
            if (request.result!=UnityWebRequest.Result.Success)
            {
                Debug.LogError("本地未存在版本文件...Version.txt");

            }
            else
            { //反序列化->VersionConfig 
              //但是内容如果为空 字典就无法在反序列的时候进行赋值 接下来的逻辑就会出错
                localVersionConfig = JsonHelper.FromJson<VersionConfig>(request.downloadHandler.text);
            }
        }


        //遍历以前的AB文件 如果服务器的配置中不存在该文件了 就将它删除掉
        DirectoryInfo directoryInfo = new DirectoryInfo(PathHelper.AppHotfixResPath);
        
        if (directoryInfo.Exists)
        {
            //检查本地的文件 是否在服务器版本文件中存在 如果不存在 则将其进行删除
            CheckFiles(directoryInfo);
        }
        else
        {
            //如果目录不存在 就进行创建
            directoryInfo.Create();
        }

        //如果本地的版本文件为空 直接下载所有文件
        if (localVersionConfig==null)
        {
            foreach (FileVersionInfo fileVersionInfo in remoteVersionConfig.FileInfoDict.Values)
            { 
                //如果对比不一样 则压入到等待下载的队列 
                this.bundles.Enqueue(fileVersionInfo.File);
                //下载文件的总大小+上这个文件的
                this.TotalSize += fileVersionInfo.Size;
            }
        }
        else
        {
            // 遍历服务器的所有文件 进行MD5对比
            foreach (FileVersionInfo fileVersionInfo in remoteVersionConfig.FileInfoDict.Values)
            {
                //获取本地的MD5码
                string localFileMD5 = GetBundleMD5(localVersionConfig, fileVersionInfo.File);
                if (fileVersionInfo.MD5 == localFileMD5)
                {
                    continue;
                }
                //如果对比不一样 则压入到等待下载的队列 
                this.bundles.Enqueue(fileVersionInfo.File);
                //下载文件的总大小+上这个文件的
                this.TotalSize += fileVersionInfo.Size;
            }
        }
        //如果需要进行更新  那么版本文件也需要进行更新
        if (this.bundles.Count > 0)
        {
            //把版本文件也压入 一起进行更新下载
            this.bundles.Enqueue("Version.txt");
            this.TotalSize += versionData.Length;
            Debug.Log("版本文件的长度:" + versionData.Length);
        }
        
    }

    //检查本地文件 如果服务器上并未存在的,那么就将它删除掉
    private void CheckFiles(DirectoryInfo directoryInfo)
    {

        FileInfo[] fileInfos = directoryInfo.GetFiles();

        foreach (FileInfo fileInfo in fileInfos)
        {
            string fileName = fileInfo.FullName.Replace(@"\","/").Replace(PathHelper.AppHotfixResPath,"");
            //Debug.Log(fileName+":"+ fileInfo.Name);

            //如果服务器的文件配置 存在该文件名称 就忽略
            if (remoteVersionConfig.FileInfoDict.ContainsKey(fileName))//fileInfo.Name
            {
                continue;
            }
            
            //如果该文件是记录版本信息的 就忽略
            if (fileName == "Version.txt")
            {
                continue;
            }
            //否则就删除掉
            fileInfo.Delete();
        }

        //子文件夹中的文件也需要进行校验
        DirectoryInfo[] dirPath = directoryInfo.GetDirectories();

        for (int i = 0; i < dirPath.Length; i++)
        {
            CheckFiles(dirPath[i]);
        }
    }

    //获取MD5
    public  string GetBundleMD5(VersionConfig localVersionConfig, string bundleName)
    {
        
        string path = Path.Combine(PathHelper.AppHotfixResPath, bundleName);
        //如果路径存在 返回该文件的MD5码
        if (File.Exists(path))
        {
            return MD5Helper.FileMD5(path);
        }

        //如果本地文件的记录中 包含该AB名称 则返回它记录的MD5
        if (localVersionConfig.FileInfoDict.ContainsKey(bundleName))
        {
            return localVersionConfig.FileInfoDict[bundleName].MD5;
        }

        //否则就返回空
        return "";
    }


    public float Progress
    {
        get
        {
            if (this.TotalSize == 0|| this.downloadedBundles.Count == 0)
            {
                size.text = "当前为最新版本,无需更新!";
                return 1;
            }
           
            //已下载字节数
            long alreadyDownloadBytes = 0;

            //遍历已下载的文件队列
            foreach (string downloadedBundle in this.downloadedBundles)
            {
                long size = 0;
                //是AB文件的大小
                if (this.remoteVersionConfig.FileInfoDict.ContainsKey(downloadedBundle))
                {
                     size = this.remoteVersionConfig.FileInfoDict[downloadedBundle].Size;
                }
                //是版本文件的大小
                else
                {
                    if (downloadedBundle=="Version.txt")
                    {
                        size = versionData.Length;
                    }
                }
               
                alreadyDownloadBytes += size;
            }
            //如果当前正在请求不等于空 那么加上当前下载的大小
            if (this.webRequest != null)
            {
                alreadyDownloadBytes += (long)this.webRequest.downloadedBytes;
            }
            //字节 /1024.00f/1024.00 M
            size.text = $"{alreadyDownloadBytes/1024.00f/ 1024.00f}M/{this.TotalSize/ 1024.00f / 1024.00f}M";
            float progress = alreadyDownloadBytes * 1f / this.TotalSize;
            progressBar.fillAmount = progress;
            Debug.Log(size.text+":"+progress);
           
            //得到一个进度
            return progress;
        }
    }

    //开始下载
    public IEnumerator DownloadAsync(Action loadComplete)
    {
        yield return StartCoroutine(StartAsync());
        if (this.bundles.Count == 0 && this.downloadingBundle == "")
        {
            //return null;
        }
        else
        {
            while (this.bundles.Count>0)
            {
                //如果没有下载任务了 就跳出循环
                //if (this.bundles.Count == 0)
                //{
                //    break;
                //}

                //将等待下载的包体出列
                this.downloadingBundle = this.bundles.Dequeue();
                //然后开始下载
                using (UnityWebRequest webRequest = UnityWebRequest.Get(GetUrl() + "StreamingAssets/" + this.downloadingBundle))
                {
                    this.webRequest = webRequest;
                    yield return webRequest.SendWebRequest();
                    //请求到的数据
                    byte[] data = webRequest.downloadHandler.data;


                    if (this.downloadingBundle.Contains("/"))
                    {
                        //url通过/分割
                        var tempPath = this.downloadingBundle.Split('/');
                        //最后一个则是文件名称
                        string dir = tempPath[tempPath.Length - 1];

                        //去掉之后 得到文件夹名称 
                        string abDir = downloadingBundle.Replace(dir, "");
                        //缓存的根目录+子文件夹名称 得到完整的文件夹路径
                        abDir = Path.Combine(PathHelper.AppHotfixResPath, abDir);
                        
                        //判断该文件夹是否包含,没有就进行创建
                        if (Directory.Exists(abDir) == false)
                        {
                            Debug.Log("未包含文件夹:" + abDir);
                            Directory.CreateDirectory(abDir);
                        }
                    }

                    //文件夹创建好之后 就开始创建文件  将数据写入到文件中即完成下载
                    string path = Path.Combine(PathHelper.AppHotfixResPath, this.downloadingBundle);
                    //如果文件已经存在 需要删除后重新下载新的
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        //FileStream fs = File.Create(path);
                        //fs.Write(data, 0, data.Length);
                        //fs.Close();
                    }
                    //通过文件流的形式创建文件 将字节数组写入 就得到AB包
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        fs.Write(data, 0, data.Length);
                    }
                }

                //已下载的bundle
                this.downloadedBundles.Add(this.downloadingBundle);
                //更新进度 -可自行修改调用方式
               // GameObject.Find("GameManager").GetComponent<GameManager>().UpdateProgressFront(Progress / 100f);
                this.downloadingBundle = "";
                this.webRequest = null;
            }
        }
        if (loadComplete != null)
        {
            loadComplete();
        }
    }
}