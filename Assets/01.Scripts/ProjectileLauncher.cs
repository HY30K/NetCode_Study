using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("���� ������")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _projectileSpwanTrm;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;

    [SerializeField] private Collider2D _playerCollider;

    [Header("���ð���")]
    [SerializeField] private float _projectileSpeed;

    [SerializeField] private float _fireCooltime;

    public UnityEvent OnFire;

    private bool _shouldFire;
    private float _prevFireTime;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool button)
    {
        _shouldFire = button;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!_shouldFire) return;

        if (Time.time < _prevFireTime + _fireCooltime) return; //���� ��Ÿ���� �ȵǾ���.

        PrimaryFireServerRpc(_projectileSpwanTrm.position, _projectileSpwanTrm.up);
        SpawnDummyProjectile(_projectileSpwanTrm.position, _projectileSpwanTrm.up); //�̰� �߰�
        _prevFireTime = Time.time;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 dir)
    {
        GameObject projectileInstance = Instantiate(_clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = dir; //�̷��Ե� ȸ���� �ȴ�.
        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        OnFire?.Invoke();
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rigidbody.transform.up * _projectileSpeed;
        }
    }

    //������ �����ϰ� �������ִ� �ż���, ServerRPC�� Ŭ�� �����ϸ� ������ ��û�� ���� ������ �����Ѵ�.
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 dir)
    {
        GameObject projectileInstance = Instantiate(_serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = dir;
        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        //�������� �̰� �ִ´�. 
        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact damage))
        {
            damage.SetOwner(OwnerClientId); //������ ����
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rigidbody.transform.up * _projectileSpeed;
        }
        //Ŭ���̾�Ʈ RPC�ݷ� �����ϱ� �� NetworkBehaviour�� ã�Ƽ� �ű⼭ �ż��带 �����Ѵ�.
        SpawnDummyProjectileClientRpc(spawnPos, dir);
    }


    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 dir)
    {
        if (IsOwner) return; //������ �ƴϸ� ����

        SpawnDummyProjectile(spawnPos, dir);
    }
}
