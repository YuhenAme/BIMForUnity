using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc4.Interfaces;
/// <summary>
/// ProductData接口
/// 类型名字
/// Product名字
/// 几何数据
/// 以及IFC中Product文件信息
/// </summary>
public interface IProductData
{
    string TypeName { get; set; }
    string ProductName { get; set; }
    //几何数据
    BimProduct ProductGeometryData { get; set; }
    GameObject TheGameObject { get; }
    bool HaveSpatialStructure { get; set; }
    IIfcProduct TheProduct { get; set; }
    //子product
    List<IProductData> DecomposedProducts { get; set; }
}

public class ProductData : MonoBehaviour, IProductData
{
    private string typeName;
    private string productName;
    private BimProduct product;
    private bool haveSpatialStructure = false;
    private IIfcProduct theProduct;
    private List<IProductData> decomposedProducts = new List<IProductData>();

    public string TypeName { get => typeName; set => typeName = value; }
    public string ProductName { get => productName; set => productName = value; }
    public BimProduct ProductGeometryData { get => product; set => product = value; }
    public GameObject TheGameObject => gameObject;
    public bool HaveSpatialStructure { get => haveSpatialStructure; set => haveSpatialStructure = value; }
    public IIfcProduct TheProduct { get => theProduct; set => theProduct = value; }
    public List<IProductData> DecomposedProducts { get => decomposedProducts; set => decomposedProducts = value; }

}
