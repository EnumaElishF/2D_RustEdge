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
        private Vector2 direction;//������V2
        private Vector3 targetPos;//������V3

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
        /// ��ʼ������Ʒ����ʼ��
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dir"></param>
        public void InitBounceItem(Vector3 target,Vector2 dir)
        {
            coll.enabled = false;
            direction = dir;
            targetPos = target;
            distance = Vector3.Distance(target, transform.position);

            spriteTrans.position += Vector3.up * 1.5f; //��Ʒ�ڽ�ɫͷ��������λ�ã�Ӧ����1.5f����ɫ�ŵ���0f

        }
        /// <summary>
        /// ��Ͷ
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
                coll.enabled = true;//��ײ���
            }
        }
    }

}
