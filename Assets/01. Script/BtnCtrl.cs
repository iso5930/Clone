using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnCtrl : MonoBehaviour
{
    public PlayerCtrl m_pPlayerCtrl = null;

	public void OnRunRtnDown()
    {
        if (m_pPlayerCtrl.m_bRunButton)
            m_pPlayerCtrl.m_bRunButton = false;
        else
            m_pPlayerCtrl.m_bRunButton = true;
    }

    public void OnAttackBtnDown()
    {
        m_pPlayerCtrl.OnAttackBtn();
    }

    public void OnExitBtn()
    {
        Application.Quit();
    }

    public void OnCancelBtn()
    {
        Application.UnloadLevel("scUI");
    }
}
