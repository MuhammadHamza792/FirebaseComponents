using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Authentication.Firebase;
using Firebase.Database;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.Scripts.Database.Firebase
{
    public class FirebaseDatabaseHandler : MonoBehaviour, IDatabase
    {
        [SerializeField] private List<DataPaths> _dataPaths;
        private FirebaseDatabase _databaseInstance;
        private DatabaseReference _databaseRef;

        private bool _initializeDatabase;
        
        public static FirebaseDatabaseHandler Instance { private set; get; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void InitializeDatabase()
        {
            if (!FirebaseAuthenticatorHandler.Instance.SignedIn)
            {
                _initializeDatabase = false;
                Debug.Log("Database failed to initialized!");
                return;
            }
            
            _databaseInstance = FirebaseDatabase.DefaultInstance;
            _databaseRef = _databaseInstance.RootReference;
            _initializeDatabase = true;
            Debug.Log("Database initialized!");
        }

        public async void AddDataToExistingEntry<T>(Queue<string> ids, string newDataName, T data, string dataPathName, string root, Action onComplete = null, Action<string> onFailure = null)
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }

            if (!SearchForField(ids, dataPathName, root, out var path))
            {
                onFailure?.Invoke("Path Not Found");
                return;
            }

            if (path == null)
            {
                onFailure?.Invoke("Path is Empty or not Found");
                return;
            }
            
            var value = data.ToString();
            var taskIsFaulted = false;
            var faultedMsg = "";

            await path.Child(newDataName).SetValueAsync(value).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                return;
            }
            
            onComplete?.Invoke();
        }

        public async void AddDataToExistingEntry<T>(string id, string newDataName, T data, string dataPathName, string root, Action onComplete = null, Action<string> onFailure = null)
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }

            if (!SearchForField(id, dataPathName, root, out var path))
            {
                onFailure?.Invoke("Path Not Found");
                return;
            }

            if (path == null)
            {
                onFailure?.Invoke("Path is Empty or not Found");
                return;
            }
            
            var value = data.ToString();
            var taskIsFaulted = false;
            var faultedMsg = string.Empty;

            await path.Child(newDataName).SetValueAsync(value).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                return;
            }
            
            onComplete?.Invoke();
        }
        
        public async void CreateNewEntry<T>(string id, T data, string root, Action onComplete = null, Action<string> onFailure = null) where T : DatabaseData
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }
            
            var json = JsonConvert.SerializeObject(data);
            var taskIsFaulted = false;
            var faultedMsg = string.Empty;
            
            await _databaseRef.Child(root).Child(id).SetRawJsonValueAsync(json).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                return;
            }
            
            onComplete?.Invoke();
        }

        public async void UpdateData(Queue<string> ids, string fieldToUpdate, string newFieldValue,
            string root = null, Action onComplete = null, Action<string> onFailure = null)
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }

            if (!SearchForField(ids, fieldToUpdate, root, out var valueToSet))
            {
                onFailure?.Invoke("Path Not Found");
                return;
            }
            
            if (valueToSet == null)
            {
                onFailure?.Invoke("Path is Empty or not Found");
                return;
            }
            
            var taskIsFaulted = false;
            var faultedMsg = string.Empty;

            await valueToSet.SetValueAsync(newFieldValue).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                return;
            }
            
            onComplete?.Invoke();
        }

        public async void UpdateData(string id, string fieldToUpdate, string newFieldValue, string root = null, Action onComplete = null,
            Action<string> onFailure = null)
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }

            if (!SearchForField(id, fieldToUpdate, root, out var valueToSet))
            {
                onFailure?.Invoke("Path Not Found");
                return;
            }

            if (valueToSet == null)
            {
                onFailure?.Invoke("Path is empty or not found.");
                return;
            }
            
            var taskIsFaulted = false;
            var faultedMsg = string.Empty;

            await valueToSet.SetValueAsync(newFieldValue).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                return;
            }
            
            onComplete?.Invoke();
        }

        public async void RetrieveData<T>(Queue<string> ids, string fieldToRetrieve, string root = null,
            Action<T> onComplete = null,
            Action<string> onFailure = null) where T : class
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }

            if (!SearchForField(ids, fieldToRetrieve, root, out var valueToSet))
            {
                onFailure?.Invoke("Path Not Found");
                return;
            }

            if (valueToSet == null)
            {
                onFailure?.Invoke("Path is empty or not Found");
                return;
            }
            
            T result = null;
            var taskIsFaulted = false;
            var faultedMsg = string.Empty;

            await valueToSet.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }

                var taskResult = task.Result.Value;

                if (taskResult == null)
                {
                    faultedMsg = "Value is null.";
                    taskIsFaulted = true;
                    return;
                }
                
                result = task.Result as T;
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                Debug.Log(faultedMsg);
                return;
            }

            onComplete?.Invoke(result);
        }

        public async void RetrieveData<T>(string id, string fieldToRetrieve, string root = null, Action<T> onComplete = null,
            Action<string> onFailure = null) where T : class
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }

            if (!SearchForField(id, fieldToRetrieve, root, out var valueToSet))
            {
                onFailure?.Invoke("Path Not Found");
                return;
            }

            if (valueToSet == null)
            {
                onFailure?.Invoke("Path is empty or not Found");
                return;
            }
            
            T result = null;
            var taskIsFaulted = false;
            var faultedMsg = string.Empty;

            await valueToSet.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }


                var taskResult = task.Result.Value;

                if (taskResult == null)
                {
                    faultedMsg = "Value is null.";
                    taskIsFaulted = true;
                    return;
                }
                
                result = task.Result as T;
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                Debug.Log(faultedMsg);
                return;
            }

            onComplete?.Invoke(result);
        }

        public async void DeleteData(Queue<string> ids, string fieldToDelete, string root = null, Action onComplete = null, Action<string> onFailure = null)
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }

            if (!SearchForField(ids, fieldToDelete, root, out var valueToSet))
            {
                onFailure?.Invoke("Path Not Found");
                return;
            }

            if (valueToSet == null)
            {
                onFailure?.Invoke("Path is empty or not found.");
                return;
            }

            var taskIsFaulted = false;
            var faultedMsg = string.Empty;
            
            await valueToSet.RemoveValueAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                return;
            }
            
            onComplete?.Invoke();
        }

        public async void DeleteData(string id, string fieldToDelete, string root = null, Action onComplete = null, Action<string> onFailure = null)
        {
            if (!CheckWhetherDatabaseIsAvailable())
            {
                onFailure?.Invoke("Database not initialized or user not authenticated.");
                return;
            }

            if (!SearchForField(id, fieldToDelete, root, out var valueToSet))
            {
                onFailure?.Invoke("Path Not Found");
                return;
            }

            if (valueToSet == null)
            {
                onFailure?.Invoke("Path is empty or not Found");
                return;
            }
            
            var taskIsFaulted = false;
            var faultedMsg = string.Empty;

            await valueToSet.RemoveValueAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    faultedMsg = "Task was canceled.";
                    Debug.LogError("Task was canceled.");
                    taskIsFaulted = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    faultedMsg = "Error encountered: " + task.Exception;
                    Debug.LogError("Error encountered: " + task.Exception);
                    taskIsFaulted = true;
                    return;
                }
            });

            if (taskIsFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                return;
            }
            
            onComplete?.Invoke();
        }
        
        private bool SearchForField(Queue<string> ids, string fieldToFind, string root, out DatabaseReference valueToSet)
        {
            var snapDataList = CheckDataPath(root, fieldToFind);
            valueToSet = null;
            
            if (snapDataList == null) return false;

            var dataPathList = snapDataList.PathList;
            foreach (var path in dataPathList)
            {
                if (valueToSet == null)
                {
                    if (path.IsId)
                    {
                        if (ids.Count >= 0)
                        {
                            valueToSet = _databaseInstance.GetReference($"{ids.Dequeue()}");
                        }
                        continue;
                    }

                    valueToSet = _databaseInstance.GetReference(snapDataList.Root);
                    continue;
                }

                if (path.IsId)
                {
                    if (ids.Count >= 0)
                    {
                        valueToSet = valueToSet.Child($"{ids.Dequeue()}");
                    }
                    continue;
                }

                valueToSet = valueToSet.Child(path.PathName);
            }

            return true;
        }

        private bool SearchForField(string id, string fieldToFind, string root, out DatabaseReference valueToSet)
        {
            var snapDataList = CheckDataPath(root, fieldToFind);

            valueToSet = null;
            if (snapDataList == null) return false;

            var dataPathList = snapDataList.PathList;
            foreach (var path in dataPathList)
            {
                if (valueToSet == null)
                {
                    if (path.IsId)
                    {
                        valueToSet = _databaseInstance.GetReference($"{id}");
                        if(valueToSet == null) return true;
                        continue;
                    }

                    valueToSet = _databaseInstance.GetReference(snapDataList.Root);
                    if(valueToSet == null) return true;
                    continue;
                }

                if (path.IsId)
                {
                    valueToSet = valueToSet.Child($"{id}");
                    if(valueToSet == null) return true;
                    continue;
                }

                valueToSet = valueToSet.Child(path.PathName);
                if(valueToSet == null) return true;
            }

            return true;
        }

        
        [CanBeNull]
        private DataPaths CheckDataPath(string root, string fieldToUpdate)
        {
            var currentDataPath = _dataPaths.FirstOrDefault(x => x.Root == root && x.PathToDataName == fieldToUpdate);
            return currentDataPath;
        }
        
        private bool CheckWhetherDatabaseIsAvailable()
        {
            if (!_initializeDatabase)
            {
                InitializeDatabase();
            }
            
            return _initializeDatabase;
        }
    }
}