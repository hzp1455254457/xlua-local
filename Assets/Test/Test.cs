using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class Test : MonoBehaviour
{
    // Start is called before the first frame update

    byte[] buff;
    void Start()
    {
        buff = new byte[1000];
        int A = 6;
        string value = "h";
      byte[] arry=  System.BitConverter.GetBytes(A);
        byte[] arry2 = System.BitConverter.GetBytes(14);
        byte[] arry1 = System.Text.Encoding.UTF8.GetBytes(value);
        print(arry1[0]);
        arry.CopyTo(buff, 0);
        arry2.CopyTo(buff, 4);
        arry1.CopyTo(buff, 8);
        //Array.Copy(arry, buff, 0);
        // Array.Copy(arry1, buff, 0);
        print( System.Text.Encoding.UTF8.GetString(arry1));
        print(System.BitConverter.ToInt32(buff,0));
        print(System.BitConverter.ToInt32(buff, 4));
    }

    // Update is called once per frame
  
}
