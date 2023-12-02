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
        PhotonNetwork.JoinLobby();  //로비에 입장
    }

    /// <summary>
    /// 생성된 방을 클릭했을때 호출
    /// 필드에 입력된 텍스트가 1자 이상일때만 새로운 방을 생성한다.
    /// </summary>
    public void OnClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            //방 이름 전달
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 3 });
        }
    }

    /// <summary>
    /// 사용자가 방에 입장했을때 호출된다.
    /// 로비패널을 비활성화, Room 패널 활성화 및 방 이름을 UI에 표시
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
    /// 방 목록이 업데이트 될 때마다 호출되며, UpdateRoomList()를 통해 UI 방목록을 새로 고침한다.
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //방이 생성되거나 파괴될대마다 호출
        UpdateRoomList(roomList);
    }

    /// <summary>
    /// 전달된 'roomList'를 이용해 현재 방 목록 업데이트한다.
    /// 기존 리스트에 있는 모든 항목 삭제 및 새로운 방 채우기
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
    /// 주어진 방 이름으로 방에 입장하는 기능
    /// </summary>
    /// <param name="roomName"></param>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// 방을 떠날때 호출
    /// </summary>
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// 방을 떠났을대 패널 비활성화 활성화
    /// </summary>
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    /// <summary>
    /// 마스터가 서버에 연결됐을때 호출
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
        //이전 항목 지워주기
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        //항목별로 생성

        //방 있는지 확인
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


    #region 방 생성 디버그
    public override void OnCreatedRoom()
    {
        Debug.Log("방이 성공적으로 생성되었습니다.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 생성 실패!:" + message);
    }
    #endregion
}
