using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageCtrl : MonoBehaviour
{
    public Text m_Text;

    private GameObject[] Players;

    private void Start()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");

        /*
        foreach(GameObject Player in Players)
        {
            if(Player.GetComponent<PhotonView>().isMine == true)
            {
                if(Player.GetComponent<PlayerCtrl>().m_PlayerState != PlayerCtrl.PLAYER_STATE.PLAYER_DIE)
                {
                    m_Text.text = "Your WIN!!";
                }
                else
                {
                    m_Text.text = "Your LOSE";
                }
            }
        }
        */

        if (PhotonNetwork.player.GetScore() != 0)
        {
            m_Text.text = "Your Team WIN!";
        }
        else
        {
            m_Text.text = "Your Team LOSE";
        }
    }

    public void OnClickGoToRoom()
    {
        foreach (GameObject Player in Players)
        {
            Destroy(Player);
        }

        if(PhotonNetwork.isMasterClient == true)
        {
            PhotonNetwork.room.open = true;
            PhotonNetwork.room.IsVisible = true;
        }

        Application.LoadLevel("scRoom");
    }
}