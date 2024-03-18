using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingletone _clientPrefab;
    [SerializeField] private HostSingletone _hostPrefab;
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        //차후 데디케이트 서버를 만들기 위한 구조
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
            HostSingletone hostSingletone = Instantiate(_hostPrefab); //순서바꾸면 안돼
            hostSingletone.CreateHost();

            ClientSingletone clientSingletone = Instantiate(_clientPrefab);
            await clientSingletone.CreateClient();

            //Go to main menu
        }
    }

}
