using UnityEngine;
using System.Collections;

public class CTEST_IK : MonoBehaviour
{
    [Header("===============floats===============")]
    [SerializeField] float _footStep_Dist = 1f;
    [SerializeField] float _distanceGround = 0f;
    [SerializeField] float _maxCheckTop = 1f;
    [SerializeField] float _maxCheckDown = 1f;
    [SerializeField] float _footStepRadius = 0.15f;
    [SerializeField] float _stepHeight = 1f;

    [SerializeField] float _footLerpSpeed = 0.3f;

    [Header("==============================")]

    [SerializeField] LayerMask m_LandLayer;


    public Transform m_BeforeLocation = null;
    [SerializeField] Vector3 m_BeforePos = Vector3.zero;
    [SerializeField] Transform m_Foot_IK_L = null;
    [SerializeField] Transform m_Foot_IK_R = null;
    [SerializeField] Transform m_Helper_IK_L = null;
    [SerializeField] Transform m_Helper_IK_R = null;

    [SerializeField] Animator m_AnimCtrl = null;

    public enum EStateFoot { NONE = 0, STEP_DOWN = 1, STEP_UP = 2, }

    public void Start()
    {
        //m_BeforeLocation.position = this.transform.position;
        //m_BeforeLocation.rotation = this.transform.rotation;

        m_AnimCtrl = this.GetComponent<Animator>();

        m_BeforePos = this.transform.position;
        StartCoroutine(CoChecking());
    }

    IEnumerator CoChecking()
    {
        while (true)
        {
            yield return null;

            //var moved = m_BeforePos - this.transform.position;
            //if (moved.sqrMagnitude >= _footStep_Dist * _footStep_Dist)
            //{
            //
            //
            //    SwapStep();
            //}


            //var dist_FL = this.transform.position - m_Foot_IK_L.position;
            //var dist_FR = this.transform.position - m_Foot_IK_R.position;
            //
            //if (dist_FL.sqrMagnitude >= _footStep_Dist * _footStep_Dist)
            //    m_Foot_IK_L.position = this.transform.position;
            //if (dist_FR.sqrMagnitude >= _footStep_Dist * _footStep_Dist)
            //    m_Foot_IK_R.position = this.transform.position;

            //m_Foot_IK_L.position = m_AnimCtrl.GetIKPosition(AvatarIKGoal.LeftFoot);
            //m_Foot_IK_R.position = m_AnimCtrl.GetIKPosition(AvatarIKGoal.RightFoot);
            //m_Helper_IK_L.position = m_Foot_IK_L.position;
            //m_Helper_IK_R.position = m_Foot_IK_R.position;




            //if (m_eIK_L == EStateFoot.STEP_DOWN)
            {
                var goal = AvatarIKGoal.LeftFoot;
                m_AnimCtrl.SetIKPositionWeight(goal, 1);
                m_AnimCtrl.SetIKRotationWeight(goal, 1);
                //HitCast(AvatarIKGoal.LeftFoot);

                var defaultPos = m_AnimCtrl.GetIKPosition(goal);

                m_AnimCtrl.SetIKPosition(goal, m_Foot_IK_L.position);
                m_AnimCtrl.SetIKRotation(goal, m_Foot_IK_L.rotation);

                m_Foot_IK_L.position = defaultPos;
            }

            //if (m_eIK_R == EStateFoot.STEP_DOWN)
            {
                var goal = AvatarIKGoal.RightFoot;
                m_AnimCtrl.SetIKPositionWeight(goal, 1);
                m_AnimCtrl.SetIKRotationWeight(goal, 1);
                //HitCast(AvatarIKGoal.RightFoot);

                var defaultPos = m_AnimCtrl.GetIKPosition(goal);

                m_AnimCtrl.SetIKPosition(goal, m_Foot_IK_R.position);
                m_AnimCtrl.SetIKRotation(goal, m_Foot_IK_R.rotation);

                m_Foot_IK_R.position = defaultPos;
            }







            HitCast(AvatarIKGoal.LeftFoot);
            HitCast(AvatarIKGoal.RightFoot);

            m_Foot_IK_R.position = m_Helper_IK_R.position;
            //Vector3.Lerp(m_Helper_IK_R.position, m_Foot_IK_R.position, 0.1f);
            m_Foot_IK_L.position = m_Helper_IK_L.position;
            //Vector3.Lerp(m_Helper_IK_L.position, m_Foot_IK_L.position, 0.1f);

        }
    }

