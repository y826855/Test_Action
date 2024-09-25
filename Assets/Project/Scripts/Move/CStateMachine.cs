using System.Collections;
using UnityEngine;

public class CStateMachine : MonoBehaviour
{
    //상태 : 대기, 숙이기, 점프, 대쉬 등등
    public enum EMoveState
    {
        IDLE = 0, WALK, RUN, SPRINT, HIDE,
        JUMP_UP, FALL,
        TURN, DODGE, BACK_DODGE,
        LIE_DOWN,
    }

    public enum EActionState
    {
        IDLE = 0, END_ACT = 5,
        CAN_COMBO, ATTACK,
        GUARD_HOLD, AIM_HOLD
    }

    public EMoveState m_CurrMoveState = EMoveState.IDLE;
    public EActionState m_CurrActState = EActionState.IDLE;

    [SerializeField] public IMover m_Mover = null;
    [SerializeField] public CPlayerInput_Ctrl m_Input = null;

    public Transform m_Body = null;


    public void Start()
    {
        m_Mover = this.GetComponent<IMover>();
        StartCoroutine(CoState_InputCheck());
    }

    IEnumerator CoState_InputCheck()
    {
        while (true)
        {
            m_Mover.Look(m_Input.m_Look);
            switch (m_CurrMoveState)
            {
                case EMoveState.IDLE:
                case EMoveState.WALK:
                case EMoveState.RUN:
                    //이동
                    m_Mover.Move(m_Input.m_Move);
                    m_Mover.AfterMove();
                    break;
            }

            m_Mover.Gravity();

            this.transform.position = Vector3.Lerp(this.transform.position, m_Body.position, Time.deltaTime / 0.1f);

            yield return null;
        }
    }
}
