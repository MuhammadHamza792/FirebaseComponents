using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace _Project.Scripts.Authentication.Firebase
{
    public class FirebaseExceptionCodeLogger
    {
        protected bool LogTaskCompletion(Task task, string operation)
        {
            var complete = false;
            if (task.IsCanceled)
            {
                Debug.Log(operation + " canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.Log(operation + " encountered an error.");
                if (task.Exception == null) return false;
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    var authErrorCode = "";
                    var firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        authErrorCode = $"AuthError.{((AuthError)firebaseEx.ErrorCode).ToString()}: ";
                    }

                    Debug.Log("number- " + authErrorCode + "the exception is- " + exception.ToString());
                    if (firebaseEx == null) continue;
                    var code = ((AuthError)firebaseEx.ErrorCode).ToString();
                    Debug.Log(code);
                }
            }
            else if (task.IsCompleted)
            {
                Debug.Log(operation + " completed");
                complete = true;
            }
            return complete;
        }
    }
}