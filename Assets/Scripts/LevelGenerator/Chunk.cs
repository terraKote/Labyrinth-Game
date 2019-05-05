using UnityEngine;
using System.Collections.Generic;

namespace Labyrinth.LevelGenerator
{
    [System.Serializable]
    public struct Chunk
    {
        public int id;
        public Vector2 position;
        public int[] type;
        public bool exit;
        public bool start;
        public bool path;
        public Chunk[] neighbours;

        public Chunk(int id, Vector2 position, int[] type, bool exit, bool start, bool path, Chunk[] neighbours)
        {
            this.id = id;
            this.position = position;
            this.type = type;
            this.exit = exit;
            this.start = start;
            this.path = path;
            this.neighbours = neighbours;
        }
    }
}
