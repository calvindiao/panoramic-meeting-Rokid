using System.Globalization;
using System;
using UnityEngine;

namespace Rokid.UXR.UI
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;

        private static object locker = "locker";

        public static T Instance()
        {
            lock (locker)
            {
                if (Singleton<T>._instance == null)
                {
                    Singleton<T>._instance = Activator.CreateInstance<T>();
                    if (Singleton<T>._instance == null)
                    {
                        Debug.LogError("Failed to create the instance of " + typeof(T) + " as singleton!");
                    }
                }
                return Singleton<T>._instance;
            }
        }
        public static void Release()
        {
            if (Singleton<T>._instance != null)
            {
                Singleton<T>._instance = (T)((object)null);
            }
        }
    }

    public class SingleMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;
        private static GameObject m_container;
        private static object locker = "locker";

        public virtual void Awake()
        {
            m_instance = this.GetComponent<T>();
        }

        public static T Instance
        {
            get
            {
                lock (locker)
                {
                    if (!m_instance)
                    {
                        GameObject go = GameObject.Find(typeof(T).ToString()); ;
                        if (go != null)
                        {
                            m_instance = go.GetComponent<T>();
                        }
                        if (!m_instance)
                        {
                            m_container = new GameObject();
                            m_container.name = typeof(T).ToString();
                            m_instance = m_container.AddComponent(typeof(T)) as T;
                        }
                    }
                    return m_instance;
                }
            }
        }

        public virtual void OnDestroy()
        {
            // #if UNITY_EDITOR
            //         DestroyImmediate(this.gameObject);
            // #else
            //         Destroy(this.gameObject);
            // #endif
        }

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            DestroyImmediate(this.gameObject);
#else
        Destroy(this.gameObject);
#endif
        }
    }
}
