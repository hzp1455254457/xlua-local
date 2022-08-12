using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

using UnityEngine.Networking;
using System.Collections.Generic;
[XLua.LuaCallCSharp]
public abstract class Global
{
    public static GameObject FindChild(Transform trans, string childName)
    {
        Transform child = trans.Find(childName);
        if (child != null)
        {
            return child.gameObject;
        }
        int count = trans.childCount;
        GameObject go = null;
        for (int i = 0; i < count; ++i)
        {
            
            child = trans.GetChild(i);
           // Debug.Log(":+"+child.name);
            go = FindChild(child, childName);
            if (go != null)
                return go;
        }
        return null;
    }
    public static T FindChild<T>(Transform trans, string childName) where T : Component
    {
        GameObject go = FindChild(trans, childName);
        if (go == null)
            return null;
        return go.GetComponent<T>();
    }
    /// <summary>
    /// 获取时间格式字符串，显示mm:ss
    /// </summary>
    /// <returns>The minute time.</returns>
    /// <param name="time">Time.</param>
    public static string GetMinuteTime(float time)
    {
        int mm, ss;
        string stime = "00:00";
        if (time <= 0) return stime;
        mm = (int)time / 60;
        ss = (int)time % 60;
        if (mm > 60)
            stime = "59:59";
        else if (mm < 10 && ss >= 10)
        {
            stime = "0" + mm + ":" + ss;
        }
        else if (mm < 10 && ss < 10)
        {
            stime = "0" + mm + ":0" + ss;
        }
        else if (mm >= 10 && ss < 10)
        {
            stime = mm + ":0" + ss;
        }
        else
        {
            stime = mm + ":" + ss;
        }
        return stime;
    }

    public static string NumToChinese(string x)
    {
        string[] pArrayNum = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        //为数字位数建立一个位数组
        string[] pArrayDigit = { "", "十", "百", "千" };
        //为数字单位建立一个单位数组
        string[] pArrayUnits = { "", "万", "亿", "万亿" };
        var pStrReturnValue = ""; //返回值
        var finger = 0; //字符位置指针
        var pIntM = x.Length % 4; //取模
        int pIntK;
        if (pIntM > 0)
            pIntK = x.Length / 4 + 1;
        else
            pIntK = x.Length / 4;
        //外层循环,四位一组,每组最后加上单位: ",万亿,",",亿,",",万,"
        for (var i = pIntK; i > 0; i--)
        {
            var pIntL = 4;
            if (i == pIntK && pIntM != 0)
                pIntL = pIntM;
            //得到一组四位数
            var four = x.Substring(finger, pIntL);
            var P_int_l = four.Length;
            //内层循环在该组中的每一位数上循环
            for (int j = 0; j < P_int_l; j++)
            {
                //处理组中的每一位数加上所在的位
                int n = Convert.ToInt32(four.Substring(j, 1));
                if (n == 0)
                {
                    if (j < P_int_l - 1 && Convert.ToInt32(four.Substring(j + 1, 1)) > 0 && !pStrReturnValue.EndsWith(pArrayNum[n]))
                        pStrReturnValue += pArrayNum[n];
                }
                else
                {
                    if (!(n == 1 && (pStrReturnValue.EndsWith(pArrayNum[0]) | pStrReturnValue.Length == 0) && j == P_int_l - 2))
                        pStrReturnValue += pArrayNum[n];
                    pStrReturnValue += pArrayDigit[P_int_l - j - 1];
                }
            }
            finger += pIntL;
            //每组最后加上一个单位:",万,",",亿," 等
            if (i < pIntK) //如果不是最高位的一组
            {
                if (Convert.ToInt32(four) != 0)
                    //如果所有4位不全是0则加上单位",万,",",亿,"等
                    pStrReturnValue += pArrayUnits[i - 1];
            }
            else
            {
                //处理最高位的一组,最后必须加上单位
                pStrReturnValue += pArrayUnits[i - 1];
            }
        }
        return pStrReturnValue;

    }
 public static int[] GetRandomSequence(int total, int count)
    {
        int[] sequence = new int[total];
        int[] output = new int[count];

        for (int i = 0; i < total; i++)
        {
            sequence[i] = i;
        }
        int end = total - 1;
        for (int i = 0; i < count; i++)
        {
            //随机一个数，每随机一次，随机区间-1
            int num =UnityEngine. Random.Range(0, end + 1);
            output[i] = sequence[num];
            //将区间最后一个数赋值到取到的数上
            sequence[num] = sequence[end];
            end--;
        }
        return output;
    }
 public static  IEnumerator Timing(Text text, UnityEngine.Events.UnityAction action, int time = 180)
    {
        // this.time = time;
        text.text = string.Format("{0}", GetMinuteTime(time));
        while (time >= 0)
        {
            yield return new WaitForSeconds(1f);
            time--;
            text.text = string.Format("{0}", GetMinuteTime(time));
        }
       time = 0;
        text.text = string.Format("{0}", GetMinuteTime(time));
        if (action != null)
        {
            action();
        }
    }
    public static IEnumerator Timing1(Text text, UnityEngine.Events.UnityAction action, int time = 180)
    {
        // this.time = time;
        text.text = string.Format("（{0}）", time);
        while (time >= 0)
        {
            yield return new WaitForSeconds(1f);
            time--;
            text.text = string.Format("（{0}）", time);
        }
        time = 0;
        text.text = string.Format("（{0}）", time);
        if (action != null)
        {
            action();
        }
    }
    //public static void Fade(Graphic[] graphics, float value, float time=2f,UnityEngine.Events.UnityAction unityAction=null,bool isUpdate=true)
    //{
    //    foreach (var item in graphics)
    //    {
    //        item.DOFade(value, time).SetUpdate(isUpdate);
    //    }
    //    unityAction?.Invoke();
    //}
    //public static void Fade(Graphic graphics, float value, float time = 2f, UnityEngine.Events.UnityAction unityAction = null, bool isUpdate = true)
    //{

