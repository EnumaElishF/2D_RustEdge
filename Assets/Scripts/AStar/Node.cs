using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition; //网格坐标
        public int gCost = 0; //距离Start格子的距离
        public int hCost = 0; //距离Target格子的距离
        public int FCost => gCost + hCost; //当前格子的值

        //目标是找到所有FCost最低的一条路，然后反向回来，得到最近的这条路径
        public bool isObstacle = false; //当前格子是否是障碍
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;

        }

        /// <summary>
        /// 比较两个节点的FCost， 对IComparable<Node>实现接口
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Node other)
        {
            // 相等返回0，小于返回-1，大于返回1
            int result = FCost.CompareTo(other.FCost);
            if(result == 0)
            {
                //如果FCost都是一样的，那么就再筛选里目标Target更近一点的格子，hCost选出最低的
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }

}
