using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AnimalSpawner : MonoBehaviour //동물 소환 컨트롤
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
        if (PhotonNetwork.IsMasterClient) //동물 소환은 master client만 담당
        {
            if (InGameManager.instance.gamestart == true && !spawned)
            {
                Debug.Log("Spawn");

                for(int i = 0; i < maxspawn; i++)
                {
      

                    int ran_speices = Random.Range(0, 2);
                    if (ran_speices == 0) //fly animal선택
                    {
                        Vector3 spawnCircle = Random.onUnitSphere; //구의형태로 랜덤한 위치를 얻는다 이때 반지름은 1
                        spawnCircle.y = Mathf.Abs(spawnCircle.y); //랜덤한 위치의 y값을 전부 양수로 변환
                        object[] data = new object[2]; //네트워크로 전송할 데이터 저장
                        data[0] = spawnDistance;
                        data[1] = spawnCircle;

                        int idx = Random.Range(0, flyanimals.Length); //fly animal 중에서 랜덤으로 선택

                        PhotonNetwork.Instantiate("Animal/fly/"+flyanimals[idx], Vector3.zero, Quaternion.identity, 0, data);
                        answerSheet.instance.SetanswerList(flyanimals[idx],Type.fly,idx);
                        InGameManager.instance.GetComponent<PhotonView>().RPC("SetAnimalnum", RpcTarget.All, true, idx);
                       

                    }
                    else //ground animal 선택
                    {
                        Vector3 spawnCircle = Random.onUnitSphere; //구의형태로 랜덤한 위치를 얻는다 이때 반지름은 1
                        spawnCircle.y = 0; //랜덤한 위치의 y값을 전부 0으로 변환
                        object[] data = new object[2]; //네트워크로 전송할 데이터 저장
                        data[0] = spawnDistance;
                        data[1] = spawnCircle;


                        int idx = Random.Range(0, groundanimals.Length); //ground animal 중에서 랜덤으로 선택

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
