using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class PlayerInfo : MonoBehaviourPun
{
    [SerializeField] private Image playerImage;
    [SerializeField] private Text playerName;
    [SerializeField] private Text winScoreText;
    [SerializeField] private Text looseScoreText;

    private int winScore;
    private int looseScore ;
    private int playerActorNumber;

    private bool hasScoreBeenUpdated;

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        playerActorNumber = _player.ActorNumber;
        Debug.Log($"PlayerInfo num : {playerActorNumber}");

        string spriteKey = _player.IsMasterClient ? "arts/Character_Slime" : "arts/Character_Yeti";
        playerImage.sprite = LoadPlayerSprite(spriteKey);
    }

    private Sprite LoadPlayerSprite(string key)
    {
        //key������ �ش��ϴ� ��������Ʈ�� �ε�
        return Resources.Load<Sprite>(key);
    }

    public void UpdateScoreRecord(int winningPlayer)
    {
        if (hasScoreBeenUpdated)
            return;

        if (playerActorNumber == winningPlayer)
        {
            winScore += 1;
            winScoreText.text = $"{winScore} ��";
        }
        else
        {
            looseScore += 1;
            looseScoreText.text = $"{looseScore} ��";
        }

        hasScoreBeenUpdated = true;
    }

    public void ResetScoreUpdateStatus()
    {
        hasScoreBeenUpdated = false;
    }
}
