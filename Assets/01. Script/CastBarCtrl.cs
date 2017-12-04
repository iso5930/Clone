using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastBarCtrl : MonoBehaviour
{
    [HideInInspector]
    public TowerCtrl m_Tower = null;

    [HideInInspector]
    public PlayerCtrl m_Player = null;

    public Image m_CastBar;

    private void Update()
    {
        Debug.Log("캐스팅 바 업데이트");

        if(m_Tower != null)
        {
            float fPersent = m_Tower.m_fTime / 3.0f * 100.0f;

            float fPersent2 = 582.0f * fPersent / 100.0f;

            m_CastBar.rectTransform.sizeDelta = new Vector2(fPersent2, 47.5f);

            transform.FindChild("CastBackGround").gameObject.SetActive(true);
            transform.FindChild("Casting").gameObject.SetActive(true);
        }
        else if(m_Player != null)
        {
            float fPersent = m_Player.m_fTime / 3.0f * 100.0f;

            float fPersent2 = 582.0f * fPersent / 100.0f;

            m_CastBar.rectTransform.sizeDelta = new Vector2(fPersent2, 47.5f);

            transform.FindChild("CastBackGround").gameObject.SetActive(true);
            transform.FindChild("Casting").gameObject.SetActive(true);
        }
        else
        {
            m_CastBar.rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);

            transform.FindChild("CastBackGround").gameObject.SetActive(false);
            transform.FindChild("Casting").gameObject.SetActive(false);
        }
    }
}
