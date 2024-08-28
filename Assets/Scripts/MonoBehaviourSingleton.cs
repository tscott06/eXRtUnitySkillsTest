using System;
using UnityEngine;

/// <summary>
/// Adds singleton behaviour to a class
/// The singleton must be references via the static Instance
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [Serializable] 
    public class SingletonSettings //Having the setting in it's own class is just a bit tidier on inspector. In full project custom inspector would help deal with this
    {
        [SerializeField] private bool _dontDestroyOnLoadScene = true;
        public bool DontDestroy => _dontDestroyOnLoadScene;
    }

    /// <summary>
    /// Settings for this singleton
    /// </summary>
    [SerializeField] SingletonSettings _singletonSettings = new();

    protected static T _instance = null;
    public static T Instance
    {
        get
        {
            //Prevent instances unintentionally being created in edit mode. Can cause headaches
            if (!Application.isPlaying)
                return null;

            return GetInstance();
        }
    }

    public static bool IsInstanceCreated => (object)_instance != null;
    public SingletonSettings SingletonInstanceSettings { get => _singletonSettings; }


    protected static T GetInstance()
    {
        if ((object)_instance == null)
        {
            //Check if already exists in Hierarachy
            _instance = (T)FindObjectOfType(typeof(T));

            //If not, create new instance
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject(typeof(T).ToString());
                _instance = singletonObject.AddComponent<T>();

                Debug.LogWarning($"Created new instance of this singleton {_instance.name} as one was not found when another class attempted to access this");
            }
        }

        return _instance;
    } 

    protected virtual void Awake()
    {
        //If an instance of this class has NOT already been created via Awake, or by an attempt to call the instance via script
        //make this the instance. Otherwise destroy it.
        if (_instance == null)
        {
            _instance = GetComponent<T>();

            if(SingletonInstanceSettings.DontDestroy)
                DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }    
}
