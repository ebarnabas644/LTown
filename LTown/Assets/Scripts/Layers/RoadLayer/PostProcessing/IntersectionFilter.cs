using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using DataTypes.Graph;
using DataTypes.Graph.Assets.Scripts.Graph;
using DataTypes.Map;

namespace Layers.RoadLayer.PostProcessing
{
    //https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
    public class IntersectionFilter<T> where T : ILocatable
    {
        public IntersectionFilter()
        {

        }

        public Map<T> Filtering(Map<T> data)
        {
            foreach (var chunk in data.GetChunks().ToList())
            {
                List<Edge<T>> chunkEdges = chunk.Value.GetEdges();
                foreach (var edge1 in chunkEdges)
                {
                    Graph<T> combined = data.CombineChunks(data.GetNeighbourChunksInRange(edge1.Start, 1));
                    foreach (var edge2 in combined.GetEdges())
                    {
                        if (!edge2.Start.Equals(edge1.Start) && !edge2.End.Equals(edge1.Start) &&
                            !edge2.Start.Equals(edge1.End) && !edge2.End.Equals(edge1.End))
                        {
                            if (!edge2.Equals(edge1) && DoIntersect(edge1.Start, edge1.End, edge2.Start, edge2.End))
                            {
                                var start = edge2.Start;
                                var end = edge2.End;
                                combined.RemoveEdge(edge2.Start, edge2.End, edge2);
                            }
                        }
                    }
                }
            }

            return data;
        }

        public Map<T> FilterIntersectionWithoutEdge(Map<T> data)
        {
            var chunks = data.GetChunks();
            foreach (var chunk in chunks)
            {
                var vertexes = chunk.Value.GetVertexes();
                var values = vertexes.Keys.ToList();
                for (int i = 0; i < values.Count; i++)
                {
                    if (vertexes[values[i]].Count == 0)
                    {
                        vertexes.Remove(values[i]);
                    }
                }
            }
            

            return data;
        }

        private bool DoIntersect(Node<T> p1, Node<T> q1, Node<T> p2, Node<T> q2)
        {
            Vec3 p1Pos = p1.GetContent.GetPosition();
            Vec3 q1Pos = q1.GetContent.GetPosition();
            Vec3 p2Pos = p2.GetContent.GetPosition();
            Vec3 q2Pos = q2.GetContent.GetPosition();
            int o1 = Orientation(p1Pos, q1Pos, p2Pos);
            int o2 = Orientation(p1Pos, q1Pos, q2Pos);
            int o3 = Orientation(p2Pos, q2Pos, p1Pos);
            int o4 = Orientation(p2Pos, q2Pos, q1Pos);
            
            if (o1 != o2 && o3 != o4) return true;
            if (o1 == 0 && OnSegment(p1Pos, p2Pos, q1Pos)) return true;
            if (o2 == 0 && OnSegment(p1Pos, q2Pos, q1Pos)) return true;
            if (o3 == 0 && OnSegment(p2Pos, p1Pos, q2Pos)) return true;
            if (o4 == 0 && OnSegment(p2Pos, q1Pos, q2Pos)) return true;
            return false;
        }
        
        private int Orientation(Vec3 p, Vec3 q, Vec3 r)
        {
            int val = (int)((q.Z - p.Z) * (r.X - q.X) -
                            (q.X - p.X) * (r.Z - q.Z));
 
            if (val == 0) return 0;
 
            return (val > 0)? 1: 2;
        }
        
        private bool OnSegment(Vec3 p, Vec3 q, Vec3 r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Z <= Math.Max(p.Z, r.Z) && q.Z >= Math.Min(p.Z, r.Z))
                return true;
 
            return false;
        }
    }
}