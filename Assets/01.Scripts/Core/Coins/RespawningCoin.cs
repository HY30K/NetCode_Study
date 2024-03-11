using System;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;
    private Vector2 _prevPos;

    public override int Collect()
    {
        if (_alreadyCollected) return 0;

        if (!IsServer)
        {
            SetVisible(false);
            return 0;  //클라는 보이지만 않게 해주고
        }

        _alreadyCollected = true;
        OnCollected?.Invoke(this);
        isActive.Value = false; //액티브를 꺼주고 

        return _coinValue;
    }

    //이것도 서버만 실행
    public void Reset()
    {
        _alreadyCollected = false;
        isActive.Value = true;
        SetVisible(true);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _prevPos = transform.position;
    }

    private void Update()
    {
        if (IsServer) return;

        if (Vector2.Distance(_prevPos, transform.position) >= 0.1f)
        {
            _prevPos = transform.position;   //이렇게 안하고 만약  coin에 active에 onchange를 걸면 어떻게 되는지도 보여줘라  액티브 된 이후에 순간이동 되는 불상사가 생긴다.
            SetVisible(true);
        }
    }

}
