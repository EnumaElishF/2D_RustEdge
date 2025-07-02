using Farm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Transition
{
    public class TransitionManager : Singleton<TransitionManager>,ISaveable
    {
       [SceneName]
        public string startSceneName = string.Empty;

        private CanvasGroup fadeCanvasGroup;
        private bool isFade;

        public string GUID => GetComponent<DataGUID>().guid;
        protected override void Awake()
        {
            base.Awake();
            // UI以Additive加到场景上，此时不是被激活的
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;

            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.EndGameEvent += OnEndGameEvent;

        }


        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;

            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent -= OnEndGameEvent;

        }



        private void Start()
        {
            //ISaveable进行注册
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            fadeCanvasGroup = FindFirstObjectByType<CanvasGroup>();
        }


        private void OnEndGameEvent()
        {
            //结束游戏，要执行：加载界面，卸载当前场景，退出加载界面
            StartCoroutine(UnloadScene());
        }
        private void OnStartNewGameEvent(int obj)
        {
            StartCoroutine(LoadSaveDataScene(startSceneName));
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            if(!isFade)
                StartCoroutine(Transition(sceneToGo, positionToGo)); //确保不是在进行场景切换的过程中，要在isFade为false才进行切换场景

        }
        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标场景</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1); //卸载场景，场景变黑，加载Loading

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return LoadSceneSetActive(sceneName);

            //移动人物坐标 :Player需要去注册这个事件
            EventHandler.CallMoveToPosition(targetPosition);

            EventHandler.CallAfterSceneLoadedEvent();

            yield return Fade(0); //卸载场景，场景变正常，加载Loading结束


        }

        /// <summary>
        /// 加载场景并设置为激活
        /// //(写一个协程: 用来异步加载场景)
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            //LoadSceneAsync有5个重载的方法，我们使用名称的方式+叠加的功能，:由于我们的场景是逐一叠加上去的，而不是整个场景切换的，这个是我们项目的一个特点
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1); //场景编号从0开始，那么场景数量-1，就能得到编号对应已经激活的场景
                                                                                   //激活这一个场景
            SceneManager.SetActiveScene(newScene);
        }

        /// <summary>
        /// 淡入淡出场景
        /// </summary>
        /// <param name="targetAlpha">1是黑，0是透明 </param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;

            fadeCanvasGroup.blocksRaycasts = true;
            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha)/Settings.fadeDuration; //速度=距离/时间
            //当不相等时，！，就让fadeCanvasGroup定量增到目标值
            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                //以定下来的速度，缓慢增量到targetAlpha
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }
            fadeCanvasGroup.blocksRaycasts = false;
            isFade = false; 
        }
        /// <summary>
        /// 加载存储游戏场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public IEnumerator LoadSaveDataScene(string sceneName)
        {
            // ！！！加载存储，游戏的存档，的时候，渐入（让我们的游戏在使用I和O变化存档，变的更真实）
            yield return Fade(1f); //1黑

            if (SceneManager.GetActiveScene().name != "PersistentScene") //在游戏过程中，加载另外游戏进度
            {
                //当前场景不是PersistentScene，那么就代表是01或者其他等场景，我们要切换场景，就先卸载他
                EventHandler.CallBeforeSceneUnloadEvent();
                //卸载当前场景
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }
            //激活新场景
            yield return LoadSceneSetActive(sceneName);

            EventHandler.CallAfterSceneLoadedEvent();
            yield return Fade(0);

        }


        private IEnumerator UnloadScene()
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            //进入加载界面，然后
            //卸载当前激活场景
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            //退出加载界面
            yield return Fade(0f);
        }


        /// <summary>
        /// 存储数据: 实现自ISaveable接口的 GenerateSaveData
        /// </summary>
        /// <returns></returns>
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.dataSceneName = SceneManager.GetActiveScene().name;

            return saveData;

        }
        /// <summary>
        /// 生成恢复数据：实现自ISaveable接口的 RestoreData
        /// </summary>
        /// <param name="saveData"></param>
        public void RestoreData(GameSaveData saveData)
        {
            //加载游戏进度场景
            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }
    }
}
