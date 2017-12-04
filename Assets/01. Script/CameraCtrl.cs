using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{

    public Transform m_TrTargetInfo;
    public float m_fDist = 10.0f;
    public float m_fHeight = 3.0f;
    public float m_fDampTrace = 20.0f;

    public float m_xSpeed = 220.0f;
    public float m_ySpeed = 100.0f;

    private Vector2 m_vEye = new Vector2(0.0f, 0.0f);

    public float yMinLimit = 20.0f;
    public float yMaxLimit = 45.0f;

    private Vector2 m_vDir = Vector2.zero;

    float ClampAngle(float fAngle, float fmin, float fmax)
    {
        if(fAngle < -360.0f)
        {
            fAngle += 360.0f;
        }

        if(fAngle > 360)
        {
            fAngle -= 360.0f;
        }

        return Mathf.Clamp(fAngle, fmin, fmax);
    }
    
    [HideInInspector]
    public Transform m_TrInfo;
    
	// Use this for initialization
	void Start ()
    {
        m_TrInfo = GetComponent<Transform>();

        Vector3 vAngle = m_TrInfo.eulerAngles;

        m_vEye.x = vAngle.y;
        m_vEye.y = vAngle.x;

        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetDir(Vector2 vDir)
    {
        m_vDir = vDir;
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        m_vEye.x += m_vDir.x * m_xSpeed * 0.015f;
        m_vEye.y += m_vDir.y * m_ySpeed * 0.015f;

        m_vEye.y = ClampAngle(m_vEye.y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(m_vEye.y, m_vEye.x, 0.0f);
        Vector3 vPostion = rotation * new Vector3(0.0f, 0.0f, -m_fDist) + m_TrTargetInfo.position;

        //transform.rotation = rotation;
        //transform.position = vPostion;

        m_TrInfo.rotation = rotation;
        m_TrInfo.position = vPostion;
    }
}
