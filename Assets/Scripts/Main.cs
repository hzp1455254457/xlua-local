using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
        LuaMgr.GetInstance().Init();
      LuaMgr.GetInstance().DoLuaFile("main");
   
        
    }

  
}
