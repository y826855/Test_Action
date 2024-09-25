using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class CTest : MonoBehaviour
{
    //awsd �Է� ��
    [SerializeField] Vector2 m_Move = Vector2.zero;

    [SerializeField] Vector2 m_MoveLerp = Vector2.zero;
    //���콺 xy ��Ÿ
    [SerializeField] Vector2 m_Look = Vector2.zero;
    //ȸ����
    [SerializeField] Vector2 m_Rotation = Vector2.zero;
    public Transform m_LookAt_Dir = null;
    public Transform m_Body = null;
    [Header("===========Input Result===============")]
    public float m_MoveSpeed = 1;
    public float m_RotateSpeed = 1;

    [SerializeField] CharacterController m_CharController = null;

    public void OnMove(InputAction.CallbackContext context)
    { m_Move = context.ReadValue<Vector2>(); }
    public void OnLook(InputAction.CallbackContext context)
    { m_Look = context.ReadValue<Vector2>(); }


    void Move(Vector2 _dir)
    {
        //�Է��� ������ ��ȯ
        if (_dir.sqrMagnitude < 0.01f)
        { return; }

        //if (m_CurrActState != EActionState.IDLE)
        //{ ActionReset(); }

        //ī�޶� ���� �������� ������
        var scaledMoveSpeed = m_MoveSpeed * Time.deltaTime;
        var rot = Quaternion.Euler(0, m_LookAt_Dir.eulerAngles.y, 0);
        var move = rot * new Vector3(_dir.x, -1, _dir.y);
        var movement = move * scaledMoveSpeed;
        //this.transform.position += movement;
        //m_Body.transform.position += movement;
        //m_Rigid.MovePosition(m_Body.transform.position + movement);
        m_CharController.Move(movement);
    }

    private void Awake()
    {
        StartCoroutine(CoState_InputCheck());
    }

    IEnumerator CoState_InputCheck()
    {
        while (true)
        {
            //Look(m_Look);
            Move(m_Move); //�Է�
            //LerpBodyLookAt(); //��ü ȸ�� ����
            //MoveLerp(); //�Է°� ����
            //this.transform.position = m_Body.position;
            //    Vector3.Lerp(m_Body.position, this.transform.position, Time.deltaTime * 50f);
            //this.transform.position =
            //    Vector3.Lerp(this.transform.position, m_Body.position, Time.deltaTime / 0.1f);

            yield return null;
        }
    }

    //ȭ����ȯ (���콺)
    void Look(Vector2 _rot)
    {
        //�Է��� ������ ��ȯ
        if (_rot.sqrMagnitude < 0.01) return;

        //�Է°��� ���� ī�޶� ȸ��
        var scaledRotateSpeed = m_RotateSpeed * Time.deltaTime;
        m_Rotation.y += _rot.x * scaledRotateSpeed;
        m_Rotation.x = Mathf.Clamp(m_Rotation.x - _rot.y * scaledRotateSpeed, -89f, 89f);
        m_LookAt_Dir.transform.localEulerAngles = m_Rotation;
    }
}
