using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using Xbim.Ifc4.Interfaces;



public class DialogTest : MonoBehaviour
{
    private GameObject model;
    private List<GameObject> productObjs = new List<GameObject>();
    public static string exeName ;

    private void Start()
    {
        model = GameObject.Find("Model");
       
    }
    private void OnGUI()
    {
        if(GUI.Button(new Rect(10, 10, 100, 50), "Open"))
        {
            OpenFileName openFileName = new OpenFileName();
            openFileName.structSize = Marshal.SizeOf(openFileName);
            openFileName.filter = "Bim文件(*.ifc;*.wexbim)\0*.ifc;*.wexbim\0\0";
            openFileName.file = new string(new char[256]);
            openFileName.maxFile = openFileName.file.Length;
            openFileName.fileTitle = new string(new char[64]);
            openFileName.maxFileTitle = openFileName.fileTitle.Length;

            openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
            openFileName.title = "窗口标题";
            openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
            if (LocalDialog.GetOpenFileName(openFileName))
            {
                if(Path.GetExtension(openFileName.file) == ".ifc")
                {
                    OpenIfcToWexbim(openFileName.file);
                    string wexbimFilePath = Path.GetDirectoryName(openFileName.file) + @"\" + Path.GetFileNameWithoutExtension(openFileName.file) + ".wexbim";
                    WexbimReader.ReadWexbim(wexbimFilePath);
                    InstantiateBimModel(openFileName.file);
                }
                else if(Path.GetExtension(openFileName.file) == ".wexbim")
                {
                    WexbimReader.ReadWexbim(openFileName.file);
                    //InstantiateBimModel();
                    string ifcFilePath = Path.GetDirectoryName(openFileName.file) + @"\" + Path.GetFileNameWithoutExtension(openFileName.file) + ".ifc";
                    //UnityEngine.Debug.Log(ifcFilePath);
                    InstantiateBimModel(ifcFilePath);
                }
            }

        }
    }

    private void OpenIfcToWexbim(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            Process exe = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            #if UNITY_EDITOR
            processStartInfo.FileName = UnityEngine.Application.dataPath + "/Plugins/IfcToWexBim.exe";
            #endif
            #if UNITY_STANDALONE_WIN
            processStartInfo.FileName = UnityEngine.Application.dataPath + "/Managed/IfcToWexBim.exe";
            #endif

            processStartInfo.Arguments = @filePath;
            exe.StartInfo = processStartInfo;
            
            exe.Start();
            exe.WaitForExit();
            exe.Close();
        }
    }
    
    private void InstantiateBimModel(string fileName)
    {
        //foreach(var p in BimGeomorty.products)
        //{
        //    GameObject product = BimModelCreater.InstantiateProduct(p);
        //    product.transform.SetParent(model.transform);
        //}
        //model.transform.rotation = Quaternion.Euler(-90, 0, 0);

        foreach(var p in BimGeomorty.products)
        {
            BimModelCreater.InstantiateProduct(p);
        }
        var projData = SpatialStructureTest.GetIfcSpatialStructure(fileName);
        BimModelCreater.InstantiateSpatialStructure(projData);
        projData.TheGameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);

        //----------
        // //将模型底部坐标归于0
        var tempFloors = GameObject.Find(SpatialStructureTest.project.Name).transform.GetComponentsInChildren<BuildingStoreyData>();
        var s = tempFloors[Mathf.FloorToInt(tempFloors.Length - 1) / 2].TheStructure as IIfcBuildingStorey;
        float yPos = (float)s.Elevation;
        projData.TheGameObject.transform.position += new Vector3(0, yPos, 0);

    }
}
