using UnityEngine;

public class CLandCheck : MonoBehaviour
{
    [SerializeField] LayerMask m_LandLayer;
    public bool m_IsLand = false;

    private void OnTriggerEnter(Collider other)
    {
        m_IsLand = true;
    }

    private void OnTriggerExit(Collider other)
    {
        m_IsLand = false;
    }
}
