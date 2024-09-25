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


    //TODO : 다리가 끌려오는듯한 느낌이다..
    //고정된 다리는 움직일때 따라오지 않게 해야함

    //상태머신. 움직인다면 LFoot,RFoot체크
    //움직임 없다면 제자리 발 체크
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
            //Transform초기화 해줘야함
            m_IK_Foot_L.position = m_AnimCtrl.GetIKPosition(goal);
            m_IK_Foot_L.rotation = m_AnimCtrl.GetIKRotation(goal);
            m_AnimCtrl.SetIKPositionWeight(goal, 1);
            m_AnimCtrl.SetIKRotationWeight(goal, 1);
            HitCast(AvatarIKGoal.LeftFoot);
        }

        {
            var goal = AvatarIKGoal.RightFoot;
            //Transform초기화 해줘야함
            m_IK_Foot_R.position = m_AnimCtrl.GetIKPosition(goal);
            m_IK_Foot_R.rotation = m_AnimCtrl.GetIKRotation(goal);
            m_AnimCtrl.SetIKPositionWeight(goal, 1);
            m_AnimCtrl.SetIKRotationWeight(goal, 1);
            HitCast(AvatarIKGoal.RightFoot);
        }

        //최종 위치 선정함
        m_AnimCtrl.SetIKPosition(AvatarIKGoal.RightFoot, m_IK_Foot_R.position);
        m_AnimCtrl.SetIKRotation(AvatarIKGoal.RightFoot, m_IK_Foot_R.rotation);
        m_AnimCtrl.SetIKPosition(AvatarIKGoal.LeftFoot , m_IK_Foot_L.position);
        m_AnimCtrl.SetIKRotation(AvatarIKGoal.LeftFoot , m_IK_Foot_L.rotation);


        //본체위치 설정함.
        //두 백터를 합하고 2로 나눔. 그렇게 중앙을 구함. 최대 스탭 높이만큼 조절함
        var midP = m_IK_Foot_R.localPosition + m_IK_Foot_L.localPosition;
        var height = Mathf.Clamp(midP.y / 2f, -_maxStepHeight, 0);

        //로컬 트랜스폼 조절함
        this.transform.localPosition = height * Vector3.up;
    }

    void HitCast(AvatarIKGoal _goal)
    {
        Ray ray = new Ray(m_AnimCtrl.GetIKPosition(_goal) +
            Vector3.up * _maxStepHeight, Vector3.down * _maxStepDown);

        //IK 실패시 발 위치. 최소 maxStepDown 만큼만 내려감
        var downVector = m_AnimCtrl.GetIKPosition(_goal) + Vector3.down * _maxStepDown;

        //레이케스트
        if (Physics.SphereCast(ray, _footStepRadius,
            out RaycastHit hitted, 1f, m_LandLayer)
            && (m_Col.position - this.transform.position).sqrMagnitude < _maxStepHeight * _maxStepHeight)
        {
            var dist = m_IK_Foot_R.position - m_IK_Foot_L.position;

            //오른발
            if (_goal == AvatarIKGoal.RightFoot)
            {
                //위치 고정 말고 수식으로 더하는 방식 생각해보자
                //m_IK_Foot_R.position = hitted.point;

                //걸을때 지정된 높이만큼 호를 그리게 함
                var sol = m_IK_Foot_R.position - hitted.point;
                m_IK_Foot_R.position = m_IK_Foot_R.position - sol
                    + (yHeightR * Vector3.up);

                m_IK_Foot_R.rotation =
                    Quaternion.LookRotation(this.transform.forward, hitted.normal);
            }
            else//왼발
            {
                //m_IK_Foot_L.position = hitted.point;

                //걸을때 지정된 높이만큼 호를 그리게 함
                var sol = m_IK_Foot_L.position - hitted.point;
                m_IK_Foot_L.position = m_IK_Foot_L.position - sol
                    + (yHeightL * Vector3.up);

                m_IK_Foot_L.rotation = 
                    Quaternion.LookRotation(this.transform.forward, hitted.normal);
            }


            //m_AnimCtrl.SetIKPosition(_goal, hitted.point);
            //m_AnimCtrl.SetIKRotation(_goal, Quaternion.LookRotation(this.transform.forward, hitted.normal));

        }
        else //레이케스트 실패(땅이 없음).
        {
            m_AnimCtrl.SetIKPosition(_goal, downVector);
            if (_goal == AvatarIKGoal.RightFoot)
                m_IK_Foot_R.position = downVector; 
            else m_IK_Foot_L.position = downVector;
        }

        //var mid = (m_IK_Foot_R - m_IK_Foot_L) / 2f;
        //var res = m_IK_Foot_R - mid;


    }

    //변수 두개 써야함
    float yHeightL = 0;
    float yHeightR = 0;

    //lerp 사용해서 높이 호 구함
    void Set_IK_Height(AvatarIKGoal _goal, float _target ,float _alpha) 
    {
        if (_goal == AvatarIKGoal.RightFoot)
            yHeightR = Mathf.Lerp(yHeightR, _target, _alpha);
        else
            yHeightL = Mathf.Lerp(yHeightL, _target, _alpha);
    }

    //커브 생성기. 애니메이션에서 걷는 시간 받아옴.
    //그냥 2차 함수의 커브 생성함
    //애니메이션 속도가 바뀌면 작동 안함.
    //기믹 변경 필요.. 이건 그냥 테스트용에 불과
    IEnumerator CoMakeCurve(AvatarIKGoal _goal, float _duration)
    {
        //2배 해줘야 싱크가 맞음.. 왜?
        _duration *= 2;
        float half = _duration / 2f;
        float during = 0f;

        while (during < _duration)
        {
            if (during < half)
            {//높이 구하기
                Set_IK_Height(_goal, _maxStepHeight, during);
                //yHeight = Mathf.Lerp(yHeight, _maxStepHeight, during * 2f); 
            }
            else
            {//높이 구하기
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

    //TODO : 기즈모를 참조하여 좀더 수정해보자
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