    //발 스왑
    void SwapStep()
    {
        
    }

    public void NextFootLoc()
    {

    }


    #region AnimCallbackFunc

    public void Step(int _rl)
    {
        //오른쪽 내딛음
        if (_rl == 0)
        { }
        else
        { }
    }

    [SerializeField] EStateFoot m_eIK_L = EStateFoot.NONE;
    [SerializeField] EStateFoot m_eIK_R = EStateFoot.NONE;


    //여기서 모든것을 처리해야함. 처리하지 못해서 실패
    private void OnAnimatorIK(int layerIndex)
    {

        //if (m_eIK_L == EStateFoot.STEP_DOWN)
        {
            var goal = AvatarIKGoal.LeftFoot;
            m_AnimCtrl.SetIKPositionWeight(goal, 1);
            m_AnimCtrl.SetIKRotationWeight(goal, 1);
            //HitCast(AvatarIKGoal.LeftFoot);

            //var defaultPos = m_AnimCtrl.GetIKPosition(goal);
            //
            //m_AnimCtrl.SetIKPosition(goal, m_Foot_IK_L.position);
            //m_AnimCtrl.SetIKRotation(goal, m_Foot_IK_L.rotation);
            //
            //m_Foot_IK_L.position = defaultPos;
        }

        //if (m_eIK_R == EStateFoot.STEP_DOWN)
        {
            var goal = AvatarIKGoal.RightFoot;
            m_AnimCtrl.SetIKPositionWeight(goal, 1);
            m_AnimCtrl.SetIKRotationWeight(goal, 1);
            //HitCast(AvatarIKGoal.RightFoot);

            //var defaultPos = m_AnimCtrl.GetIKPosition(goal);
            //
            //m_AnimCtrl.SetIKPosition(goal, m_Foot_IK_R.position);
            //m_AnimCtrl.SetIKRotation(goal, m_Foot_IK_R.rotation);
            //
            //m_Foot_IK_R.position = defaultPos;
        }
    }

    void HitCast(AvatarIKGoal _goal)
    {
        //레이케스트
        Ray ray = new Ray(m_AnimCtrl.GetIKPosition(_goal) +
            Vector3.up * _maxCheckTop, Vector3.down * _maxCheckDown);
        if (Physics.SphereCast(ray, _footStepRadius,
        //if (Physics.Raycast(ray,
            out RaycastHit hitted, _distanceGround + 1f, m_LandLayer))
        {
            Vector3 footPosition = hitted.point;
            //m_AnimCtrl.SetIKPosition(_goal, footPosition);
            //m_AnimCtrl.SetIKRotation(_goal, Quaternion.LookRotation(this.transform.forward, hitted.normal));

            if (_goal == AvatarIKGoal.LeftFoot)
            {
                m_Helper_IK_L.position = footPosition;
                m_Helper_IK_L.rotation = 
                    Quaternion.LookRotation(this.transform.forward, hitted.normal);
            }
            else
            {
                m_Helper_IK_R.position = footPosition;
                m_Helper_IK_R.rotation =
                    Quaternion.LookRotation(this.transform.forward, hitted.normal);
            }

            //Lerp_IK_Pos(_goal, hitted.point);
            //Lerp_IK_Rot(_goal, hitted.normal);
        }
    }

    void Lerp_IK_Pos(AvatarIKGoal _goal, Vector3 _footPos)
    {
        var currPos = m_AnimCtrl.GetIKPosition(_goal);
        var res = Vector3.Lerp(currPos, _footPos, Time.deltaTime * _footLerpSpeed);
        m_AnimCtrl.SetIKPosition(_goal, res);
    }

