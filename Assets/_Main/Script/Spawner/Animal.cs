using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum Type { fly, ground }
public class Animal : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable //동물 개체 컨트롤
{
    [SerializeField] Type type;
    [SerializeField] GameObject animalSpawner;

    [SerializeField] float moveDistance = 0.5f;
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] float rotSpeed = 0.5f;
    [SerializeField] Vector3 destPos;

    [SerializeField] Animator anim;
    [SerializeField] Collider selfcol;

    [SerializeField] bool isfish;
    [SerializeField] float minwaitTime=1.0f;
    [SerializeField] float maxwaitTIme=3.0f;
    [SerializeField] float maxRange = 10f;

    bool move;
    bool stop;
    bool colliderspawn;
    private void Awake()
    {
        animalSpawner = GameObject.FindGameObjectWithTag("spawner");
        anim = GetComponent<Animator>();
        selfcol = GetComponent<Collider>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {

    

        if (photonView.IsMine)
        {

            if (stop) //멈춘다면 밑에 코드를 무시
            {
                return;
            }


            if (!move) //움직인다면
            {
                if (PhotonNetwork.IsMasterClient) //마스터일때만 랜덤 위치 설정
                {
                    Vector3 randomCircle = Random.onUnitSphere; //구의형태로 랜덤한 위치를 얻는다 이때 반지름은 1    
                    if (type == Type.ground)
                    {
                        randomCircle.y = 0;
                    }
                    else
                    {
                        if ((transform.position + (randomCircle * moveDistance)).y < 0) //이동값이 plane보다 아래로 가묜
                        {
                            randomCircle.y = 0;
                        }
                    }
             
                    destPos = transform.position + (randomCircle * moveDistance); //현재위치를 기준으로 이동할 moveDistance만큼의 랜덤한 위치를 얻는다.
                    if(!(destPos.x>maxRange || destPos.z > maxRange || destPos.y>maxRange)) //특정 범위를 넘어설경우 경로 재설정
                    {
                        colliderspawn = true;
                        move = true;
                    }
                 
                }


            }
            else //움직임
            {

                if (!isfish) //fish 움직임이 아닐때
                {
                    if (type == Type.fly)
                    {
                        anim.SetBool("doFly", true);
                    }
                    else
                    {
                        anim.SetBool("doWalk", true);
                    }

                    transform.position = Vector3.MoveTowards(transform.position, destPos, moveSpeed * Time.deltaTime);
                    Vector3 dir = destPos - transform.position;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
                }
                else //fish 움직임일때 Lerp로 이동
                {
                    float ranspeed = moveSpeed * Random.Range(0.1f, 1f);
                    transform.position = Vector3.Lerp(transform.position, destPos, ranspeed* Time.deltaTime);
                    Vector3 dir = destPos - transform.position;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
                }
               

            }

            if (colliderspawn) //목적지에 일정한 구가 spawn되고 그곳에 닿았을때 랜덤한 위치를 새로잡는다
            {
                Collider[] randcol = Physics.OverlapSphere(destPos, 0.2f);
                foreach (Collider col in randcol)
                {
                    if (col == selfcol) //동물 개체가 목적지 근처에 도착한 경우 일정시간 멈췄다가 다시 이동
                    {
                        //Debug.Log("도착");

                        colliderspawn = false;
                        move = false;
                        StartCoroutine(Stay());

                    }
                }
            }


        }
        else //위치 동기화
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, currLocalPos, Time.smoothDeltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, currRot, Time.smoothDeltaTime * 10);
        }

    }

    IEnumerator Stay() //멈춤
    {
        Debug.Log("stay");
        if (!isfish)
        {
            if (type == Type.fly)
            {
                anim.SetBool("doFly", false);
            }
            else
            {
                anim.SetBool("doWalk", false);
            }
        }

        stop = true;
        yield return new WaitForSeconds(Random.Range(minwaitTime, maxwaitTIme));

        stop = false;
        move = false;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) //생성시 로컬 포지션으로 이동
    {
        object[] data = info.photonView.InstantiationData;
        Debug.Log(data[0]);
        Vector3 spawnCircle = (Vector3)data[1];
        Vector3 spawnPos = animalSpawner.transform.position + (spawnCircle * (float)data[0]); //center위치를 기준으로 spawnDistance만큼의 랜덤한 위치를 얻는다.

        Vector3 localspawnPos = transform.InverseTransformDirection(spawnPos);

        transform.parent = animalSpawner.transform;

        transform.position = localspawnPos;
    }


    [PunRPC]
    void setDestination(Vector3 pos) //목적지 설정
    {
        destPos = transform.position + (pos * moveDistance);
        colliderspawn = true;
        move = true;
    }


    #region Sync
    Vector3 currLocalPos;
    Quaternion currRot;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //localposition과 rotation을 동기화시키기위해 사용
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.localPosition);
            stream.SendNext(transform.rotation);
        }
        else
        {
            currLocalPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }

    #endregion
}
