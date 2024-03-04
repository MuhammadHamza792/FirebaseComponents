using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.HelperClasses
{
    public static class AsyncHelper
    {
        public static async Task<bool> LoadSceneAsync(string sceneToLoad = null, Func<bool> onComplete = null)
        {
            var task = SceneManager.LoadSceneAsync(sceneToLoad);
            while (!task.isDone)
            {
                await Task.Yield();
            }
            var onTaskCompleted = onComplete?.Invoke();
            return onTaskCompleted != null && onTaskCompleted.Value;
        }
    
        public static async Task<bool> LoadAdditiveSceneAsync(string sceneToLoad = null, Func<bool> onComplete = null)
        {
            var task = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            while (!task.isDone)
            {
                await Task.Yield();
            }
            var onTaskCompleted = onComplete?.Invoke();
            return onTaskCompleted != null && onTaskCompleted.Value;
        }
    
        public static async Task<bool> UnLoadSceneAsync(string sceneToLoad = null, Func<bool> onComplete = null)
        {
            var task = SceneManager.UnloadSceneAsync(sceneToLoad);
            while (!task.isDone)
            {
                await Task.Yield();
            }
            var onTaskCompleted = onComplete?.Invoke();
            return onTaskCompleted != null && onTaskCompleted.Value;
        }
    }
}