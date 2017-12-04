using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCtrl : MonoBehaviour
{
    Transform m_trInfo;

    [HideInInspector]
    public float m_fTime = 0.0f;

    bool m_bCheck = false;

    public enum ePLAYER_TEAM
    {
        PLAYER_RED,
        PLAYER_BLUE,
        PLAYER_END
    }

    ePLAYER_TEAM m_eCheckTeam = ePLAYER_TEAM.PLAYER_END;

    public ePLAYER_TEAM m_eTeam = ePLAYER_TEAM.PLAYER_BLUE;

	void Start ()
    {
        m_trInfo = GetComponent<Transform>();
	}

    private void Update()
    {
        if(m_bCheck == true)
        {
            Vector3 vPos = m_trInfo.position;

            if(m_eCheckTeam == ePLAYER_TEAM.PLAYER_RED)
            {
                //올라가라

                if (vPos.y <= 10.0f)
                {
                    vPos.y += 5.0f * Time.deltaTime;
                    m_trInfo.position = vPos;
                }
                else
                {
                    m_eCheckTeam = ePLAYER_TEAM.PLAYER_END;
                    m_bCheck = false;

                    m_eTeam = ePLAYER_TEAM.PLAYER_RED;

                    GetComponent<CapsuleCollider>().isTrigger = true;
                }
            }
            else if(m_eCheckTeam == ePLAYER_TEAM.PLAYER_BLUE)
            {
                //내려 가라

                if (vPos.y >= -2.59f)
                {
                    vPos.y -= 5.0f * Time.deltaTime;
                    m_trInfo.position = vPos;
                }
                else
                {
                    m_eCheckTeam = ePLAYER_TEAM.PLAYER_END;
                    m_bCheck = false;

                    m_eTeam = ePLAYER_TEAM.PLAYER_BLUE;

                    GetComponent<CapsuleCollider>().isTrigger = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(m_eCheckTeam == ePLAYER_TEAM.PLAYER_END)
            {
                Debug.Log("벽과 충돌 시작");

                m_fTime = 0.0f;

                PlayerCtrl.PLAYER_TEAM Team = other.gameObject.GetComponent<PlayerCtrl>().m_PlayerTeam;

                if (Team == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_RED && m_eTeam != ePLAYER_TEAM.PLAYER_RED)
                {
                    m_eCheckTeam = ePLAYER_TEAM.PLAYER_RED;

                    GameObject CastBar = GameObject.FindGameObjectWithTag("HUD");

                    CastBar.SetActive(true);

                    CastBar.GetComponent<CastBarCtrl>().m_Tower = this;
                }
                else if(Team == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_BLUE && m_eTeam != ePLAYER_TEAM.PLAYER_BLUE)
                {
                    m_eCheckTeam = ePLAYER_TEAM.PLAYER_BLUE;

                    GameObject CastBar = GameObject.FindGameObjectWithTag("HUD");

                    CastBar.SetActive(true);

                    CastBar.GetComponent<CastBarCtrl>().m_Tower = this;
                }
                
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerCtrl.PLAYER_TEAM Team = other.gameObject.GetComponent<PlayerCtrl>().m_PlayerTeam;

            if((Team == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_RED && m_eCheckTeam == ePLAYER_TEAM.PLAYER_RED) || (Team == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_BLUE && m_eCheckTeam == ePLAYER_TEAM.PLAYER_BLUE))
            {
                m_fTime += Time.deltaTime;

                if (m_fTime >= 3.0f)
                {
                    GetComponent<CapsuleCollider>().isTrigger = false;
                }

                Debug.Log("벽과 충돌 중!!");
            }
        }   
    }   

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerCtrl.PLAYER_TEAM Team = other.gameObject.GetComponent<PlayerCtrl>().m_PlayerTeam;

            if ((Team == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_RED && m_eCheckTeam == ePLAYER_TEAM.PLAYER_RED) || (Team == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_BLUE && m_eCheckTeam == ePLAYER_TEAM.PLAYER_BLUE))
            {
                if (m_fTime >= 3.0f)
                {
                    m_bCheck = true;

                    if (m_eCheckTeam == ePLAYER_TEAM.PLAYER_RED)
                    {
                        Debug.Log("벽아 올라가라");

                        GetComponentInChildren<Light>().intensity = 0.0f;
                    }
                    else if(m_eCheckTeam == ePLAYER_TEAM.PLAYER_BLUE)
                    {
                        Debug.Log("벽아 내려가라!");

                        GetComponentInChildren<Light>().intensity = 5.0f;
                    }

                    GameObject CastBar = GameObject.FindGameObjectWithTag("HUD");

                    CastBar.GetComponent<CastBarCtrl>().m_Tower = null;

                    //CastBar.SetActive(false);
                }
                else
                {
                    m_fTime = 0.0f;

                    GameObject CastBar = GameObject.FindGameObjectWithTag("HUD");

                    CastBar.GetComponent<CastBarCtrl>().m_Tower = null;

                    //CastBar.SetActive(false);

                    m_eCheckTeam = ePLAYER_TEAM.PLAYER_END;
                }
            }

            Debug.Log("벽과 충돌 끝");
        }
    }
}
