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
    /// ���̵�� 2���� �̻�
    /// </summary>
    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 2)
        {
            //����ڰ� �Է��� �̸� ���� �̸����� ����
            PhotonNetwork.NickName = usernameInput.text;    
            buttonText.text = "������...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SoundManager.Instance.StopMusic();
        SceneManager.LoadScene("GomokuScene");
    }
}
