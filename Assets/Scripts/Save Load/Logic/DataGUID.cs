using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ExecuteAlways此代码一直运行
[ExecuteAlways]
public class DataGUID : MonoBehaviour
{
    public string guid;

    private void Awake()
    {
        if(guid == string.Empty)
        {
            //通过System去生成新的guid是一个16为的字符串
            guid = System.Guid.NewGuid().ToString();
        }
    }
}
