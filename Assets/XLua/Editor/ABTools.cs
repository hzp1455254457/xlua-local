using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ABTools :Editor
{
    //资源打包后存储的根目录
    static string saveRoot {
        get {
            return Application.dataPath + "/../../Release/";
        }
    }

    [MenuItem("Tools/打开沙盒目录")]
    public static void OpenPreDirectory() {
        //Application.persistentDataPath 沙盒路径
        Application.OpenURL(Application.persistentDataPath);
    }

    //需要写入版本文件
    //拷贝资源到StreamAssets文件夹中 需先切换到目标平台后再拷贝 或者自己做扩展
    [MenuItem("Tools/将AB拷贝到工程内部")]
    public static void CopyABToStreamingAssets()
    {
        string path =Path.Combine(Application.streamingAssetsPath,Application.productName);
        if (Directory.Exists(path))
        {
            Directory.Delete(path,true);
        }
        AssetDatabase.Refresh();
        Directory.CreateDirectory(path);

        string buildFold = saveRoot + EditorUserBuildSettings.activeBuildTarget + "/StreamingAssets/";

        FileHelper.CopyDirectory(buildFold, path);
        AssetDatabase.Refresh();
        
    }

    

    //PC -64
    [MenuItem("Tools/资源打包/PC")]
    public static void BuildAB_PC() {
        BuildAssetBundle(BuildTarget.StandaloneWindows);
        string path= saveRoot + BuildTarget.StandaloneWindows + "/StreamingAssets/";
        //生成版本文件
        GenerateVersionInfo(path);
    }

    [MenuItem("Tools/资源打包/安卓")]
    public static void BuildAB_Android()
    {
        BuildAssetBundle(BuildTarget.Android);
        string path = saveRoot + BuildTarget.Android + "/StreamingAssets/";
        //生成版本文件
        GenerateVersionInfo(path);
    }

    //ios
    [MenuItem("Tools/资源打包/IOS")]
    public static void BuildAB_IOS()
    {
        BuildAssetBundle(BuildTarget.iOS);
        string path = saveRoot + BuildTarget.iOS + "/StreamingAssets/";
        //生成版本文件
        GenerateVersionInfo(path);
    }

    /// <summary>
    /// 打包的核心接口
    /// </summary>
    private static void BuildAssetBundle(BuildTarget buildTarget)
    {
        //1.确定资源打包到哪个文件夹里面 /../父目录
        //不同平台 子目录不同
        string buildFold = saveRoot + buildTarget+ "/StreamingAssets/";
        //2.对文件的逻辑,如果文件夹存在先删除掉,重新创建
        if (Directory.Exists(buildFold))
        {
            Directory.Delete(buildFold, true);
        }
        Directory.CreateDirectory(buildFold);
        //3.才是往这个文件夹里面加入AB文件.
        BuildPipeline.BuildAssetBundles(buildFold, BuildAssetBundleOptions.None, buildTarget);
    }

    //生成版本文件
    private static void GenerateVersionInfo(string dir)
    {
        //构造versionProto 
        VersionConfig versionProto = new VersionConfig();
       // versionProto.TotalSize = 1000;
        versionProto.Version = 1;
        //生成版本信息 赋值给versionProto
        GenerateVersionProto(dir, versionProto, "");

        using (FileStream fileStream = new FileStream($"{dir}/Version.txt", FileMode.Create))
        {
            //versionProto对象进行json序列化,得到json格式的字符串,再转换成数组,给FileStream写入到本地文件中
            byte[] bytes = JsonHelper.ToJson(versionProto).ToByteArray();
            fileStream.Write(bytes, 0, bytes.Length);
        }

    }

    //生成版本信息
    private static void GenerateVersionProto(string dir, VersionConfig versionProto, string relativePath)
    {
        //遍历内部的文件
        foreach (string file in Directory.GetFiles(dir))
        {
            string md5 = MD5Helper.FileMD5(file);
            FileInfo fi = new FileInfo(file);
            long size = fi.Length;
            //文件路径: 如果没有相对路径 则等于文件本身的名称
            //如果有相对路径,就拼接到一起
            string filePath = relativePath == "" ? fi.Name : $"{relativePath}/{fi.Name}";

            versionProto.FileInfoDict.Add(filePath, new FileVersionInfo
            {
                File = filePath,
                MD5 = md5,
                Size = size,
            });
        }

        //遍历内部的文件夹 
        foreach (string directory in Directory.GetDirectories(dir))
        {
            DirectoryInfo dinfo = new DirectoryInfo(directory);
            //相对路径,如果没有就是文件夹本身的名字,如果有,就是相对路径的格式:路径/文件夹名称
            string rel = relativePath == "" ? dinfo.Name : $"{relativePath}/{dinfo.Name}";
            GenerateVersionProto($"{dir}/{dinfo.Name}", versionProto, rel);
        }
        versionProto.EndInit();
    }


    [MenuItem("Tools/自动设置AB名称")]
    // 获取目标文件夹目录
    public static void AutoSetBundleName()
    {
        //完整路径+AB名称
        Dictionary<string, string> dir = new Dictionary<string, string>();
        string resPath;

        //资源文件夹的完整路径 根目录
        resPath = Path.Combine(Application.dataPath, "Res");

        //获取资源文件 缓存到字典中
        GetResFile(resPath, dir);

        //遍历缓存的文件 进行设置Assetbundle名称
        foreach (var filePath in dir.Keys)
        {
            //Debug.Log("文件路径:" + item);
            //Debug.Log("AB名称:" + dir[item]);
            // 通过在工程内的路径获取 AssetImporter
            string objPath = filePath.Replace(Application.dataPath, "Assets");
            Debug.Log("objPath:" + objPath);
            //获取到要设置AB名称的物体
            AssetImporter ai = AssetImporter.GetAtPath(objPath);
            //设置物体的assetbundle名称
            ai.assetBundleName = dir[filePath];
        }
    }

    //获取要打包的资源文件
    static void GetResFile(string path, Dictionary<string, string> dir)
    {
        //E:\游戏学院-Unity\Unity-热更新(三)框架搭建\资料\ABFrame\Assets\Res\sprite\loginBG.jpg
        //--->sprite/loginBG.jpg  -->sprite/loginBG.unity3d
        //E:/游戏学院-Unity\Unity-热更新(三)框架搭建/资料\ABFrame/Assets/Res/
        //统一使用"/" 用于字符串查找替换
        string resRoot = Path.Combine(Application.dataPath, "Res").Replace(@"\", "/");

        //文件夹对象
        DirectoryInfo dirInfo = new DirectoryInfo(path);

        //遍历文件夹下的文件
        FileInfo[] fileInfos = dirInfo.GetFiles();
        for (int i = 0; i < fileInfos.Length; i++)
        {
            //排除.meta文件,这个文件是版本同步用的,非unity里的文件
            if (fileInfos[i].Name.Contains(".meta"))
            {
                continue;
            }
            //文件的完整路径 统一使用/符号
            string fullPath = fileInfos[i].FullName.Replace(@"\", "/");
            if (File.Exists(fullPath))
            {
               
                //去掉前缀,并且将后缀替换为.unity3d

                string abName = fullPath.Replace(resRoot +"/", "")
             .Replace(fileInfos[i].Extension, ".unity3d");
               //压入字典中
                dir.Add(fullPath, abName);
            }
        }

        //获取子文件夹
        DirectoryInfo[] subDir = dirInfo.GetDirectories();
        //遍历子文件夹 获取子文件夹下的文件 存储到dir字典中
        for (int i = 0; i < subDir.Length; i++)
        {
            if (Directory.Exists(subDir[i].FullName))
            {
                GetResFile(subDir[i].FullName, dir);
            }
        }

    }

}

