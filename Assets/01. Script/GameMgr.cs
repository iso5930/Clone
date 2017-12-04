using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Text m_txtConnect;
    public Text m_txtReady;
    public Text m_txtRoomName;

    public GameObject m_ScrollContents;
    public GameObject m_UserData;

    public GameObject m_BlueTeamList;
    public GameObject m_RedTeamList;

    private bool m_bReady = false;

    private PhotonView m_PhotonView;

    private void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true;

        m_PhotonView = GetComponent<PhotonView>();

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.player.SetScore(1);
        }
        else
            PhotonNetwork.player.SetScore(0);

        PhotonPlayer[] players = PhotonNetwork.playerList;

        int iBlueCnt = 0;
        int iRedCnt = 0;

        foreach (PhotonPlayer _player in players)
        {
            if (_player.GetTeam() == PunTeams.Team.blue)
            {
                ++iBlueCnt;
            }
            else if (_player.GetTeam() == PunTeams.Team.red)
            {
                ++iRedCnt;
            }
        }

        if(iBlueCnt > iRedCnt)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.red);
        }
        else if(iBlueCnt < iRedCnt)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
        }
        else
        {
            int iRandom = Random.RandomRange(0, 1);

            if(iRandom == 0)
                PhotonNetwork.player.SetTeam(PunTeams.Team.red);
            else
                PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
        }

        GetConnectPlayerCount();

        OnUserListUpdate();

        SyncText();
    }

    void GetConnectPlayerCount()
    {
        Room currRoom = PhotonNetwork.room;

        m_txtConnect.text = currRoom.PlayerCount.ToString() + " / " + currRoom.MaxPlayers.ToString();

        m_txtRoomName.text = currRoom.Name;
    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log(newPlayer.ToStringFull()); //새로들어온 플레이어의 이름

        GetConnectPlayerCount();

        OnUserListUpdate();

        SyncText();
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer outPlayer)
    {
        GetConnectPlayerCount();

        OnUserListUpdate();

        SyncText();
    }

    public void OnClickExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void OnLeftRoom()
    {
        Application.LoadLevel("scLobby");
    }

    public void OnClickReadyAndStart()
    {
        if(PhotonNetwork.isMasterClient == true)
        {
            PhotonPlayer[] players = PhotonNetwork.playerList;

            PhotonNetwork.player.SetScore(1);

            int iCnt = 0;

            foreach (PhotonPlayer _player in players)
            {
                if(_player.GetScore() == 0)
                {
                    ++iCnt;
                }
            }

            if(iCnt == 0)
            {
                PhotonNetwork.room.open = false;
                PhotonNetwork.room.IsVisible = false;

                //OnPhotonCustomRoomPropertiesChanged();

                FieldScene();

                m_PhotonView.RPC("FieldScene", PhotonTargets.Others, null);
            }
        }
        else
        {
            if (m_bReady)
            {
                m_bReady = false;
                PhotonNetwork.player.SetScore(0);
            }
            else
            {
                m_bReady = true;
                PhotonNetwork.player.SetScore(1);
            }

            OnUserListUpdate();

            m_PhotonView.RPC("OnUserListUpdate", PhotonTargets.Others, null);
        }
    }

    [PunRPC]
    void FieldScene()
    {
        PhotonNetwork.isMessageQueueRunning = false;

        Application.LoadLevel("scField");
        Application.LoadLevelAdditive("scPad");
    }

    public void SyncText()
    {
        if (PhotonNetwork.isMasterClient)
        {
            m_txtReady.text = "START";
        }
        else
        {
            m_txtReady.text = "READY";
        }
    }

    [PunRPC]
    public void OnUserListUpdate()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("USER_DATA"))
        {
            Destroy(obj);
        }

        PhotonPlayer[] players = PhotonNetwork.playerList;

        foreach(PhotonPlayer _player in players)
        {
            GameObject _User = (GameObject)Instantiate(m_UserData);

            UserData _UserData = _User.GetComponent<UserData>();
            _UserData.m_UserName = _player.name;

            if (_player.GetTeam() == PunTeams.Team.blue)
            {
                _UserData.m_UserTeam = "Blue Team";
                _User.transform.SetParent(m_BlueTeamList.transform, false);
            }
            else
            {
                _UserData.m_UserTeam = "Red Team";
                _User.transform.SetParent(m_RedTeamList.transform, false);
            }

            int iScore = _player.GetScore();

            if (iScore == 1)
            {
                _UserData.m_UserState = "READY";
            }
            else
                _UserData.m_UserState = "";

            _UserData.SyncUserData();
        }

        /*
        int iRowCnt = 0;

        Debug.Log("UserDataUpdate");

        RectTransform RcTransform = m_ScrollContents.GetComponent<RectTransform>();

        RcTransform.sizeDelta = Vector2.zero;
        RcTransform.pivot = new Vector2(0.0f, 1.0f);

        PhotonPlayer[] players = PhotonNetwork.playerList;

        foreach (PhotonPlayer _player in players)
        {
            GameObject _User = (GameObject)Instantiate(m_UserData);

            _User.transform.SetParent(m_ScrollContents.transform, false);

            UserData _UserData = _User.GetComponent<UserData>();
            _UserData.m_UserName = _player.name;

            _player.SetTeam(PunTeams.Team.none);
            
            //우선은 A팀으로 넣고 A팀이 꽉차있다면 B팀으로 우선적으로 넣기..

            int iScore = _player.GetScore();

            if(iScore == 1)
            {
                _UserData.m_UserState = "READY";
            }
            else
                _UserData.m_UserState = "";

            _UserData.SyncUserData();

            m_ScrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++iRowCnt;

            //RcTransform.sizeDelta += new Vector2(0.0f, 85.0f);
        }
        */
    }

    public void BlueTeamChange()
    {
        //블루팀으로.

        //우선 블루팀의 인원수 체크

        PhotonPlayer[] players = PhotonNetwork.playerList;

        int iCnt = 0;

        foreach(PhotonPlayer _player in players)
        {
            if(_player.GetTeam() == PunTeams.Team.blue)
            {
                ++iCnt;
            }
        }

        if(iCnt < 4)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.blue);

            OnUserListUpdate();

            m_PhotonView.RPC("OnUserListUpdate", PhotonTargets.Others, null);
        }

        //팀이 바뀌었으니 플레이어 리스트 다시 생성.

    }

    public void RedTeamChange()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;

        int iCnt = 0;

        foreach (PhotonPlayer _player in players)
        {
            if (_player.GetTeam() == PunTeams.Team.red)
            {
                ++iCnt;
            }
        }

        if (iCnt < 4)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.red);

            OnUserListUpdate();

            m_PhotonView.RPC("OnUserListUpdate", PhotonTargets.Others, null);
        }

        //팀이 바뀌었으니 플레이어 리스트 다시 생성.


    }
}
