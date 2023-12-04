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

    private void SendChatMessage(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            photonView.RPC("ReceiveMessage", RpcTarget.All, PhotonNetwork.NickName, message);
            chatInputField.text = string.Empty;
        }
    }

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
