using Farm.Map;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Farm.AStar
{
    public class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;

        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodeList; //当前选中的Node周围的8个点
        private HashSet<Node> closeNodeList; //所有被选中的点

        private bool pathFound;

        /// <summary>
        /// 构建路径更新Stack的每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="npcMovementStack"></param>
        public void BuildPath(string sceneName,Vector2Int startPos,Vector2Int endPos,Stack<MovementStep> npcMovementStack)
        {
            pathFound = false;
            if (GenerateGridNodes(sceneName, startPos, endPos))
            {
                //查找最短路径
                if (FindShortestPath())
                {
                    //构建NPC最短路径
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                }


            }
        }
        /// <summary>
        /// 构建网格节点信息，初始化两个列表
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="startPos">起点</param>
        /// <param name="endPos">终点</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos,Vector2Int endPos)
        {
            if(GridMapManager.Instance.GetGridDimensions(sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin))
            {
                //根据瓦片地图范围构建网格移动节点范围数据
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();
                closeNodeList = new HashSet<Node>();
            }
            else
            {
                return false;
            }
            //gridNodes的范围是从0,0开始，所以需要减去原点坐标得到实际位置
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            for(int x = 0; x < gridWidth; x++)
            {
                for(int y = 0; y < gridHeight; y++)
                {
                    //按之前加场景的固定规则，建立key
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    var key = tilePos.x + "x" + tilePos.y + "y" + sceneName;

                    TileDetails tile = GridMapManager.Instance.GetTileDetails(key);

                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCObstacle)
                            node.isObstacle = true;
                    }
                }
            }
            return true;

        }
        /// <summary>
        /// 找到最短路径 所有Node添加到 closedNodeList
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            //添加起点
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                //节点排序，Node内涵比较函数
                openNodeList.Sort();

                Node closeNode = openNodeList[0]; //最近的节点

                openNodeList.RemoveAt(0);
                closeNodeList.Add(closeNode);
                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                //计算周围8个Node补充到OpenNodeList
                EvaluateNeighbourNodes(closeNode);
            }
            return pathFound;


        }

        /// <summary>
        /// 评估周围8个点，并生成对应消耗值
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;
            //循环了8次，除了中心点以外都循环了
            for(int x= -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    validNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);
                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            //连接父节点
                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 找到有效的Node，非障碍，非已选择的点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNode(int x,int y)
        {
            if(x>=gridWidth || y>=gridHeight || x < 0 || y < 0)
            {
                return null;
            }
            Node neighbourNode = gridNodes.GetGridNode(x, y);
            if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode)) //不能是障碍，也不能是被选择的点
                return null;
            else
                return neighbourNode;
        }
        /// <summary>
        /// 返回两点之间距离值（寻路可走只能单格子走或者斜方向一个格子
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14的倍数+10的倍数</returns>
        private int GetDistance(Node nodeA,Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);
            if (xDistance > yDistance)
            {
                //横格距离是10，斜着是14≈14.14
                return 14 * yDistance + 10 * (xDistance - yDistance);

            }
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
        /// <summary>
        /// 更新每一步的坐标和场景名称  (堆栈, 从终点反向推回来，然后压入栈中)
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="npcMovementStep"></param>
        //
        private void UpdatePathOnMovementStepStack(string sceneName,Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;

            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                //压入堆栈中
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;

            }
        }
    }

}
