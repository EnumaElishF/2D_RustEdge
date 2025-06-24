using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    //希望每一个Prefab，对应生成一个对象池，所以使用两个对象池
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    private Queue<GameObject> soundQueue = new Queue<GameObject>();
    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
        EventHandler.InitSoundEffect += InitSoundEffect;
    }
    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        EventHandler.InitSoundEffect -= InitSoundEffect;

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
            ParticalEffectType.Rock => poolEffectList[2],
            ParticalEffectType.ReapableScenery => poolEffectList[3],
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

    //private void InitSoundEffect(SoundDetails soundDetails)
    //{
    //    ObjectPool<GameObject> pool = poolEffectList[4];
    //    var obj = pool.Get();

    //    obj.GetComponent<Sound>().SetSound(soundDetails);

    //    StartCoroutine(DisableSound(pool, obj, soundDetails));
    //}
    //private IEnumerator DisableSound(ObjectPool<GameObject> pool,GameObject obj,SoundDetails soundDetails)
    //{
    //    //直到音效播放的长度时间，结束之后
    //    yield return new WaitForSeconds(soundDetails.soundClip.length);
    //    //再执行
    //    pool.Release(obj);
    //}

    private void CreateSoundPool()
    {
        var parent = new GameObject(poolPrefabs[4].name).transform;
        parent.SetParent(transform);

        for(int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);
            //压入队列中
            soundQueue.Enqueue(newObj);
        }
    }
    private GameObject GetSoundPoolObject()
    {
        if (soundQueue.Count < 2)
        {
            CreateSoundPool();
        }
        //把队列的第一个启动，拿出来
        return soundQueue.Dequeue();
    }
    private void InitSoundEffect(SoundDetails soundDetails)
    {
        var obj = GetSoundPoolObject();
        obj.GetComponent<Sound>().SetSound(soundDetails);
        obj.SetActive(true);
        // 持续等待时间为soundDetails.soundClip.length
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }
    /// <summary>
    /// 音效，对象池的返回
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="duration">持续时间</param>
    /// <returns></returns>
    private IEnumerator DisableSound(GameObject obj,float duration)
    {
        //持续等待时间
        yield return new WaitForSeconds(duration);
        //关闭对象
        obj.SetActive(false);
        //压回队列中
        soundQueue.Enqueue(obj);
    }
}
