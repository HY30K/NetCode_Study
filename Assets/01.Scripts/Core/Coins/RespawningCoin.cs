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
            return 0;  //Ŭ��� �������� �ʰ� ���ְ�
        }

        _alreadyCollected = true;
        OnCollected?.Invoke(this);
        isActive.Value = false; //��Ƽ�긦 ���ְ� 

        return _coinValue;
    }

    //�̰͵� ������ ����
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
            _prevPos = transform.position;   //�̷��� ���ϰ� ����  coin�� active�� onchange�� �ɸ� ��� �Ǵ����� �������  ��Ƽ�� �� ���Ŀ� �����̵� �Ǵ� �һ�簡 �����.
            SetVisible(true);
        }
    }

}
