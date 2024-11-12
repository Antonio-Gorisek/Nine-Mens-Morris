using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.98u125xf2ipd")]
public class Singleton<T> : MonoBehaviour where T:Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindFirstObjectByType<T>();

                if(instance == null)
                {
                    GameObject gameObject = new GameObject("SingletonController");
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this as T; 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
