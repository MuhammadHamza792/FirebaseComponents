using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.HelperClasses
{
    public static class EnumeratorHelper
    {
        public static IEnumerator AddDelay(float maxDelay)
        {
            var delay = 0f;
            while (delay <= maxDelay)
            {
                delay += Time.deltaTime;
                yield return null;
            }
        }
    
        public static IEnumerator LoadScene(string sceneToLoad = null, Action onComplete = null)
        {
            var task = SceneManager.LoadSceneAsync(sceneToLoad);
            task.allowSceneActivation = false;
            while (task.progress < .9f)
            {
                yield return null;
            }
            onComplete?.Invoke();
            task.allowSceneActivation = true;
        }
        
        public static IEnumerator UnLoadScene(string sceneToLoad = null, Action onComplete = null)
        {
            var task = SceneManager.UnloadSceneAsync(sceneToLoad);
            while (!task.isDone)
            {
                yield return null;
            }
            onComplete?.Invoke();
        }
    }
}