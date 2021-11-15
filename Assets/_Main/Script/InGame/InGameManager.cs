using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public class InGameManager : MonoBehaviourPunCallbacks //�ΰ��� �� ��� ��Ʈ���ϴ� Ŭ����
{
    [Header("UI")]
    [SerializeField] Text joinedPlayerTxt;  //���� �÷��̾� ǥ�� �ؽ�Ʈ
    [SerializeField] GameObject InGameMenu; //�ΰ��� �޴� ������Ʈ
    [SerializeField] public GameObject indicator; //placeindicator ������Ʈ
    [SerializeField] GameObject placementBtn; //��ġ ��ư

    [SerializeField] GameObject waitMenu; //�÷��̾� ��� �޴�

    [SerializeField] Text maxPlayerTxt; //�ִ� �÷��̾� �ؽ�Ʈ
    [SerializeField] Text curReadyTxt; //���� �÷��̾� �ؽ�Ʈ

    [SerializeField] GameObject TimeScreen; //�ð� ǥ�� 
   
    [SerializeField] GameObject animalCtnScreen; //���� �� ����
    [SerializeField] Text animalCtnTxt; //���� �� ���� �ؽ�Ʈ

    [SerializeField] Text timeTxt;

    [Header("ResultUI")]
    [SerializeField] GameObject wrongTxt; //���� ȭ��
    [SerializeField] GameObject winScreen; //�¸�ȭ��

    [SerializeField] Text winnerTxt; //�¸��� �ؽ�Ʈ
  

    [SerializeField] GameObject answerScreen; //����� ȭ��
    [SerializeField] Text endtimeTxt; //���� �ð� �ؽ�Ʈ

    [Header("Functional")]
    [SerializeField] GameObject spanweObject; //��ȯ�� animal spawner
    [SerializeField] float time; //�ð���
    [SerializeField] AnimalSpawner spawner; //��ȯ�� spawner



    [SerializeField]  int ready = 0; //���� �غ����� �ο���
    public bool gamestart; //���� ���� ���
    public bool gameend; //���� ���� ���

    public static InGameManager instance;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        joinedPlayerTxt.enabled = false;
    }

    public void showInGame() //�ΰ��� �޴��� Ų��
    {
        InGameMenu.SetActive(true);
    }

    [PunRPC]
    public void PlusReady() //��Ʈ��ũ�� ���� ready ���� �ø���
    {
        ready++;
    }

    [PunRPC]
    public void JoindMessage(string nick) //��Ʈ��ũ�� ���� � �÷��̾ �����ߴ��� ����
    {
        SoundManager.instance.playMenuSound();
        StartCoroutine(showJoinedMessage(nick));
 
    }

    [PunRPC]
    public void LeaveMessage(string nick) //���� ��� ���� �������� ����.
    {
        SoundManager.instance.playMenuSound();
        StartCoroutine(showLeaveMessage(nick));
  
    }

    IEnumerator showJoinedMessage(string nick) //2���ִ� ���� �޼����� ������
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


    public void ActiveIndicator() //plane�� �νĵǸ� ��ư�� Ȱ��ȭ�ȴ�.
    {
        SoundManager.instance.playMenuSound();
        InGameMenu.SetActive(true);
        MenuManager.instance.shakeMenu.SetActive(false);
        //indicator.SetActive(true);
        placementBtn.SetActive(true);
       
    }



    public void PlaceSpawner()//���� ��ȯ ���� ��ġ
    {
        SoundManager.instance.playMenuSound();
        
        TimeScreen.SetActive(true);
        animalCtnScreen.SetActive(true);

        indicator.SetActive(false);
        placementBtn.SetActive(false);
        Transform pos = indicator.transform;
        Instantiate(spanweObject, pos.position, Quaternion.identity); //spawner ����

        photonView.RPC("PlusReady", RpcTarget.All); //ready �� ����
        waitMenu.SetActive(true);


    }


    private void Update()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (PhotonNetwork.CurrentRoom.PlayerCount != 0 &&(PhotonNetwork.CurrentRoom.PlayerCount <= ready) && !gamestart) //��� plane�� ��ġ�ߴٸ� ���� ����
        {
            SoundManager.instance.playInGameMusic(); //�ΰ��� ���� ����

            spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<AnimalSpawner>();

            waitMenu.SetActive(false);
            gamestart = true;
            answerScreen.SetActive(true);
        }

        if (gamestart && !gameend) //������ ���۵����� �ð��ʸ� ����
        {
            time += Time.deltaTime;
            timeTxt.text = Mathf.Round(time).ToString();
        }

        maxPlayerTxt.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        curReadyTxt.text = ready.ToString();
    }

    public void answerWrong() //���� Ʋ���� ��� Ʋ�� �޼����� �˾���Ų��
    {
        SoundManager.instance.playWrongSound(); //ȿ���� ���
        StartCoroutine(wrongPop());
    }

    IEnumerator wrongPop()
    {
        wrongTxt.SetActive(true);
        yield return new WaitForSeconds(2f);
        wrongTxt.SetActive(false);
    }

    public void answerCorrect(string name) //������ �´ٸ� ������ �����Ų��
    {
        photonView.RPC("GameEnd",RpcTarget.All,name);
    }

    [PunRPC]
    public void GameEnd(string name) //��Ʈ��ũ�� ��� �÷��̾�� ��������� ���ڸ� �˸���
    {
        SoundManager.instance.playWinMusic(); //ȿ���� ���
        answerScreen.SetActive(false);

        gameend = true;
        winnerTxt.text = name;
        endtimeTxt.text= Mathf.Round(time).ToString();
        winScreen.SetActive(true);
        
    }

    public void Reset() //�޴��� ���ư��⸦ ������ ���� �ٽúҷ��´�
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Ingame");
    }


   public void SetAnimalCnt(int cnt) //��� �����÷��̾�� �� ���� �� ����
    {
        photonView.RPC("LocalSetAnimalCnt", RpcTarget.All, cnt);
    }

    [PunRPC]
    public void LocalSetAnimalCnt(int cnt) //�������� ��ο��� �����Ѵ�
    {
        animalCtnTxt.text = "x" + cnt;
    }




    [PunRPC]
    public void SetAnimalnum(bool fly, int idx) //������ spawn�ɶ� ��� �÷��̾�� �� ���� ��Ͻ�Ų��
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
