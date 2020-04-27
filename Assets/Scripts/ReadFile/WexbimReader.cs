using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;

public class WexbimReader 
{
    public static void ReadWexbim(string fileName)
    {
        Vector3 offset;
        using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            using (var br = new BinaryReader(fs))
            {
                var magicalNum = br.ReadInt32();
                var version = br.ReadByte();
                var shapeCount = br.ReadInt32();
                var vertexCount = br.ReadInt32();
                var triangleCount = br.ReadInt32();
                var matrixCount = br.ReadInt32();
                var productCount = br.ReadInt32();
                var styleCount = br.ReadInt32();
                var meter = br.ReadSingle();
                var regionCount = br.ReadInt16();

                for (int i = 0; i < regionCount; i++)
                {
                    var population = br.ReadInt32();
                    var centreX = br.ReadSingle();centreX /= meter;
                    var centreY = br.ReadSingle();centreY /= meter;
                    var centreZ = br.ReadSingle();centreZ /= meter;
                    var boundsBytes = br.ReadBytes(6 * sizeof(float));
                    var modelBounds = XbimRect3D.FromArray(boundsBytes);
                    modelBounds.X /= meter;modelBounds.Y /= meter;modelBounds.Z /= meter;
                    modelBounds.SizeX /= meter;modelBounds.SizeY /= meter;modelBounds.SizeZ /= meter;
                    BimGeomorty.regions.Add(new BimRegion(population, centreX, centreY, centreZ, (float)modelBounds.SizeX, (float)modelBounds.SizeY, (float)modelBounds.SizeZ));
                }
                //中心偏移
                offset = BimGeomorty.regions[0].position;

                for(int i = 0; i < styleCount; i++)
                {
                    var styleId = br.ReadInt32();
                    var red = br.ReadSingle();
                    var green = br.ReadSingle();
                    var blue = br.ReadSingle();
                    var alpha = br.ReadSingle();
                    BimGeomorty.styles.Add(new BimStyle(styleId, red, green, blue, alpha));
                }

                for(int i = 0; i < productCount; i++)
                {
                    var productLabel = br.ReadInt32();
                    var productType = br.ReadInt16();
                    var boxBytes = br.ReadBytes(6 * sizeof(float));
                    XbimRect3D bb = XbimRect3D.FromArray(boxBytes);
                    BimGeomorty.products.Add(new BimProduct(productLabel, productType));
                }

                for(int i = 0; i < shapeCount; i++)
                {
                    var shapeRepetition = br.ReadInt32();
                    BimShapeInstance shapeInstance;
                    if (shapeRepetition > 1)
                    {
                        List<BimShapeInstance> tempInstances = new List<BimShapeInstance>();
                        for(int j = 0; j < shapeRepetition; j++)
                        {
                            var ifcProductLabel = br.ReadInt32();
                            var ifcTypeId = br.ReadInt16();
                            var instanceLabel = br.ReadInt32();
                            var styleLabel = br.ReadInt32();
                            var transform = XbimMatrix3D.FromArray(br.ReadBytes(sizeof(double) * 16));

                            shapeInstance = new BimShapeInstance(ifcProductLabel, ifcTypeId, instanceLabel, styleLabel, transform);
                            BimGeomorty.shapeInstances.Add(shapeInstance);
                            tempInstances.Add(shapeInstance);
                            var p = BimGeomorty.products.Find(product => product.productLabel == ifcProductLabel);
                            p.AddShapeInstance(shapeInstance);
                        }
                        var triangulation = br.ReadShapeTriangulation();
                        foreach(var temp in tempInstances)
                        {
                            var tri = new BimTriangulation(triangulation, offset, meter, temp.xbimMatrix, true);
                            temp.AddTriangulation(tri);
                            BimGeomorty.triangulations.Add(tri);
                        }
                    }
                    else if(shapeRepetition == 1)
                    {
                        var ifcProductLabel = br.ReadInt32();
                        var ifcTypeId = br.ReadInt16();
                        var instanceLabel = br.ReadInt32();
                        var styleLabel = br.ReadInt32();

                        shapeInstance = new BimShapeInstance(ifcProductLabel, ifcTypeId, instanceLabel, styleLabel);
                        BimGeomorty.shapeInstances.Add(shapeInstance);
                        var p = BimGeomorty.products.Find(product => product.productLabel == ifcProductLabel);
                        p.AddShapeInstance(shapeInstance);

                        XbimShapeTriangulation triangulation = br.ReadShapeTriangulation();
                        var tri = new BimTriangulation(triangulation, offset, meter);
                        BimGeomorty.triangulations.Add(tri);
                        shapeInstance.AddTriangulation(tri);
                    }
                }
            }
        }
    }
}
