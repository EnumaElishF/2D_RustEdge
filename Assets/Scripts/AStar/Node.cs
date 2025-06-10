using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition; //��������
        public int gCost = 0; //����Start���ӵľ���
        public int hCost = 0; //����Target���ӵľ���
        public int FCost => gCost + hCost; //��ǰ���ӵ�ֵ

        //Ŀ�����ҵ�����FCost��͵�һ��·��Ȼ����������õ����������·��
        public bool isObstacle = false; //��ǰ�����Ƿ����ϰ�
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;

        }

        /// <summary>
        /// �Ƚ������ڵ��FCost�� ��IComparable<Node>ʵ�ֽӿ�
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Node other)
        {
            // ��ȷ���0��С�ڷ���-1�����ڷ���1
            int result = FCost.CompareTo(other.FCost);
            if(result == 0)
            {
                //���FCost����һ���ģ���ô����ɸѡ��Ŀ��Target����һ��ĸ��ӣ�hCostѡ����͵�
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }

}
