using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonInit : MonoBehaviour
{
    public string version = "v1.0";

    public InputField m_userID;

    private void Awake()
    {
        //Screen.SetResolution(1920, 1080, false);

        PhotonNetwork.ConnectUsingSettings(version);
    }

    void OnJoinedLobby()
    {
        Debug.Log("Enter Lobby");
        m_userID.text = GetUserID();
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    string GetUserID()
    {
        string strusedID = PlayerPrefs.GetString("USER_ID");

        if(string.IsNullOrEmpty(strusedID))
        {
            strusedID = "USER_" + Random.Range(0, 999).ToString();
        }

        return strusedID;
    }

    public void OnClickJoinRandomRoom()
    {
        PhotonNetwork.player.name = m_userID.text;

        PlayerPrefs.SetString("USER_ID", m_userID.text);

        //PhotonNetwork.JoinRandomRoom();

        StartCoroutine(this.LoadLobbyScene());
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("No Room!");
    }

    IEnumerator LoadLobbyScene()
    {
        PhotonNetwork.isMessageQueueRunning = false;

        AsyncOperation ao = Application.LoadLevelAsync("scLobby");

        yield return ao;
    }
}
