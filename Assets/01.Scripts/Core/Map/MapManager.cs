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
    //해당지점의 반지름 내에서 동전을 배치가능한 타일의 월드좌표를 반환하는 함수
    public List<Vector3> GetAvailablePositionList(Vector3 center, float radius)
    {
        int radiusInt = Mathf.CeilToInt(radius); //반지름 올림처리
        Vector3Int tileCenter = _tilemap.WorldToCell(center);

        List<Vector3> pointList = new List<Vector3>();
        for (int i = -radiusInt; i <= radiusInt; i++)
        {
            for (int j = -radiusInt; j <= radiusInt; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) > radius) continue; //반지름을 벗어나는 영역은 안센다

                Vector3Int cellPoint = tileCenter + new Vector3Int(j, i); //새로운 셀포인트 구하고
                TileBase tile = _tilemap.GetTile(cellPoint);

                if (tile != null) continue; //해당 타일 위치에는 장애물이 존재

                Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPoint);
                var col = Physics2D.OverlapCircle(worldPos, 0.5f, _whatIsObstacle);

                if (col != null) continue;
                //여기까지 왔다면 아무것도 겹치는게 없었다.
                pointList.Add(worldPos);
            }
        }

        return pointList;
    }
}
