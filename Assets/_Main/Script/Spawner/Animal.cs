using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum Type { fly, ground }
public class Animal : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable //���� ��ü ��Ʈ��
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

            if (stop) //����ٸ� �ؿ� �ڵ带 ����
            {
                return;
            }


            if (!move) //�����δٸ�
            {
                if (PhotonNetwork.IsMasterClient) //�������϶��� ���� ��ġ ����
                {
                    Vector3 randomCircle = Random.onUnitSphere; //�������·� ������ ��ġ�� ��´� �̶� �������� 1    
                    if (type == Type.ground)
                    {
                        randomCircle.y = 0;
                    }
                    else
                    {
                        if ((transform.position + (randomCircle * moveDistance)).y < 0) //�̵����� plane���� �Ʒ��� ����
                        {
                            randomCircle.y = 0;
                        }
                    }
             
                    destPos = transform.position + (randomCircle * moveDistance); //������ġ�� �������� �̵��� moveDistance��ŭ�� ������ ��ġ�� ��´�.
                    if(!(destPos.x>maxRange || destPos.z > maxRange || destPos.y>maxRange)) //Ư�� ������ �Ѿ��� ��� �缳��
                    {
                        colliderspawn = true;
                        move = true;
                    }
                 
                }


            }
            else //������
            {

                if (!isfish) //fish �������� �ƴҶ�
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
                else //fish �������϶� Lerp�� �̵�
                {
                    float ranspeed = moveSpeed * Random.Range(0.1f, 1f);
                    transform.position = Vector3.Lerp(transform.position, destPos, ranspeed* Time.deltaTime);
                    Vector3 dir = destPos - transform.position;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
                }
               

            }

            if (colliderspawn) //�������� ������ ���� spawn�ǰ� �װ��� ������� ������ ��ġ�� ������´�
            {
                Collider[] randcol = Physics.OverlapSphere(destPos, 0.2f);
                foreach (Collider col in randcol)
                {
                    if (col == selfcol) //���� ��ü�� ������ ��ó�� ������ ��� �����ð� ����ٰ� �ٽ� �̵�
                    {
                        //Debug.Log("����");

                        colliderspawn = false;
                        move = false;
                        StartCoroutine(Stay());

                    }
                }
            }


        }
        else //��ġ ����ȭ
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, currLocalPos, Time.smoothDeltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, currRot, Time.smoothDeltaTime * 10);
        }

    }

    IEnumerator Stay() //����
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

    public void OnPhotonInstantiate(PhotonMessageInfo info) //������ ���� ���������� �̵�
    {
        object[] data = info.photonView.InstantiationData;
        Debug.Log(data[0]);
        Vector3 spawnCircle = (Vector3)data[1];
        Vector3 spawnPos = animalSpawner.transform.position + (spawnCircle * (float)data[0]); //center��ġ�� �������� spawnDistance��ŭ�� ������ ��ġ�� ��´�.

        Vector3 localspawnPos = transform.InverseTransformDirection(spawnPos);

        transform.parent = animalSpawner.transform;

        transform.position = localspawnPos;
    }


    [PunRPC]
    void setDestination(Vector3 pos) //������ ����
    {
        destPos = transform.position + (pos * moveDistance);
        colliderspawn = true;
        move = true;
    }


    #region Sync
    Vector3 currLocalPos;
    Quaternion currRot;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //localposition�� rotation�� ����ȭ��Ű������ ���
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
