using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class answerSheet : MonoBehaviourPunCallbacks //����� ��Ʈ��
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

    public void openSheet() //������� ��������
    {
        SoundManager.instance.playMenuSound2(); //ȿ���� ���
        open = !open; //���

        if (open)
        {
            anim.SetBool("doOpen",true);
        }else
        {
            anim.SetBool("doOpen", false);
        }
    }

    public void SetanswerList(string stag,Type type, int idx) //��Ʈ��ũ�� ��������� list�� ä���
    {
        photonView.RPC("addAnswerlist", RpcTarget.All, stag,type,idx);
    }

    [PunRPC]
    public void addAnswerlist(string stag, Type type, int idx) //list �߰�
    {
        bool check = false;
        foreach(answerlist ans in answerlists) //���� list�� �ִ��� Ȯ��
        {
            if (ans.answertag == stag) 
            {
                check = true;
            }
        }

        if (!check) // ������ answerlist�� �����͸� �ְ� �߰�
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

    public void submit() //������ �� �˻�
    {
        //SoundManager.instance.playMenuSound(); //ȿ���� ���

        GameObject[] answers = GameObject.FindGameObjectsWithTag("answerlist"); //������� �ִ� ����� �����´�
        bool success=true;
        foreach(GameObject answer in answers)
        {
            answerlist temp = answer.GetComponent<answerlist>();
            if (temp.type == Type.fly) //���� �ٴϴ� �����϶�
            {
                if (animalSpawner.flynum[temp.index] != temp.value) //�� ������ ���ڸ� Ʋ������
                {
                    success = false;
                }

                
            }
            else
            {
                if (animalSpawner.groundnum[temp.index] != temp.value) //�� ������ ���ڸ� Ʋ������
                {
                    success = false;
                }

            }
        }

        if (success) //������ �ش� player ���� �¸�
        {
            InGameManager.instance.answerCorrect(PhotonNetwork.LocalPlayer.NickName);
        }
        else //���н� ���� �޼��� �˾�
        {
            InGameManager.instance.answerWrong();
        }
    }

}
