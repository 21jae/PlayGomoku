using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameManager gm;

    public InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    private void Start()
    {
        PhotonNetwork.JoinLobby();  //�κ� ����
    }

    /// <summary>
    /// ������ ���� Ŭ�������� ȣ��
    /// �ʵ忡 �Էµ� �ؽ�Ʈ�� 1�� �̻��϶��� ���ο� ���� �����Ѵ�.
    /// </summary>
    public void OnClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            //�� �̸� ����
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 3 });
        }
    }

    /// <summary>
    /// ����ڰ� �濡 ���������� ȣ��ȴ�.
    /// �κ��г��� ��Ȱ��ȭ, Room �г� Ȱ��ȭ �� �� �̸��� UI�� ǥ��
    /// </summary>
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        gm.player = PhotonNetwork.IsMasterClient ? 1 : 2;
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    /// <summary>
    /// �� ����� ������Ʈ �� ������ ȣ��Ǹ�, UpdateRoomList()�� ���� UI ������ ���� ��ħ�Ѵ�.
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //���� �����ǰų� �ı��ɴ븶�� ȣ��
        UpdateRoomList(roomList);
    }

    /// <summary>
    /// ���޵� 'roomList'�� �̿��� ���� �� ��� ������Ʈ�Ѵ�.
    /// ���� ����Ʈ�� �ִ� ��� �׸� ���� �� ���ο� �� ä���
    /// </summary>
    /// <param name="list"></param>
    private void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }

        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            Text textComponent = newRoom.GetComponentInChildren<Text>();
            newRoom.roomName = textComponent;
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    /// <summary>
    /// �־��� �� �̸����� �濡 �����ϴ� ���
    /// </summary>
    /// <param name="roomName"></param>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// ���� ������ ȣ��
    /// </summary>
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// ���� �������� �г� ��Ȱ��ȭ Ȱ��ȭ
    /// </summary>
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    /// <summary>
    /// �����Ͱ� ������ ��������� ȣ��
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdatePlayerList()
    {
        //���� �׸� �����ֱ�
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        //�׸񺰷� ����

        //�� �ִ��� Ȯ��
        if (PhotonNetwork.CurrentRoom == null)
            return;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItemsList.Add(newPlayerItem);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }


    #region �� ���� �����
    public override void OnCreatedRoom()
    {
        Debug.Log("���� ���������� �����Ǿ����ϴ�.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("�� ���� ����!:" + message);
    }
    #endregion
}