    void Lerp_IK_Rot(AvatarIKGoal _goal, Vector3 _footNormal)
    {
        var target = Quaternion.LookRotation(this.transform.forward, _footNormal);
        var currRot = m_AnimCtrl.GetIKRotation(_goal);
        var res = Quaternion.Lerp(currRot, target, Time.deltaTime * _footLerpSpeed);
        m_AnimCtrl.SetIKRotation(_goal, res);
    }

    IEnumerator CoMakeCurve(float _duration) 
    {
        
        float half = _duration / 2f;
        float during = 0f;
        float res = 0;

        while (during < _duration) 
        {
            if (during < half) 
            { res = Mathf.Lerp(res, _stepHeight, during * 2f); }
            else 
            { res = Mathf.Lerp(res, 0, (during - half) * 2f); }


            during += Time.deltaTime;
            yield return null;
        }

        

        yield return null;
    }


    //void Lerp_IK_Pos(AvatarIKGoal _goal, Vector3 _footPos)
    //{
    //    var currPos = m_AnimCtrl.GetIKPosition(_goal);
    //    var res = Vector3.Lerp(currPos, _footPos, Time.deltaTime * _footLerpSpeed);
    //    //m_Foot_IK_L.position = res;
    //    m_AnimCtrl.SetIKPosition(_goal, res);
    //}

    //void Lerp_IK_Rot(AvatarIKGoal _goal, Vector3 _footNormal)
    //{
    //    var target = Quaternion.LookRotation(this.transform.forward, _footNormal);
    //    var currRot = m_AnimCtrl.GetIKRotation(_goal);
    //    var res = Quaternion.Lerp(currRot, target, Time.deltaTime * _footLerpSpeed);
    //    //m_Foot_IK_L.rotation = res;
    //    m_AnimCtrl.SetIKRotation(_goal, res);
    //}



    //for gizmo
    Vector3 GetSphereCast_Res(AvatarIKGoal _goal)
    {
        //레이케스트
        Ray ray = new Ray(m_AnimCtrl.GetIKPosition(_goal) +
            Vector3.up * _maxCheckTop, Vector3.down * _maxCheckDown);

        if (Physics.SphereCast(ray, _footStepRadius,
            out RaycastHit hitted, _distanceGround + 1f, m_LandLayer))
        { return hitted.point; }

        else return Vector3.zero;
    }


    public void StepUpR(float _duration) 
    {
        Debug.Log(_duration);
        m_eIK_R = EStateFoot.STEP_UP;
        StartCoroutine(CoMakeCurve(_duration));
    }

    public void StepDownR() 
    { m_eIK_R = EStateFoot.STEP_DOWN; }

    public void StepUpL(float _duration)
    { 
        m_eIK_L = EStateFoot.STEP_UP;
        StartCoroutine(CoMakeCurve(_duration));
    }

    public void StepDownL()
    { m_eIK_L = EStateFoot.STEP_DOWN; }


    private void OnDrawGizmos()
    {
        Vector3 footL = m_AnimCtrl.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 footR = m_AnimCtrl.GetIKPosition(AvatarIKGoal.RightFoot);

        Gizmos.color = Color.red;

        Gizmos.DrawLine(footL + 
            Vector3.up * _maxCheckTop, footL + Vector3.down * _maxCheckDown);
        Gizmos.DrawLine(footR +
            Vector3.up * _maxCheckTop, footR + Vector3.down * _maxCheckDown);

        //발 올릴 지점에 와이어 스피어 그림
        Gizmos.color = Color.green;
        var footLP = GetSphereCast_Res(AvatarIKGoal.LeftFoot);
        var footRP = GetSphereCast_Res(AvatarIKGoal.RightFoot);
        if (footLP == Vector3.zero) footLP = footL;
        if (footRP == Vector3.zero) footRP = footR;
        Gizmos.DrawWireSphere(footLP, _footStepRadius);
        Gizmos.DrawWireSphere(footRP, _footStepRadius);


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(m_AnimCtrl.GetIKPosition(AvatarIKGoal.LeftFoot), _footStepRadius);

        
    }


    #endregion
}
