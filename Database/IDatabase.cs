using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Database
{
    public interface IDatabase
    {
        public void CreateNewEntry<T>(string id, T data, string root, Action onComplete = null, Action<string> onFailure = null) where T : DatabaseData;
        public void AddDataToExistingEntry<T>(string id, string newDataName, T data, string dataPathName, string root, Action onComplete = null, Action<string> onFailure = null);
        public void AddDataToExistingEntry<T>(Queue<string> id, string newDataName, T data, string dataPathName, string root, Action onComplete = null, Action<string> onFailure = null);
        public void UpdateData(Queue<string> ids, string fieldToUpdate, string newFieldValue, string root = null, Action onComplete = null, Action<string> onFailure = null);
        public void UpdateData(string id, string fieldToUpdate, string newFieldValue, string root = null, Action onComplete = null, Action<string> onFailure = null);
        public void RetrieveData<T>(Queue<string> ids, string fieldToRetrieve, string root = null,
            Action<T> onComplete = null, Action<string> onFailure = null) where T : class;
        public void RetrieveData<T>(string id, string fieldToRetrieve, string root = null,
            Action<T> onComplete = null, Action<string> onFailure = null) where T : class;
        public void DeleteData(Queue<string> ids, string fieldToDelete, string root = null, Action onComplete = null, Action<string> onFailure = null);
        public void DeleteData(string id, string fieldToDelete, string root = null, Action onComplete = null, Action<string> onFailure = null);
    }
}