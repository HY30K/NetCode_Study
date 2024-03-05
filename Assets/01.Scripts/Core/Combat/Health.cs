using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    //���� Ŭ���̾�Ʈ�� ����ȭ�� ������ OnValueChanged �̺�Ʈ ������ ���濡 ���� ��ȭ�� �����ϴ�.
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    private bool _isDead;

    public Action<Health> OnDie;
    //������, ���ݰ�, �ִ�ġ���� ����
    public UnityEvent<int, int, float> OnHealthChanged;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            currentHealth.OnValueChanged += HealthChangeHandle;
            HealthChangeHandle(0, MaxHealth);
        }

        if (!IsServer) return;
        currentHealth.Value = MaxHealth; //ü���ʱ�ȭ�� ������ ���ش�.
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
        if (_isDead) return;   //����
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, MaxHealth);
        if (currentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            _isDead = true;
        }
    }
}

