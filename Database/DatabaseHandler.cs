using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Authentication;
using _Project.Scripts.Authentication.Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Database
{
    public class DatabaseHandler : MonoBehaviour
    {
        private IDatabase _database;

        public static event Action OnUserCreated;
        public static event Action OnCreatingUser;
        public static event Action<string> OnFailedToCreateUser;
        
        public static event Action OnAddingEntryToCreatedUser;
        public static event Action OnEntryAddedToCreatedUser;
        public static event Action<string> OnFailedToAddEntryToCreatedUser;
        
        public static event Action OnUserDataUpdated;
        public static event Action OnUpdatingUserData;
        public static event Action<string> OnFailedToUpdateUserData;
        
        public static event Action OnUserDataRetrieved;
        public static event Action OnRetrievingUserData;
        public static event Action<string> OnFailedToRetrieveUserData;
        
        public static event Action OnUserDataDeleted;
        public static event Action OnDeletingUserData;
        public static event Action<string> OnFailedToDeleteUserData;
        
        public static DatabaseHandler Instance { private set; get; }

        private string _userId;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _database = GetComponent<IDatabase>();
        }
        
        private bool _creatingNewUser;
        public void CreateNewEntry<T>(string id, T data, string root = null, Action onComplete = null, Action onFailure = null) where T : DatabaseData
        {
            if(_creatingNewUser) return;
            _creatingNewUser = _database != null;
            OnCreatingUser?.Invoke();
            _database?.CreateNewEntry(id, data,root, () =>
            {
                _creatingNewUser = false;
                OnUserCreated?.Invoke();
                onComplete?.Invoke();
            }, (exc) =>
            {
                _creatingNewUser = false;
                OnFailedToCreateUser?.Invoke(exc);
                onFailure?.Invoke();
            });
        }

        private bool _addingToExistingData;
        public void AddDataToExistingEntry<T>(string id, string newDataName, T data, string dataPathName, string root, Action onComplete = null, Action onFailure = null)
        {
            if (_addingToExistingData) return;
            _addingToExistingData = _database != null;
            OnAddingEntryToCreatedUser?.Invoke();
            _database?.AddDataToExistingEntry<T>(id, newDataName, data, dataPathName, root, () =>
            {
                _addingToExistingData = false;
                OnEntryAddedToCreatedUser?.Invoke();
                onComplete?.Invoke();
            }, (exc) =>
            {
                _addingToExistingData = false;
                OnFailedToAddEntryToCreatedUser?.Invoke(exc);
                onFailure?.Invoke();
            });
        }


        private bool _updatingData;
        public void UpdateData(string id, string fieldToUpdate, string newFieldValue, string dataRoot = null, Action onComplete = null, Action onFailure = null)
        {
            if(_updatingData) return;
            _updatingData = _database != null;
            OnUpdatingUserData?.Invoke();
            _database?.UpdateData(id, fieldToUpdate, newFieldValue, dataRoot, () =>
            {
                _updatingData = false;
                OnUserDataUpdated?.Invoke();
                onComplete?.Invoke();
            }, (exc) =>
            {
                _updatingData = false;
                OnFailedToUpdateUserData?.Invoke(exc);
                onFailure?.Invoke();
            });
        }
        
        public void UpdateData(Queue<string> ids, string fieldToUpdate, string dataPathToUpdate, string dataRoot = null, Action onComplete = null, Action onFailure = null)
        {
            if(_updatingData) return;
            _updatingData = _database != null;
            OnUpdatingUserData?.Invoke();
            _database?.UpdateData(ids, fieldToUpdate, fieldToUpdate, dataRoot, () =>
            {
                _updatingData = false;
                OnUserDataUpdated?.Invoke();
                onComplete?.Invoke();
            }, (exc) =>
            {
                _updatingData = false;
                OnFailedToUpdateUserData?.Invoke(exc);
                onFailure?.Invoke();
            });
        }

        private bool _retrievingData;
        public void RetrieveData<T>(string id, string fieldToRetrieve, string root = null, Action<T> onComplete = null, Action onFailure = null) where T : class
        {
            if (_retrievingData) return;
            _retrievingData = _database != null;
            OnRetrievingUserData?.Invoke();
            _database?.RetrieveData<T>(id, fieldToRetrieve, root ,(data) =>
            {
                _retrievingData = false;
                OnUserDataRetrieved?.Invoke();
                onComplete?.Invoke(data);
            }, (exc) =>
            {
                _retrievingData = false;
                OnFailedToRetrieveUserData?.Invoke(exc);
                onFailure?.Invoke();
            });
        }
        
        public void RetrieveData<T>(Queue<string> ids, string fieldToRetrieve, string root = null, Action<T> onComplete = null, Action onFailure = null) where T : class
        {
            if (_retrievingData) return;
            _retrievingData = _database != null;
            OnRetrievingUserData?.Invoke();
            _database?.RetrieveData<T>(ids, fieldToRetrieve, root ,(data) =>
            {
                _retrievingData = false;
                OnUserDataRetrieved?.Invoke();
                onComplete?.Invoke(data);
            }, (exc) =>
            {
                _retrievingData = false;
                OnFailedToRetrieveUserData?.Invoke(exc);
                onFailure?.Invoke();
            });
        }

        private bool _deletingData;
        public void DeleteData(string id, string fieldToDelete, string root = null, Action onComplete = null, Action onFailure = null)
        {
            if(_deletingData) return;
            _deletingData = _database != null;
            OnDeletingUserData?.Invoke();
            _database?.DeleteData(id, fieldToDelete, root, () =>
            {
                _deletingData = false;
                OnUserDataDeleted?.Invoke();
                onComplete?.Invoke();
            }, (exc) =>
            {
                _deletingData = false;
                OnFailedToDeleteUserData?.Invoke(exc);
                onFailure?.Invoke();
            });
        }
        
        public void DeleteData(Queue<string> ids, string fieldToDelete, string root = null, Action onComplete = null, Action onFailure = null)
        {
            if(_deletingData) return;
            _deletingData = _database != null;
            OnDeletingUserData?.Invoke();
            _database?.DeleteData(ids, fieldToDelete, root, () =>
            {
                _deletingData = false;
                OnUserDataDeleted?.Invoke();
                onComplete?.Invoke();
            }, (exc) =>
            {
                _deletingData = false;
                OnFailedToDeleteUserData?.Invoke(exc);
                onFailure?.Invoke();
            });
        }
    }

    [Serializable]
    public class DataPaths
    {
        [FormerlySerializedAs("DataPathName")] 
        public string PathToDataName;
        public string Root;
        public List<PathList> PathList;
    }

    [Serializable]
    public class PathList
    {
        public string PathName;
        [FormerlySerializedAs("IsVariable")] 
        public bool IsId;
    }
    

    public class UserData : DatabaseData
    {
        public string Email;
        public double Progress;
        public int Level;
        public int Coins;
    }

    public class DatabaseData
    {
        public string Name;
        public DateTime CreatedOn;
    }
}
