using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AnimalSpawner : MonoBehaviour //���� ��ȯ ��Ʈ��
{

    [SerializeField] string[] flyanimals;
    [SerializeField] string[] groundanimals;

    [SerializeField] public int[] flynum;
    [SerializeField] public int[] groundnum;

    [SerializeField] public int maxspawn;

    bool spawned;
    float spawnDistance = 0.5f;
    void Start()
    {
        flynum = Enumerable.Repeat<int>(0,flyanimals.Length).ToArray<int>();
        groundnum = Enumerable.Repeat<int>(0, groundanimals.Length).ToArray<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient) //���� ��ȯ�� master client�� ���
        {
            if (InGameManager.instance.gamestart == true && !spawned)
            {
                Debug.Log("Spawn");

                for(int i = 0; i < maxspawn; i++)
                {
      

                    int ran_speices = Random.Range(0, 2);
                    if (ran_speices == 0) //fly animal����
                    {
                        Vector3 spawnCircle = Random.onUnitSphere; //�������·� ������ ��ġ�� ��´� �̶� �������� 1
                        spawnCircle.y = Mathf.Abs(spawnCircle.y); //������ ��ġ�� y���� ���� ����� ��ȯ
                        object[] data = new object[2]; //��Ʈ��ũ�� ������ ������ ����
                        data[0] = spawnDistance;
                        data[1] = spawnCircle;

                        int idx = Random.Range(0, flyanimals.Length); //fly animal �߿��� �������� ����

                        PhotonNetwork.Instantiate("Animal/fly/"+flyanimals[idx], Vector3.zero, Quaternion.identity, 0, data);
                        answerSheet.instance.SetanswerList(flyanimals[idx],Type.fly,idx);
                        InGameManager.instance.GetComponent<PhotonView>().RPC("SetAnimalnum", RpcTarget.All, true, idx);
                       

                    }
                    else //ground animal ����
                    {
                        Vector3 spawnCircle = Random.onUnitSphere; //�������·� ������ ��ġ�� ��´� �̶� �������� 1
                        spawnCircle.y = 0; //������ ��ġ�� y���� ���� 0���� ��ȯ
                        object[] data = new object[2]; //��Ʈ��ũ�� ������ ������ ����
                        data[0] = spawnDistance;
                        data[1] = spawnCircle;


                        int idx = Random.Range(0, groundanimals.Length); //ground animal �߿��� �������� ����

                        PhotonNetwork.Instantiate("Animal/ground/" + groundanimals[idx], Vector3.zero, Quaternion.identity, 0, data);
                        answerSheet.instance.SetanswerList(groundanimals[idx],Type.ground,idx);
                        InGameManager.instance.GetComponent<PhotonView>().RPC("SetAnimalnum", RpcTarget.All, false, idx);
                    }

                }
                InGameManager.instance.SetAnimalCnt(maxspawn);
                spawned = true;
            }
        }
       
    }


}
