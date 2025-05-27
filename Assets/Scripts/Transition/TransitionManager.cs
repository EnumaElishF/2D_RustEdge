using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
       [SceneName]
        public string startSceneName = string.Empty;

        private CanvasGroup fadeCanvasGroup;
        private bool isFade;

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }
        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }


        private IEnumerator Start()
        {
            //TODO 方法临时，因为代码更新的原因
            fadeCanvasGroup = FindFirstObjectByType<CanvasGroup>();
            yield return StartCoroutine(LoadSceneSetActive(startSceneName));
            //需要在游戏开始时就以协程的方式，执行CallAfterSceneLoadedEvent。因为在CursorManager需要使用场景加载后才能得到的方法
            EventHandler.CallAfterSceneLoadedEvent();

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

    }
}
