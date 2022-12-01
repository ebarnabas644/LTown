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

        [SerializeField] 
        public float buildingScale = 0.25f;

        public bool BuildBuilding(Vec3 pos, PlotType plotType, float maxRadius, string name, int id = -1)
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
                    if (houseProperties[id].sizeRadius * buildingScale >= maxRadius) return false;
                    building = Instantiate(houseTypes[id]);
                    break;
                case PlotType.Market:
                    if (marketProperties[id].sizeRadius * buildingScale >= maxRadius) return false;
                    building = Instantiate(marketTypes[id]);
                    break;
                case PlotType.Park:
                    if (parkProperties[id].sizeRadius * buildingScale >= maxRadius) return false;
                    building = Instantiate(parkTypes[id]);
                    break;
            }
            //house.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            building.name = name;
            building.transform.position = new Vector3(pos.X, pos.Y, pos.Z);
            building.transform.localScale = new Vector3(buildingScale, buildingScale, buildingScale);

            return true;
        }
    }
}