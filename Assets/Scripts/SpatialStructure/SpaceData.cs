using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc4.Interfaces;

public interface ISpaceData : ISpatialData
{

}

public class SpaceData : MonoBehaviour, ISpaceData
{
    private string name;
    private IIfcSpace theSpace;
    private List<ISpatialData> subSpatials = new List<ISpatialData>();
    private List<IProductData> subProducts = new List<IProductData>();


    public string Name { get => name; set => name = value; }
    public IIfcSpatialStructureElement TheStructure { get => theSpace; set { if (value is IIfcSpace) theSpace = (IIfcSpace)value; } }
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
