using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text roomName;
    LobbyManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<LobbyManager>();

        if (roomName == null)
        {
            roomName = GetComponentInChildren<Text>();
        }
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
