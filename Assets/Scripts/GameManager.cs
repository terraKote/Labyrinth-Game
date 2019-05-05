using UnityEngine;
using UnityEngine.UI;
using System;

namespace Labyrinth
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Transform m_Player;
        [SerializeField] Text m_Caption;
        [SerializeField] GameObject m_StatsPanel;
        [SerializeField] Text m_TimeCaption;
        [SerializeField] Text m_DistanceCaption;
        [SerializeField] ParticleSystem m_Confetti;

        private Vector2 m_PreviousPosition;
        private float m_Distance;

        private float m_Time;

        private bool m_TrackDistance = false;
        private bool m_TrackTime = true;
        private bool m_LevelCompleted = false;
        [SerializeField] Vector2 m_Exit;

        private static GameManager m_Instance;

        public bool levelCompleted { get => m_LevelCompleted; }
        public Vector2 exit { get => m_Exit; set => m_Exit = value; }
        public static GameManager instance { get => m_Instance; }

        private void Awake()
        {
            m_Instance = this;

            m_StatsPanel.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            m_PreviousPosition = m_Player.position;
        }

        private void Update()
        {
            if (m_LevelCompleted)
            {
                m_StatsPanel.gameObject.SetActive(true);

                m_TimeCaption.text = string.Format("Time: {0}", TimeSpan.FromSeconds(m_Time).ToString(@"mm\:ss"));
                m_DistanceCaption.text = string.Format("Distance: {0}", m_Distance);

                return;
            }

            Collider2D collider = Physics2D.OverlapCircle(m_Exit, 0.4f);
            if (collider && collider.tag == "Player")
            {
                m_LevelCompleted = true;
                m_Confetti.transform.position = (Vector3)m_Exit + new Vector3(0, 0, -5);
                m_Confetti.Play();
            }

            if (m_TrackTime)
                m_Time += Time.deltaTime;

            m_Caption.text = string.Format("{0}{1}{2}", TimeSpan.FromSeconds(m_Time).ToString(@"mm\:ss"), Environment.NewLine, m_Distance.ToString("F1"));
        }

        public void TrackDistance()
        {
            if (m_LevelCompleted)
                return;

            if (!m_TrackDistance)
            {
                m_PreviousPosition = m_Player.position;
                m_TrackDistance = true;
            }

            m_Distance += Vector2.Distance(m_Player.position, m_PreviousPosition);
            m_PreviousPosition = m_Player.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(m_Exit, 0.4f);
        }
    }
}
