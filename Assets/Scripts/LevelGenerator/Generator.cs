using UnityEngine;
using System.Collections.Generic;
using Labyrinth.Characters;

namespace Labyrinth.LevelGenerator
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] float m_ChunkSize = 1;
        [SerializeField] Vector2Int m_Size;
        [SerializeField] Vector2Int m_Start, m_Exit;
        [SerializeField] Cluster m_Cluster;

        [SerializeField] CharacterEntity m_Player;
        [SerializeField] CharacterEntity[] m_Enemies;

        private Map m_Map;
        [SerializeField] List<Vector2Int> m_ImpossibleChunks = new List<Vector2Int>();

        private void Start()
        {
            Generate();
        }

        private void Generate()
        {
            m_Map = new Map();
            GenerateRough();
            MakePath();
            Draw();
            SpawnEntities();
        }

        private void GenerateRough()
        {
            Chunk[,] chunks = new Chunk[m_Size.x, m_Size.y];

            if (Random.value > 0.5f)
            {
                m_Exit = new Vector2Int(Random.Range(0, m_Size.x), (int)Random.value);
            }
            else
            {
                m_Exit = new Vector2Int((int)Random.value, Random.Range(0, m_Size.y));
            }

            m_Start = new Vector2Int(m_Size.x / 2, m_Size.y / 2);

            for (int y = 0; y < m_Size.y; y++)
            {
                for (int x = 0; x < m_Size.x; x++)
                {
                    int[] type = new int[] { Random.Range(0, 4) };
                    Vector2 position = new Vector2(x * m_ChunkSize, y * m_ChunkSize);

                    bool start = x == m_Start.x && y == m_Start.y;
                    bool exit = x == m_Exit.x && y == m_Exit.y;

                    Chunk chunk = new Chunk(y + x, new Vector2(position.x, position.y), type, exit, start, false, new Chunk[0]);

                    if (start)
                    {
                        chunk.type[0] = -1;
                        m_Map.start = chunk;
                    }

                    if (exit)
                    {
                        GameManager.instance.exit = chunk.position;
                    }

                    chunks[x, y] = chunk;
                }
            }

            m_Map.chunks = chunks;
        }

        private void MakePath()
        {
            Vector2Int start = m_Exit;
            int trust = (m_Size.x * m_Size.y) / 2;

            int iteration = 0;

            List<Chunk> path = new List<Chunk>();

            bool connected = false;

            while (!connected)
            {
                Vector2Int nextChunkDirection = Vector2Int.zero;

                int direction = Random.Range(0, 4);

                if (trust <= 0)
                {
                    if (start.x > m_Start.x)
                    {
                        direction = 2;
                    }
                    else if (start.x < m_Start.x)
                    {
                        direction = 0;
                    }
                    else
                    {
                        if (start.y > m_Start.y)
                        {
                            direction = 3;
                        }
                        else
                        {
                            direction = 1;
                        }
                    }

                    trust++;
                }

                switch (direction)
                {
                    case 0:
                        nextChunkDirection = new Vector2Int(1, 0);
                        break;

                    case 1:
                        nextChunkDirection = new Vector2Int(0, 1);
                        break;

                    case 2:
                        nextChunkDirection = new Vector2Int(-1, 0);
                        break;

                    case 3:
                        nextChunkDirection = new Vector2Int(0, -1);
                        break;
                }

                Vector2Int nextChunk = start + nextChunkDirection;

                if (!m_ImpossibleChunks.Contains(nextChunk))
                {
                    if (ChunkExists(nextChunk))
                    {
                        Chunk startChunk = m_Map.chunks[start.x, start.y];

                        List<Chunk> neighbours = new List<Chunk>();
                        neighbours.AddRange(startChunk.neighbours);
                        neighbours.Add(m_Map.chunks[nextChunk.x, nextChunk.y]);
                        startChunk.neighbours = neighbours.ToArray();
                        m_Map.chunks[start.x, start.y] = startChunk;


                        Chunk chunk = m_Map.chunks[nextChunk.x, nextChunk.y];
                        neighbours.Clear();
                        neighbours.AddRange(chunk.neighbours);
                        neighbours.Add(m_Map.chunks[start.x, start.y]);
                        chunk.neighbours = neighbours.ToArray();
                        chunk.path = true;
                        m_Map.chunks[nextChunk.x, nextChunk.y] = chunk;

                        start = nextChunk;

                        trust++;
                    }
                    else
                    {
                        m_ImpossibleChunks.Add(nextChunk);
                        trust--;
                    }
                }

                iteration++;


                if (nextChunk == m_Start)
                {
                    connected = true;
                }
            }
        }

        private void Draw()
        {
            for (int y = 0; y < m_Map.chunks.GetLength(1); y++)
            {
                for (int x = 0; x < m_Map.chunks.GetLength(0); x++)
                {
                    Chunk chunk = m_Map.chunks[x, y];

                    bool up = false;
                    bool down = false;
                    bool left = false;
                    bool right = false;

                    for (int i = 0; i < m_Cluster.entrances.Length; i++)
                    {
                        m_Cluster.entrances[i].gameObject.SetActive(true);
                    }


                    if (chunk.neighbours.Length > 0)
                    {
                        foreach (Chunk neighbour in chunk.neighbours)
                        {
                            if (neighbour.position.y > chunk.position.y)
                                up = true;

                            if (neighbour.position.y < chunk.position.y)
                                down = true;

                            if (neighbour.position.x > chunk.position.x)
                                right = true;

                            if (neighbour.position.x < chunk.position.x)
                                left = true;
                        }
                    }

                    if (right)
                    {
                        m_Cluster.entrances[0].gameObject.SetActive(false);
                    }

                    if (up)
                    {
                        m_Cluster.entrances[1].gameObject.SetActive(false);
                    }

                    if (left)
                    {
                        m_Cluster.entrances[2].gameObject.SetActive(false);
                    }

                    if (down)
                    {
                        m_Cluster.entrances[3].gameObject.SetActive(false);
                    }

                    if (chunk.start || chunk.exit)
                    {
                        for (int i = 0; i < m_Cluster.entrances.Length; i++)
                        {
                            m_Cluster.entrances[i].gameObject.SetActive(false);
                        }
                    }

                    Transform cluster = Instantiate(m_Cluster.body, new Vector3(chunk.position.x, chunk.position.y), m_Cluster.body.rotation);
                }
            }
        }

        private void SpawnEntities()
        {
            List<CharacterEntity> entities = new List<CharacterEntity>();
            m_Player.owner.transform.position = m_Map.start.position;
            entities.Add(m_Player);

            int enemyCount = 50;

            for (int y = 0; y < m_Map.chunks.GetLength(1); y++)
            {
                for (int x = 0; x < m_Map.chunks.GetLength(0); x++)
                {
                    Chunk chunk = m_Map.chunks[x, y];

                    if (chunk.neighbours.Length > 0)
                    {
                        switch (chunk.type[0])
                        {
                            case 0:
                                if (enemyCount > 0)
                                {
                                    CharacterEntity enemy = new CharacterEntity(Instantiate(m_Enemies[0].owner), m_Enemies[0].health, m_Enemies[0].maxHealth, m_Enemies[0].damage, m_Enemies[0].speed, m_Enemies[0].level, m_Enemies[0].userControlled);
                                    enemy.owner.transform.position = chunk.position;
                                    entities.Add(enemy);
                                    enemyCount--;
                                }
                                break;
                        }
                    }
                }

                CharacterControllManager.instance.entities = entities.ToArray();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (m_Map.chunks != null)
            {
                for (int y = 0; y < m_Map.chunks.GetLength(1); y++)
                {
                    for (int x = 0; x < m_Map.chunks.GetLength(0); x++)
                    {
                        Chunk chunk = m_Map.chunks[x, y];

                        if (chunk.path)
                        {
                            Gizmos.color = Color.white;
                            Gizmos.DrawCube(chunk.position, Vector3.one);
                        }

                        switch (chunk.type[0])
                        {
                            case 1:
                                Gizmos.color = Color.red;
                                Gizmos.DrawLine(chunk.position, chunk.position + new Vector2(1, 0));
                                break;

                            case 2:
                                Gizmos.color = Color.green;
                                Gizmos.DrawLine(chunk.position, chunk.position + new Vector2(0, 1));
                                break;

                            case 3:
                                Gizmos.color = Color.blue;
                                Gizmos.DrawLine(chunk.position, chunk.position + new Vector2(-1, 0));
                                break;

                            case 4:
                                Gizmos.color = Color.yellow;
                                Gizmos.DrawLine(chunk.position, chunk.position + new Vector2(0, -1));
                                break;

                            default:
                                Gizmos.color = Color.black;
                                break;
                        }

                        if (chunk.exit)
                        {
                            Gizmos.color = Color.magenta;
                            Gizmos.DrawCube(chunk.position, Vector3.one);
                        }

                        if (chunk.start)
                        {
                            Gizmos.color = Color.cyan;
                            Gizmos.DrawCube(chunk.position, Vector3.one);
                        }

                        if (chunk.neighbours != null)
                        {
                            foreach (Chunk neighbour in chunk.neighbours)
                            {
                                Gizmos.color = new Color(0, 1, 1);
                                Gizmos.DrawLine(chunk.position, neighbour.position);
                            }
                        }

                        Gizmos.DrawWireCube(chunk.position, Vector3.one);
                    }
                }
            }
            else
            {
                for (int y = 0; y < m_Size.y; y++)
                {
                    for (int x = 0; x < m_Size.x; x++)
                    {
                        Gizmos.DrawWireCube(new Vector3(x * m_ChunkSize, y * m_ChunkSize), Vector3.one * m_ChunkSize);
                    }
                }
            }
        }

        private bool ChunkExists(Vector2Int index)
        {
            bool xFound = index.x < m_Map.chunks.GetLength(0) && index.x >= 0;
            bool yFound = index.y < m_Map.chunks.GetLength(1) && index.y >= 0;

            return xFound && yFound;
        }
    }
}
