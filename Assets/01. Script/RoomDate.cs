using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomDate : MonoBehaviour
{
    [HideInInspector]
    public string roomName = "";

    [HideInInspector]
    public int m_iPlayerCnt = 0;

    [HideInInspector]
    public int m_iMaxPlayerCnt = 0;

    public Text textRoomName;

    public Text textConnectInfo;

    public void DispRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text = "(" + m_iPlayerCnt.ToString() + " / " + m_iMaxPlayerCnt.ToString() + ")";
    }    
}
