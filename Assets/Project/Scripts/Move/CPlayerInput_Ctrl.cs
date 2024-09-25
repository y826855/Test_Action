using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class CPlayerInput_Ctrl : MonoBehaviour
{
    public Animator m_AnimCtrl = null;
    public CAnimFunc m_AnimFunc = null;

    //awsd ภิทย รเ
    [Header("===========Dash===============")]
    public Vector2 m_Move = Vector2.zero;
    public Vector2 m_Look = Vector2.zero;

    //InputVector Press AWSD
    public void OnMove(InputAction.CallbackContext context)
    { m_Move = context.ReadValue<Vector2>(); }

    //InputVector Move MOUSE
    public void OnLook(InputAction.CallbackContext context)
    { m_Look = context.ReadValue<Vector2>(); }
   
}
