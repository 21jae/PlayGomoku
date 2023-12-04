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
        PhotonNetwork.JoinLobby();  //�κ� ����
        SoundManager.Instance.PlayBackgroundLobbyMusic();
    }

    /// <summary>
    /// ������ ���� Ŭ�������� ȣ��
    /// �ʵ忡 �Էµ� �ؽ�Ʈ�� 2�� �̻��϶��� ���ο� ���� �����Ѵ�.
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
    /// ����ڰ� �濡 ���������� ȣ��ȴ�.
    /// �κ��г��� ��Ȱ��ȭ, Room �г� Ȱ��ȭ �� �� �̸��� UI�� ǥ��
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            Debug.Log("���� �÷��̾�� ������ Ŭ���̾�Ʈ�Դϴ�.");
        else
            Debug.Log("���� �÷��̾�� ������ Ŭ���̾�Ʈ�� �ƴմϴ�.");

        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        gameManager.player = PhotonNetwork.IsMasterClient ? 1 : 2;
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
        gameManager.InitializePlayerReadyStatus();
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
    /// �־��� �� �̸����� �濡 �����ϴ� ���
    /// </summary>
    /// <param name="roomName"></param>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
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
    /// ���� �濡 �ִ� �÷��̾� Ȯ��
    /// </summary>
    private void UpdatePlayerList()
    {
        foreach (PlayerInfo item in playerInfoList)
        {
            Destroy(item.gameObject);
        }
        playerInfoList.Clear();

        //�� �ִ��� Ȯ��
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
