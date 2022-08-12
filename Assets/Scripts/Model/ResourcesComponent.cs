using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
[XLua.LuaCallCSharp]
public class ResourcesComponent : SingletonAutoMono<ResourcesComponent>
{
   
    //是否使用Assetbundle模式
    public  static bool useAB = true;

    public static AssetBundleManifest AssetBundleManifestObject { get; set; }

    Dictionary<string, Dictionary<string, UnityEngine.Object>> resourceCache = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

    Dictionary<string, ABInfo> bundles = new Dictionary<string, ABInfo>();
   // public static ResourcesComponent Instance;
  

    //销毁时候的事件
    private void OnDestroy()
    {
        Debug.LogError("释放掉了!");
        foreach (var abInfo in this.bundles)
        {
            abInfo.Value.Dispose();
        }

        this.bundles.Clear();
        this.resourceCache.Clear();
    }

    //加载资源对外暴漏的接口 bundleName=路径
    public UnityEngine.Object GetAsset(string obj,string bundleName,bool isScene=false)
    {
        //思想:检查所有资源是否已经加载缓存到字典里
        //然后从缓存的字典中取出
        bundleName = bundleName.StringToAB();

        if (!useAB)
        {
            //检查资源是否加载缓存
            CheckResources(obj,bundleName);
        }
        else
        {
            
            //检查Assetbundle是否加载缓存
            CheckAssetBundle(bundleName);
        }

        
       
        if (isScene==true)
        {
            
            return null;
        }
        else
        {
            //从缓存中加载资源
            Dictionary<string, UnityEngine.Object> dict;

            //对字典的外层(资源名称)进行检查 key不存在就抛出异常
            if (!this.resourceCache.TryGetValue(bundleName, out dict))
            {
                throw new Exception($"not found asset: {bundleName} {obj}");
            }

            UnityEngine.Object resource = null;
            //对字典内层(物体名称)进行检查 key不存在就抛出异常
            if (!dict.TryGetValue(obj, out resource))
            {
                throw new Exception($"not found asset: {bundleName} {obj}");
            }
            //如果都没问题 就返回缓存中取到的物体
            return resource;
        }
        
       
    }

    //编辑器模式下,非AB模式的资源加载方式
    private void CheckResources(string obj, string bundleName) {
#if UNITY_EDITOR
        ABInfo abInfo;
        //先从缓存中查询 如果已经存在 直接加上引用次数即可
        if (this.bundles.TryGetValue(bundleName, out abInfo))
        {
            ++abInfo.RefCount;
            return;
        }

        //资源缓存
        //string realPath = Path.Combine(PathHelper.EditorResourcesRoot, bundleName + ".prefab");
        //Debug.LogError("资源路径:"+realPath);
        //UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(realPath);
        //if (resource==null)
        //{
        //    Debug.LogError("resource==null");
        //}
        //else
        //{
        //    AddResource(bundleName, obj, resource);
        //}

        //因为所有资源路径都是唯一的 所以这里长度永远只会是1
        List<string> realPath = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName).ToList();
        if (realPath.Count>1)
        {
            Debug.LogError("realPath > 1");
        }
        UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(realPath[0]);
        AddResource(bundleName, obj, resource);
      
