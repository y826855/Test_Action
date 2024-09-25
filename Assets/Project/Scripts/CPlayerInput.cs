using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class CPlayerInput : MonoBehaviour
{
    //상태 : 대기, 숙이기, 점프, 대쉬 등등
    public enum EMoveState
    {
        IDLE = 0, WALK, RUN, SPRINT,
        HIDE, JUMP_UP, FALL,
        RUNING_TURN = 20,
        DODGE, BACK_DODGE,
        LIE_DOWN,
        CANT_MOVE,
    }

    public enum EActionState
    {
        IDLE = 0,
        ATTACK,

        GUARD_HOLD, AIM_HOLD
    }

    //연속 공격 약공
    //연속 공격 강공
    //회피
    //전회

    public EMoveState m_MoveState = EMoveState.IDLE;

    public Transform m_LookAt_Dir = null;
    public Transform m_Body = null;
    public Animator m_AnimCtrl = null;
    public CAnimFunc m_AnimFunc = null;

    [Header("===========Input Result===============")]
    public float m_MoveSpeed = 1;
    public float m_RotateSpeed = 1;

    [SerializeField] Vector2 m_Move = Vector2.zero;
    [SerializeField] Vector2 m_MoveLerp = Vector2.zero;
    [SerializeField] Vector2 m_Look = Vector2.zero;
    [SerializeField] Vector2 m_Rotation = Vector2.zero;


    public enum EInputState { OPEN_UI = 0, CHAR_MOVE };
    public EInputState m_CurrState = EInputState.OPEN_UI;

    Coroutine coState_InputCheck = null;

    private void Awake()
    {
        m_CurrState = EInputState.CHAR_MOVE;
        coState_InputCheck = StartCoroutine(CoState_InputCheck());

        m_AnimFunc.m_Stop_Input = Stop_Input_Turn;
        m_AnimFunc.m_Resume_Input = Resume_Input_Turn;
    }

    //Input Once Shift 
    public void OnDash(InputAction.CallbackContext context)
    { 
        if(context.phase == InputActionPhase.Performed)
            m_StateDash = true; 
    }

    //InputVector Press AWSD
    public void OnMove(InputAction.CallbackContext context)
    { m_Move = context.ReadValue<Vector2>(); }

    //InputVector Move MOUSE
    public void OnLook(InputAction.CallbackContext context) 
    { m_Look = context.ReadValue<Vector2>(); }

    void Move(Vector2 _dir)
    {
        //입력이 없으면 반환
        if (_dir.sqrMagnitude < 0.01f)
        { return; }

        //카메라 기준 방향으로 움직임
        var scaledMoveSpeed = m_MoveSpeed * Time.deltaTime;
        var rot = Quaternion.Euler(0, m_LookAt_Dir.eulerAngles.y, 0);
        var move = rot * new Vector3(_dir.x, 0, _dir.y);
        var movement = m_StateDash ? move * scaledMoveSpeed * 2 : move * scaledMoveSpeed;
        this.transform.position += movement;

        if(m_LockOn_Target == null) 
            m_BodyRot_Dest = move.normalized;
    }


    [SerializeField] Transform m_LockOn_Target = null;
    [SerializeField] Vector3 m_BodyRot_Dest = Vector3.zero;
    [SerializeField] float m_BodyRotSpeed = 5f;
    [SerializeField] bool m_StateDash = false;

    //대상 록온
    public void LockOn()
    {
        m_LookAt_Dir.LookAt(m_LockOn_Target);
        m_BodyRot_Dest = m_LookAt_Dir.rotation * Vector3.forward;
        //m_LookAt_Dir.rotation = Quaternion.Euler(0, m_LookAt_Dir.eulerAngles.y, 0);
    }

    //public void BodyLookAt() 
    //{
    //    //상시로 회전을 지정하기에 루트모션이 작동하지 않음.

    //    if (m_LockOn_Target != null)
    //    {
    //        m_BodyRot_Dest = m_LookAt_Dir.rotation * Vector3.forward;
    //        m_Body.rotation = Quaternion.LookRotation(m_BodyRot_Dest);
    //        m_AnimCtrl.SetFloat("Dir_X", m_Move.x);
    //        m_AnimCtrl.SetFloat("Dir_Y", m_Move.y);
    //    }

    //    else
    //    {
    //        m_Body.rotation = Quaternion.LookRotation(m_BodyRot_Dest);
    //        m_AnimCtrl.SetFloat("Dir_X", 0);
    //        m_AnimCtrl.SetFloat("Dir_Y", 1);
    //    }
    //}


    public void Stop_Input_Turn() 
    {
    }

    public void Resume_Input_Turn() 
    {
        m_StateDash = false;
        m_MoveState = EMoveState.IDLE;
    }


    //회전 속도 조절
    public void LerpBodyLookAt() 
    {
        var dest = Quaternion.LookRotation(m_BodyRot_Dest);

        m_Body.rotation = Quaternion.RotateTowards(m_Body.rotation, dest, 
            Time.deltaTime * m_BodyRotSpeed);
    }



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

            if(m_Move == Vector2.zero)
                m_MoveLerp = Vector3.Slerp(m_MoveLerp, m_Move, Time.deltaTime * lerpSpeed);
            else if(Vector3.Dot(nmLerp, nmmove) < -0.99f)
            {
                if (m_StateDash == true && m_MoveLerp.sqrMagnitude > 0.5f) 
                {
                    m_AnimCtrl.SetTrigger("Turn");
                    m_MoveState = EMoveState.RUNING_TURN;
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

    [SerializeField] float lerpSpeed = 1;
    IEnumerator CoState_InputCheck() 
    {
        while (true)
        {
            switch (m_CurrState) 
            {
                case EInputState.CHAR_MOVE:
                    Look(m_Look); //카메라 움직이기

                    if (m_MoveState == EMoveState.RUNING_TURN) break;

                    Move(m_Move); //움직이기
                    LerpBodyLookAt(); //몸체 회전 보간
                    MoveLerp(); //입력값 보간

                    break;

                case EInputState.OPEN_UI:
                default:
                    break;
            }

            yield return null;
        }
    }

}
