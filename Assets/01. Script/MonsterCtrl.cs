using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    private Vector2 m_vDir = Vector2.zero;
    private Transform m_TrInfo;
    public float m_fMoveSpeed = 2.0f;
    private Animator m_Animator;

    private bool m_bDie = false;
    private NavMeshAgent m_NaviMesh;
    private float m_fTime = 0.0f;
    private PhotonView m_PhotonView;

    private Vector3 m_CurrPos = Vector3.zero;
    private Quaternion m_CurrRot = Quaternion.identity;

    public enum MONSTER_STATE
    {
        MONSTER_IDLE,
        MONSTER_WALK,
        MONSTER_DIE,
        MONSTER_END
    };

    public MONSTER_STATE m_MonsterState = MONSTER_STATE.MONSTER_IDLE;

    // Use this for initialization
    void Start ()
    {
        m_TrInfo = GetComponent<Transform>();
        m_Animator = GetComponent<Animator>();

        m_Animator.speed = 2.0f;

        m_NaviMesh = GetComponent<NavMeshAgent>();
        m_NaviMesh.speed = m_fMoveSpeed;
        m_PhotonView = GetComponent<PhotonView>();
        m_PhotonView.synchronization = ViewSynchronization.UnreliableOnChange;
        m_PhotonView.ObservedComponents[0] = this;

        m_fTime = 10.0f;

        m_CurrPos = m_TrInfo.position;
        m_CurrRot = m_TrInfo.rotation;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(m_TrInfo.position);
            stream.SendNext(m_TrInfo.rotation);
        }
        else
        {
            m_CurrPos = (Vector3)stream.ReceiveNext();
            m_CurrRot = (Quaternion)stream.ReceiveNext();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(m_bDie)
            return;

        if(m_PhotonView.isMine)
        {
            m_fTime += Time.deltaTime;

            if (m_fTime >= 10.0f)
            {
                m_fTime = Random.Range(-1.0f, 3.0f);

                //MonsterState();

                if(m_MonsterState == MONSTER_STATE.MONSTER_IDLE)
                {
                    Vector3 vDestPos = Vector3.zero;

                    vDestPos.x = Random.Range(-50.0f, 50.0f);
                    vDestPos.z = Random.Range(-50.0f, 50.0f);

                    m_NaviMesh.destination = vDestPos;
                    m_NaviMesh.Resume();

                    m_MonsterState = MONSTER_STATE.MONSTER_WALK;

                    m_Animator.SetBool("IsWalk", true);
                }
            }

            float fDist = Vector3.Distance(m_TrInfo.position, m_NaviMesh.destination);

            if (fDist <= 2.0f)
            {
                m_MonsterState = MONSTER_STATE.MONSTER_IDLE;
                m_Animator.SetBool("IsWalk", false);
                m_NaviMesh.Stop();
            }
        }
        else
        {
            Vector3 vPos = m_TrInfo.position;

            m_TrInfo.position = Vector3.Lerp(m_TrInfo.position, m_CurrPos, Time.deltaTime * 3.0f);
            m_TrInfo.rotation = Quaternion.Slerp(m_TrInfo.rotation, m_CurrRot, Time.deltaTime * 3.0f);

            if (vPos == m_TrInfo.position)
            {
                m_MonsterState = MONSTER_STATE.MONSTER_IDLE;
                m_Animator.SetBool("IsWalk", false);
            }
            else
            {
                m_MonsterState = MONSTER_STATE.MONSTER_WALK;
                m_Animator.SetBool("IsWalk", true);
            }
        }
	}

    void MonsterState()
    {
        switch(m_MonsterState)
        {
            case MONSTER_STATE.MONSTER_IDLE:

                Vector3 vDestPos = Vector3.zero;

                vDestPos.x = Random.Range(-50.0f, 50.0f);
                vDestPos.z = Random.Range(-50.0f, 50.0f);

                m_NaviMesh.destination = vDestPos;
                m_NaviMesh.Resume();

                m_MonsterState = MONSTER_STATE.MONSTER_WALK;

                m_Animator.SetBool("IsWalk", true);

                break;

            case MONSTER_STATE.MONSTER_WALK:

                float fRandom = Random.Range(0.0f, 100.0f);

                fRandom = fRandom % 2;

                if(fRandom == 0)
                {
                    m_MonsterState = MONSTER_STATE.MONSTER_IDLE;
                    m_Animator.SetBool("IsWalk", false);
                    m_NaviMesh.Stop();
                }
                else
                {
                    Vector3 vPos = Vector3.zero;

                    vPos.x = Random.Range(-50.0f, 50.0f);
                    vPos.z = Random.Range(-50.0f, 50.0f);

                    m_NaviMesh.destination = vPos;
                    m_NaviMesh.Resume();
                }

                break;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(m_bDie)
        {
            return;
        }

        if(other.gameObject.tag == "PLAYER_ATTACK" || other.gameObject.tag == "NET_PLAYER_ATTACK")
        {
            MonsterDie();

            m_PhotonView.RPC("MonsterDie", PhotonTargets.Others);
        }
    }

    [PunRPC]
    void MonsterDie()
    {
        m_Animator.SetTrigger("IsDie");
        m_bDie = true;
        m_NaviMesh.Stop();
        m_MonsterState = MONSTER_STATE.MONSTER_DIE;

        GetComponent<CapsuleCollider>().enabled = false;

        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<NavMeshAgent>());
    }
}
