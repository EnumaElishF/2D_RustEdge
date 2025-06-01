using System.Collections;
using System.Collections.Generic;
using Farm.Inventory;
using UnityEngine;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;
    // HoldItem�����SpriteRenderer��Ĭ���ǹرյ�״̬
    public SpriteRenderer holdItem;

    [Header("�����ֶ����б�")]
    public List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();
    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        foreach(var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;


    }
    /// <summary>
    /// ������������Ʒ,���ɵ���ɫͷ��
    /// </summary>
    /// <param name="ID"></param>
    private void OnHarvestAtPlayerPosition(int ID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetItemDetails(ID).itemOnWorldSprite;
        holdItem.enabled = true;
        if(holdItem.enabled == false)
        {
            StartCoroutine(ShowItem(itemSprite));
        }
    }

    private IEnumerator ShowItem(Sprite itemSprite)
    {
        holdItem.sprite = itemSprite;
        holdItem.enabled = true;
        yield return new WaitForSeconds(1f);
        holdItem.enabled = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        //�л�����ʱ��Ҫȡ����ɫ���еĶ������ý�ɫ�ָ����״̬
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnItemSelectEvent(ItemDetails itemDetails, bool isSelected)
    {
        // WORKFLOW : ��ͬ�Ĺ��߷��ز�ͬ�Ķ��������ﲹȫ
        PartType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity => PartType.Carry,
            ItemType.HoeTool => PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.CollectTool => PartType.Collect,
            _=> PartType.None
         };
        if (isSelected == false)
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if (currentType == PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
            else
            {
                //����Ƿ�Carry�����Ʒ����ִ�е��Ƕ�������������Ӧ��ȡ�����������Ʒ
                holdItem.enabled = false;
            }
        }
        SwitchAnimator(currentType);
    }

    private void SwitchAnimator(PartType partType)
    {
        foreach(var item in animatorTypes)
        {
            if(item.partType == partType)
            {
                //�л���ʱʹ�õ�AnimatorController
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
