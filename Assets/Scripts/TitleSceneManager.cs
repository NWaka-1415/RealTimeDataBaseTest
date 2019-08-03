using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject menu = null;
    [SerializeField] private GameObject startPanel = null;
    [SerializeField] private GameObject playerSelectPanel = null;
    [SerializeField] private TMP_InputField userNameInputField = null;

    private void Awake()
    {
        menu.transform.localScale = Vector3.zero;
        menu.SetActive(false);
        playerSelectPanel.SetActive(false);
    }

    private void Start()
    {
        startPanel.SetActive(OverAllManager.Count <= 0);
    }

    /// <summary>
    /// メニューを開く
    /// </summary>
    public void OpenMenu()
    {
        menu.SetActive(true);
        menu.transform.DOScale(1f, 0.3f);
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public void CloseMenu()
    {
        menu.transform.DOScale(0f, 0.3f).OnComplete(() => menu.SetActive(false));
    }

    /// <summary>
    /// データのリセット（デバッグ用）
    /// </summary>
    public void OnclickResetData()
    {
        OverAllManager.ResetData();
    }

    public void OnclickInputText()
    {
        OverAllManager.SetUser(userNameInputField.text);
        startPanel.transform.DOScale(0f, 0.3f);
        startPanel.SetActive(false);
    }

    public void OnclickOpenPlayerSelect()
    {
        playerSelectPanel.SetActive(true);
    }

    /// <summary>
    /// メインシーンに遷移
    /// </summary>
    public void MoveToMain()
    {
        SceneManager.LoadSceneAsync(OverAllManager.SceneName.MainScene);
    }

    /// <summary>
    /// 3人対戦セット
    /// </summary>
    public void Onclick3Players()
    {
        OverAllManager.PlayerNumber = 3;
        MoveToMain();
    }

    /// <summary>
    /// 4人対戦セット
    /// </summary>
    public void Onclick4Players()
    {
        OverAllManager.PlayerNumber = 4;
        MoveToMain();
    }

    /// <summary>
    /// 5人対戦セット
    /// </summary>
    public void Onclick5Players()
    {
        OverAllManager.PlayerNumber = 5;
        MoveToMain();
    }

    /// <summary>
    /// 6人対戦セット
    /// </summary>
    public void Onclick6Players()
    {
        OverAllManager.PlayerNumber = 6;
        MoveToMain();
    }
}