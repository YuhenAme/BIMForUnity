using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc4.Interfaces;
/// <summary>
/// 空间结构信息接口
/// </summary>
public interface ISpatialData
{
    string Name { get; set; }
    //空间结构元素(site,space,building,buildingstorey )
    IIfcSpatialStructureElement TheStructure { get; set; }
    List<ISpatialData> SubSpatials { get; set; }
    List<IProductData> SubProducts { get; set; }
    GameObject TheGameObject { get; }
    void AddSpatial(ISpatialData s);
    void AddProduct(IProductData p);
    int EntityLabel { get; }

}