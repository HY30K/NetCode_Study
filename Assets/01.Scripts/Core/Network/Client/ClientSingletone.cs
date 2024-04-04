using System.Threading.Tasks;
using TMPro.EditorUtilities;
using UnityEngine;

public class ClientSingletone : MonoBehaviour
{
    private static ClientSingletone instance;

    public ClientGameManager GameManager { get; set; }
    public static ClientSingletone Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<ClientSingletone>();

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

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();

        return await GameManager.InitAsync();
    }
}
