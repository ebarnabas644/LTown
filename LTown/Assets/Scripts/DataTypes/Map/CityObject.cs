using UnityEngine;

namespace DataTypes.Map
{
    public class CityObject : ILocatable
    {
        private GameObject _gameObject;
        
        public CityObject(GameObject gameObject)
        {
            _gameObject = gameObject;
        }
        
        public GameObject GetGameObject()
        {
            return _gameObject;
        }
        
        public Vec3 GetPosition()
        {
            Vector3 pos = _gameObject.transform.position;
            return new Vec3(pos.x, pos.y, pos.z);
        }
    }
}