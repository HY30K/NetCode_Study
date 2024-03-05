using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    //서버 클라이언트간 동기화된 변수로 OnValueChanged 이벤트 구독시 변경에 따른 변화도 가능하다.
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    private bool _isDead;

    public Action<Health> OnDie;
    //이전값, 지금값, 최대치와의 비율
    public UnityEvent<int, int, float> OnHealthChanged;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            currentHealth.OnValueChanged += HealthChangeHandle;
            HealthChangeHandle(0, MaxHealth);
        }

        if (!IsServer) return;
        currentHealth.Value = MaxHealth; //체력초기화는 서버만 해준다.
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
            currentHealth.OnValueChanged -= HealthChangeHandle;
    }

    private void HealthChangeHandle(int prev, int newValue)
    {
        OnHealthChanged?.Invoke(prev, newValue, (float)newValue / MaxHealth);
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(int value)
    {
        if (_isDead) return;   //과제
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, MaxHealth);
        if (currentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            _isDead = true;
        }
    }
}

