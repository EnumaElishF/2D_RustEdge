using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.Inventory
{
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;
        public InventoryBag_SO boxBagData;

        public GameObject mouseIcon;

        private bool canOpen = false;
        private bool isOpen;

        private void OnEnable()
        {
            //如果boxBagData不为空，那么就是提前在场景中摆放好的箱子，这个箱子是不挂Furniture的
            if (boxBagData == null)
            {
                //是空的就复制一份出来
                boxBagData = Instantiate(boxBagTemplate);
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = true;
                mouseIcon.SetActive(true);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;
                mouseIcon.SetActive(false);
            }
        }
        private void Update()
        {
            //如果，是没有被打开的状态，且可以打开，且点击鼠标右键
            if(!isOpen && canOpen && Input.GetMouseButton(1))
            {
                //打开箱子
                EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
                isOpen = true;
            }
            //如果 不能被打开（也就是角色脱离了可打开的范围），且是开大的状态
            if(!canOpen  && isOpen)
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
            //打开中的状态，按esc关上
            if(isOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }
    }

}
