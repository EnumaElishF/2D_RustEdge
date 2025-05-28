using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTilemap;

    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            if (mapData != null)
            {
                mapData.tileProperties.Clear();
            }
        }
    }
    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();

            UpdateTileProperties();
            //ȷ���������ֻ����UNITY_EDITOR��Unity�����ʹ�ã���Ϊ���Ǵ������Ϸ��Ҫ������ȥʹ�õ�
#if UNITY_EDITOR
            if (mapData != null)
                EditorUtility.SetDirty(mapData); //��mapData���dirty�����ܽ���ʵʱ���޸ĺͱ���
#endif
        }
    }
    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();//Compressѹ���̶��Ѿ������Ƶ���Ƭ��ͼ 
        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                //�ѻ��Ʒ�Χ�����½�����
                Vector3Int startPos = currentTilemap.cellBounds.min;
                //�ѻ��Ʒ�Χ�����Ͻ�����
                Vector3Int endPos = currentTilemap.cellBounds.max;
                //�����ѭ��
                for(int x = startPos.x; x < endPos.x; x++)
                {
                    for(int y = startPos.y; y < endPos.y; y++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tileCoordinate = new Vector2Int(x, y),
                                gridType = this.gridType,
                                boolTypeValue = true
                            };
                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
