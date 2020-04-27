using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc4.Interfaces;
public interface IProjectData
{
    GameObject TheGameObject { get; }
    IIfcProject TheProject { get; set; }
    string Name { get; set; }
    List<IProductData> SubProducts { get; set; }
    List<ISpatialData> SubSpatials { get; set; }
}
public class ProjectData : MonoBehaviour,IProjectData
{
    private string name;
    private IIfcProject theProject;
    private List<ISpatialData> subSpatials = new List<ISpatialData>();
    private List<IProductData> subProducts = new List<IProductData>();


    public string Name { get => name; set => name = value; }
    public IIfcProject TheProject { get => theProject; set { if (value is IIfcProject) theProject = (IIfcProject)value; } }
    public List<ISpatialData> SubSpatials { get => subSpatials; set => subSpatials = value; }
    public List<IProductData> SubProducts { get => subProducts; set => subProducts = value; }
    public GameObject TheGameObject { get => gameObject; }
   

    public void AddProduct(IProductData p)
    {
        subProducts.Add(p);
    }

    public void AddSpatial(ISpatialData s)
    {
        subSpatials.Add(s);
    }
}
