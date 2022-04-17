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

        public Map(int chunkSize = 20)
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
            if (ChunkExists(vertexChunkPos1) && ChunkExists(vertexChunkPos2))
            {
                combined.AddGraph(_chunkMap[vertexChunkPos1]);
                if (!vertexChunkPos1.Equals(vertexChunkPos2))
                {
                    combined.AddGraph(_chunkMap[vertexChunkPos2]);
                }

                combined.AddEdge(edge);
            }
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

        public Dictionary<Node<T>, HashSet<Edge<T>>> GetVertexesInRange(Node<T> vertex, int range)
        {
            Vec3 vertexChunkPos = ConvertPositionToChunkPosition(vertex.GetContent.GetPosition());
            Graph<T> combined = CombineNeighbours(range, vertexChunkPos);
            return combined.GetVertexes();
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

        private Graph<T> CombineNeighbours(int range, Vec3 tempChunkPos)
        {
            Vec3 convertedChunkPos = ConvertPositionToChunkPosition(tempChunkPos);
            if (range == 0)
            {
                if (!ChunkExists(convertedChunkPos))
                {
                    CreateChunk(convertedChunkPos);
                    Debug.Log(tempChunkPos);
                }
                return _chunkMap[convertedChunkPos];
            }
            
            Graph<T> combined = new Graph<T>();
            for (int z = -range; z <= range; z++)
            {
                for (int x = -range; x <= range; x++)
                {
                    Vec3 tempPos = new Vec3(tempChunkPos.X + (ChunkSize * x),
                        tempChunkPos.Y, tempChunkPos.Z + (ChunkSize * z));
                    combined.AddGraph(GetChunk(tempPos));
                }
            }
            return combined;
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
            if (ChunkExists(converted))
            {
                return this._chunkMap[converted];
            }

            return new Graph<T>();
        }

        public int NumberOfChunks()
        {
            return this._chunkMap.Count;
        }

        public void CreateChunk(Vec3 pos)
        {
            Vec3 chunkPos = ConvertPositionToChunkPosition(pos);
            for (int z = -1; z <= 1; z++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vec3 tempPos = new Vec3(chunkPos.X + (ChunkSize * x),
                        chunkPos.Y, chunkPos.Z + (ChunkSize * z));
                    if (!ChunkExists(tempPos))
                    {
                        this._chunkMap.Add(tempPos, new Graph<T>());
                    }
                }
            }
        }

        protected bool ChunkExists(Vec3 pos)
        {
            return _chunkMap.ContainsKey(pos);
        }
    }
}