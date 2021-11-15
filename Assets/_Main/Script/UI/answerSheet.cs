using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class answerSheet : MonoBehaviourPunCallbacks //답안지 컨트롤
{

    [SerializeField] GameObject content;
    [SerializeField] GameObject answerlist;
    [SerializeField] AnimalSpawner animalSpawner;
   
    [SerializeField] List<answerlist> answerlists=new List<answerlist>();


    Animator anim;
    bool open;

    public static answerSheet instance;



    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
        animalSpawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<AnimalSpawner>();
    }

    private void Start()
    {
        answerlists.Clear();
    }

    public void openSheet() //답안지를 열었을때
    {
        SoundManager.instance.playMenuSound2(); //효과음 재생
        open = !open; //토글

        if (open)
        {
            anim.SetBool("doOpen",true);
        }else
        {
            anim.SetBool("doOpen", false);
        }
    }

    public void SetanswerList(string stag,Type type, int idx) //네트워크로 답안지안의 list를 채운다
    {
        photonView.RPC("addAnswerlist", RpcTarget.All, stag,type,idx);
    }

    [PunRPC]
    public void addAnswerlist(string stag, Type type, int idx) //list 추가
    {
        bool check = false;
        foreach(answerlist ans in answerlists) //같은 list가 있는지 확인
        {
            if (ans.answertag == stag) 
            {
                check = true;
            }
        }

        if (!check) // 없을때 answerlist에 데이터를 넣고 추가
        {
            GameObject temp = Instantiate(answerlist, content.transform);
            
            Sprite sprite=Resources.Load<Sprite>("Animal/icon/"+stag);
            Debug.Log(sprite);
            temp.GetComponent<answerlist>().answertag = stag;
            temp.GetComponent<answerlist>().icon.sprite = sprite;
            temp.GetComponent<answerlist>().index =idx;
            temp.GetComponent<answerlist>().type = type;
            answerlists.Add(temp.GetComponent<answerlist>());
 
        }
    }

    public void submit() //제출후 답 검사
    {
        //SoundManager.instance.playMenuSound(); //효과음 재생

        GameObject[] answers = GameObject.FindGameObjectsWithTag("answerlist"); //답안지의 있는 목록을 가져온다
        bool success=true;
        foreach(GameObject answer in answers)
        {
            answerlist temp = answer.GetComponent<answerlist>();
            if (temp.type == Type.fly) //날아 다니는 동물일때
            {
                if (animalSpawner.flynum[temp.index] != temp.value) //한 동물의 숫자를 틀렸을때
                {
                    success = false;
                }

                
            }
            else
            {
                if (animalSpawner.groundnum[temp.index] != temp.value) //한 동물의 숫자를 틀렸을때
                {
                    success = false;
                }

            }
        }

        if (success) //성공시 해당 player 게임 승리
        {
            InGameManager.instance.answerCorrect(PhotonNetwork.LocalPlayer.NickName);
        }
        else //실패시 실패 메세지 팝업
        {
            InGameManager.instance.answerWrong();
        }
    }

}
