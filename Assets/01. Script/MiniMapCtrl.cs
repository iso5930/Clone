using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapCtrl : MonoBehaviour
{
    //public Transform m_TrTargetInfo;

    public Image m_Image;

    //이미지 추가를 해야한다...?
    private List<Image> m_pImage = new List<Image>();
    private List<Transform> m_pPlayerTransform = new List<Transform>();

    public Image m_LT, m_RT, m_LD, m_RD;

    private GameObject LT_Tower, RT_Tower, LD_Tower, RD_Tower;
    
    void Start()
    {
        //m_TrTargetInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        PhotonPlayer[] Players = PhotonNetwork.playerList;

        LT_Tower = GameObject.Find("towerLT");
        RT_Tower = GameObject.Find("towerRT");
        LD_Tower = GameObject.Find("towerLD");
        RD_Tower = GameObject.Find("towerRD");

        //m_pImage[i].transform.localPosition = new Vector3(vPos.x * 3.0f, vPos.z * 3.0f, 0.0f);

        Transform trInfo = LT_Tower.GetComponent<Transform>();

        Vector3 vPos = trInfo.position;

        m_LT.transform.localPosition = new Vector3(vPos.x * 3.0f, vPos.z * 3.0f, 0.0f);

        trInfo = RT_Tower.GetComponent<Transform>();
        vPos = trInfo.position;

        m_RT.transform.localPosition = new Vector3(vPos.x * 3.0f, vPos.z * 3.0f, 0.0f);


        trInfo = LD_Tower.GetComponent<Transform>();
        vPos = trInfo.position;

        m_LD.transform.localPosition = new Vector3(vPos.x * 3.0f, vPos.z * 3.0f, 0.0f);


        trInfo = RD_Tower.GetComponent<Transform>();
        vPos = trInfo.position;

        m_RD.transform.localPosition = new Vector3(vPos.x * 3.0f, vPos.z * 3.0f, 0.0f);

        foreach (PhotonPlayer Player in Players)
        {
            if(Player.GetTeam() == PhotonNetwork.player.GetTeam())
            {
                //팀이 같다면 

                Image NewPlayerImage = Instantiate(m_Image);

                m_pImage.Add(NewPlayerImage);

                NewPlayerImage.transform.SetParent(this.transform, false);
            }
        }        
    }

    private void LateUpdate()
    {
        //Vector3 vPos = m_TrTargetInfo.position;

        //m_Image.transform.localPosition = new Vector3(vPos.x * 5.0f, vPos.z * 5.0f, 0.0f);

        if(m_pPlayerTransform.Count == 0)
        {
            GameObject[] pPlayers = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject Player in pPlayers)
            {
                if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
                {
                    if (Player.GetComponent<PlayerCtrl>().m_PlayerTeam == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_RED)
                    {
                        m_pPlayerTransform.Add(Player.GetComponent<Transform>());
                    }
                }
                else if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
                {
                    if (Player.GetComponent<PlayerCtrl>().m_PlayerTeam == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_BLUE)
                    {
                        m_pPlayerTransform.Add(Player.GetComponent<Transform>());
                    }
                }
            }

            for (int i = 0; i < m_pPlayerTransform.Count; ++i)
            {
                Vector3 vPos = m_pPlayerTransform[i].position;

                m_pImage[i].transform.localPosition = new Vector3(vPos.x * 3.0f, vPos.z * 3.0f, 0.0f);
            }

            m_pPlayerTransform.Clear();
        }
        else
        {
            for(int i = 0; i < m_pPlayerTransform.Count; ++i)
            {
                Vector3 vPos = m_pPlayerTransform[i].position;

                m_pImage[i].transform.localPosition = new Vector3(vPos.x * 3.0f, vPos.z * 3.0f, 0.0f);
            }
        }
        //각 타워의 상태에 따라 색을 바꾸자.

        TowerCtrl Tower = LT_Tower.GetComponent<TowerCtrl>();

        if(Tower.m_eTeam == TowerCtrl.ePLAYER_TEAM.PLAYER_BLUE)
        {
            m_LT.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }
        else
        {
            m_LT.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }

        Tower = RT_Tower.GetComponent<TowerCtrl>();

        if (Tower.m_eTeam == TowerCtrl.ePLAYER_TEAM.PLAYER_BLUE)
        {
            m_RT.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }
        else
        {
            m_RT.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }

        Tower = LD_Tower.GetComponent<TowerCtrl>();

        if (Tower.m_eTeam == TowerCtrl.ePLAYER_TEAM.PLAYER_BLUE)
        {
            m_LD.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }
        else
        {
            m_LD.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }

        Tower = RD_Tower.GetComponent<TowerCtrl>();

        if (Tower.m_eTeam == TowerCtrl.ePLAYER_TEAM.PLAYER_BLUE)
        {
            m_RD.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }
        else
        {
            m_RD.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }

    }
}