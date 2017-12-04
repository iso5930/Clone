using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public Image m_Stick = null;
    private Vector3 m_vPos = Vector3.zero;
    private Vector3 m_vDir = Vector3.zero;

    private float m_fRadius;

    private void Start()
    {
        m_fRadius = GetComponent<RectTransform>().sizeDelta.y / 4;
        m_vPos = m_Stick.transform.position;
    }

    public void OnDrag()
    {
        //Touch touch = Input.GetTouch(0);

        //Vector3 vPos = Input.mousePosition;

        //Vector3 vPos = touch.position;

        Vector3 vPos = Vector3.zero;
        
        if (Application.platform == RuntimePlatform.Android)
        {
            Touch touch = Input.GetTouch(0);

            vPos = touch.position;
        }
        else
        {
            vPos = Input.mousePosition;
        }

        m_vDir = (vPos - m_vPos).normalized;

        float fDistance = Vector3.Distance(vPos, m_vPos);

        if(fDistance > m_fRadius)
        {
            m_Stick.transform.position = m_vPos + m_vDir * m_fRadius;
        }
        else
        {
            m_Stick.transform.position = m_vPos + m_vDir * fDistance;
        }
        
        Debug.Log("컨트롤러 들어옴");
    }

    public void OnEndDrag()
    {
        if(m_Stick != null)
        {
            m_Stick.rectTransform.position = m_vPos;
            m_vDir = Vector3.zero;
        }
    }

    public Vector2 GetDir()
    {
        return new Vector2(m_vDir.x, m_vDir.y);
    }
}
