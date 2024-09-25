using UnityEngine;

public class CArea : MonoBehaviour
{
    //TODO : �׳� vector2 2���� �ٲ���...
    [SerializeField] Vector2 m_SizeX = Vector2.zero;
    [SerializeField] Vector2 m_SizeY = Vector2.zero;

    [SerializeField] Transform m_PlayerL = null;
    [SerializeField] Transform m_PlayerR = null;

    [SerializeField] float m_NetThick = 0.4f;

    public CBall m_Ball = null;
    //public void 

    private void Awake()
    {
        CGameManager_Pika.Instance.m_GameArea = this;
    }

    //���� ĳ���� ����
    public Vector3 Check_LeftArea(Vector3 _pos) 
    {
        if (_pos.x <= m_SizeX.x) _pos.x = m_SizeX.x;
        if (_pos.x >= -m_NetThick) _pos.x = -m_NetThick;

        return _pos;
    }

    //������ ĳ���� ����
    public Vector3 Check_RightArea(Vector3 _pos)
    {
        if (_pos.x >= m_SizeX.y) _pos.x = m_SizeX.y;
        if (_pos.x <= m_NetThick) _pos.x = m_NetThick;

        Debug.Log("right");

        return _pos;
    }


    //���� ���� �������
    public void BallCheck(Rigidbody2D _ball, float _radius)
    {
        var pos = _ball.transform.localPosition;
        if (m_SizeX.x + _radius > pos.x)//��
        { _ball.linearVelocityX *= -1; pos.x = m_SizeX.x + _radius; }
        else if (m_SizeX.y - _radius < pos.x)//��
        { _ball.linearVelocityX *= -1; pos.x = m_SizeX.y - _radius; }
        if (m_SizeY.x + _radius > pos.y )//�Ʒ�
        { 
            _ball.linearVelocityY *= -1; pos.y = m_SizeY.x + _radius;
            BallTouchGroud();
        }
        else if (m_SizeY.y - _radius < pos.y )//��
        { _ball.linearVelocityY *= -1; pos.y = m_SizeY.y - _radius; }

        _ball.transform.localPosition = pos;
    }

    //���� ���� ����
    public void BallTouchGroud() 
    {
        Debug.Log("Ball Touch the Ground");
        
    }

    private void OnDrawGizmos()
    {
        var mapLD = this.transform.position + new Vector3(m_SizeX.x, 0, 0);
        var mapLU = this.transform.position + new Vector3(m_SizeX.x, m_SizeY.y, 0);
        var mapRD = this.transform.position + new Vector3(m_SizeX.y, 0, 0);
        var mapRU = this.transform.position + new Vector3(m_SizeX.y, m_SizeY.y, 0);

        Gizmos.DrawLine(mapLD, mapRD);
        Gizmos.DrawLine(mapRD, mapRU);
        Gizmos.DrawLine(mapRU, mapLU);
        Gizmos.DrawLine(mapLU, mapLD);
    }
}
