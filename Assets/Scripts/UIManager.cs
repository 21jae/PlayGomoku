using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameManager gm;

    private void Start()
    {
        gm = GameManager.GetInstance();

        if (gm == null)
        {
            Debug.LogError("GameManager instance isn`t found");
        }
    }

    public void OnReadyButtonClicked()
    {
        if (gm != null)
            gm.OnPlayerReady();

        else
            Debug.LogError("GM reference is null");
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
