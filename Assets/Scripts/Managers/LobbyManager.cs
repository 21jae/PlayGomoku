using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_InputField roomTitleInputField;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private GomokuRoom gomokuRoom;

    private List<GomokuRoom> gomokuRoomList = new List<GomokuRoom>();
    [SerializeField] private Transform gomokuRoomPosition;

    private List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private Transform playerInfoParent;

    private void Start()
    {
        PhotonNetwork.JoinLobby();  //로비에 입장
        SoundManager.Instance.PlayBackgroundLobbyMusic();
    }

    /// <summary>
    /// 생성된 방을 클릭했을때 호출
    /// 필드에 입력된 텍스트가 2자 이상일때만 새로운 방을 생성한다.
    /// </summary>
    public void OnClickCreate()
    {
        if (roomTitleInputField.text.Length >= 2)
        {
            PhotonNetwork.CreateRoom(roomTitleInputField.text, new RoomOptions() { MaxPlayers = 2 });
            SoundManager.Instance.PlayButtonAndClickSound();
        }
    }

    /// <summary>
    /// 사용자가 방에 입장했을때 호출된다.
    /// 로비패널을 비활성화, Room 패널 활성화 및 방 이름을 UI에 표시
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            Debug.Log("현재 플레이어는 마스터 클라이언트입니다.");
        else
            Debug.Log("현재 플레이어는 마스터 클라이언트가 아닙니다.");

        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        gameManager.player = PhotonNetwork.IsMasterClient ? 1 : 2;
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
        gameManager.InitializePlayerReadyStatus();
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
        foreach (GomokuRoom item in gomokuRoomList)
        {
            Destroy(item.gameObject);
        }
        gomokuRoomList.Clear();

        foreach (RoomInfo room in list)
        {
            if (!room.RemovedFromList)
            {
                GomokuRoom newRoom = Instantiate(gomokuRoom, gomokuRoomPosition);
                TMP_Text textComponent = newRoom.GetComponentInChildren<TMP_Text>();
                newRoom.roomName = textComponent;
                newRoom.SetRoomName(room.Name);
                gomokuRoomList.Add(newRoom);
            }
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
    /// 현재 방에 있는 플레이어 확인
    /// </summary>
    private void UpdatePlayerList()
    {
        foreach (PlayerInfo item in playerInfoList)
        {
            Destroy(item.gameObject);
        }
        playerInfoList.Clear();

        //방 있는지 확인
        if (PhotonNetwork.CurrentRoom == null)
            return;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerInfo newPlayerItem = Instantiate(playerInfo, playerInfoParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerInfoList.Add(newPlayerItem);
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
}
