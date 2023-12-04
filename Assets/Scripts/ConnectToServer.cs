using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text buttonText;

    private void Start()
    {
        SoundManager.Instance.PlayBackgroundLoginMusic();
    }

    /// <summary>
    /// 아이디는 2글자 이상
    /// </summary>
    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 2)
        {
            //사용자가 입력한 이름 포톤 이름으로 설정
            PhotonNetwork.NickName = usernameInput.text;    
            buttonText.text = "접속중...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SoundManager.Instance.StopMusic();
        SceneManager.LoadScene("GomokuScene");
    }
}
