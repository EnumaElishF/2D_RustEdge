using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    //希望每一个Prefab，对应生成一个对象池，所以使用两个对象池
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
    }
    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;

    }



    private void Start()
    {
        CreatePool();
    }
    /// <summary>
    /// 生成对象池
    /// </summary>
    private void CreatePool()
    {
        foreach(GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            //e是gameObject，下面按技术文档的设置进行编写
            var newPool = new ObjectPool<GameObject>(
                ()=>Instantiate(item,parent),
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
                e => { Destroy(e); }
             );

            poolEffectList.Add(newPool);
        }
    }

    private void OnParticleEffectEvent(ParticalEffectType effectType, Vector3 pos)
    {
        //WORKFLOW : 根据特效补全
        //根据类型返回对应的对象池
        ObjectPool<GameObject> objPool = effectType switch
        {
            ParticalEffectType.LeaveFalling01 => poolEffectList[0],
            ParticalEffectType.LeaveFalling02 => poolEffectList[1],
            _ => null,
        };

        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool,GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
    }
}
