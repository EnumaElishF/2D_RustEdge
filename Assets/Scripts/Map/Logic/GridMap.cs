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
            //确保下面命令，只有在UNITY_EDITOR，Unity里才能使用，因为我们打包的游戏是要发布出去使用的
#if UNITY_EDITOR
            if (mapData != null)
                EditorUtility.SetDirty(mapData); //将mapData标记dirty，才能进行实时的修改和保存
#endif
        }
    }
    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();//Compress压缩固定已经被绘制的瓦片地图 
        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                //已绘制范围的左下角坐标
                Vector3Int startPos = currentTilemap.cellBounds.min;
                //已绘制范围的右上角坐标
                Vector3Int endPos = currentTilemap.cellBounds.max;
                //矩阵的循环
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
