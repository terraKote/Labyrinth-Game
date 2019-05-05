using UnityEngine;
using System.Collections;

namespace Labyrinth.Characters
{
    public class CharacterControllManager : MonoBehaviour
    {
        [SerializeField] CharacterEntity[] m_Entities;
        [SerializeField] float m_RecoveryTime = 0.5f;
        [SerializeField] float m_BlinkFrequency = 24f;
        [SerializeField] float m_BlinkTimer = 0;
        [SerializeField] Gradient m_Blink;
        [SerializeField] Transform m_BulletsOrigin;
        [SerializeField] Transform m_Bullet;

        public static CharacterControllManager instance { get => m_Instance; }
        public CharacterEntity[] entities { get => m_Entities; set => m_Entities = value; }

        private static CharacterControllManager m_Instance;

        private void Awake()
        {
            m_Instance = this;
        }

        private void Update()
        {
            if (GameManager.instance.levelCompleted)
                return;

            for (int i = 0; i < m_Entities.Length; i++)
            {
                if (m_Entities[i].health > 0)
                {
                    Vector2 input = Vector2.zero;
                    if (m_Entities[i].userControlled)
                    {
                        GameManager.instance.TrackDistance();

                        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                        Items.ItemManager.instance.ProcessAimCursor(m_Entities[0].owner.transform.position);

                        bool shoot = Input.GetButton("Fire1");

                        if (shoot)
                        {
                            Items.ItemManager.instance.Shoot();
                        }
                    }
                    else
                    {
                        switch (m_Entities[i].direction)
                        {
                            case 0:
                                input = new Vector2(1, 0);
                                break;

                            case 1:
                                input = new Vector2(0, 1);
                                break;

                            case 2:
                                input = new Vector2(-1, 0);
                                break;

                            case 3:
                                input = new Vector2(0, -1);
                                break;
                        }

                        Collider2D collider = Physics2D.OverlapCircle((Vector2)m_Entities[i].owner.transform.position + input * 0.7f, 0.2f);

                        if (collider && collider.gameObject != m_Entities[i].owner)
                        {
                            m_Entities[i].direction = Random.Range(0, 4);

                            if (collider.tag == "Player")
                            {
                                StartCoroutine(Blink(0));
                                StartCoroutine(Recover(0));
                            }
                        }
                    }

                    m_Entities[i].rigidbody.velocity = input * m_Entities[i].speed;
                    m_Entities[i].animator.speed = input.magnitude;

                    Flip(m_Entities[i].owner.transform, input);
                }
            }
        }

        private void Flip(Transform transform, Vector2 axis)
        {
            Vector2 scale = transform.localScale;

            if (axis.x != 0)
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(axis.x);

            transform.localScale = scale;
        }

        private IEnumerator Blink(int index)
        {
            m_BlinkTimer = Mathf.PingPong(m_BlinkFrequency * Time.time, 1);
            m_Entities[index].renderer.color = m_Blink.Evaluate(m_BlinkTimer);

            if (!m_Entities[index].damaged)
            {
                m_Entities[index].health--;
                m_Entities[index].damaged = true;
            }

            if (m_Entities[index].health <= 0)
            {
                if (index == 0)
                {

                }
                else
                {
                    m_Entities[index].owner.SetActive(false);
                }
            }

            yield return new WaitForEndOfFrame();

            if (m_Entities[index].damaged)
                StartCoroutine(Blink(index));
        }

        private IEnumerator Recover(int index)
        {
            yield return new WaitForSeconds(m_RecoveryTime);
            m_Entities[index].renderer.color = Color.white;
            m_Entities[index].damaged = false;
        }

        public void Hit(GameObject owner)
        {
            for (int i = 1; i < m_Entities.Length; i++)
            {
                if (m_Entities[i].owner == owner)
                {
                    StartCoroutine(Blink(i));
                    StartCoroutine(Recover(i));
                    break;
                }
            }
        }
    }
}
