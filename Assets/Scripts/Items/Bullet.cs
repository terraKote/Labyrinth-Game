using UnityEngine;

namespace Labyrinth.Items
{
    [System.Serializable]
    public struct Bullet
    {
        public Transform body;
        public bool launched;
        public float speed;
    }
}
