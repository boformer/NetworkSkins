using UnityEngine;

namespace NetworkSkins
{
    public class ModSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var gameObject = new GameObject(typeof(T).Name);
                        _instance = gameObject.AddComponent<T>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        public static T Ensure()
        {
            return instance;
        }

        public static void Uninstall()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }
        }
    }
}
