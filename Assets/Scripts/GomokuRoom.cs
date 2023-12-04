using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GomokuRoom : MonoBehaviour
{
    public TMP_Text roomName;
    private LobbyManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<LobbyManager>();

        if (roomName == null)
            roomName = GetComponentInChildren<TMP_Text>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }
    
    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
