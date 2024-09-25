using UnityEngine;
using System.Collections;

public class C_IK_TEST03 : MonoBehaviour
{
    [SerializeField] Animator m_AnimCtrl = null;
    [SerializeField] LayerMask m_LandLayer;
    [SerializeField] Transform m_Col = null;

    [SerializeField] float _maxStepHeight = 0.3f;
    [SerializeField] float _maxStepDown = 0.3f;
    [SerializeField] float _footStepRadius = 0.1f;

    //[SerializeField] Vector3 m_IK_Foot_L = Vector3.zero;
    //[SerializeField] Vector3 m_IK_Foot_R = Vector3.zero;

    [SerializeField] Transform m_IK_Foot_L = null;
    [SerializeField] Transform m_IK_Foot_R = null;


    //TODO : �ٸ��� �������µ��� �����̴�..
    //������ �ٸ��� �����϶� ������� �ʰ� �ؾ���

    //���¸ӽ�. �����δٸ� LFoot,RFootüũ
    //������ ���ٸ� ���ڸ� �� üũ
    public void StepUpR(float _duration)
    {
        Debug.Log(_duration);
        StartCoroutine(CoMakeCurve(AvatarIKGoal.RightFoot, _duration));
    }

    public void StepDownR()
    { }

    public void StepUpL(float _duration)
    {
        Debug.Log(_duration);
        StartCoroutine(CoMakeCurve(AvatarIKGoal.LeftFoot, _duration));
    }

    public void StepDownL()
    { }

    enum EIK_State { NONE, STEP_PLACE, FOOT_L, FOOT_R }
    IEnumerator CoStateMachine() 
    {
        while (true) 
        {

            yield return null;
        }
    }


    private void OnAnimatorIK(int layerIndex)
    {
        {
            var goal = AvatarIKGoal.LeftFoot;
            //Transform�ʱ�ȭ �������
            m_IK_Foot_L.position = m_AnimCtrl.GetIKPosition(goal);
            m_IK_Foot_L.rotation = m_AnimCtrl.GetIKRotation(goal);
            m_AnimCtrl.SetIKPositionWeight(goal, 1);
            m_AnimCtrl.SetIKRotationWeight(goal, 1);
            HitCast(AvatarIKGoal.LeftFoot);
        }

        {
            var goal = AvatarIKGoal.RightFoot;
            //Transform�ʱ�ȭ �������
            m_IK_Foot_R.position = m_AnimCtrl.GetIKPosition(goal);
            m_IK_Foot_R.rotation = m_AnimCtrl.GetIKRotation(goal);
            m_AnimCtrl.SetIKPositionWeight(goal, 1);
            m_AnimCtrl.SetIKRotationWeight(goal, 1);
            HitCast(AvatarIKGoal.RightFoot);
        }

        //���� ��ġ ������
        m_AnimCtrl.SetIKPosition(AvatarIKGoal.RightFoot, m_IK_Foot_R.position);
        m_AnimCtrl.SetIKRotation(AvatarIKGoal.RightFoot, m_IK_Foot_R.rotation);
        m_AnimCtrl.SetIKPosition(AvatarIKGoal.LeftFoot , m_IK_Foot_L.position);
        m_AnimCtrl.SetIKRotation(AvatarIKGoal.LeftFoot , m_IK_Foot_L.rotation);


        //��ü��ġ ������.
        //�� ���͸� ���ϰ� 2�� ����. �׷��� �߾��� ����. �ִ� ���� ���̸�ŭ ������
        var midP = m_IK_Foot_R.localPosition + m_IK_Foot_L.localPosition;
        var height = Mathf.Clamp(midP.y / 2f, -_maxStepHeight, 0);

        //���� Ʈ������ ������
        this.transform.localPosition = height * Vector3.up;
    }

