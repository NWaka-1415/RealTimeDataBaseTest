using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

public class OverAllManager : SingletonMonoBehaviour<OverAllManager>
{
    private DatabaseReference _database;

    public static class TagNames
    {
        public static string CardDefaultField { get; } = "CardDefaultField";
        public static string DroppableField { get; } = "DroppableField";
        public static string GameController { get; } = "GameController";
    }

    public static class PathName
    {
        public static string UserId { get; } = "UserId";
    }

    /// <summary>
    /// スカルもしくは花のカードを表すクラス
    /// </summary>
    [Serializable]
    public class Card
    {
        public enum CardTypes
        {
            Skull,
            Flower
        }

        public enum States
        {
            Open,
            Close
        }

        [SerializeField] private CardTypes _cardType;
        [SerializeField] private States _state;

        public Card(CardTypes cardType)
        {
            _cardType = cardType;
            _state = States.Close;
        }

        public Card(CardTypes cardType, States state)
        {
            _cardType = cardType;
            _state = state;
        }

        public void Open()
        {
            _state = States.Open;
        }

        public void Close()
        {
            _state = States.Close;
        }

        public CardTypes CardType => _cardType;

        public States State => _state;
    }

    [Serializable]
    public class UserData
    {
        /// <summary>
        /// ユーザーの名前
        /// </summary>
        [SerializeField] private string _username;

        /// <summary>
        /// ユーザーId
        /// </summary>
        [SerializeField] private string _userId;

        /// <summary>
        /// 所持しているカード
        /// 計4枚からスタート
        /// </summary>
        [SerializeField] private Card[] _havingCards = new Card[4];

        [SerializeField] private int _life;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="username"></param>
        /// <param name="json"></param>
        public UserData(string username, string userId)
        {
            _username = username;
            _userId = userId;
            _life = 2;
            for (int i = 0; i < _havingCards.Length - 1; i++)
            {
                _havingCards[i] = new Card(Card.CardTypes.Flower);
            }

            _havingCards[3] = new Card(Card.CardTypes.Skull);
        }

        public void SetUserName(string userName)
        {
            _username = userName;
        }

        /// <summary>
        /// ライフを1減らす
        /// </summary>
        public void DecreaseLife()
        {
            _life--;
            if (_life < 0) _life = 0;
        }

        public int Life => _life;

        /// <summary>
        /// 自分自身のJson形式を返却
        /// </summary>
        /// <returns>string Jsonデータ</returns>
        public string CreateJson()
        {
            return JsonUtility.ToJson(this);
        }

        public string Username => _username;

        public string UserId => _userId;

        public Card[] HavingCards => _havingCards;

        public static UserData CreateUserDataFromJson(string json)
        {
            return JsonUtility.FromJson<UserData>(json);
        }
    }

    /// <summary>
    /// 自分のユーザーデータ
    /// </summary>
    private static UserData _yourUserData;

    /// <summary>
    /// リアルタイム同期にて取得するユーザーデータ
    /// </summary>
    private static UserData[] _userData;

    private static int _playerNumber;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
            }
        });

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://realtimedatebasetest.firebaseio.com/");
        _database = FirebaseDatabase.DefaultInstance.GetReference("userData");
        _playerNumber = 3;

        _database.ValueChanged += HandleValueChanged;
    }

    /// <summary>
    /// データ読み込み
    /// </summary>
    /// <returns></returns>
    public UserData ReadUserData()
    {
        string jsonData = "";
        _database.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("データ取得できず");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                jsonData = dataSnapshot.GetRawJsonValue();
            }
        });
        if (jsonData == "") return null;
        return UserData.CreateUserDataFromJson(jsonData);
    }

    /// <summary>
    /// データ書き込み
    /// </summary>
    /// <param name="userData"></param>
    public void WriteUserData(UserData userData)
    {
        _database.Child(userData.Username).SetRawJsonValueAsync(userData.CreateJson());
#if UNITY_EDITOR
        Debug.Log("Complete");
#endif
    }

    /// <summary>
    /// DataBase上で変化があった際呼ばれる
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        ReadUserData();
    }

    /// <summary>
    /// load game data
    /// </summary>
    public static void Load()
    {
        _yourUserData = new UserData("", PlayerPrefs.GetString(PathName.UserId, "hogehoge1415"));
    }

    /// <summary>
    /// save game data
    /// </summary>
    public static void Save()
    {
        PlayerPrefs.SetString(PathName.UserId, _yourUserData.UserId);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// create your user data.
    /// </summary>
    public void SetUser(string username)
    {
        _yourUserData.SetUserName(username);
    }

    /// <summary>
    /// 3-6のみ
    /// </summary>
    public static int PlayerNumber
    {
        get { return _playerNumber; }
        set
        {
            if (value < 3) _playerNumber = 3;
            else if (value > 6) _playerNumber = 6;
            else _playerNumber = value;
        }
    }

    public static UserData YourUserData => _yourUserData;

    public static UserData[] UserDatas => _userData;
}