using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _menu = null;
    [SerializeField] private GameObject _startPanel = null;
    [SerializeField] private TMP_InputField _userNameInputField = null;

    private void Awake()
    {
        _menu.transform.localScale = Vector3.zero;
        _menu.SetActive(false);
    }

    private void Start()
    {
        _startPanel.SetActive(OverAllManager.Count <= 0);
    }

    /// <summary>
    /// メニューを開く
    /// </summary>
    public void OpenMenu()
    {
        _menu.SetActive(true);
        _menu.transform.DOScale(1f, 0.3f);
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public void CloseMenu()
    {
        _menu.transform.DOScale(0f, 0.3f).OnComplete(() => _menu.SetActive(false));
    }

    public void OnclickInputText()
    {
        OverAllManager.SetUser(_userNameInputField.text);
        _startPanel.transform.DOScale(0f, 0.3f);
        _startPanel.SetActive(false);
    }

    /// <summary>
    /// メインシーンに遷移
    /// </summary>
    public void MoveToMain()
    {
        SceneManager.LoadSceneAsync("MainScene");
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