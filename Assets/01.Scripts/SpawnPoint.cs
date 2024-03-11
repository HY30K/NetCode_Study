using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPoint : NetworkBehaviour
{
    public string pointName;
    public Vector3 Position => transform.position;
    [field: SerializeField] public float Radius { get; private set; } = 10f;
    public List<Vector3> SpawnPoints { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //스폰가능한 포인트들
            SpawnPoints = MapManager.Instance.GetAvailablePositionList(transform.position, Radius);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
        Gizmos.color = Color.white;
    }
#endif
}
