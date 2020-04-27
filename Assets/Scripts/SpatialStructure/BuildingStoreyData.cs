using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc4.Interfaces;

public interface IBuildingStoreyData :ISpatialData
{

}


public class BuildingStoreyData : MonoBehaviour,IBuildingStoreyData
{
    private string name;
    private IIfcBuildingStorey theBuildingStorey;
    private List<ISpatialData> subSpatials = new List<ISpatialData>();
    private List<IProductData> subProducts = new List<IProductData>();


    public string Name { get => name; set => name = value; }
    public IIfcSpatialStructureElement TheStructure { get => theBuildingStorey; set { if (value is IIfcBuildingStorey) theBuildingStorey = (IIfcBuildingStorey)value; } }
    public List<ISpatialData> SubSpatials { get => subSpatials; set => subSpatials = value; }
    public List<IProductData> SubProducts { get => subProducts; set => subProducts = value; }
    public GameObject TheGameObject { get => gameObject; }
    public int EntityLabel { get => TheStructure.EntityLabel; }

    public void AddProduct(IProductData p)
    {
        subProducts.Add(p);
    }

    public void AddSpatial(ISpatialData s)
    {
        subSpatials.Add(s);
    }
}
