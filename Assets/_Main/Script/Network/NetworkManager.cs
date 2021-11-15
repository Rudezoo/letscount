using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks //��Ʈ��ũ ��Ʈ��
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

    public void Connect() //���� �õ�
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() //Master�� �����Ѵ�
    {
        Debug.Log("���� ���� �Ϸ�");
        PhotonNetwork.LocalPlayer.NickName = MenuManager.instance.input.text;

        PhotonNetwork.JoinLobby(); //�κ�����
    }

    public override void OnJoinedLobby() //�κ� �����ϸ� ���� ����ų� �����Ѵ�
    {
        PhotonNetwork.JoinOrCreateRoom("roomTemp",new RoomOptions {MaxPlayers=5 },null);
    }

    public override void OnJoinedRoom() //�濡 �����ϸ�..
    {
        Debug.Log("�� ����");
    
        photonView.RPC("JoindMessage", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);

        MenuManager.instance.showShake();
    }



    private void OnApplicationQuit() //����� ��Ʈ��ũ�� ������ ���´�.
    {
        Debug.Log("����");
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() //���� ������, leaveMessage�� ����
    {
        base.OnLeftRoom();
        Debug.Log("LeaveRoom");
        if (!photonView.IsMine)
            photonView.RPC("LeaveMessage", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
    }






}
