using Farm.Map;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Farm.AStar
{
    public class AStar : MonoBehaviour
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;

        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodeList; //��ǰѡ�е�Node��Χ��8����
        private HashSet<Node> closeNodeList; //���б�ѡ�еĵ�

        private bool pathFound;

        public void BuilPath(string sceneName,Vector2Int startPos,Vector2Int endPos)
        {
            pathFound = false;
            if (GenerateGridNodes(sceneName, startPos, endPos))
            {
                //�������·��
                if (FindShortestPath())
                {
                    //����NPC���·��

                }


            }
        }
        /// <summary>
        /// ��������ڵ���Ϣ����ʼ�������б�
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="startPos">���</param>
        /// <param name="endPos">�յ�</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos,Vector2Int endPos)
        {
            if(GridMapManager.Instance.GetGridDimensions(sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin))
            {
                //������Ƭ��ͼ��Χ���������ƶ��ڵ㷶Χ����
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
            //gridNodes�ķ�Χ�Ǵ�0,0��ʼ��������Ҫ��ȥԭ������õ�ʵ��λ��
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            for(int x = 0; x < gridWidth; x++)
            {
                for(int y = 0; y < gridHeight; y++)
                {
                    //��֮ǰ�ӳ����Ĺ̶����򣬽���key
                    var key = (x + originX) + "x" + (y + originY) + "y" + sceneName;
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);
                    if(tile!= null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);
                        if (tile.isNPCObstacle)
                            node.isObstacle = true;
                    }
                }
            }
            return true;

        }

        private bool FindShortestPath()
        {
            //������
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                //�ڵ�����
                openNodeList.Sort();

                Node closeNode = openNodeList[0]; //����Ľڵ�

                openNodeList.RemoveAt(0);
                closeNodeList.Add(closeNode);
                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                //������Χ8��Node���䵽OpenNodeList

            }
            return pathFound;


        }

    }

}
