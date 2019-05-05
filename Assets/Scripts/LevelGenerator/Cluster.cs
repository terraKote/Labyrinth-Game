using UnityEngine;

namespace Labyrinth.LevelGenerator
{
    [System.Serializable]
    public struct Cluster
    {
        public Transform body;
        public Transform[] entrances;
    }
}
