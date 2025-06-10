using Farm.CropPlant;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Farm.Map
{
    //д��Singleton����ģʽ
    public class GridMapManager : Singleton<GridMapManager>
    {
        [Header("�ֵ���Ƭ�л���Ϣ")]
        public RuleTile digTile;
        public RuleTile waterTile;

        private Tilemap digTilemap;
        private Tilemap waterTilemap;

        [Header("��ͼ��Ϣ")]
        public List<MapData_SO> mapDataList;

        private Season currentSeason;



        //��������+����Ͷ�Ӧ����Ƭ��Ϣ ��key�ǳ�������+���֣����磺x����y����01Field )(value��TileDetails)
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        //��¼�����Ƿ��һ�μ���
        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();

        //�Ӳ��б�
        private List<ReapItem> itemsInRadius;

        private Grid currentGrid;
        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += RefreshMap;
        }


        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;


        }


        private void Start()
        {
            foreach (var mapData in mapDataList)
            {
                firstLoadDict.Add(mapData.sceneName, true);
                InitTileDetailsDict(mapData);
            }
        }

        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindAnyObjectByType<Grid>();
            digTilemap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            if (firstLoadDict[SceneManager.GetActiveScene().name]) //true���ǵ�һ�μ��أ�
            {
                //Ԥ������ũ����
                EventHandler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false; //���������������������ǵ�һ�μ���
            }



            RefreshMap();
        }
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daySinceWatered > -1)
                {
                    tile.Value.daySinceWatered = -1;
                }
                if (tile.Value.daySinceDug > -1)
                {
                    tile.Value.daySinceDug++;
                }
                //��������ڿ�
                if (tile.Value.daySinceDug > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daySinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if (tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }

        /// <summary>
        /// ���ݵ�ͼ��Ϣ�����ֵ�
        ///  �����Ե��ŵ�ͼ��д��ʼ����Ȼ�����ϲ���ѭ�� ����List���ÿһ��
        /// </summary>
        /// <param name="mapData"></param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y,
                };
                //�ֵ�key
                string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;

                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }
                if (GetTileDetails(key) != null)
                {
                    tileDetailsDict[key] = tileDetails;//����о͸���tileDetails
                }
                else
                {
                    tileDetailsDict.Add(key, tileDetails);//���û�оͰ�tileDetails������ȥ
                }
            }
        }
        /// <summary>
        /// ����key������Ƭ��Ϣ
        /// </summary>
        /// <param name="key">x+y+��ͼ����</param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key]; //���û��tileDetailsDict.ContainsKey(key) ֱ��return��û���ҵ��ֵ�Ļ�����ֱ�ӱ���
            }
            return null;
        }

        /// <summary>
        /// ��������������꣬������Ƭ��Ϣ
        /// </summary>
        /// <param name="mouseGridPos">�����������</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }

        /// <summary>
        /// ִ��,ʵ�ʹ��߻���Ʒ�Ĺ���
        /// </summary>
        /// <param name="mouseWorldPos">�������</param>
        /// <param name="itemDetails">��Ʒ��Ϣ</param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentTile != null)
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);

                // WORKFLOW : ��Ʒʹ��ʵ�ʹ��� +��������
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTile); //����ũ����
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos,itemDetails.itemType);
                        break;
                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType); //������Ʒ����Ʒʵ������
                        break;
                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daySinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        //��Ч
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daySinceWatered = 0;
                        //��Ч
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        // ִ���ո��߼�   :?.����currentCrop���Ϊ�մ���
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case ItemType.CollectTool:
                        // ִ���ո��߼�
                        currentCrop.ProcessToolAction(itemDetails,currentTile);
                        break;
                    case ItemType.ReapTool:
                        var reapCount = 0;
                        for(int i = 0; i < itemsInRadius.Count; i++)
                        {
                            EventHandler.CalllParticleEffectEvent(ParticalEffectType.ReapableScenery, itemsInRadius[i].transform.position+Vector3.up);
                            itemsInRadius[i].SpawnHarvestItems();
                            Destroy(itemsInRadius[i].gameObject);
                            reapCount++;
                            if (reapCount >= Settings.reapAmount)
                                break;
                        }
                        break;
                }
                UpdateTileDetails(currentTile);
            }
        }
        /// <summary>
        /// ͨ���������ж������λ�õ�ũ����
        /// </summary>
        /// <param name="mouseWorldPos">�������</param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();
            }
            return currentCrop;
        }
        /// <summary>
        /// ���ع��߷�Χ���Ӳ�
        /// </summary>
        /// <param name="tool">��Ʒ��Ϣ</param>
        /// <returns></returns>
        public bool HaveReapableItemsInRadius(Vector3 mouseWorldPos,ItemDetails tool)
        {
            itemsInRadius = new List<ReapItem>();
            //!!! OverlapCircleNonAlloc�Ѿ����ã���Ҫ���޸�
            Collider2D[] colliders = new Collider2D[20];
            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, tool.itemUseRadius, colliders);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemsInRadius.Add(item);
                        }
                    }
                }
            }
            return itemsInRadius.Count > 0;
        }
        /// <summary>
        /// ���ع��߷�Χ���Ӳ�,23.1�汾
        /// </summary>
        /// <param name="tool">��Ʒ��Ϣ</param>
        /// <returns></returns>
        public bool HaveReapableItemsInRadiusNew(Vector3 mouseWorldPos, ItemDetails tool)
        {
            itemsInRadius = new List<ReapItem>();

            // ʹ�� OverlapCircleAll ��� OverlapCircle����ȡ������ײ��
            //OverlapCircleAll�ʺϸ�ݣ���ȡ��ⷶΧȫ������ײ��
            //OverlapCircleֻ�᷵�ص�һ����ײ��
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mouseWorldPos, tool.itemUseRadius);

            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider != null && collider.GetComponent<ReapItem>() != null)
                    {
                        itemsInRadius.Add(collider.GetComponent<ReapItem>());
                    }
                }
            }
            return itemsInRadius.Count > 0;
        }
        /// <summary>
        /// ��ʾ�ڿ���Ƭ
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (digTilemap != null)
            {
                digTilemap.SetTile(pos, digTile);
            }
        }
        /// <summary>
        /// ��ʾ��ˮ��Ƭ
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (waterTilemap != null)
            {
                waterTilemap.SetTile(pos, waterTile);
            }
        }

        /// <summary>
        /// ������Ƭ��Ϣ
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }
        /// <summary>
        /// ˢ�µ�ǰ��ͼ
        /// ��������ˢ�������������������
        /// </summary>
        private void RefreshMap()
        {
            if (digTilemap != null)
                digTilemap.ClearAllTiles(); // ʹ���Դ��ķ���ֱ��Clear��Ƭ
            if (waterTilemap != null)
                waterTilemap.ClearAllTiles();

            //�� ʹ�� FindObjectsByType �����ʱ�� FindObjectsOfType
            foreach (var crop in FindObjectsByType<Crop>(FindObjectsSortMode.None))
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// ��ʾ��ͼ��Ƭ
        /// </summary>
        /// <param name="sceneName">��������</param>
        private void DisplayMap(string sceneName)
        {
            foreach(var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;
                if (key.Contains(sceneName))
                {
                    if (tileDetails.daySinceDug > -1)
                    {
                        SetDigGround(tileDetails);
                    }
                    if (tileDetails.daySinceWatered > -1)
                    {
                        SetWaterGround(tileDetails);
                    }
                    //����
                    if(tileDetails.seedItemID > -1)
                    {
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails); 
                    }
                }
            }
        }

    }
}

