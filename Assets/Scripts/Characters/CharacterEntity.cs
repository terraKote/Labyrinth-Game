using UnityEngine;

namespace Labyrinth.Characters
{
    [System.Serializable]
    public struct CharacterEntity
    {
        public GameObject owner;
        public int health;
        public int maxHealth;
        public int damage;
        public float speed;
        public int level;
        public bool userControlled;
        public int direction;
        public bool damaged;

        public CharacterEntity(GameObject owner, int health, int maxHealth, int damage, float speed, int level, bool userControlled)
        {
            this.owner = owner;
            this.health = health;
            this.maxHealth = maxHealth;
            this.damage = damage;
            this.speed = speed;
            this.level = level;
            this.userControlled = userControlled;
            this.direction = 0;
            this.damaged = false;
        }

        public GameObject entity { get => owner; }
        public Animator animator { get => owner.GetComponent<Animator>(); }
        public Rigidbody2D rigidbody { get => owner.GetComponent<Rigidbody2D>(); }
        public SpriteRenderer renderer { get => owner.GetComponent<SpriteRenderer>(); }
    }
}
