using Unity.Netcode;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{
    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Coin>(out Coin coin))
        {
            int value = coin.Collect();

            if (!IsServer) return; //������ �ƴҰ�� ���� �ݷ�Ʈ ����
            totalCoins.Value += value;
        }
    }
}
