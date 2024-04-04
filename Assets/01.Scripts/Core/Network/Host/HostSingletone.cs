using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class HostSingletone : MonoBehaviour
{
    private static HostSingletone instance;

    public HostGameManager GameManager { get; private set; }
    public static HostSingletone Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<HostSingletone>();

            if (instance == null)
            {
                Debug.LogError("No Client Singletone");
                return null;
            }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
}
