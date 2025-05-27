using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.Inventory
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTrans;
        private BoxCollider2D coll;
        public float gravity = -3.5f;
        private bool isGound;
        private float distance;
        private Vector2 direction;//方向用V2
        private Vector3 targetPos;//坐标用V3

        private void Awake()
        {
            spriteTrans = transform.GetChild(0);
            coll = GetComponent<BoxCollider2D>();
            coll.enabled = false;

        }
        private void Update()
        {
            Bounce();
        }
        /// <summary>
        /// 初始生成物品，开始扔
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dir"></param>
        public void InitBounceItem(Vector3 target,Vector2 dir)
        {
            coll.enabled = false;
            direction = dir;
            targetPos = target;
            distance = Vector3.Distance(target, transform.position);

            spriteTrans.position += Vector3.up * 1.5f; //物品在角色头顶的坐标位置，应该是1.5f。角色脚底是0f

        }
        /// <summary>
        /// 抛投
        /// </summary>
        private void Bounce()
        {
            isGound = spriteTrans.position.y <= transform.position.y;
            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position += (Vector3)direction * distance * (-gravity) * Time.deltaTime;
            }
            if (!isGound)
            {
                spriteTrans.position += Vector3.up * gravity * Time.deltaTime;
            }
            else
            {
                spriteTrans.position = transform.position;
                coll.enabled = true;//碰撞体打开
            }
        }
    }

}
