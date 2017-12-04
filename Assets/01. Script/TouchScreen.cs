using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchScreen : MonoBehaviour
{
    private Vector3 m_vPos = Vector3.zero;
    private CameraCtrl pCameraCtrl = null;

    private Vector2 m_vDir = Vector3.zero;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(pCameraCtrl == null)
        {
            pCameraCtrl = Camera.main.GetComponent<CameraCtrl>();
        }
        else if(pCameraCtrl != null)
        {
            //여기서 카메라의 함수를 호출하는 형식으로??

            pCameraCtrl.SetDir(m_vDir);
        }
	}

    public void BeginDrag()
    {
        //Touch touch = Input.GetTouch(0);

        //m_vPos = touch.position;

        m_vPos = Input.mousePosition;

        m_vDir = Vector2.zero;
    }

    public void OnDrag()
    {
        //Touch touch = Input.GetTouch(0);

        //Vector3 vPos = touch.position;

        Vector3 vPos = Input.mousePosition;

        Vector3 vDir = (vPos - m_vPos).normalized;

        m_vDir = new Vector2(vDir.x, vDir.y);

        Debug.Log("터치 스크린 들어옴");
    }

    public void OnEndDrag()
    {
        m_vPos = Vector3.zero;

        m_vDir = Vector2.zero;
    }
}
