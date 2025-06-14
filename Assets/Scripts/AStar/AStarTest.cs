using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Farm.AStar
{
    public class AStarTest : MonoBehaviour
    {
        private AStar aStar;
        public Vector2Int startPos;
        public Vector2Int finishPos;
        public Tilemap displayMap;
        public TileBase displayTile;
        public bool displayStartAndFinish;
        public bool displayPath;

        private Stack<MovementStep> npcMovementStepStack;

        [Header("测试移动NPC")]
        public NPCMovement npcMovement;
        public bool moveNPC;
        [SceneName] public string targetScene;
        public Vector2Int targetPos;

        public AnimationClip stopClip;

        private void Awake()
        {
            aStar = GetComponent<AStar>();
            npcMovementStepStack = new Stack<MovementStep>();
        }
        private void Update()
        {
            ShowPathOnGridMap();
            if (moveNPC)
            {
                moveNPC = false;
                var schedule = new ScheduleDetails(0, 0, 0, 0, Season.春天, targetScene, targetPos, stopClip, true);
                npcMovement.BuildPath(schedule);
            }
        }
        private void ShowPathOnGridMap()
        {
            if(displayMap!=null && displayTile != null)
            {
                if (displayStartAndFinish)
                {
                    displayMap.SetTile((Vector3Int)startPos, displayTile);
                    displayMap.SetTile((Vector3Int)finishPos , displayTile);
                }
                else
                {
                    //不显示出来
                    displayMap.SetTile((Vector3Int)startPos, null);
                    displayMap.SetTile((Vector3Int)finishPos, null);
                }
                if (displayPath)
                {
                    var sceneName = SceneManager.GetActiveScene().name;
                    aStar.BuildPath(sceneName, startPos, finishPos, npcMovementStepStack);
                    foreach(var step in npcMovementStepStack)
                    {
                        displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile); //生成最短路径
                    }
                }
                else
                {
                    if(npcMovementStepStack.Count >0)
                    {
                        foreach(var step in npcMovementStepStack)
                        {
                            displayMap.SetTile((Vector3Int)step.gridCoordinate, null); //清空路径
                        }
                        npcMovementStepStack.Clear();
                    }
                }
            }
        }

    }

}
