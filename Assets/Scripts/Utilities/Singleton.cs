
using UnityEngine;

//Singleton实现泛型单例
public class Singleton<T> : MonoBehaviour where T: Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get => instance;
    }
    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;
    }
    protected virtual void Destory()
    {
        if (instance == this)
            instance = null;
    }
}
