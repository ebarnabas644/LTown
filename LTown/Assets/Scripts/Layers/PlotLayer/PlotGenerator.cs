using System.Collections.Generic;
using System.Linq;
using DataTypes;
using DataTypes.Graph;
using DataTypes.Graph.Assets.Scripts.Graph;
using DataTypes.Map;

namespace Layers.PlotLayer
{
    public class PlotGenerator<T> where T : ILocatable
    {
        private Map<T> map;
        private Dictionary<Node<T>, HashSet<Edge<T>>> vertexes;

        public PlotGenerator(Map<T> map)
        {
            this.map = map;
            vertexes = map.GetVertexes();
        }
        public List<Polygon<T>> GenerateFromMap()
        {
            //List<Polygon<T>> plots = new List<Polygon<T>>();

            foreach (var vertex in vertexes)
            {
                //var plot = StartSearch(vertex);
                StartSearch(vertex);
                /*
                if (!plots.Contains(plot))
                {
                    //plots.Add(StartSearch(vertex));
                    StartSearch(vertex);
                }*/
            }

            return plots;
        }

        private void StartSearch(KeyValuePair<Node<T>, HashSet<Edge<T>>> startPoint)
        {
            foreach (var edge in startPoint.Value)
            {
                var searchResult = new Polygon<T>();
                searchResult.AddPoint(startPoint.Key);
                if (!edge.Start.Equals(startPoint.Key))
                {
                    //searchResult = Search(startPoint.Key, edge.Start, searchResult, 8);

                    if (Search(startPoint.Key, edge.Start, searchResult, 8) && searchResult.Lenght > 2)
                    {
                        plots.Add(searchResult);
                    }
                }
                else
                {
                    //searchResult = Search(startPoint.Key, edge.End, searchResult, 8);
                    if (Search(startPoint.Key, edge.End, searchResult, 8) && searchResult.Lenght > 2)
                    {
                        plots.Add(searchResult);
                    }
                }
            }
        }
        
        private bool Search(Node<T> startPoint, Node<T> currentPoint, Polygon<T> pathInProgress, int maxIteration)
        {
            if (maxIteration <= 0) return false;
            foreach (var edge in vertexes[currentPoint])
            {
                var endPoint = edge.Start.Equals(currentPoint) ? edge.End : edge.Start;
                if (endPoint.Equals(startPoint))
                {
                    if (!pathInProgress.AddPoint(currentPoint))
                    {
                        return false;
                    }
                    return true;
                }
                var success = Search(startPoint, endPoint, pathInProgress, --maxIteration);
                if (success)
                {
                    if (!pathInProgress.AddPoint(currentPoint))
                    {
                        return false;
                    }
                    return true;
                }
            }

            return false;
        }
        
        /*private Polygon<T> StartSearch2(KeyValuePair<Node<T>, HashSet<Edge<T>>> startPoint)
        {
            var shortestCycle = new Polygon<T>();
            foreach (var edge in startPoint.Value)
            {
                var searchResult = new Polygon<T>();
                searchResult.AddPoint(startPoint.Key);
                if (!edge.Start.Equals(startPoint.Key))
                {
                    //searchResult = Search(startPoint.Key, edge.Start, searchResult, 8);
                    Search(startPoint.Key, edge.Start, searchResult, 8);
                }
                else
                {
                    //searchResult = Search(startPoint.Key, edge.End, searchResult, 8);
                    Search(startPoint.Key, edge.End, searchResult, 8);
                }

                if (searchResult.Lenght > 2 &&
                    (shortestCycle.Lenght == 0 || searchResult.Lenght < shortestCycle.Lenght))
                {
                    shortestCycle = searchResult;
                }
            }

            return shortestCycle;
        }*/

        /*
        private Polygon<T> Search2(Node<T> startPoint, Node<T> currentPoint, Polygon<T> pathInProgress, int maxIteration)
        {
            if (startPoint.Equals(currentPoint) && pathInProgress.Lenght > 2)
            {
                return pathInProgress;
            }
            if (maxIteration > 0)
            {
                foreach (var edge in vertexes[currentPoint])
                {
                    if (!edge.Start.Equals(currentPoint))
                    {
                        var addResult = pathInProgress.AddPoint(edge.Start);
                        if(!addResult) continue;
                        return Search(startPoint, edge.Start, new Polygon<T>(pathInProgress), --maxIteration);
                    }
                    else
                    {
                        var addResult = pathInProgress.AddPoint(edge.End);
                        if(!addResult) continue;
                        return Search(startPoint, edge.End, new Polygon<T>(pathInProgress), --maxIteration);
                    }
                }
            }

            return new Polygon<T>();
        }*/

        private List<Polygon<T>> plots = new List<Polygon<T>>();
        
        /*private bool Search(Node<T> startPoint, Node<T> currentPoint, Polygon<T> pathInProgress, int maxIteration)
        {
            if (maxIteration <= 0) return false;
            if (startPoint.Equals(currentPoint))
            {
                pathInProgress.AddPoint(currentPoint);
                return true;
            }
    
            foreach (var edge in vertexes[currentPoint])
            {
                var endPoint = edge.Start.Equals(currentPoint) ? edge.End : edge.Start;
                var success = Search(startPoint, endPoint, new Polygon<T>(pathInProgress), --maxIteration);
                if (success)
                {
                    pathInProgress.AddPoint(currentPoint);
                    return true;
                }
            }

            return false;
        }*/
        

        
    }
}