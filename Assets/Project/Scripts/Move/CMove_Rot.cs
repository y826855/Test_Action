using UnityEngine;

public class CMove_Rot : MonoBehaviour, IMover
{
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

    public Animator m_AnimCtrl = null;

    //이동
    public void Move(Vector2 _dir)
    {
        Debug.Log("inputed" + _dir);
        m_Move = _dir;

        //입력이 없으면 반환
        if (_dir.sqrMagnitude < 0.01f)
        { return; }

        //카메라 기준 방향으로 움직임
        var scaledMoveSpeed = m_MoveSpeed * Time.deltaTime;
        var rot = Quaternion.Euler(0, m_LookAt_Dir.eulerAngles.y, 0);
        var move = rot * new Vector3(_dir.x, 0, _dir.y);

        var movement = move * scaledMoveSpeed;
        m_CharController.Move(movement);
        
        //회전방향 설정
        m_BodyRot_Dest = move.normalized;
    }

    //Move 이후
    public void AfterMove() 
    {
        LerpBodyLookAt();
        MoveLerp();
    }


    //화면전환 (마우스)
    public void Look(Vector2 _rot)
    {
        m_Look = _rot;
        //입력이 없으면 반환
        if (_rot.sqrMagnitude < 0.01) return;

        //입력값에 따른 카메라 회전
        var scaledRotateSpeed = m_RotateSpeed * Time.deltaTime;
        m_Rotation.y += _rot.x * scaledRotateSpeed;
        m_Rotation.x = Mathf.Clamp(m_Rotation.x - _rot.y * scaledRotateSpeed, -89f, 89f);
        m_LookAt_Dir.transform.localEulerAngles = m_Rotation;
    }


    //회전 속도 조절
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

        //awsd입력값 보간
        if (m_MoveLerp != m_Move)
        {//두 벡터의 각도가 180도가 넘어가면 slerp 작동시 오류

            var nmLerp = m_MoveLerp.normalized;
            var nmmove = m_Move.normalized;

            Debug.Log("ROT");

            if (m_Move == Vector2.zero)
                m_MoveLerp = Vector3.Slerp(m_MoveLerp, m_Move, Time.deltaTime * lerpSpeed);
            else if (Vector3.Dot(nmLerp, nmmove) < -0.99f)
            {
                // 임의의 작은 노이즈 벡터를 추가
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
        //추가적인 땅 체크 필요함
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
