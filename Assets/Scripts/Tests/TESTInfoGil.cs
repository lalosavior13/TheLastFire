using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

namespace Voidless
{
[Serializable]

/// Class to callback in EditorWindows scripts and save values
public class TESTInfoGil
{
    [SerializeField] private string[] _arrayInfo;
  
    public string[] arrayInfo
    {
        get { return _arrayInfo; }
        set { _arrayInfo = value; }
    }

     // Constructor
    public TESTInfoGil()
    {
            arrayInfo = new string[0];
    }

    //Length modifier
    public void ResizeInfoArray(int _size)
        {
            Array.Resize(ref _arrayInfo, _size);
        }

}
}

