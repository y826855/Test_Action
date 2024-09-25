using UnityEngine;

public class CMove_Rot : MonoBehaviour, IMover
{
    [Header("===========Input Result===============")]
    public float m_MoveSpeed = 1;
    public float m_RotateSpeed = 1;
    //awsd �Է� ��
    [SerializeField] Vector2 m_Move = Vector2.zero;

    [SerializeField] Vector2 m_MoveLerp = Vector2.zero;
    //���콺 xy ��Ÿ
    [SerializeField] Vector2 m_Look = Vector2.zero;
    //ȸ����
    [SerializeField] Vector2 m_Rotation = Vector2.zero;

    //ȭ�� ��ȯ �ӵ�
    [SerializeField] float lerpSpeed = 1;
    //���� ���� �޺�
    [SerializeField] int m_Combo = -1;

    [Header("===========Rotation===============")]
    public Transform m_LookAt_Dir = null;
    public Transform m_Body = null;
    public Rigidbody m_Rigid = null;
    public CharacterController m_CharController = null;
    [SerializeField] Vector3 m_BodyRot_Dest = Vector3.zero;
    [SerializeField] float m_BodyRotSpeed = 5f;

    public Animator m_AnimCtrl = null;

    //�̵�
    public void Move(Vector2 _dir)
    {
        Debug.Log("inputed" + _dir);
        m_Move = _dir;

        //�Է��� ������ ��ȯ
        if (_dir.sqrMagnitude < 0.01f)
        { return; }

        //ī�޶� ���� �������� ������
        var scaledMoveSpeed = m_MoveSpeed * Time.deltaTime;
        var rot = Quaternion.Euler(0, m_LookAt_Dir.eulerAngles.y, 0);
        var move = rot * new Vector3(_dir.x, 0, _dir.y);

        var movement = move * scaledMoveSpeed;
        m_CharController.Move(movement);
        
        //ȸ������ ����
        m_BodyRot_Dest = move.normalized;
    }

    //Move ����
    public void AfterMove() 
    {
        LerpBodyLookAt();
        MoveLerp();
    }


    //ȭ����ȯ (���콺)
    public void Look(Vector2 _rot)
    {
        m_Look = _rot;
        //�Է��� ������ ��ȯ
        if (_rot.sqrMagnitude < 0.01) return;

        //�Է°��� ���� ī�޶� ȸ��
        var scaledRotateSpeed = m_RotateSpeed * Time.deltaTime;
        m_Rotation.y += _rot.x * scaledRotateSpeed;
        m_Rotation.x = Mathf.Clamp(m_Rotation.x - _rot.y * scaledRotateSpeed, -89f, 89f);
        m_LookAt_Dir.transform.localEulerAngles = m_Rotation;
    }


    //ȸ�� �ӵ� ����
    public void LerpBodyLookAt()
    {
        m_BodyRot_Dest.y = 0;
        var dest = Quaternion.LookRotation(m_BodyRot_Dest);

        m_Body.rotation = Quaternion.RotateTowards(m_Body.rotation, dest,
            Time.deltaTime * m_BodyRotSpeed);
    }

    void MoveLerp()
    {
        var moveSqr = m_MoveLerp.sqrMagnitude;
        var move = m_MoveLerp;

        m_AnimCtrl.SetFloat("Dir_Y", moveSqr);

        //awsd�Է°� ����
        if (m_MoveLerp != m_Move)
        {//�� ������ ������ 180���� �Ѿ�� slerp �۵��� ����

            var nmLerp = m_MoveLerp.normalized;
            var nmmove = m_Move.normalized;

            Debug.Log("ROT");

            if (m_Move == Vector2.zero)
                m_MoveLerp = Vector3.Slerp(m_MoveLerp, m_Move, Time.deltaTime * lerpSpeed);
            else if (Vector3.Dot(nmLerp, nmmove) < -0.99f)
            {
                // ������ ���� ������ ���͸� �߰�
                nmLerp += new Vector2(0.01f, 0.01f);
                nmLerp.Normalize();
                m_MoveLerp = Vector3.Slerp(nmLerp, nmmove, Time.deltaTime * lerpSpeed);
            }
            else
                m_MoveLerp = Vector3.Slerp(nmLerp, nmmove, Time.deltaTime * lerpSpeed);
        }
    }


    [Header("============GRAVITY==============")]
    [SerializeField] float m_Gravity = 1f;
    [SerializeField] float m_Gravity_Acc = 1.1f;
    [SerializeField] float m_Jump = 10f;
    [SerializeField] CLandCheck m_CheckLand = null;

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
}
