using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientPrefab;
    [SerializeField] private HostSingleton _hostPrefab;
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        //���� ��������Ʈ ������ ����� ���� ����
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            //do somethin in later
        }
        else
        {
            HostSingleton hostSingletone = Instantiate(_hostPrefab); //�����ٲٸ� �ȵ�
            hostSingletone.CreateHost();

            ClientSingleton clientSingletone = Instantiate(_clientPrefab);
            bool authenticated = await clientSingletone.CreateClient();

            if (authenticated)
            {
                // ���� �̰��� ���� �ε��κ��� ���� �Ѵ�.
                Debug.Log("Load");
                ClientSingleton.Instance.GameManager.GotoMenu();
            }
            else
            {
                Debug.LogError("UGS Service login failed");
            }
        }
    }

}
