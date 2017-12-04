using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCtrl : MonoBehaviour
{
    private Vector2 m_vDir = Vector2.zero;
    private Transform m_TrInfo;
    public float m_fMoveSpeed = 10.0f;
    public float m_fRunSpeed = 5.0f;
    public float m_fRotSpeed = 100.0f;
    private Animator m_Animator;

    private SphereCollider m_PunchCollider;

    private PhotonView m_PhotonView = null;

    private Vector3 m_CurrPos = Vector3.zero;
    private Quaternion m_CurrRot = Quaternion.identity;

    private bool m_bDie = false;

    public bool m_bRunButton = false;

    public enum PLAYER_STATE
    {
        PLAYER_IDLE,
        PLAYER_WALK,
        PLAYER_RUN,
        PLAYER_ATTACK,
        PLAYER_DIE,
        PLAYER_END
    };

    public enum PLAYER_TEAM
    {
        PLAYER_TEAM_RED,
        PLAYER_TEAM_BLUE,
        PLAYER_TEAM_END
    }

    public PLAYER_STATE m_PlayerState;

    private PLAYER_STATE m_CurrState;

    public PLAYER_TEAM m_PlayerTeam;

    private PLAYER_TEAM m_NetworkPlayerTeam;

    private bool m_bFindBtn = false;

    private Controller m_pInput = null;

    private bool m_bFindInput = false;

    [HideInInspector]
    public float m_fTime = 0.0f;

    private NavMeshAgent m_NaviMesh;

    public AudioClip PunchSound;
    public AudioClip PootSound;

    private AudioSource source = null;

    // Use this for initialization
    void Awake()
    {
        m_TrInfo = GetComponent<Transform>();
        m_Animator = GetComponent<Animator>();
        m_PunchCollider = GetComponentInChildren<SphereCollider>();

        m_Animator.speed = 2.0f;

        m_PhotonView = GetComponent<PhotonView>();
        m_PhotonView.synchronization = ViewSynchronization.UnreliableOnChange;
        m_PhotonView.ObservedComponents[0] = this;
        m_NaviMesh = GetComponent<NavMeshAgent>();

        source = GetComponent<AudioSource>();

        if (m_PhotonView.isMine)
        {
            Camera.main.GetComponent<CameraCtrl>().m_TrTargetInfo = m_TrInfo;
            m_PunchCollider.tag = "PLAYER_ATTACK";

            if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
                m_PlayerTeam = PLAYER_TEAM.PLAYER_TEAM_RED;
            else
                m_PlayerTeam = PLAYER_TEAM.PLAYER_TEAM_BLUE;

            //GameObject.FindGameObjectWithTag("RUN_BTN").GetComponent<RunBtnCtrl>().m_PlayerCtrl = this;
        }
        else
        {
            m_PunchCollider.tag = "NET_PLAYER_ATTACK";
        }

        m_Animator.SetBool("IsAttack", false);

        m_PunchCollider.isTrigger = false;

        m_CurrPos = m_TrInfo.position;
        m_CurrRot = m_TrInfo.rotation;
        m_NetworkPlayerTeam = PLAYER_TEAM.PLAYER_TEAM_END;

        m_PlayerState = PLAYER_STATE.PLAYER_IDLE;

        transform.FindChild("Canvas").gameObject.SetActive(false);  //같은 팀의 위에 뜨도록 만들자..
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(m_TrInfo.position);
            stream.SendNext(m_TrInfo.rotation);
            stream.SendNext(m_PlayerState);
            stream.SendNext(m_PlayerTeam);
            //stream.SendNext(m_fTime);
        }
        else
        {
            m_CurrPos = (Vector3)stream.ReceiveNext();
            m_CurrRot = (Quaternion)stream.ReceiveNext();
            m_CurrState = (PLAYER_STATE)stream.ReceiveNext();
            m_NetworkPlayerTeam = (PLAYER_TEAM)stream.ReceiveNext();
            //m_fTime = (float)stream.ReceiveNext();
            //m_PlayerTeam = m_NetworkPlayerTeam;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bDie)
        {
            return;
        }

        if (m_PhotonView.isMine)
        {
            if (m_bFindBtn == false)
            {
                GameObject pGameObject = GameObject.FindGameObjectWithTag("BTN_MGR");

                if (pGameObject != null)
                {
                    pGameObject.GetComponent<BtnCtrl>().m_pPlayerCtrl = this;
                    m_bFindBtn = true;

                    Debug.Log("BtnMgr_Link");
                }
            }

            if (m_bFindInput == false)
            {
                GameObject pGameobject = GameObject.FindGameObjectWithTag("JOY_STICK");

                if (pGameobject != null)
                {
                    m_pInput = pGameobject.GetComponent<Controller>();

                    Debug.Log("인풋 연결!");

                    m_bFindInput = true;
                }
            }

            if (m_pInput != null)
            {
                m_vDir = m_pInput.GetDir();
            }

            /*
            if (Application.platform != RuntimePlatform.Android)
            {
                m_vDir.x = Input.GetAxis("Horizontal");
                m_vDir.y = Input.GetAxis("Vertical");
            }       
            */                        
            
            if (m_vDir.y != 0.0f)
            {
                PlayerMove();
            }
            else if (m_PlayerState != PLAYER_STATE.PLAYER_ATTACK)
            {
                m_PlayerState = PLAYER_STATE.PLAYER_IDLE;

                m_NaviMesh.Stop();
            }

            /*
            if (m_vDir.x != 0.0f)
            {
                m_TrInfo.Rotate(Vector3.up * Time.deltaTime * m_fRotSpeed * m_vDir.x);
            }
            */
        }
        else
        {
            Vector3 vPos = m_TrInfo.position;

            m_TrInfo.position = Vector3.Lerp(m_TrInfo.position, m_CurrPos, Time.deltaTime * 3.0f);
            m_TrInfo.rotation = Quaternion.Slerp(m_TrInfo.rotation, m_CurrRot, Time.deltaTime * 3.0f);

            //대략 0.2초마다 다음위치를 받는다..

            //어짜피 네비메쉬를 탄 위치를 받기 때문에 isMine인 것만 처리를 해준다면 여기는 딱히 처리를 하지 않아도 괜찮다.

            m_PlayerState = m_CurrState;

            m_PlayerTeam = m_NetworkPlayerTeam;

            if (m_PlayerTeam == PLAYER_TEAM.PLAYER_TEAM_RED && PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
            {
                transform.FindChild("Canvas").gameObject.SetActive(true);
            }
            else if (m_PlayerTeam == PLAYER_TEAM.PLAYER_TEAM_BLUE && PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
            {
                transform.FindChild("Canvas").gameObject.SetActive(true);
            }
        }

        switch (m_PlayerState)
        {
            case PLAYER_STATE.PLAYER_IDLE:

                m_Animator.SetBool("IsWalk", false);
                m_Animator.SetBool("IsRun", false);
                m_Animator.SetBool("IsAttack", false);
                m_PunchCollider.isTrigger = false;
                m_NaviMesh.Stop();

                break;

            case PLAYER_STATE.PLAYER_WALK:

                m_Animator.SetBool("IsWalk", true);
                m_Animator.SetBool("IsRun", false);
                m_Animator.SetBool("IsAttack", false);
                m_PunchCollider.isTrigger = false;
                m_NaviMesh.Resume();

                break;

            case PLAYER_STATE.PLAYER_RUN:

                m_Animator.SetBool("IsWalk", false);
                m_Animator.SetBool("IsRun", true);
                m_Animator.SetBool("IsAttack", false);
                m_PunchCollider.isTrigger = false;
                m_NaviMesh.Resume();

                break;

            case PLAYER_STATE.PLAYER_ATTACK:

                m_Animator.SetBool("IsWalk", false);
                m_Animator.SetBool("IsRun", false);
                m_Animator.SetBool("IsAttack", true);
                m_PunchCollider.isTrigger = true;
                m_NaviMesh.Stop();

                break;
        }

        if (m_Animator.GetBool("IsAttack"))
        {
            float m_fTime = m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 2.0f;

            //Debug.Log(m_fTime.ToString());

            if (m_fTime >= 0.8f)
            {
                //Debug.Log("초기화");

                m_PlayerState = PLAYER_STATE.PLAYER_IDLE;

                m_Animator.SetBool("IsWalk", false);
                m_Animator.SetBool("IsRun", false);
                m_Animator.SetBool("IsAttack", false);
                m_PunchCollider.isTrigger = false;

                m_NaviMesh.Stop();
            }
        }
    }

    [PunRPC]
    void PlayerAttack()
    {
        m_PlayerState = PLAYER_STATE.PLAYER_ATTACK;

        source.PlayOneShot(PunchSound, 0.9f);
    }

    [PunRPC]
    void PlayerDie()
    {
        m_Animator.SetBool("IsDie", true);
        m_bDie = true;

        m_PlayerState = PLAYER_STATE.PLAYER_DIE;

        GetComponent<CapsuleCollider>().isTrigger = true;
    }

    [PunRPC]
    void PlayerRespawn()
    {
        m_Animator.SetBool("IsDie", false);
        m_bDie = false;

        m_PlayerState = PLAYER_STATE.PLAYER_IDLE;

        GetComponent<CapsuleCollider>().isTrigger = false;
    }

    public void OnAttackBtn()
    {
        if(m_PlayerState != PLAYER_STATE.PLAYER_ATTACK)
        {
            Debug.Log("공격버튼 클릭");

            PlayerAttack();

            m_PhotonView.RPC("PlayerAttack", PhotonTargets.Others, null);
        }        
    }

    public void Revival()
    {
        PlayerRespawn();

        m_PhotonView.RPC("PlayerRespawn", PhotonTargets.Others, null);
    }

    void PlayerMove()
    {
        if (m_PlayerState == PLAYER_STATE.PLAYER_ATTACK)
        {
            return;
        }

        Vector3 vDir = (Vector3.forward * m_vDir.y);

        Debug.Log("Move");

        //source.PlayOneShot(PootSound, 0.9f);

        if (m_bRunButton == false) //나중에 버튼눌린걸로 구분.
        {
            //m_TrInfo.Translate(vDir * Time.deltaTime * m_fMoveSpeed, Space.Self);

            //기존의 네비메쉬를 타지 않는 코드

            //vDir = m_TrInfo.rotation * vDir;

            //Vector3 vDestPos = (vDir * m_fMoveSpeed) + m_TrInfo.position;

            Vector3 vNewDir = new Vector3(m_vDir.x, 0.0f, m_vDir.y);

            vNewDir = Camera.main.GetComponent<CameraCtrl>().m_TrInfo.rotation * vNewDir;

            Vector3 vDestPos = (vNewDir * m_fMoveSpeed) + m_TrInfo.position;

            m_NaviMesh.destination = vDestPos;
            m_NaviMesh.Resume();
            m_NaviMesh.speed = m_fMoveSpeed;
            //네비메쉬를 타도록 수정한 코드
            
            m_PlayerState = PLAYER_STATE.PLAYER_WALK;

            //Camera.main.GetComponent<CameraCtrl>().m_TrInfo.rotation = m_TrInfo;

            //카메라의 y회전 축 만큼 더 회전.
        }
        else
        {
            //m_TrInfo.Translate(vDir * Time.deltaTime * m_fRunSpeed, Space.Self);
            //기존의 네비메쉬를 타지 않는 코드

            //vDir = m_TrInfo.rotation * vDir;

            //Vector3 vDestPos = (vDir * m_fRunSpeed) + m_TrInfo.position;

            //m_NaviMesh.destination = vDestPos;
            //m_NaviMesh.Resume();
            //m_NaviMesh.speed = m_fRunSpeed;
            ////네비메쉬를 타도록 수정한 코드

            Vector3 vNewDir = new Vector3(m_vDir.x, 0.0f, m_vDir.y);

            vNewDir = Camera.main.GetComponent<CameraCtrl>().m_TrInfo.rotation * vNewDir;

            Vector3 vDestPos = (vNewDir * m_fRunSpeed) + m_TrInfo.position;

            m_NaviMesh.destination = vDestPos;
            m_NaviMesh.Resume();
            m_NaviMesh.speed = m_fMoveSpeed;
            //네비메쉬를 타도록 수정한 코드

            m_PlayerState = PLAYER_STATE.PLAYER_RUN;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_PhotonView.isMine)
        {
            PlayerCtrl NetworkPlayerCtrl = other.gameObject.GetComponentInParent<PlayerCtrl>();

            if (NetworkPlayerCtrl == null)
                return;

            if (other.gameObject.tag == "NET_PLAYER_ATTACK")
            {
                if (m_PlayerTeam != NetworkPlayerCtrl.m_NetworkPlayerTeam)
                {
                    PlayerDie();
                    m_PhotonView.RPC("PlayerDie", PhotonTargets.Others, null);
                }
            }
            else
            {
                if (m_PlayerTeam == NetworkPlayerCtrl.m_NetworkPlayerTeam)
                {
                    if (NetworkPlayerCtrl.m_bDie == true)
                    {
                        m_fTime = 0.0f;
                        GameObject CastBar = GameObject.FindGameObjectWithTag("HUD");
                        CastBar.SetActive(true);
                        CastBar.GetComponent<CastBarCtrl>().m_Player = this;
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerCtrl NetworkPlayerCtrl = other.gameObject.GetComponentInParent<PlayerCtrl>();

        if (NetworkPlayerCtrl == null || NetworkPlayerCtrl == this)
            return;

        if (NetworkPlayerCtrl.m_PlayerTeam == m_PlayerTeam)
        {
            if(m_PhotonView.isMine)
            {
                m_fTime += Time.deltaTime;

                if(m_fTime >= 3.0f)
                {
                    NetworkPlayerCtrl.GetComponent<CapsuleCollider>().isTrigger = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerCtrl NetworkPlayerCtrl = other.gameObject.GetComponentInParent<PlayerCtrl>();

        if (NetworkPlayerCtrl == null || NetworkPlayerCtrl == this)
            return;

        if (NetworkPlayerCtrl.m_PlayerTeam == m_PlayerTeam)
        {
            if (m_fTime >= 3.0f)
            {
                if (m_PhotonView.isMine)
                {
                    NetworkPlayerCtrl.Revival();
                }
            }

            m_fTime = 0.0f;

            GameObject CastBar = GameObject.FindGameObjectWithTag("HUD");
            CastBar.GetComponent<CastBarCtrl>().m_Player = null;
        }
    }
}
