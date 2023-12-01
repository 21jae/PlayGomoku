using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public InputField usernameInput;
    public Text buttonText;

    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1) //비어있는 이름 X
        {
            //사용자가 입력한 이름 포톤 이름으로 설정
            PhotonNetwork.NickName = usernameInput.text;    
            
            //버튼 클릭시 정보호출
            buttonText.text = "Connecting...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