    //    graphics.DOFade(value, time).SetUpdate(isUpdate).onComplete+=()=> unityAction?.Invoke();
        
        
    //}
    public static IEnumerator  Delay(float time, UnityEngine.Events.UnityAction unityAction = null)
    {
        yield return new WaitForSeconds(time);
      
        unityAction?.Invoke();
    }
    public static Sprite sprite1;
   public static   IEnumerator UnityWebRequestGetData(Image _imageComp, string _url)
    {
       // print("下载图片+++" + _url);
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result== UnityWebRequest.Result.ConnectionError) Debug.Log(uwr.error);
            else
            {
                if (uwr.isDone)
                {
                   
                    //Texture2D texture2d = new Texture2D(width, height);
                    Texture2D texture2d = DownloadHandlerTexture.GetContent(uwr);
                    int width = texture2d.width;
                    int height = texture2d.height;
                   // print(width);
                   // print(height);
                    Sprite tempSprite = Sprite.Create(texture2d, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                    _imageComp.sprite = tempSprite;
                  //  _imageComp.SetNativeSize();
                    Resources.UnloadUnusedAssets();
                    
                }
            }
        }
    }
    public static IEnumerator UnityWebRequestGetSprite( Sprite sprite, string _url)
    {
        // print("下载图片+++" + _url);
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log(uwr.error);
            else
            {
                if (uwr.isDone)
                {

                    //Texture2D texture2d = new Texture2D(width, height);
                    Texture2D texture2d = DownloadHandlerTexture.GetContent(uwr);
                    int width = texture2d.width;
                    int height = texture2d.height;
                   // Debug.LogError(width);
                    //Debug.LogError(height);
                    Sprite tempSprite = Sprite.Create(texture2d, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                    sprite = tempSprite;
                    sprite1 = tempSprite;
                    //  _imageComp.SetNativeSize();
                    //Resources.UnloadUnusedAssets();

                }
            }
        }
    }

    public static void RandomSort<T>(ref List<T> ts)
    {
        for (int i = 0; i < ts.Count; i++)
        {
            T t = ts[i];
            int index = UnityEngine.Random.Range(0, ts.Count);
            ts[i] = ts[index];
            ts[index] = t;
        }
    } 
}
