using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

public class OverAllManager : SingletonMonoBehaviour<OverAllManager>
{
    private DatabaseReference _database;

    protected override void Awake()
    {
        base.Awake();
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://realtimedatebasetest.firebaseio.com/");
        _database = FirebaseDatabase.DefaultInstance.GetReference("userData");
        
        //Read
        _database.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("ダメです");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                string jsonData = dataSnapshot.GetRawJsonValue();
                Debug.Log(jsonData);
            }
        });
        
        
        //Write
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}