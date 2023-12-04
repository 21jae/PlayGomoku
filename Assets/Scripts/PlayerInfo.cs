using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviourPun
{
    [SerializeField] private Image playerImage;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text winScoreText;
    [SerializeField] private TMP_Text looseScoreText;

    private int winScore                        { get; set; }
    private int looseScore                      { get; set; }
    private int playerActorNumber               { get; set; }
    private bool hasScoreBeenUpdated            { get; set; }

    public void SetPlayerInfo(Player player)
    {
        playerName.text = player.NickName;
        playerActorNumber = player.ActorNumber;
        string spriteKey = player.IsMasterClient ? "arts/Character_Slime" : "arts/Character_Yeti";
        playerImage.sprite = Resources.Load<Sprite>(spriteKey);
    }

    public void UpdateScoreRecord(int winningPlayer)
    {
        if (hasScoreBeenUpdated)
            return;

        if (playerActorNumber == winningPlayer)
        {
            winScore++;
            winScoreText.text = $"{winScore} ½Â";
        }
        else
        {
            looseScore++;
            looseScoreText.text = $"{looseScore} ÆÐ";
        }

        hasScoreBeenUpdated = true;
    }

    public void ResetScoreUpdateStatus()
    {
        hasScoreBeenUpdated = false;
    }
}
