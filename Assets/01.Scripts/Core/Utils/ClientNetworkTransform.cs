using Unity.Netcode.Components;

public class ClientTransform : NetworkTransform
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner; //소유자는 변경이 가능하게
    }

    protected override bool OnIsServerAuthoritative()
    {
        //서버인증을 false로 놓고
        return false;
    }

    protected override void Update()
    {
        CanCommitToTransform = IsOwner;
        base.Update();

        //이건 NetworkTransform안에 있는 거다. 정확히는 NetworkBehaviour라는 것 밑에
        //얘는 NetworkObject에 붙은 NetworkManager를 가져오는거 (이게 어짜피 네트워크 싱글톤임)
        //구현부를 읽어보면 차후 여러개의 네트워크 매니저 구동시에 해당 오브젝트를 소유한 네트워크 매니저를 리턴하도록 만들 예정
        if (NetworkManager != null)
        {
            //클라이언트로서 연결되어있거나 서버로서 리스닝을 하고 있는 중이라면 
            if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                if (CanCommitToTransform)
                {
                    //그럼 서버로 자신의 transform과 현재시간으로 동기화를 시도한다. 
                    //시간을 주면 interpolate가 가능하다.

                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                }
            }
        }
    }
}
