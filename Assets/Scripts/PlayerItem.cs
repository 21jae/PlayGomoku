using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class PlayerItem : MonoBehaviour
{
    public Text playerName;
    public Image playerImage;

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;

        string spriteKey = _player.IsMasterClient ? "arts/Character_Slime" : "arts/Character_Yeti";
        playerImage.sprite = LoadPlayerSprite(spriteKey);
    }

    private Sprite LoadPlayerSprite(string key)
    {
        //key에따라 해당하는 스프라이트를 로드
        return Resources.Load<Sprite>(key);
    }
}
