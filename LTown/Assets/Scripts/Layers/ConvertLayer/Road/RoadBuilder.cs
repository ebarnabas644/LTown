using System;
using System.Collections.Generic;
using DataTypes;
using DataTypes.Graph;
using UnityEngine;

namespace ConvertLayer
{
    public class RoadBuilder : MonoBehaviour
    {
        [SerializeField]
        public List<GameObject> roadTypes;
        [SerializeField]
        public List<GameObject> intersectionTypes;

        public RoadBuilder()
        {
            roadTypes = new List<GameObject>();
        }

        public GameObject BuildRoad(Edge<Unit> edge, int id)
        {
            GameObject roadBlock = Instantiate(roadTypes[id]);
            Vector3 position = new Vector3(edge.Start.GetContent.X, edge.Start.GetContent.Y, edge.Start.GetContent.Z);
            roadBlock.transform.position = position;
            float xDistance = edge.End.GetContent.X - edge.Start.GetContent.X;
            float zDistance = edge.End.GetContent.Z - edge.Start.GetContent.Z;
            float distance = (float)Math.Sqrt(xDistance * xDistance + zDistance * zDistance);
            float angle = (float)((180 / Math.PI) * Math.Atan2(xDistance, zDistance));
            roadBlock.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            roadBlock.transform.localScale = new Vector3(1, 1, distance);
            return roadBlock;
        }

        public GameObject BuildIntersection(Node<Unit> node, int id)
        {
            GameObject roadBlock = Instantiate(intersectionTypes[id]);
            Vector3 position = new Vector3(node.GetContent.X, node.GetContent.Y, node.GetContent.Z);
            roadBlock.transform.position = position;
            roadBlock.transform.rotation = Quaternion.Euler(0f, node.GetContent.Angle, 0f);
            return roadBlock;
        }
    }
}