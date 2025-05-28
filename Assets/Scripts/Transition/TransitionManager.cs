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
            //TODO ������ʱ����Ϊ������µ�ԭ��
            fadeCanvasGroup = FindFirstObjectByType<CanvasGroup>();
            yield return StartCoroutine(LoadSceneSetActive(startSceneName));
            //��Ҫ����Ϸ��ʼʱ����Э�̵ķ�ʽ��ִ��CallAfterSceneLoadedEvent����Ϊ��CursorManager��Ҫʹ�ó������غ���ܵõ��ķ���
            EventHandler.CallAfterSceneLoadedEvent();

        }


        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            if(!isFade)
                StartCoroutine(Transition(sceneToGo, positionToGo)); //ȷ�������ڽ��г����л��Ĺ����У�Ҫ��isFadeΪfalse�Ž����л�����

        }
        /// <summary>
        /// �����л�
        /// </summary>
        /// <param name="sceneName">Ŀ�곡��</param>
        /// <param name="targetPosition">Ŀ��λ��</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1); //ж�س�����������ڣ�����Loading

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return LoadSceneSetActive(sceneName);

            //�ƶ��������� :Player��Ҫȥע������¼�
            EventHandler.CallMoveToPosition(targetPosition);

            EventHandler.CallAfterSceneLoadedEvent();

            yield return Fade(0); //ж�س���������������������Loading����


        }

        /// <summary>
        /// ���س���������Ϊ����
        /// //(дһ��Э��: �����첽���س���)
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            //LoadSceneAsync��5�����صķ���������ʹ�����Ƶķ�ʽ+���ӵĹ��ܣ�:�������ǵĳ�������һ������ȥ�ģ����������������л��ģ������������Ŀ��һ���ص�
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1); //������Ŵ�0��ʼ����ô��������-1�����ܵõ���Ŷ�Ӧ�Ѿ�����ĳ���
                                                                                   //������һ������
            SceneManager.SetActiveScene(newScene);
        }

        /// <summary>
        /// ���뵭������
        /// </summary>
        /// <param name="targetAlpha">1�Ǻڣ�0��͸�� </param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;

            fadeCanvasGroup.blocksRaycasts = true;
            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha)/Settings.fadeDuration; //�ٶ�=����/ʱ��
            //�������ʱ����������fadeCanvasGroup��������Ŀ��ֵ
            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                //�Զ��������ٶȣ�����������targetAlpha
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }
            fadeCanvasGroup.blocksRaycasts = false;
            isFade = false; 
        }

    }
}
