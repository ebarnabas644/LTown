using DataTypes;
using DataTypes.Graph;
using DataTypes.Map;
using NUnit.Framework;
using UnityEngine;

public class CityObjectTest
{
    
    
    // A Test behaves as an ordinary method
    [Test]
    public void SamePositionCityObjectShouldBeEqual()
    {
        var cityObj1 = new Node<CityObject>(new CityObject(new GameObject()));
        cityObj1.GetContent.GetGameObject().transform.position = new Vector3(1, 2, 3);
        var cityObj2 = new Node<CityObject>(new CityObject(new GameObject()));
        cityObj2.GetContent.GetGameObject().transform.position = new Vector3(1, 2, 3);

        var result = cityObj1.Equals(cityObj2);
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void DifferentPositionCityObjectShouldBeNotEqual()
    {
        var cityObj1 = new Node<CityObject>(new CityObject(new GameObject()));
        cityObj1.GetContent.GetGameObject().transform.position = new Vector3(1, 2, 3);
        var cityObj2 = new Node<CityObject>(new CityObject(new GameObject()));
        cityObj2.GetContent.GetGameObject().transform.position = new Vector3(1, 3, 3);

        var result = cityObj1.Equals(cityObj2);
        
        Assert.That(result, Is.False);
    }
}