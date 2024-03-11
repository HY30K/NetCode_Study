using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : NetworkBehaviour
{
    [Header("���� ��")]
    [SerializeField] private RespawningCoin _coinPrefab;
    [SerializeField] private DecalCircle _decalCircle;

    [Header("���ð�")]
    [SerializeField] private int _maxCoins = 30; //30���� ����Ǯ ����
    [SerializeField] private int _coinValue = 10; //���δ� 10
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _spawningTerm = 30f;
    //[SerializeField] private float _spawnRadius = 8f;
    private bool _isSpawning = false; //������ ���ΰ�?
    private float _spawnTime = 0;
    private int _spawnCountTime = 10; //10��ī�����ϰ� ����


    public List<SpawnPoint> spawnPointList; //������ �������� ����Ʈ
    private float _coinRadius;

    private Stack<RespawningCoin> _coinPool = new Stack<RespawningCoin>(); //����Ǯ
    private List<RespawningCoin> _activeCoinList = new List<RespawningCoin>(); //Ȱ��ȭ�� ���ε�


    //�� �ż���� ������ �����մϴ�.
    private RespawningCoin SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(_coinPrefab, Vector3.zero, Quaternion.identity);
        coinInstance.SetValue(_coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn(); //������ Ŭ��鿡�� ������ �˸�
        coinInstance.OnCollected += HandleCoinCollected;

        return coinInstance;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        //�ݷ�Ʈ�� ������ ����Ʈ�� �ٽ� �ִ´�
        _activeCoinList.Remove(coin); //����Ʈ���� ���� �����ϰ� 
        coin.SetVisible(false);
        _coinPool.Push(coin);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        //ó�� �����ϸ� ������ �ִ� ���θ�ŭ ������ ���Ѽ� Ǯ���Ѵ�.
        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < _maxCoins; i++)
        {
            var coin = SpawnCoin();
            coin.SetVisible(false); //ó�� ������ �ֵ��� ���ش�.
            _coinPool.Push(coin);
        }
    }

    public override void OnNetworkDespawn()
    {
        StopAllCoroutines(); //�ڷ�ƾ ��� ����
    }

    private void Update()
    {
        if (!IsServer) return; //���� �ƴϰ�� ī��Ʈ �ʿ� ����.

        //Ȱ��ȭ�� ������ ���� ���������� �ƴ϶�� �ð��� ���ϱ� ����
        if (!_isSpawning && _activeCoinList.Count == 0)
        {
            _spawnTime += Time.deltaTime;
            if (_spawnTime >= _spawningTerm)
            {
                _spawnTime = 0;
                StartCoroutine(SpawnCoroutine());
            }
        }
    }

    [ClientRpc]
    private void ServerCountDownMessageClientRpc(int sec, int pointIdx, int coinCount)
    {
        if (!_decalCircle.showDecal)
        {
            _decalCircle.OpenCircle(spawnPointList[pointIdx].Position, 8f);
        }
        Debug.Log($"{pointIdx} �� �������� {sec}���� {coinCount}���� ������ �����˴ϴ�.");
    }

    [ClientRpc]
    private void DecalCircleClientRpc()
    {
        _decalCircle.CloseCircle();
    }

    IEnumerator SpawnCoroutine()
    {
        //�̰� ������ �����ϴϱ� ���� �Ȱɷ��� ��
        _isSpawning = true;
        int pointIdx = Random.Range(0, spawnPointList.Count);
        var point = spawnPointList[pointIdx];
        int maxCoinCount = Mathf.Min(_maxCoins + 1, point.SpawnPoints.Count);
        int coinCount = Random.Range(_maxCoins / 2, maxCoinCount);


        for (int i = _spawnCountTime; i > 0; i--)
        {
            ServerCountDownMessageClientRpc(i, pointIdx, coinCount);
            yield return new WaitForSeconds(1f);
        }

        for (int i = 0; i < coinCount; ++i)
        {
            int end = point.SpawnPoints.Count - i - 1;
            int idx = Random.Range(0, end + 1);
            Vector2 pos = point.SpawnPoints[idx];
            (point.SpawnPoints[idx], point.SpawnPoints[end]) = (
                point.SpawnPoints[end], point.SpawnPoints[idx]);

            var coin = _coinPool.Pop();
            coin.transform.position = pos;
            coin.Reset();
            _activeCoinList.Add(coin);
            yield return new WaitForSeconds(4f); //4�ʸ��� ���� �ϳ���
        }

        _isSpawning = false;
        DecalCircleClientRpc(); //Ŭ�󿡼� �ݾ���
    }
}
