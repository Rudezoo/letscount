using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks //네트워크 컨트롤
{

    public static NetworkManager instance;
    private void Awake()
    {
        instance = this;

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

    }
    private void Update()
    {
        MenuManager.instance.StatTxt.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void Connect() //연결 시도
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() //Master에 연결한다
    {
        Debug.Log("서버 접속 완료");
        PhotonNetwork.LocalPlayer.NickName = MenuManager.instance.input.text;

        PhotonNetwork.JoinLobby(); //로비에접속
    }

    public override void OnJoinedLobby() //로비에 접속하면 방을 만들거나 접속한다
    {
        PhotonNetwork.JoinOrCreateRoom("roomTemp",new RoomOptions {MaxPlayers=5 },null);
    }

    public override void OnJoinedRoom() //방에 접속하면..
    {
        Debug.Log("방 접속");
    
        photonView.RPC("JoindMessage", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);

        MenuManager.instance.showShake();
    }



    private void OnApplicationQuit() //종료시 네트워크와 연결을 끝는다.
    {
        Debug.Log("종료");
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() //방을 나갈때, leaveMessage를 띄운다
    {
        base.OnLeftRoom();
        Debug.Log("LeaveRoom");
        if (!photonView.IsMine)
            photonView.RPC("LeaveMessage", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
    }






}
