using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr_Field : MonoBehaviour
{
    private bool m_bCheck = false;

    public float m_fTime = 0.0f;

    private Text m_Time = null;
    private Text m_BlueScore = null;
    private Text m_RedScore = null;


    void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true;

        CreatePlayer();        

        if (PhotonNetwork.isMasterClient == true)
        {
            CreateMonster();
        }
    }

    void CreatePlayer()
    {
        float fX = Random.Range(-50.0f, 50.0f);
        float fZ = Random.Range(-50.0f, 50.0f);

        PhotonNetwork.Instantiate("PlayerCharacter", new Vector3(fX, 0.0f, fZ), Quaternion.identity, 0);
    }

    void CreateMonster()
    {
        GameObject[] GameObj = GameObject.FindGameObjectsWithTag("MONSTER");

        foreach (GameObject Monster in GameObj)
        {
            float fX = Random.Range(-50.0f, 50.0f);

            float fZ = Random.Range(-50.0f, 50.0f);

            Monster.GetComponent<Transform>().position = new Vector3(fX, 0.0f, fZ);
        }

        /*
        int iRandom = Random.Range(30, 40);

        for(int i = 0; i < iRandom; ++i)
        {
            float fPos = Random.Range(-50.0f, 50.0f);

            PhotonNetwork.Instantiate("MonsterCharacter", new Vector3(fPos, 0.0f, fPos), Quaternion.identity, 0);
        }
        */
    }

    private void LateUpdate()
    {
        if(m_bCheck == false)
        {
            CheckPlayer();

            m_fTime += Time.deltaTime;

            float fTime = 600.0f - m_fTime;

            int iMin = (int)(fTime / 60.0f);

            int iSceond = (int)(fTime % 60);

            if (m_Time != null)
            {
                m_Time.text = "Time    " + iMin.ToString() + "  :  " + iSceond.ToString();
            }
        }

        if(m_Time == null)
        {
            m_Time = GameObject.Find("Time").GetComponent<Text>();
            m_BlueScore = GameObject.Find("BlueScore").GetComponent<Text>();
            m_RedScore = GameObject.Find("RedScore").GetComponent<Text>();            
        }

        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.LoadLevelAdditive("scUI");
            }
        }
        //이거 나중에 언제든지 사용할수 있도록 수정..
    }

    void CheckPlayer()
    {
        GameObject[] Towers = GameObject.FindGameObjectsWithTag("TOWER");

        int iRedTeamCnt = 0;

        foreach(GameObject Tower in Towers)
        {
            if(Tower.GetComponent<TowerCtrl>().m_eTeam == TowerCtrl.ePLAYER_TEAM.PLAYER_RED)
            {
                ++iRedTeamCnt;
            }
        }

        if(iRedTeamCnt == 4)
        {
            //레드팀의 4개탑 점거 성공..

            PhotonPlayer[] Players = PhotonNetwork.playerList;

            foreach(PhotonPlayer Player in Players)
            {
                if (Player.GetTeam() == PunTeams.Team.red)
                {
                    Player.SetScore(1);
                }
                else if(Player.GetTeam() == PunTeams.Team.blue)
                {
                    Player.SetScore(0);
                }
            }

            m_bCheck = true;

            Application.LoadLevelAdditive("scMessage");
        }
        else if(m_fTime >= 2.0f)
        {
            if (m_fTime >= 600.0f)
            {
                //제한시간 초과 블루 팀 승리
                PhotonPlayer[] PHPlayers = PhotonNetwork.playerList;

                foreach (PhotonPlayer Player in PHPlayers)
                {
                    if (Player.GetTeam() == PunTeams.Team.red)
                    {
                        Player.SetScore(0);
                    }
                    else if (Player.GetTeam() == PunTeams.Team.blue)
                    {
                        Player.SetScore(1);
                    }
                }

                m_bCheck = true;

                Application.LoadLevelAdditive("scMessage");
            }
            else
            {
                GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

                int iRedCnt = 0;
                int iDieCnt = 0;
                int iBlueCnt = 0;
                int iBlueDieCnt = 0;

                foreach (GameObject Player in Players)
                {
                    PlayerCtrl _PlayerCtrl = Player.GetComponent<PlayerCtrl>();

                    if(_PlayerCtrl.m_PlayerTeam == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_RED)
                    {
                        ++iRedCnt;

                        if(_PlayerCtrl.m_PlayerState == PlayerCtrl.PLAYER_STATE.PLAYER_DIE)
                        {
                            ++iDieCnt;
                        }
                    }
                    else if(_PlayerCtrl.m_PlayerTeam == PlayerCtrl.PLAYER_TEAM.PLAYER_TEAM_BLUE)
                    {
                        ++iBlueCnt;

                        if (_PlayerCtrl.m_PlayerState == PlayerCtrl.PLAYER_STATE.PLAYER_DIE)
                        {
                            ++iBlueDieCnt;
                        }
                    }
                }

                //여기서 살아있는 숫자를 표시해주자.

                if(m_BlueScore != null && m_RedScore != null)
                {
                    m_BlueScore.text = "Blue Die : " + iBlueDieCnt.ToString();
                    m_RedScore.text = "Red Die : " + iDieCnt.ToString();
                }

                if(iRedCnt == iDieCnt)
                {
                    //레드 팀 전원 사망 블루 팀 승리!

                    PhotonPlayer[] PHPlayers = PhotonNetwork.playerList;

                    foreach (PhotonPlayer Player in PHPlayers)
                    {
                        if (Player.GetTeam() == PunTeams.Team.red)
                        {
                            Player.SetScore(0);
                        }
                        else if (Player.GetTeam() == PunTeams.Team.blue)
                        {
                            Player.SetScore(1);
                        }
                    }

                    m_bCheck = true;

                    Application.LoadLevelAdditive("scMessage");
                }
                else if(iBlueCnt == iBlueDieCnt)
                {
                    PhotonPlayer[] PHPlayers = PhotonNetwork.playerList;

                    foreach (PhotonPlayer Player in PHPlayers)
                    {
                        if (Player.GetTeam() == PunTeams.Team.red)
                        {
                            Player.SetScore(1);
                        }
                        else if (Player.GetTeam() == PunTeams.Team.blue)
                        {
                            Player.SetScore(0);
                        }
                    }

                    m_bCheck = true;

                    Application.LoadLevelAdditive("scMessage");
                }
            }
        }
    }
}
