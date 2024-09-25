using UnityEngine;

public class CAnimFunc : MonoBehaviour
{

    public System.Action m_Stop_Input = null;
    public System.Action m_Resume_Input = null;

    public System.Action m_EndAct = null;
    public System.Action m_StartATK = null;
    public System.Action m_EndATK = null;

    public System.Action m_StartComboCheck = null;
    public System.Action m_EndComboCheck = null;

    public System.Action m_StartDodge = null;
    public System.Action m_EndDodge = null;

    public System.Action m_RootMotion = null;


    public void Stop_Input() 
    { if (m_Stop_Input != null) m_Stop_Input(); }
    public void Resume_Input() 
    { if (m_Resume_Input != null) m_Resume_Input(); }


    public void EndAct()
    { if (m_EndAct != null) m_EndAct(); }
    public void StartATK() 
    { if (m_StartATK != null) m_StartATK(); }
    public void EndATK()
    { if (m_EndATK != null) m_EndATK(); }

    public void StartComboCheck()
    { if (m_StartComboCheck != null) m_StartComboCheck(); }
    public void EndComboCheck() 
    { if (m_EndComboCheck != null) m_EndComboCheck(); }

    public void StartDodge()
    { if (m_StartDodge != null) m_StartDodge(); }
    public void EndDodge()
    { if (m_EndDodge != null) m_EndDodge(); }


    private void OnAnimatorMove()
    { if(m_RootMotion != null) m_RootMotion(); }


    [SerializeField] LayerMask m_LandLayer;
    [SerializeField] float distanceGround = 0;
    [SerializeField] Animator m_AnimCtrl = null;

    private void OnAnimatorIK(int layerIndex)
    {
        
        m_AnimCtrl.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        m_AnimCtrl.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
        HitCast(AvatarIKGoal.LeftFoot);

        m_AnimCtrl.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        m_AnimCtrl.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        HitCast(AvatarIKGoal.RightFoot);
    }

    
    void HitCast(AvatarIKGoal _goal)
    {
        Vector3 res = Vector3.zero;
        Vector3 pos = m_AnimCtrl.GetIKPosition(_goal);
        

        Ray ray = new Ray(pos + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitted, distanceGround + 1f, m_LandLayer))
        {
            Vector3 footPosition = hitted.point;

            res = this.transform.position - footPosition;


            //footPosition.y += temp.y;
            //footPosition.y += distanceGround;
            //footPosition.y += distanceGround;

            m_AnimCtrl.SetIKPosition(_goal, footPosition);

            //수정필요. 발목꺾기
            m_AnimCtrl.SetIKRotation(_goal, Quaternion.LookRotation(this.transform.forward, hitted.normal));
            
            // 걸을 수 있는 땅이라면
            if (hitted.transform.tag == "WalkableGround")
            {     }
        }
    }

    //void HitCast(AvatarIKGoal _goal)
    //{
    //    Vector3 pos = m_AnimCtrl.GetIKPosition(_goal);


    //    Ray ray = new Ray(pos + Vector3.up, Vector3.down);
    //    if (Physics.Raycast(ray, out RaycastHit hitted, distanceGround + 1f, m_LandLayer))
    //    {
    //        Vector3 footPosition = hitted.point;

    //        temp = this.transform.position - footPosition;
    //        //footPosition.y += temp.y;
    //        //footPosition.y += distanceGround;
    //        //footPosition.y += distanceGround;

    //        m_AnimCtrl.SetIKPosition(_goal, footPosition);

    //        //수정필요
    //        m_AnimCtrl.SetIKRotation(_goal, Quaternion.LookRotation(this.transform.forward, hitted.normal));

    //        // 걸을 수 있는 땅이라면
    //        if (hitted.transform.tag == "WalkableGround")
    //        { }
    //    }
    //}

    public void Step() 
    {
        Debug.Log("ON STEP");
    }


    private void OnDrawGizmos()
    {
        Vector3 footL = m_AnimCtrl.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 footR = m_AnimCtrl.GetIKPosition(AvatarIKGoal.RightFoot);

        Gizmos.color = Color.red;

        Gizmos.DrawLine(footL + Vector3.up, footL + Vector3.down);
        Gizmos.DrawLine(footR + Vector3.up, footR + Vector3.down);

        Gizmos.color = Color.white;
    }
}
