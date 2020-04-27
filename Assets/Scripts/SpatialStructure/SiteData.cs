using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc4.Interfaces;


public interface ISiteData : ISpatialData
{

}

public class SiteData : MonoBehaviour,ISiteData
{
    private IIfcSite theSite;
    private string name;
    private List<IProductData> subProducts = new List<IProductData>();
    private List<ISpatialData> subSpatials = new List<ISpatialData>();

    public IIfcSpatialStructureElement TheStructure { get => theSite; set { if (value is IIfcSite) theSite = (IIfcSite)value; } }
    public string Name { get => name; set => name = value; }
    public List<IProductData> SubProducts { get => subProducts; set => subProducts = value; }
    public GameObject TheGameObject { get => gameObject; }
    public List<ISpatialData> SubSpatials { get => subSpatials; set => subSpatials = value; }
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
