using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserData : MonoBehaviour
{
    [HideInInspector]
    public string m_UserName = "";

    [HideInInspector]
    public string m_UserState = "";

    [HideInInspector]
    public string m_UserTeam = "";

    public Text m_TextUserName;
    public Text m_TextUserState;
    public Text m_TextUserTeam;

	public void SyncUserData()
    {
        m_TextUserName.text = m_UserName;
        m_TextUserState.text = m_UserState;
        m_TextUserTeam.text = m_UserTeam;
    }
}
