using System.Collections.Generic;
using System.Linq;
using DataTypes;
using DataTypes.Graph.Assets.Scripts.Graph;
using DataTypes.Map;

namespace Layers.PlotLayer
{
    public class PlotGenerator
    {
        public List<Polygon> GenerateFromMap(Map<Unit> map)
        {
            var vertexes = map.GetVertexes();
            List<Polygon> plots = new List<Polygon>();

            foreach (var vertex in vertexes)
            {

            }

            return plots;
        }
        
    }
}