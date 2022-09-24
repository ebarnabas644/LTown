using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes.Graph;
using DataTypes.Graph.Assets.Scripts.Graph;
using UnityEngine;

namespace DataTypes.Map
{
    public class Map<T> where T : ILocatable
    {
        protected Dictionary<Vec3, Graph<T>> _chunkMap;
        public int ChunkSize { get; }
        public int CombineRange { get; }

        public Map(int chunkSize = 100)
        {
            this.ChunkSize = chunkSize;
            this._chunkMap = new Dictionary<Vec3, Graph<T>>();
            this.CombineRange = 1;
        }

        public void AddVertex(Node<T> vertex)
        {
            Vec3 chunkPos = ConvertPositionToChunkPosition(vertex.GetContent.GetPosition());
            if (!ChunkExists(chunkPos))
            {
                CreateChunk(chunkPos);
            }
            
            _chunkMap[chunkPos].AddVertex(vertex);
        }

        public void AddEdge(Edge<T> edge)
        {
            Vec3 vertexChunkPos1 = ConvertPositionToChunkPosition(edge.Start.GetContent.GetPosition());
            Vec3 vertexChunkPos2 = ConvertPositionToChunkPosition(edge.End.GetContent.GetPosition());
            Graph<T> combined = new Graph<T>();
            combined.AddGraph(_chunkMap[vertexChunkPos1]);
            if (!vertexChunkPos1.Equals(vertexChunkPos2))
            {
                combined.AddGraph(_chunkMap[vertexChunkPos2]);
            }

            combined.AddEdge(edge);
        }

        public Dictionary<Vec3, Graph<T>> GetChunks()
        {
            return _chunkMap;
        }

        public List<Edge<T>> GetVertexEdges(Node<T> vertex)
        {
            Vec3 vertexChunkPos = ConvertPositionToChunkPosition(vertex.GetContent.GetPosition());
            return _chunkMap[vertexChunkPos].GetVertexEdges(vertex);
        }

        public List<Graph<T>> GetNeighbourChunksInSnapRange(Node<T> vertex, float snapRange)
        {
            Vec3 vertexPos = vertex.GetContent.GetPosition();
            List<Graph<T>> combined = CombineNeighbours(vertexPos, snapRange);
            return combined;
        }

        public List<Graph<T>> GetNeighbourChunksInRange(Node<T> vertex, int neighbourRange)
        {
            List<Graph<T>> combined = new List<Graph<T>>();
            Vec3 vertexChunkPos = ConvertPositionToChunkPosition(vertex.GetContent.GetPosition());
            for (int z = -neighbourRange; z <= neighbourRange; z++)
            {
                for (int x = -neighbourRange; x <= neighbourRange; x++)
                {
                    Vec3 tempPos = new Vec3(vertexChunkPos.X + (ChunkSize * x),
                        vertexChunkPos.Y, vertexChunkPos.Z + (ChunkSize * z));
                    combined.Add(GetChunk(tempPos));
                }
            }

            return combined;
        }

        public Graph<T> CombineChunks(List<Graph<T>> chunks)
        {
            Graph<T> combined = new Graph<T>();
            foreach (var chunk in chunks)
            {
                combined.AddGraph(chunk);
            }

            return combined;
        }

        public Dictionary<Node<T>, HashSet<Edge<T>>> GetVertexes()
        {
            Dictionary<Node<T>, HashSet<Edge<T>>> combined = new Dictionary<Node<T>, HashSet<Edge<T>>>();
            foreach (var chunk in _chunkMap)
            {
                chunk.Value.GetVertexes().ToList().ForEach(x => combined.Add(x.Key, x.Value));
            }

            return combined;
        }

        public List<Edge<T>> GetEdges()
        {
            HashSet<Edge<T>> edges = new HashSet<Edge<T>>();
            foreach (var chunk in _chunkMap)
            {
                foreach (var edge in chunk.Value.GetEdges())
                {
                    edges.Add(edge);
                }
            }

            return edges.ToList();
        }

