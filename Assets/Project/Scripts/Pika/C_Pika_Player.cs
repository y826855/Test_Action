using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using System.Collections;

public class C_Pika_Player : MonoBehaviour, IPikaMover
{

    public bool m_IsLeft = true;

    [Header("-----------------------------")]
    [SerializeField] Animator m_AnimCtrl = null;
    [SerializeField] Vector2 m_Move = Vector2.zero;
    [SerializeField] Vector2 m_MoveLerp = Vector2.zero;
    [SerializeField] float lerpSpeed = 1;

    [Header("-----------------------------")]
    [SerializeField] float m_MoveSpeed = 1f;
    [SerializeField] float m_JumpPower = 10f;
    [SerializeField] float m_SmashPower = 2f;

    [Header("-----------------------------")]
    [SerializeField] float m_CurrGrav = 0f;
    [SerializeField] float m_GravAcc = 1f;
    [SerializeField] float m_GravAccPow = 1.1f;

    [Header("-----------------------------")]
    [SerializeField] bool m_IsSpike = false;
    [SerializeField] float m_SpikePower = 3f;

    public void OnMove(InputAction.CallbackContext context)
    { m_Move = context.ReadValue<Vector2>(); }

    //회피
    public void OnDodge(InputAction.CallbackContext context)
    {
        switch (context.phase) 
        {
            case InputActionPhase.Performed:
                m_IsSpike = true; break;
            case InputActionPhase.Canceled:
                m_IsSpike = false; break;
        }
    }

    

    //점프. 누른 시간 반영해서 낮점 구현?
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("jump");
        }
    }


    ////////////////////////////////////////////////////

    public void Start()
    {
        StartCoroutine(CoState_InputCheck());
    }

    IEnumerator CoState_InputCheck()
    {
        while (true) 
        {
            Move(m_Move); //입력
            MoveLerp();
            Gravity();

            yield return null;
        }
    }

    //이동
    public void Move(Vector2 _dir)
    {
        if (_dir == Vector2.zero) return;

        if (_dir.y > 0) Jump();

        //한계점 지정
        var move = _dir.x * Time.deltaTime * m_MoveSpeed * Vector3.right;

        Vector3 res = Vector3.zero;

        if (m_IsLeft == true)
            res = CGameManager_Pika.Instance.m_GameArea.Check_LeftArea(this.transform.localPosition + move);
        else
            res = CGameManager_Pika.Instance.m_GameArea.Check_RightArea(this.transform.localPosition + move);

        this.transform.localPosition = res;

    }

    //애니메이션 가중치 감소량 보간
    void MoveLerp()
    {
        var moveSqr = m_MoveLerp.sqrMagnitude;
        var move = m_MoveLerp;

        if(m_IsLeft == true)
            m_AnimCtrl.SetFloat("Dir_X", move.x);
        else
            m_AnimCtrl.SetFloat("Dir_X", -move.x);
        //m_AnimCtrl.SetFloat("Dir_Y", move.y);

        //if (m_CurrMoveState == EMoveState.TURN) return;

        //awsd입력값 보간
        if (m_MoveLerp != m_Move)
        {//두 벡터의 각도가 180도가 넘어가면 slerp 작동시 오류

            var nmLerp = m_MoveLerp.normalized;
            var nmmove = m_Move.normalized;

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

    public void Dodge()
    {

    }

    public void Jump()
    {
        //공중 점프 ㄴㄴ
        if (this.transform.localPosition.y > 0.01f) return;

        m_CurrGrav -= m_JumpPower;
    }

    //중력
    public void Gravity() 
    {
        if (this.transform.localPosition.y < 0.01f && m_CurrGrav >= 0) return;

        //현재 중력 연산
        var grav = m_CurrGrav * Time.deltaTime * Vector3.down;

        //가속도 계산
        m_GravAcc += m_GravAcc * m_GravAccPow * Time.deltaTime;
        m_CurrGrav += m_GravAcc;

        var res = this.transform.localPosition + grav;

        //Ground
        if (res.y < 0.01f)
        {
            res.y = 0;
            m_CurrGrav = 0;
            m_GravAcc = 1.1f;
        }

        this.transform.localPosition = res;
    }

    //사용 안함
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Ball") return;

        //despike spike 합치기 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Ball") return;

        var ball = CGameManager_Pika.Instance.m_GameArea.m_Ball;
        if (m_IsSpike == true)
        {
            Debug.Log("Spike");
            ball.ActiveSpike(m_SpikePower);
        }

        else
        {
            Debug.Log("DeSpike");
            ball.DeactiveSpike();
        }
    }
}
