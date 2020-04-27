using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Common.Geometry;
using Xbim.Geometry;
using Xbim.Ifc;
/// <summary>
/// 由5个结构体BimRegion，BimStyle，BimProduct，BimShapeInstance，BimTriangulation组成
/// </summary>
public static class BimGeomorty 
{
    public static List<BimRegion> regions = new List<BimRegion>();
    public static List<BimStyle> styles = new List<BimStyle>();
    public static List<BimProduct> products = new List<BimProduct>();
    public static List<BimShapeInstance> shapeInstances = new List<BimShapeInstance>();
    public static List<BimTriangulation> triangulations = new List<BimTriangulation>();
}

public struct BimRegion
{
    public int population;
    public Vector3 position;
    public Vector3 size;
    public XbimRegion xbimRegion;
    public BimRegion(int population,float centreX,float centreY,float centreZ,float sizeX,float sizeY,float sizeZ)
    {
        this.population = population;
        this.position = new Vector3(centreX, centreY, centreZ);
        this.size = new Vector3(sizeX, sizeY, sizeZ);
        xbimRegion = new XbimRegion()
        {
            Population = population,
            Centre = new XbimPoint3D(centreX, centreY, centreZ),
            Size = new XbimVector3D(sizeX, sizeY, sizeZ)
        };
    }
}

public struct BimStyle
{
    public int styleLabel;
    public float r, g, b, a;
    public Color color;
    public XbimColour xbimColour;
    public BimStyle(int styleLabel,float r,float g,float b,float a)
    {
        this.styleLabel = styleLabel;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
        color = new Color(r, g, b, a);
        xbimColour = new XbimColour(r, g, b, a);
    }
}
/// <summary>
/// 一个Product持有多个shapeInstance
/// </summary>
public struct BimProduct
{
    public int productLabel;
    public short productType;
    public List<BimShapeInstance> bimShapeInstances;
    public BimProduct(int productLabel,short productType)
    {
        this.productLabel = productLabel;
        this.productType = productType;
        bimShapeInstances = new List<BimShapeInstance>();
    }
    public void AddShapeInstance(BimShapeInstance bimShapeInstance)
    {
        bimShapeInstances.Add(bimShapeInstance);
    }
}
/// <summary>
/// 一个shapeInstance持有多个shapeTriangulation
/// </summary>
public struct BimShapeInstance
{
    public XbimShapeInstance xbimShapeInstance;
    public int productLabel;
    public short typeId;
    public int instanceLabel;
    public int styleLabel;
    public List<BimTriangulation> triangulations;
    public XbimMatrix3D xbimMatrix;
    public BimShapeInstance(int productLabel,short typeId,int instanceLabel,int styleLabel,XbimMatrix3D xbimMatrix= default)
    {
        this.productLabel = productLabel;
        this.typeId = typeId;
        this.instanceLabel = instanceLabel;
        this.styleLabel = styleLabel;
        this.triangulations = new List<BimTriangulation>();
        this.xbimMatrix = xbimMatrix;
        xbimShapeInstance = new XbimShapeInstance(this.instanceLabel)
        {
            IfcProductLabel = this.productLabel,
            IfcTypeId = this.typeId,
            StyleLabel = this.styleLabel,
            Transformation = this.xbimMatrix
        };
    }
    public void AddTriangulation(BimTriangulation triangulation)
    {
        triangulations.Add(triangulation);
    }

}

public struct BimTriangulation
{
    public XbimShapeTriangulation shapeTriangulation;
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector3> normals;
    public BimTriangulation(XbimShapeTriangulation shapeTriangulation,Vector3 offsite,float scale,XbimMatrix3D xbimMatrix = default,bool shapeRepetite = false) 
    {
        if (shapeRepetite)
        {
            this.shapeTriangulation = shapeTriangulation.Transform(xbimMatrix);

        }
        else
        {
            this.shapeTriangulation = shapeTriangulation;
        }

        vertices = new List<Vector3>();
        normals = new List<Vector3>();

        this.shapeTriangulation.ToPointsWithNormalsAndIndices(out List<float[]> points, out List<int> indices);
        triangles = indices;
        foreach(var p in points)
        {
            Vector3 vertice = new Vector3(p[0], p[1], p[2]) / scale - offsite;
            Vector3 normal = new Vector3(p[3], p[4], p[5]);
            vertices.Add(vertice);
            normals.Add(normal);
        }

    }
}

