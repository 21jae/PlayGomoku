using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviourPun
{
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private GameObject chatContent;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private ScrollRect scrollRect;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        chatInputField.onEndEdit.AddListener(SendChatMessage);
    }

    /// <summary>
    /// 메세지 보내기
    /// 빈칸은 보낼수 없음
    /// </summary>
    /// <param name="message"></param>
    private void SendChatMessage(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            photonView.RPC("ReceiveMessage", RpcTarget.All, PhotonNetwork.NickName, message);
            chatInputField.text = string.Empty;
        }
    }

    /// <summary>
    /// 채팅 생성 구현 및 스크롤 정리
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="message"></param>
    [PunRPC]
    public void ReceiveMessage(string playerName, string message)
    {
        GameObject newMessage = Instantiate(messagePrefab, chatContent.transform);
        newMessage.GetComponent<TMP_Text>().text = $"{playerName}: {message}";
        StartCoroutine(ScrollUpdate());
    }
    private IEnumerator ScrollUpdate()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