        //资源缓存
        //foreach (string s in realPath)
        //{
        //    string assetName = Path.GetFileNameWithoutExtension(s);//.ToLower();
        //    Debug.Log("assetName:" + assetName);
        //    UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
        //    AddResource(bundleName, assetName, resource);
        //}
        //在编辑器模式下 是直接加载资源 所以只需要缓存名称 而无需缓存assetbundle
        abInfo = new ABInfo()
        {
            Name = bundleName,
            AssetBundle = null,
            RefCount=1,
        };
        //AB缓存
        this.bundles[bundleName] = abInfo;
        return;
#endif

    }

    //加载Assetbundle
    private void CheckAssetBundle(string bundleName) {
        
        //通过AssetBundleManifestObject来确定各个AB之间的依赖 所以它如果是空 要先加载它
        if (AssetBundleManifestObject==null)
        {
            AssetBundle mainAB = AssetBundle.LoadFromFile(Path.Combine(PathHelper.AppHotfixResPath, "StreamingAssets"));
            //通过它来查看AB之间的依赖
            AssetBundleManifestObject = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        //先获取所有的依赖
        List<string> allAB = AssetBundleHelper.GetAllLoadAB(bundleName);
        Debug.Log(allAB.Count);
        for (int i = 0; i < allAB.Count; i++)
        {

            //先从缓存中查询 如果已经存在 直接加上引用次数即可 进入下一个AB的检查
            ABInfo abInfo;
            if (this.bundles.TryGetValue(allAB[i], out abInfo))
            {
                ++abInfo.RefCount;
                continue;
            }
            //如果不存在 
            
            string p = Path.Combine(PathHelper.AppHotfixResPath, allAB[i]);
            Debug.Log("加载的路径是:" + p);
            AssetBundle assetBundle = null;
            //从文件路径下进行加载
            //LoadFromFile从文件中进行加载
            assetBundle = AssetBundle.LoadFromFile(p);
            if (assetBundle == null)
            {
                throw new Exception($"assets bundle not found: {bundleName}");
            }
            
            //该资源如果不是场景资源
            if (!assetBundle.isStreamedSceneAssetBundle)
            {
                // 异步load资源到内存cache住
                UnityEngine.Object[] assets = assetBundle.LoadAllAssets();
                Debug.Log("assets:" + assets.Length);
                foreach (UnityEngine.Object asset in assets)
                {
                    Debug.Log("资源名称:" + asset.name);
                    AddResource(allAB[i], asset.name, asset);
                }
            }
            
            abInfo = new ABInfo()
            {
                Name = allAB[i],
                AssetBundle = assetBundle,
                RefCount=1,
            };
            this.bundles[allAB[i]] = abInfo;
        }
    }


    //卸载资源对外暴漏的接口
    public void UnloadBundle(string bundleName)//,bool isAll=false)
    {
        bundleName = bundleName.StringToAB();
        List<string> allBundle;
        if (useAB==false)
        {
            //allBundle = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName).ToList();
            allBundle = new List<string>();
            allBundle.Add(bundleName);
        }
        else
        {
            //获取到所有要卸载的AB路径
            allBundle = AssetBundleHelper.GetAllLoadAB(bundleName);
            //if (isAll==false)
            //{
            //    allBundle = new List<string>();
            //    allBundle.Add(bundleName);
            //}
            //else
            //{
            //    //获取到所有要卸载的AB路径
            //    allBundle = AssetBundleHelper.GetAllLoadAB(bundleName);
            //}
          
        }
        
        //卸载资源 实际上是由引用计数来决定的 如果减掉一次引用计数后等于0 才释放
        foreach (string bundle in allBundle)
        {
            ABInfo abInfo;
            if (!this.bundles.TryGetValue(bundle, out abInfo))
            {
                throw new Exception($"not found assetBundle: {bundle}");
            }

            //计数--
            --abInfo.RefCount;

            //如果引用次数大于0 就继续检查下个AB
            if (abInfo.RefCount > 0)
            {
                continue;
            }

            //当它没有被引用了 计数=0的时候 就会从缓存移除掉
            //从字典缓存移除 
            this.bundles.Remove(bundle);
            this.resourceCache.Remove(bundle);
            //但是要从内存清除的话 Dispose-> this.AssetBundle.Unload(true);
            abInfo.Dispose();
        }
    }

    /// <summary>
    /// 缓存AB包加载出来的资源
    /// </summary>
    /// <param name="bundleName">AB包名称</param>
    /// <param name="assetName">引用的资源名称</param>
    /// <param name="resource">引用的资源</param>
    private void AddResource(string bundleName, string assetName, UnityEngine.Object resource)
    {
        //思路:全局变量resourceCache是字典嵌套字典的结构
        //外层的字典 key是AB名称 
        //内层的字典是:资源名称:对应的物体Object
       
        Dictionary<string, UnityEngine.Object> dict;
        //先检索外层的key是否存在 如果不存在
        if (!this.resourceCache.TryGetValue(bundleName.BundleNameToLower(), out dict))
        {
            //构建一个字典
            dict = new Dictionary<string, UnityEngine.Object>();
            //bundleName 指向dict
            this.resourceCache[bundleName.BundleNameToLower()] = dict;
        }
        //如果存在 直接设置key value
        dict[assetName] = resource;
    }

    /// <summary>
    /// 打印缓存中现在拥有的资源
    /// </summary>
    /// <returns></returns>
    public string DebugString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (ABInfo abInfo in this.bundles.Values)
        {
            sb.Append($"{abInfo.Name}:{abInfo.RefCount}\n");
        }
        return sb.ToString();
    }
}

//AB实体信息
public class ABInfo : IDisposable
{
    public string Name { get; set; }

    //引用计数 
    public int RefCount { get; set; }

    public AssetBundle AssetBundle;

    public void Dispose()
    {
        //Log.Debug($"desdroy assetbundle: {this.Name}");

        if (this.AssetBundle != null)
        {
            this.AssetBundle.Unload(true);
        }

        this.RefCount = 0;
        this.Name = "";
    }
}



// 用于字符串转换，减少GC
public static class AssetBundleHelper
{
    public static readonly Dictionary<int, string> IntToStringDict = new Dictionary<int, string>();

    public static readonly Dictionary<string, string> StringToABDict = new Dictionary<string, string>();

    public static readonly Dictionary<string, string> BundleNameToLowerDict = new Dictionary<string, string>()
        {
            { "StreamingAssets", "StreamingAssets" }
        };

    // 缓存包依赖，不用每次计算
    public static Dictionary<string, List<string>> DependenciesCache = new Dictionary<string, List<string>>();

    public static string IntToString(this int value)
    {
        string result;
        if (IntToStringDict.TryGetValue(value, out result))
        {
            return result;
        }

        result = value.ToString();
        IntToStringDict[value] = result;
        return result;
    }

    public static string StringToAB(this string value)
    {
        string result;
        if (StringToABDict.TryGetValue(value, out result))
        {
            return result;
        }

        result = (value + ".unity3d").BundleNameToLower();
        StringToABDict[value] = result;
        return result;
    }

    public static string IntToAB(this int value)
    {
        return value.IntToString().StringToAB();
    }

    /// <summary>
    /// 将AB包的名称转化为小写的
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string BundleNameToLower(this string value)
    {
        string result;
        if (BundleNameToLowerDict.TryGetValue(value, out result))
        {
            return result;
        }

        result = value.ToLower();
        BundleNameToLowerDict[value] = result;
        return result;
    }

    //获取所有要加载的AB = 依赖+自身
    public static List<string> GetAllLoadAB(string assetBundleName)
    {
       
        List<string> dependencies ;
        //从缓存中直接获取
        if (DependenciesCache.TryGetValue(assetBundleName, out dependencies))
        {
            return dependencies;
        }


        //如果缓存里没有 并且是AB模式 就直接获取依赖项 并且添加到缓存中
        dependencies = ResourcesComponent.AssetBundleManifestObject.GetAllDependencies(assetBundleName).ToList();
        dependencies.Add(assetBundleName);
        DependenciesCache.Add(assetBundleName, dependencies);
        return dependencies;
    }
}