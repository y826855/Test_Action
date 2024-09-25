using System.Collections;
using UnityEngine;

public class CBall : MonoBehaviour
{
    //평상 스피드
    public float m_Speed = 10f;
    public Vector2 m_Direction = Vector2.down;
    
    //최소 최대 속력
    [SerializeField] float m_MinSpeed = 3f;
    [SerializeField] float m_MaxSpeed = 30f;

    [SerializeField] Rigidbody2D m_Rigid = null;
    //공 반지름
    [SerializeField] float _radius = 0f;

    //스파이크 이펙트
    [SerializeField] TrailRenderer m_Trail = null;

    public void Start()
    {
        _radius = this.transform.localScale.x / 2f;
        m_Rigid.AddForce(m_Direction * m_Speed);

        

        //m_Rigid.add
        StartCoroutine(CoMove());
    }

    //[SerializeField] float currSpeed = 0;

    IEnumerator CoMove() 
    {
        while (true) 
        {
            //외벽 나가는지 체크
            CGameManager_Pika.Instance.m_GameArea.BallCheck(m_Rigid, _radius);

            //Debug.Log("work");
            //속도 조절
            var currSpeed = m_Rigid.linearVelocity.magnitude;
            if (currSpeed < m_MinSpeed || currSpeed > m_MaxSpeed) 
            {
                //Debug.Log(currSpeed);
                currSpeed = Mathf.Clamp(currSpeed, m_MinSpeed, m_MaxSpeed);
                m_Rigid.linearVelocity = m_Rigid.linearVelocity.normalized * currSpeed;
                //Debug.Log("clamped");
            }


            yield return null;  
        }
    }


    //스파이크 활성화
    public void ActiveSpike(float _power) 
    {
        //가속 추가
        m_Rigid.AddForce(m_Rigid.linearVelocity.normalized * _power, ForceMode2D.Impulse);
        m_Trail.enabled = true;
    }

    //스파이크 비활성화
    public void DeactiveSpike() 
    {
        m_Trail.enabled = false;
        //속도 정상화
        var currSpeed = m_Rigid.linearVelocity.magnitude;
        if (currSpeed > m_Speed)
        {
            currSpeed = Mathf.Clamp(currSpeed, m_MinSpeed, m_Speed);
            m_Rigid.linearVelocity = m_Rigid.linearVelocity.normalized * currSpeed;
        }
    }

    //공이 땅에 닿음
    public void BallTouchGround() 
    {
        m_Rigid.linearVelocityY = m_Speed;
    }

    Vector2 dir = Vector2.zero;
    Vector3 inNormal;
    Vector3 inDir;
    Vector3 hitPoint;

    public LayerMask m_LayerMask;


    //더이상 안씀
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.tag != "Player") return;
        //collision.ClosestPoint

        //Debug.Log("TRIGGERD");

        ////
        //var ray = (collision.transform.position - this.transform.position).normalized;
        ////var dir = (this.transform.position - collision.transform.position).normalized;
        //var dir = m_Rigid.linearVelocity.normalized;

        //var hitted = Physics2D.CircleCast(this.transform.position, _radius+0.1f,
        //    m_Rigid.linearVelocity.normalized, 10f, m_LayerMask);
        ////var hitted = Physics2D.Raycast(this.transform.position, ray, 5f, m_LayerMask);

        //if (hitted.collider == null) return;

        //hitPoint = hitted.point;
        //inNormal = hitted.normal;
        //inDir = dir;

        //var res = Vector2.Reflect(dir, hitted.normal);


        //m_Rigid.linearVelocity = Vector2.zero;
        //m_Rigid.AddForce(res * m_Speed, ForceMode2D.Impulse);

    }

    private void OnDrawGizmos()
    {
        //var dir = m_Direction * _radius;
        //var moved = dir * m_Speed * Time.deltaTime;
        //Gizmos.DrawLine(this.transform.position, moved);

        //Vector3 v = -dir * m_Rigid.linearVelocity.magnitude * 2f;

        Vector3 v = m_Rigid.linearVelocity.normalized * m_Rigid.linearVelocity.magnitude * 2f;
        Gizmos.DrawLine(this.transform.position, this.transform.position + v);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(hitPoint - inDir, hitPoint);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(hitPoint + inNormal, hitPoint);
        Gizmos.color = Color.blue;
        Vector3 refl = Vector2.Reflect(inDir, inNormal);
        Gizmos.DrawLine(hitPoint + refl, hitPoint);
    }
}
