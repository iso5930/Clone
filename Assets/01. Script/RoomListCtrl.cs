using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListCtrl : MonoBehaviour
{
    private RectTransform m_Parent;
    private GridLayoutGroup m_Grid;

	// Use this for initialization
	void Awake ()
    {
        m_Parent = gameObject.GetComponent<RectTransform>();
        m_Grid = gameObject.GetComponent<GridLayoutGroup>();

        float m_fWiddth = m_Parent.rect.bottom * 2.0f;

        Vector2 vCellSize = m_Grid.cellSize;

        m_Grid.cellSize = new Vector2(m_fWiddth, vCellSize.y);

        //일단 이거 나중에 수정하도록 하자...
    }
}
