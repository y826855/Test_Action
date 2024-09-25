using UnityEngine;
using UnityEngine.InputSystem;

public class CMover_IK02 : MonoBehaviour, IMover
{

    public CharacterController m_CharController = null;
    [SerializeField] Animator m_AnimCtrl = null;

    [SerializeField] Vector2 m_Move = Vector2.zero;
    [SerializeField] Vector2 m_MoveLerp = Vector2.zero;
    [SerializeField] float lerpSpeed = 1;
    [SerializeField] float m_MoveSpeed = 3f;

    [SerializeField] Vector2 m_Look = Vector2.zero;
    [SerializeField] float m_Gravity = 1f;
    [SerializeField] float m_Gravity_Acc = 1.1f;
    //InputVector Press AWSD
    public void OnMove(InputAction.CallbackContext context)
    { m_Move = context.ReadValue<Vector2>(); }

    //InputVector Move MOUSE
    public void OnLook(InputAction.CallbackContext context)
    { m_Look = context.ReadValue<Vector2>(); }

    private void Update()
    {
        Move(m_Move);
        MoveLerp();
        Gravity();
    }

    public void AfterMove()
    {
        throw new System.NotImplementedException();
    }

    public void Gravity()
    {
        //�߰����� �� üũ �ʿ���
        if (m_CharController.isGrounded == false)
        {
            m_Gravity += m_Gravity * m_Gravity_Acc * Time.deltaTime;
            m_Gravity = Mathf.Clamp(m_Gravity, -1000f, 1000f);

            var grav = Vector3.down * m_Gravity;
            m_CharController.Move(grav);
        }
        else
        {
            m_Gravity = 1f;
            var grav = Vector3.down * m_Gravity;
            m_CharController.Move(grav);
        }
    }

    public void Look(Vector2 _rot)
    {
        throw new System.NotImplementedException();
    }

    public void Move(Vector2 _dir)
    {
        m_AnimCtrl.SetFloat("Dir_X", _dir.x);
        m_AnimCtrl.SetFloat("Dir_Y", _dir.y);

        //�Է��� ������ ��ȯ
        if (_dir.sqrMagnitude < 0.01f)
        { return; }

        Vector3 dir = new Vector3(_dir.x, 0, _dir.y);
        var movement = dir * m_MoveSpeed * Time.deltaTime;
        m_CharController.Move(movement);

    }

    void MoveLerp()
    {
        var moveSqr = m_MoveLerp.sqrMagnitude;
        var move = m_MoveLerp;

        m_AnimCtrl.SetFloat("Dir_X", move.x);
        m_AnimCtrl.SetFloat("Dir_Y", move.y);

        //if (m_CurrMoveState == EMoveState.TURN) return;

        //awsd�Է°� ����
        if (m_MoveLerp != m_Move)
        {//�� ������ ������ 180���� �Ѿ�� slerp �۵��� ����

            var nmLerp = m_MoveLerp.normalized;
            var nmmove = m_Move.normalized;

            if (m_Move == Vector2.zero)
                m_MoveLerp = Vector3.Slerp(m_MoveLerp, m_Move, Time.deltaTime * lerpSpeed);
            else if (Vector3.Dot(nmLerp, nmmove) < -0.99f)
            {
                //��ȸ���� ���� �ִϸ��̼�
                //if (m_StateDash == true && m_MoveLerp.sqrMagnitude > 0.5f)
                //{
                //    m_AnimCtrl.SetTrigger("Turn");
                //    m_MoveLerp = nmmove;
                //    return;
                //}

                // ������ ���� ������ ���͸� �߰�
                nmLerp += new Vector2(0.01f, 0.01f);
                nmLerp.Normalize();
                m_MoveLerp = Vector3.Slerp(nmLerp, nmmove, Time.deltaTime * lerpSpeed);
            }
            else
                m_MoveLerp = Vector3.Slerp(nmLerp, nmmove, Time.deltaTime * lerpSpeed);
        }
    }



    public void StepUpR(float _duration) { }
    public void StepDownR() { }
    public void StepUpL(float _duration) { }
    public void StepDownL() { }
}
