using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public class InGameManager : MonoBehaviourPunCallbacks //인게임 내 요소 컨트롤하는 클래스
{
    [Header("UI")]
    [SerializeField] Text joinedPlayerTxt;  //참여 플레이어 표시 텍스트
    [SerializeField] GameObject InGameMenu; //인게임 메뉴 오브젝트
    [SerializeField] public GameObject indicator; //placeindicator 오브젝트
    [SerializeField] GameObject placementBtn; //설치 버튼

    [SerializeField] GameObject waitMenu; //플레이어 대기 메뉴

    [SerializeField] Text maxPlayerTxt; //최대 플레이어 텍스트
    [SerializeField] Text curReadyTxt; //현재 플레이어 텍스트

    [SerializeField] GameObject TimeScreen; //시간 표시 
   
    [SerializeField] GameObject animalCtnScreen; //동물 총 개수
    [SerializeField] Text animalCtnTxt; //동물 총 개수 텍스트

    [SerializeField] Text timeTxt;

    [Header("ResultUI")]
    [SerializeField] GameObject wrongTxt; //실패 화면
    [SerializeField] GameObject winScreen; //승리화면

    [SerializeField] Text winnerTxt; //승리자 텍스트
  

    [SerializeField] GameObject answerScreen; //대답지 화면
    [SerializeField] Text endtimeTxt; //끝난 시간 텍스트

    [Header("Functional")]
    [SerializeField] GameObject spanweObject; //소환할 animal spawner
    [SerializeField] float time; //시간초
    [SerializeField] AnimalSpawner spawner; //소환한 spawner



    [SerializeField]  int ready = 0; //현재 준비중인 인원수
    public bool gamestart; //게임 시작 토글
    public bool gameend; //게임 종료 토글

    public static InGameManager instance;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        joinedPlayerTxt.enabled = false;
    }

    public void showInGame() //인게임 메뉴를 킨다
    {
        InGameMenu.SetActive(true);
    }

    [PunRPC]
    public void PlusReady() //네트워크를 통해 ready 수를 올린다
    {
        ready++;
    }

    [PunRPC]
    public void JoindMessage(string nick) //네트워크를 통해 어떤 플레이어가 접속했는지 띄운다
    {
        SoundManager.instance.playMenuSound();
        StartCoroutine(showJoinedMessage(nick));
 
    }

    [PunRPC]
    public void LeaveMessage(string nick) //나간 경우 누가 나갔는지 띄운다.
    {
        SoundManager.instance.playMenuSound();
        StartCoroutine(showLeaveMessage(nick));
  
    }

    IEnumerator showJoinedMessage(string nick) //2초있다 참여 메세지가 꺼진다
    {
        joinedPlayerTxt.enabled = true;
        joinedPlayerTxt.text = nick + " Joined";
        yield return new WaitForSeconds(2f);
        joinedPlayerTxt.enabled = false;
    }

    IEnumerator showLeaveMessage(string nick)
    {
        joinedPlayerTxt.enabled = true;
        joinedPlayerTxt.text = nick + " Leaved";
        yield return new WaitForSeconds(2f);
        joinedPlayerTxt.enabled = false;
    }


    public void ActiveIndicator() //plane이 인식되면 버튼이 활성화된다.
    {
        SoundManager.instance.playMenuSound();
        InGameMenu.SetActive(true);
        MenuManager.instance.shakeMenu.SetActive(false);
        //indicator.SetActive(true);
        placementBtn.SetActive(true);
       
    }



    public void PlaceSpawner()//동물 소환 지역 설치
    {
        SoundManager.instance.playMenuSound();
        
        TimeScreen.SetActive(true);
        animalCtnScreen.SetActive(true);

        indicator.SetActive(false);
        placementBtn.SetActive(false);
        Transform pos = indicator.transform;
        Instantiate(spanweObject, pos.position, Quaternion.identity); //spawner 생성

        photonView.RPC("PlusReady", RpcTarget.All); //ready 값 증가
        waitMenu.SetActive(true);


    }


    private void Update()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (PhotonNetwork.CurrentRoom.PlayerCount != 0 &&(PhotonNetwork.CurrentRoom.PlayerCount <= ready) && !gamestart) //모두 plane을 설치했다면 게임 시작
        {
            SoundManager.instance.playInGameMusic(); //인게임 뮤직 시작

            spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<AnimalSpawner>();

            waitMenu.SetActive(false);
            gamestart = true;
            answerScreen.SetActive(true);
        }

        if (gamestart && !gameend) //게임이 시작됐을때 시간초를 센다
        {
            time += Time.deltaTime;
            timeTxt.text = Mathf.Round(time).ToString();
        }

        maxPlayerTxt.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        curReadyTxt.text = ready.ToString();
    }

    public void answerWrong() //답이 틀렸을 경우 틀린 메세지를 팝업시킨다
    {
        SoundManager.instance.playWrongSound(); //효과음 재생
        StartCoroutine(wrongPop());
    }

    IEnumerator wrongPop()
    {
        wrongTxt.SetActive(true);
        yield return new WaitForSeconds(2f);
        wrongTxt.SetActive(false);
    }

    public void answerCorrect(string name) //정답이 맞다면 게임을 종료시킨다
    {
        photonView.RPC("GameEnd",RpcTarget.All,name);
    }

    [PunRPC]
    public void GameEnd(string name) //네트워크로 모든 플레이어에게 게임종료와 승자를 알린다
    {
        SoundManager.instance.playWinMusic(); //효과음 재생
        answerScreen.SetActive(false);

        gameend = true;
        winnerTxt.text = name;
        endtimeTxt.text= Mathf.Round(time).ToString();
        winScreen.SetActive(true);
        
    }

    public void Reset() //메뉴로 돌아가기를 누르면 신을 다시불러온다
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Ingame");
    }


   public void SetAnimalCnt(int cnt) //모든 로컬플레이어에게 총 동물 수 지정
    {
        photonView.RPC("LocalSetAnimalCnt", RpcTarget.All, cnt);
    }

    [PunRPC]
    public void LocalSetAnimalCnt(int cnt) //동물수를 모두에게 전달한다
    {
        animalCtnTxt.text = "x" + cnt;
    }




    [PunRPC]
    public void SetAnimalnum(bool fly, int idx) //동물이 spawn될때 모든 플레이어에게 그 수를 등록시킨다
    {
        if (fly)
        {
            spawner.flynum[idx] += 1;
        }
        else
        {
            spawner.groundnum[idx] += 1;
        }
    }
}
