using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonInit_Lobby : MonoBehaviour
{
    private string m_RoomName;

    public GameObject m_ScrollContents;
    public GameObject m_RoomItem;

    void Awake()
    {
        m_RoomName = "ROOM_" + Random.Range(0, 999).ToString("000");

        PhotonNetwork.isMessageQueueRunning = true;

        OnReceivedRoomListUpdate();
    }

    public void OnClickCreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.isOpen = true;
        roomOptions.isVisible = true;
        roomOptions.maxPlayers = 20;

        PhotonNetwork.CreateRoom(m_RoomName, roomOptions, TypedLobby.Default);

        Debug.Log("OnClickCreateRoom");
    }

    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
    }

    public void OnClickRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("No Rooms");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Enter Room!");

        StartCoroutine(this.LoadRoomScene());
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    IEnumerator LoadRoomScene()
    {
        PhotonNetwork.isMessageQueueRunning = false;

        AsyncOperation ao = Application.LoadLevelAsync("scRoom");

        yield return ao;
    }

    void OnReceivedRoomList()
    {
        Debug.Log("RoomList");
    }

    void OnReceivedRoomListUpdate()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

       

        Debug.Log("RoomListUpdate");

        m_ScrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
       
        m_ScrollContents.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1.0f);

        foreach (RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            Debug.Log(_room.Name);

            GameObject room = (GameObject)Instantiate(m_RoomItem);

            room.transform.SetParent(m_ScrollContents.transform, false);

            RoomDate roomDate = room.GetComponent<RoomDate>();
            roomDate.roomName = _room.Name;
            roomDate.m_iPlayerCnt = _room.PlayerCount;
            roomDate.m_iMaxPlayerCnt = _room.MaxPlayers;

            roomDate.DispRoomData();

            //m_ScrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++iRowCnt;

            m_ScrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 110);

            roomDate.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClickRoomItem(roomDate.roomName); });
        }
    }

    void OnClickRoomItem(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