    void HitCast(AvatarIKGoal _goal)
    {
        Ray ray = new Ray(m_AnimCtrl.GetIKPosition(_goal) +
            Vector3.up * _maxStepHeight, Vector3.down * _maxStepDown);

        //IK ���н� �� ��ġ. �ּ� maxStepDown ��ŭ�� ������
        var downVector = m_AnimCtrl.GetIKPosition(_goal) + Vector3.down * _maxStepDown;

        //�����ɽ�Ʈ
        if (Physics.SphereCast(ray, _footStepRadius,
            out RaycastHit hitted, 1f, m_LandLayer)
            && (m_Col.position - this.transform.position).sqrMagnitude < _maxStepHeight * _maxStepHeight)
        {
            var dist = m_IK_Foot_R.position - m_IK_Foot_L.position;

            //������
            if (_goal == AvatarIKGoal.RightFoot)
            {
                //��ġ ���� ���� �������� ���ϴ� ��� �����غ���
                //m_IK_Foot_R.position = hitted.point;

                //������ ������ ���̸�ŭ ȣ�� �׸��� ��
                var sol = m_IK_Foot_R.position - hitted.point;
                m_IK_Foot_R.position = m_IK_Foot_R.position - sol
                    + (yHeightR * Vector3.up);

                m_IK_Foot_R.rotation =
                    Quaternion.LookRotation(this.transform.forward, hitted.normal);
            }
            else//�޹�
            {
                //m_IK_Foot_L.position = hitted.point;

                //������ ������ ���̸�ŭ ȣ�� �׸��� ��
                var sol = m_IK_Foot_L.position - hitted.point;
                m_IK_Foot_L.position = m_IK_Foot_L.position - sol
                    + (yHeightL * Vector3.up);

                m_IK_Foot_L.rotation = 
                    Quaternion.LookRotation(this.transform.forward, hitted.normal);
            }


            //m_AnimCtrl.SetIKPosition(_goal, hitted.point);
            //m_AnimCtrl.SetIKRotation(_goal, Quaternion.LookRotation(this.transform.forward, hitted.normal));

        }
        else //�����ɽ�Ʈ ����(���� ����).
        {
            m_AnimCtrl.SetIKPosition(_goal, downVector);
            if (_goal == AvatarIKGoal.RightFoot)
                m_IK_Foot_R.position = downVector; 
            else m_IK_Foot_L.position = downVector;
        }

        //var mid = (m_IK_Foot_R - m_IK_Foot_L) / 2f;
        //var res = m_IK_Foot_R - mid;


    }

    //���� �ΰ� �����
    float yHeightL = 0;
    float yHeightR = 0;

    //lerp ����ؼ� ���� ȣ ����
    void Set_IK_Height(AvatarIKGoal _goal, float _target ,float _alpha) 
    {
        if (_goal == AvatarIKGoal.RightFoot)
            yHeightR = Mathf.Lerp(yHeightR, _target, _alpha);
        else
            yHeightL = Mathf.Lerp(yHeightL, _target, _alpha);
    }

    //Ŀ�� ������. �ִϸ��̼ǿ��� �ȴ� �ð� �޾ƿ�.
    //�׳� 2�� �Լ��� Ŀ�� ������
    //�ִϸ��̼� �ӵ��� �ٲ�� �۵� ����.
    //��� ���� �ʿ�.. �̰� �׳� �׽�Ʈ�뿡 �Ұ�
    IEnumerator CoMakeCurve(AvatarIKGoal _goal, float _duration)
    {
        //2�� ����� ��ũ�� ����.. ��?
        _duration *= 2;
        float half = _duration / 2f;
        float during = 0f;

        while (during < _duration)
        {
            if (during < half)
            {//���� ���ϱ�
                Set_IK_Height(_goal, _maxStepHeight, during);
                //yHeight = Mathf.Lerp(yHeight, _maxStepHeight, during * 2f); 
            }
            else
            {//���� ���ϱ�
                Set_IK_Height(_goal, 0, (during - half) * 2f);
                //yHeight = Mathf.Lerp(yHeight, 0, (during - half) * 2f); 
            }


            during += Time.deltaTime;
            yield return null;
        }

        Debug.Log(during);


        if (_goal == AvatarIKGoal.RightFoot)
            yHeightR = 0;
        else yHeightL = 0;

        yield return null;
    }

    //TODO : ����� �����Ͽ� ���� �����غ���
    private void OnDrawGizmos()
    {
        //var mid = m_IK_Foot_R - m_IK_Foot_L;
        //mid.x = 0; mid.z = 0; if (mid.y > 0) mid.y *= -1;
        //var res = m_Col.transform.position + mid;

        var mid = (m_IK_Foot_R.position - m_IK_Foot_L.position) /2f;
        var res = m_IK_Foot_R.position - mid;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(res, _footStepRadius*2);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_IK_Foot_R.position, _footStepRadius);
        Gizmos.DrawWireSphere(m_IK_Foot_L.position, _footStepRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_IK_Foot_R.position, m_IK_Foot_L.position);

        Gizmos.DrawLine(m_IK_Foot_R.position + Vector3.up * _maxStepHeight,
            m_IK_Foot_R.position + Vector3.down * _maxStepDown);
        Gizmos.DrawLine(m_IK_Foot_L.position + Vector3.up * _maxStepHeight,
            m_IK_Foot_L.position + Vector3.down * _maxStepDown);
    }
}
