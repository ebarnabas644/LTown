using System.Collections.Generic;
using DataTypes;
using UnityEngine;

namespace Layers.BuildingLayer
{
    public class BuildingBuilder: MonoBehaviour
    {
        [SerializeField] 
        public float houseChance;
        
        [SerializeField]
        public List<GameObject> houseTypes;
        [SerializeField]
        public List<BuildingProperty> houseProperties;
        
        [SerializeField] 
        public float parkChance;
        [SerializeField]
        public List<GameObject> parkTypes;

        [SerializeField] 
        public List<BuildingProperty> parkProperties;

        [SerializeField] 
        public float marketChance;
        [SerializeField]
        public List<GameObject> marketTypes;

        [SerializeField] 
        public List<BuildingProperty> marketProperties;

        public GameObject BuildBuilding(Vec3 pos, PlotType plotType, int id = -1)
        {
            GameObject building = new GameObject();
            if (id == -1)
            {
                switch (plotType)
                {
                    case PlotType.Housing:
                        id = Random.Range(0, houseTypes.Count);
                        break;
                    case PlotType.Market:
                        id = Random.Range(0, marketTypes.Count);
                        break;
                    case PlotType.Park:
                        id = Random.Range(0, parkTypes.Count);
                        break;
                }
            }
            switch (plotType)
            {
                case PlotType.Housing:
                    building = Instantiate(houseTypes[id]);
                    break;
                case PlotType.Market:
                    building = Instantiate(marketTypes[id]);
                    break;
                case PlotType.Park:
                    building = Instantiate(parkTypes[id]);
                    break;
            }
            //house.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            building.transform.position = new Vector3(pos.X, pos.Y, pos.Z);
            building.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            return building;
        }
    }
}