using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingletone _clientPrefab;
    [SerializeField] private HostSingletone _hostPrefab;
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
            HostSingletone hostSingletone = Instantiate(_hostPrefab); //�����ٲٸ� �ȵ�
            hostSingletone.CreateHost();

            ClientSingletone clientSingletone = Instantiate(_clientPrefab);
            bool authenticated = await clientSingletone.CreateClient();

            if (authenticated)
            {
                // ���� �̰��� ���� �ε��κ��� ���� �Ѵ�.
                Debug.Log("Load");
                ClientSingletone.Instance.GameManager.GotoMenu();
            }
            else
            {
                Debug.LogError("UGS Service login failed");
            }
        }
    }

}
