using UnityEngine;
using System.Collections;

namespace Labyrinth.Items
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] Bullet m_Bullet;
        [SerializeField] Transform m_BulletCursor;
        [SerializeField] ParticleSystem m_Puff;

        private float m_BulletDirection;
        private static ItemManager m_Instance;

        public static ItemManager instance { get => m_Instance; }

        private void Awake()
        {
            m_Instance = this;
        }

        public void ProcessAimCursor(Vector3 position)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5.23f;

            Vector3 objectPos = Camera.main.WorldToScreenPoint(m_BulletCursor.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;

            m_BulletDirection = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            m_BulletCursor.rotation = Quaternion.Euler(new Vector3(0, 0, m_BulletDirection));

            m_BulletCursor.transform.position = position;

            SpriteRenderer renderer = m_BulletCursor.GetChild(0).GetComponent<SpriteRenderer>();
            Color color = renderer.color;
            if (m_Bullet.launched)
            {
                color.a = 0.5f;
            }
            else
            {
                color.a = 1;
            }
            renderer.color = color;
        }

        public void Shoot()
        {
            if (!m_Bullet.launched)
            {
                m_Bullet.body.gameObject.SetActive(true);
                m_Bullet.body.transform.position = m_BulletCursor.GetChild(0).position;
                StartCoroutine(MoveBullet(m_BulletCursor.GetChild(0).position - m_BulletCursor.position, m_BulletCursor.rotation));
                m_Bullet.launched = true;
            }
        }

        private IEnumerator MoveBullet(Vector3 direction, Quaternion rotation)
        {
            Vector2 velocity = direction * m_Bullet.speed;
            m_Bullet.body.rotation = rotation;
            m_Bullet.body.GetComponent<Rigidbody2D>().velocity = velocity;
            yield return new WaitForEndOfFrame();
            Collider2D collider = Physics2D.OverlapCircle(m_Bullet.body.position, 0.05f);

            if (collider && collider.gameObject != m_Bullet.body.gameObject)
            {
                m_Bullet.body.gameObject.SetActive(false);
                m_Bullet.launched = false;
                Characters.CharacterControllManager.instance.Hit(collider.gameObject);
                m_Puff.transform.position = collider.transform.position + new Vector3(0, 0, -5);
                m_Puff.Play();
            }

            if (m_Bullet.launched)
            {
                StartCoroutine(MoveBullet(direction, rotation));
            }
        }
    }
}
