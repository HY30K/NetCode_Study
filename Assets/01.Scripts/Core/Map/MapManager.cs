using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("TileMap").GetComponent<MapManager>();
                if (_instance == null)
                {
                    Debug.LogError("There are no Tilemap");
                }
            }

            return _instance;
        }
    }

    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private LayerMask _whatIsObstacle;
    //�ش������� ������ ������ ������ ��ġ������ Ÿ���� ������ǥ�� ��ȯ�ϴ� �Լ�
    public List<Vector3> GetAvailablePositionList(Vector3 center, float radius)
    {
        int radiusInt = Mathf.CeilToInt(radius); //������ �ø�ó��
        Vector3Int tileCenter = _tilemap.WorldToCell(center);

        List<Vector3> pointList = new List<Vector3>();
        for (int i = -radiusInt; i <= radiusInt; i++)
        {
            for (int j = -radiusInt; j <= radiusInt; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) > radius) continue; //�������� ����� ������ �ȼ���

                Vector3Int cellPoint = tileCenter + new Vector3Int(j, i); //���ο� ������Ʈ ���ϰ�
                TileBase tile = _tilemap.GetTile(cellPoint);

                if (tile != null) continue; //�ش� Ÿ�� ��ġ���� ��ֹ��� ����

                Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPoint);
                var col = Physics2D.OverlapCircle(worldPos, 0.5f, _whatIsObstacle);

                if (col != null) continue;
                //������� �Դٸ� �ƹ��͵� ��ġ�°� ������.
                pointList.Add(worldPos);
            }
        }

        return pointList;
    }
}
