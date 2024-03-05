using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("참조 변수들")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _projectileSpwanTrm;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;

    [SerializeField] private Collider2D _playerCollider;

    [Header("셋팅값들")]
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

        if (Time.time < _prevFireTime + _fireCooltime) return; //아직 쿨타임이 안되었다.

        PrimaryFireServerRpc(_projectileSpwanTrm.position, _projectileSpwanTrm.up);
        SpawnDummyProjectile(_projectileSpwanTrm.position, _projectileSpwanTrm.up); //이거 추가
        _prevFireTime = Time.time;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 dir)
    {
        GameObject projectileInstance = Instantiate(_clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = dir; //이렇게도 회전이 된다.
        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        OnFire?.Invoke();
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rigidbody.transform.up * _projectileSpeed;
        }
    }

    //서버가 검증하고 실행해주는 매서드, ServerRPC는 클라가 실행하면 서버에 요청이 들어가서 서버가 실행한다.
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 dir)
    {
        GameObject projectileInstance = Instantiate(_serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = dir;
        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        //서버에만 이걸 넣는다. 
        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact damage))
        {
            damage.SetOwner(OwnerClientId); //소유주 설정
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rigidbody.transform.up * _projectileSpeed;
        }
        //클라이언트 RPC콜로 날리니까 이 NetworkBehaviour를 찾아서 거기서 매서드를 실행한다.
        SpawnDummyProjectileClientRpc(spawnPos, dir);
    }


    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 dir)
    {
        if (IsOwner) return; //주인이 아니면 실행

        SpawnDummyProjectile(spawnPos, dir);
    }
}
