using UnityEngine;
using System.Collections;


public class CMove_2D_Real : MonoBehaviour, IMover
{
    [SerializeField] CPlayerInput_Ctrl m_Player_Input = null;
    [SerializeField] Rigidbody2D m_Rigid = null;

    [SerializeField] float m_Speed = 2f;
    [SerializeField] float m_JumpPower = 2f;

    public void Start()
    {
        StartCoroutine(CoState());
    }

    IEnumerator CoState() 
    {
        while (true) 
        {
            Move(m_Player_Input.m_Move);
            yield return null;
        }
    }

    public void AfterMove()
    {
    }

    public void Gravity()
    {
    }

    public void Look(Vector2 _rot)
    {
    }

    public void Move(Vector2 _dir)
    {
        if (m_Rigid.linearVelocity.y <= 0 && _dir.y > 0) 
        { m_Rigid.AddForce(Vector2.up * m_JumpPower, ForceMode2D.Impulse); }
        _dir.y = 0;
        var acc = Time.deltaTime * m_Speed;
        var move = (Vector2)this.transform.position + (acc * _dir);
        m_Rigid.MovePosition(move);
    }
}
