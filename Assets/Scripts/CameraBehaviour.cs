using UnityEngine;

namespace Labyrinth
{
    public class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] Transform m_Target;
        [SerializeField] float m_Speed;

        private void Update()
        {
            if (m_Target)
            {
                transform.position = Vector3.Lerp(transform.position, m_Target.position + new Vector3(0, 0, -10), m_Speed);
            }
        }
    }
}
