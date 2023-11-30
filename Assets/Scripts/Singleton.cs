using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    static private T _instance = null;
    static public T GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<T>();
        }

        if (_instance == null )
        {
            GameObject singletonGO = new ();
            singletonGO.name = typeof(T).Name;
            _instance = singletonGO.AddComponent<T>();
        }

        return _instance;
    }
}
