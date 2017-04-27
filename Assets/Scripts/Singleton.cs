using UnityEngine;
using System.Collections;

/// <summary>
/// MONOBEHAVIOR PSEUDO SINGLETON ABSTRACT CLASS
/// usage    : best is to be attached to a gameobject but if not that is ok,
///      : this will create one on first access
/// example  : '''public sealed class MyClass : Singleton<MyClass> {'''
/// references   : http://tinyurl.com/d498g8c
///      : http://tinyurl.com/cc73a9h
///      : http://unifycommunity.com/wiki/index.php?title=Singleton
///      : http://zingweb.com/blog/2012/04/26/unity-singletons/
/// in your child class you can implement Awake()
/// and add any initialization code you want such as
/// DontDestroyOnLoad(go);
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _instance = null;
    /// <summary>
    /// gets the instance of this Singleton
    /// use this for all instance calls:
    /// MyClass.Instance.MyMethod();
    /// or make your public methods static
    /// and have them use Instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    string goName = typeof(T).ToString();
                    GameObject go = GameObject.Find(goName);
                    if (go == null)
                    {
                        go = new GameObject();
                        go.name = goName;
                        DontDestroyOnLoad(go);
                    }
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// for garbage collection
    /// </summary>
    public virtual void OnApplicationQuit()
    {
        // release reference on exit
        _instance = null;
    }

    virtual public void Initialize()
    {
    }

    virtual public void Release()
    {
    }
}
