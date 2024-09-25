using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class CCharMover : MonoBehaviour
{
    //상태 : 대기, 숙이기, 점프, 대쉬 등등
    public enum EMoveState
    {
        IDLE = 0, WALK, RUN, SPRINT, HIDE,
        JUMP_UP, FALL,
        RUNING_TURN, DODGE, BACK_DODGE,
        LIE_DOWN,
    }

    public enum EActionState
    {
        IDLE = 0, END_ACT = 5,
        CAN_COMBO, ATTACK,
        GUARD_HOLD, AIM_HOLD
    }


    public Animator m_AnimCtrl = null;
    public CAnimFunc m_AnimFunc = null;
    Coroutine coState_InputCheck = null;

    public EMoveState m_CurrMoveState = EMoveState.IDLE;
    public EActionState m_CurrActState = EActionState.IDLE;

    private void Awake()
    {
        coState_InputCheck = StartCoroutine(CoState_InputCheck());

        m_AnimFunc.m_Stop_Input = Stop_Input_Turn;
        m_AnimFunc.m_Resume_Input = Resume_Input_Turn;
        m_AnimFunc.m_EndAct = EndAct;
        m_AnimFunc.m_StartATK = StartATK;
        m_AnimFunc.m_EndATK = EndATK;
        m_AnimFunc.m_StartComboCheck = StartComboCheck;
        m_AnimFunc.m_EndComboCheck = EndComboCheck;
        m_AnimFunc.m_StartDodge = StartDodge;
        m_AnimFunc.m_EndDodge = EndDodge;
    }

    IEnumerator CoState_InputCheck()
    {
        while (true)
        {
            Look(m_Look);
            switch (m_CurrMoveState)
            {
                case EMoveState.IDLE:
                case EMoveState.WALK:
                case EMoveState.RUN:
                    Move(m_Move); //입력
                    LerpBodyLookAt(); //몸체 회전 보간
                    MoveLerp(); //입력값 보간
                    break;
            }

            //this.transform.position = m_Body.position;
            //    Vector3.Lerp(m_Body.position, this.transform.position, Time.deltaTime * 50f);
            this.transform.position =
                Vector3.Lerp(this.transform.position, m_Body.position, Time.deltaTime / 0.1f);

            yield return null;
        }
    }


    //=========================================//
    public void Stop_Input_Turn() { }

    public void Resume_Input_Turn() { }

    public void EndAct()
    { m_CurrActState = EActionState.END_ACT; }
    public void StartATK()
    {
        m_AnimCtrl.SetBool("ATK_Normal", false);
    }
    public void EndATK() { }

    public void StartComboCheck()
    {
        m_CurrActState = EActionState.CAN_COMBO;
    }
    public void EndComboCheck()
    {
        Debug.Log("END COMBO");
        ActionReset();
    }

    public void StartDodge() { }
    public void EndDodge()
    {
        m_CurrMoveState = EMoveState.IDLE;
        Debug.Log("DODGE");
    }
    //=========================================//

    [Header("======================================")]

    [SerializeField] Transform m_LockOn_Target = null;

    [Header("===========Input Result===============")]
    public float m_MoveSpeed = 1;
    public float m_RotateSpeed = 1;

    //awsd 입력 축
    [SerializeField] Vector2 m_Move = Vector2.zero;

    [SerializeField] Vector2 m_MoveLerp = Vector2.zero;
    //마우스 xy 델타
    [SerializeField] Vector2 m_Look = Vector2.zero;
    //회전량
    [SerializeField] Vector2 m_Rotation = Vector2.zero;
    //화면 전환 속도
    [SerializeField] float lerpSpeed = 1;
    //현재 공격 콤보
    [SerializeField] int m_Combo = -1;

    [Header("===========Rotation===============")]
    public Transform m_LookAt_Dir = null;
    public Transform m_Body = null;
    public Rigidbody m_Rigid = null;
    public CharacterController m_CharController = null;
    [SerializeField] Vector3 m_BodyRot_Dest = Vector3.zero;
    [SerializeField] float m_BodyRotSpeed = 5f;
    [SerializeField] bool m_StateDash = false;
    [SerializeField] bool m_CanDodge = true;

    //InputVector Press AWSD
    public void OnMove(InputAction.CallbackContext context)
    { m_Move = context.ReadValue<Vector2>(); }

    //InputVector Move MOUSE
    public void OnLook(InputAction.CallbackContext context)
    { m_Look = context.ReadValue<Vector2>(); }


    Coroutine coInputDodge = null;
    public void OnDash(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                coInputDodge = StartCoroutine(CoInputDodge());
                m_StateDash = true;
                break;
            case InputActionPhase.Canceled:
                if (m_CanDodge == true
                    && m_CurrActState <= EActionState.END_ACT
                    && m_CurrMoveState <= EMoveState.HIDE)
                {
                    if (coInputDodge != null) StopCoroutine(coInputDodge);
                    m_AnimCtrl.SetTrigger("Dodge");
                    m_CurrMoveState = EMoveState.DODGE;
                }
                m_StateDash = false;
                break;
        }
    }

    IEnumerator CoInputDodge()
    {
        m_CanDodge = true;
        yield return new WaitForSeconds(0.5f);
        m_CanDodge = false;
        coInputDodge = null;
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        //m_StateDash =  
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("ATTACK:");

            if (m_CurrActState <= EActionState.CAN_COMBO)
            {
                m_Combo++;
                m_CurrActState = EActionState.ATTACK;
                m_AnimCtrl.SetTrigger("ATK_Normal");
                m_AnimCtrl.SetBool("ATK_Normal", true);
                m_AnimCtrl.SetInteger("Combo", m_Combo);
                m_AnimCtrl.SetFloat("Dir_X", 0);
                m_AnimCtrl.SetFloat("Dir_Y", 0);
            }
        }
    }

    //이동
    void Move(Vector2 _dir)
    {
        //입력이 없으면 반환
        if (_dir.sqrMagnitude < 0.01f
            || m_CurrActState > EActionState.END_ACT)
        { return; }

        //if (m_CurrActState != EActionState.IDLE)
        //{ ActionReset(); }

        //카메라 기준 방향으로 움직임
        var scaledMoveSpeed = m_MoveSpeed * Time.deltaTime;
        var rot = Quaternion.Euler(0, m_LookAt_Dir.eulerAngles.y, 0);
        var move = rot * new Vector3(_dir.x, -1, _dir.y);
        var movement = m_StateDash ? move * scaledMoveSpeed * 2 : move * scaledMoveSpeed;
        //this.transform.position += movement;
        //m_Body.transform.position += movement;
        //m_Rigid.MovePosition(m_Body.transform.position + movement);
        m_CharController.Move(m_Body.transform.position + movement);
        if (m_LockOn_Target == null)
            m_BodyRot_Dest = move.normalized;
    }

    //화면전환 (마우스)
    void Look(Vector2 _rot)
    {
        //락온 대상이 있다면 락온 바라봄
        if (m_LockOn_Target != null)
        { LockOn(); return; }

        //입력이 없으면 반환
        if (_rot.sqrMagnitude < 0.01) return;

        //입력값에 따른 카메라 회전
        var scaledRotateSpeed = m_RotateSpeed * Time.deltaTime;
        m_Rotation.y += _rot.x * scaledRotateSpeed;
        m_Rotation.x = Mathf.Clamp(m_Rotation.x - _rot.y * scaledRotateSpeed, -89f, 89f);
        m_LookAt_Dir.transform.localEulerAngles = m_Rotation;
    }
    public void LockOn()
    {
        m_LookAt_Dir.LookAt(m_LockOn_Target);
        m_BodyRot_Dest = m_LookAt_Dir.rotation * Vector3.forward;
        //m_LookAt_Dir.rotation = Quaternion.Euler(0, m_LookAt_Dir.eulerAngles.y, 0);
    }

    //회전 속도 조절
    public void LerpBodyLookAt()
    {
        var dest = Quaternion.LookRotation(m_BodyRot_Dest);

        m_Body.rotation = Quaternion.RotateTowards(m_Body.rotation, dest,
            Time.deltaTime * m_BodyRotSpeed);
    }


    void MoveLerp()
    {
        var moveSqr = m_StateDash ? m_MoveLerp.sqrMagnitude * 2 : m_MoveLerp.sqrMagnitude;
        var move = m_StateDash ? m_MoveLerp * 2 : m_MoveLerp;

        //ANIM
        if (m_LockOn_Target == null) //타겟 없으면 앞으로 걷는 애니메이션만
        { m_AnimCtrl.SetFloat("Dir_Y", moveSqr); }
        else
        {//타겟 있으면 4방 걸음 애님
            m_AnimCtrl.SetFloat("Dir_X", move.x);
            m_AnimCtrl.SetFloat("Dir_Y", move.y);
        }


        //awsd입력값 보간
        if (m_MoveLerp != m_Move)
        {//두 벡터의 각도가 180도가 넘어가면 slerp 작동시 오류

            var nmLerp = m_MoveLerp.normalized;
            var nmmove = m_Move.normalized;

            if (m_Move == Vector2.zero)
                m_MoveLerp = Vector3.Slerp(m_MoveLerp, m_Move, Time.deltaTime * lerpSpeed);
            else if (Vector3.Dot(nmLerp, nmmove) < -0.99f)
            {
                if (m_StateDash == true && m_MoveLerp.sqrMagnitude > 0.5f)
                {
                    m_AnimCtrl.SetTrigger("Turn");
                    m_MoveLerp = nmmove;
                    return;
                }

                // 임의의 작은 노이즈 벡터를 추가
                nmLerp += new Vector2(0.01f, 0.01f);
                nmLerp.Normalize();
                m_MoveLerp = Vector3.Slerp(nmLerp, nmmove, Time.deltaTime * lerpSpeed);
            }
            else
                m_MoveLerp = Vector3.Slerp(nmLerp, nmmove, Time.deltaTime * lerpSpeed);
        }
    }

    public void ActionReset()
    {
        m_Combo = -1;
        m_AnimCtrl.SetBool("ATK_Normal", false);
        m_AnimCtrl.SetInteger("Combo", m_Combo);
        m_CurrActState = EActionState.IDLE;
    }


}
