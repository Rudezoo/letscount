using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] public Text StatTxt;
    [SerializeField] public InputField input;
    [SerializeField] public GameObject Debug_input;

    [Header("ShakeMenu")]
    [SerializeField] public GameObject shakeMenu;



    public static MenuManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void TryConnect() //입력된 닉네임으로 접속시작
    {
        NetworkManager.instance.Connect();
        mainMenu.SetActive(false);

        //shakeMenu.SetActive(true);

        SoundManager.instance.playMenuSound();

    }

    public void showShake()
    {
        shakeMenu.SetActive(true);

        //Debug
        //InGameManager.instance.ActiveIndicator();
        
    }


    public void DebugStart()
    {
        NetworkManager.instance.Connect();
    }

    

    public void GameEnd()
    {
        SoundManager.instance.playMenuSound();
        Application.Quit();
    }
}