        private List<Graph<T>> CombineNeighbours(Vec3 vertexPos, float snapRange)
        {
            List<Graph<T>> graphlist = new List<Graph<T>>();
            bool[] borderList = FillBorderList(vertexPos, snapRange);
            Vec3 convertedChunkPos = ConvertPositionToChunkPosition(vertexPos);
            if (CheckIfNearChunkBorder(borderList))
            {
                if (CheckIfAll(borderList))
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            Vec3 tempPos = new Vec3(convertedChunkPos.X + (ChunkSize * x),
                                convertedChunkPos.Y, convertedChunkPos.Z + (ChunkSize * z));
                            graphlist.Add(GetChunk(tempPos));
                        }
                    }
                }
                else
                {
                    if (CheckTopRight(borderList))
                    {
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X + ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z)));
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X + ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z - ChunkSize)));
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X, convertedChunkPos.Y,
                            convertedChunkPos.Z - ChunkSize)));
                    }
                    else if (CheckBottomRight(borderList))
                    {
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X, convertedChunkPos.Y,
                            convertedChunkPos.Z - ChunkSize)));
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X - ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z - ChunkSize)));
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X - ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z)));
                    }
                    else if (CheckBottomLeft(borderList))
                    {
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X - ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z)));
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X - ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z + ChunkSize)));
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X, convertedChunkPos.Y,
                            convertedChunkPos.Z + ChunkSize)));
                    }
                    else if (CheckTopLeft(borderList))
                    {
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X, convertedChunkPos.Y,
                            convertedChunkPos.Z + ChunkSize)));
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X + ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z + ChunkSize)));
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X + ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z)));
                    }
                    else if (CheckTop(borderList))
                    {
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X + ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z)));
                    }
                    else if (CheckRight(borderList))
                    {
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X, convertedChunkPos.Y,
                            convertedChunkPos.Z - ChunkSize)));
                    }
                    else if (CheckBottom(borderList))
                    {
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X - ChunkSize, convertedChunkPos.Y,
                            convertedChunkPos.Z)));
                    }
                    else
                    {
                        graphlist.Add(GetChunk(new Vec3(convertedChunkPos.X, convertedChunkPos.Y,
                            convertedChunkPos.Z + ChunkSize)));
                    }

                    graphlist.Add(GetChunk(convertedChunkPos));
                }
            }
            else
            {
                graphlist.Add(GetChunk(convertedChunkPos));
            }

            return graphlist;
        }

        public Vec3 ConvertPositionToChunkPosition(Vec3 pos)
        {
            float convertedX = (float)Math.Floor(pos.X / ChunkSize) * ChunkSize;
            float convertedY = (float)Math.Floor(pos.Y / ChunkSize) * ChunkSize;
            float convertedZ = (float)Math.Floor(pos.Z / ChunkSize) * ChunkSize;
            return new Vec3(convertedX, convertedY, convertedZ);
        }

        public Graph<T> GetChunk(Vec3 chunkPos)
        {
            Vec3 converted = ConvertPositionToChunkPosition(chunkPos);
            if (!ChunkExists(converted))
            {
                CreateChunk(converted);
            }
            
            return this._chunkMap[converted];
        }

        public int NumberOfChunks()
        {
            return this._chunkMap.Count;
        }

        public void CreateChunk(Vec3 pos)
        {
            Vec3 chunkPos = ConvertPositionToChunkPosition(pos);
            this._chunkMap.Add(chunkPos, new Graph<T>());
        }

        protected bool ChunkExists(Vec3 pos)
        {
            return _chunkMap.ContainsKey(pos);
        }
        
        private bool[] FillBorderList(Vec3 vertexPos, float snapRange)
        {
            bool[] borderList = new bool[6];
            borderList[0] = Mod((int)vertexPos.X, this.ChunkSize) < snapRange; // bottom side
            borderList[1] = Mod((int)vertexPos.Z, this.ChunkSize) < snapRange; // right side
            borderList[2] = Mod((int)vertexPos.X, this.ChunkSize) > (this.ChunkSize - snapRange); // top side
            borderList[3] = Mod((int)vertexPos.Z, this.ChunkSize) > (this.ChunkSize - snapRange); // left side
            if (borderList.Contains(true))
            {
                borderList[4] = true;
            }
            
            int cnt = 0;
            foreach (var border in borderList)
            {
                if (border)
                {
                    cnt++;
                }
            }

            if (cnt >= 3)
            {
                borderList[5] = true;
            }
            
            return borderList;
        }
        
        private bool CheckTopLeft(bool[] borderList)
        {
            return CheckLeft(borderList) && CheckTop(borderList);
        }

        private bool CheckBottomLeft(bool[] borderList)
        {
            return CheckLeft(borderList) && CheckBottom(borderList);
        }

        private bool CheckTopRight(bool[] borderList)
        {
            return CheckRight(borderList) && CheckTop(borderList);
        }

        private bool CheckBottomRight(bool[] borderList)
        {
            return CheckRight(borderList) && CheckBottom(borderList);
        }

        private bool CheckTop(bool[] borderList)
        {
            return borderList[2];
        }

        private bool CheckRight(bool[] borderList)
        {
            return borderList[1];
        }

        private bool CheckBottom(bool[] borderList)
        {
            return borderList[0];
        }

        private bool CheckLeft(bool[] borderList)
        {
            return borderList[3];
        }

        private bool CheckIfNearChunkBorder(bool[] borderList)
        {
            return borderList[4];
        }

        private bool CheckIfAll(bool[] borderList)
        {
            return borderList[5];
        }
        
        //https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
        private int Mod(int x, int m) {
            int r = x%m;
            return r<0 ? r+m : r;
        }
    }
}