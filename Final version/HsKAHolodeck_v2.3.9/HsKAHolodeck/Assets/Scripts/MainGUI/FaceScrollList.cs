using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
#if !UNITY_EDITOR
using Windows.Storage;
#endif
using System.Linq;
public struct FaceLink
{
    public string Name;
    public string Path2FileObj;
    public string Path2FilePng;
}

public class FaceScrollList : MonoBehaviour {

    private char[] Seperators= {'\\','/' };
    private FaceLink[] FaceList= new FaceLink[0];
    //public GameObject prefab;
    public SimpleObjectPool FaceButtonObjectPool;
    public Transform contentPanel;
    public GameObject MainMenu;
    public GameObject FaceListPar;
    public int NumberOfFaces;


    void Start()
    {
        
    }



    public void ScanForFaces()
    {

#if !UNITY_EDITOR
        string[] FileList = Directory.GetFiles(ApplicationData.Current.LocalFolder.Path);// ...\LocalState FolderList
#else
        string[] FileList =new string[] {"This String is just here so Unity would shut the fuck up"};
#endif


        FaceLink[] NewFaceList =new FaceLink[FileList.Length]; //empty list
        int NumberOfNewFaces = 0;
        //create list with all .obj files
        for (int i = 0; i < FileList.Length; i++)
        {
            int ObjExtBeginn = FileList[i].IndexOf(".obj");
            //check if file is a .obj file
            if (ObjExtBeginn != -1)
            {

                int ObjFileNameBegin = FileList[i].LastIndexOfAny(Seperators);
                //get the name of the .obj file
                string TempName = FileList[i].Substring(ObjFileNameBegin + 1, ObjExtBeginn - ObjFileNameBegin - 1);
                //search the directory for TempName.png File
                string PngName = TempName + ".png";
                int ii = 0;
                int PngExist = -1;
                while (ii< FileList.Length)
                {
                    PngExist = FileList[ii].IndexOf(PngName);
                    //check if file is the TempName.png file
                    if (PngExist != -1)
                    {
                        NewFaceList[NumberOfNewFaces].Name = TempName;
                        NewFaceList[NumberOfNewFaces].Path2FileObj = FileList[i];
                        NewFaceList[NumberOfNewFaces].Path2FilePng = FileList[ii];
                        NumberOfNewFaces++;
                        ii = FileList.Length;
                    }
                    ii++;
                }
            }
        }
        CheckForFaceRemove(NewFaceList);//check if faces were removed
        CheckForFaceAdded(NewFaceList, NumberOfNewFaces);//check if new faces were added
        FaceList = NewFaceList;
        NumberOfFaces = NumberOfNewFaces;
        MainMenu.SetActive(false);
        FaceListPar.SetActive(true);
    }

    private void AddButton(FaceLink newFace)
    {
        GameObject newFaceButton = FaceButtonObjectPool.GetObject();
        newFaceButton.transform.SetParent(contentPanel, false);
        SampleFaceButton sampleButton = newFaceButton.GetComponent<SampleFaceButton>();
        sampleButton.Setup(newFace);
    }

    private void RemoveButton(FaceLink removedFace)
    {
        for(int i=0;i<contentPanel.childCount;i++)
        {
            GameObject TempFaceButton = contentPanel.GetChild(i).gameObject;
            SampleFaceButton TempButton = TempFaceButton.GetComponent<SampleFaceButton>();
            if (removedFace.Name.Equals(TempButton.FaceName.text.ToString()) )
            {
                FaceButtonObjectPool.ReturnObject(TempFaceButton);
            }
        }
        


    }

    private void  CheckForFaceRemove(FaceLink[] NewList)
    {
        //check if a file from the working dir existed in the old list but not in the new
        // if so remove that button and return it back to the object pool
        FaceLink Temp = new FaceLink();
        Temp.Name = null;
        for (int i=0;i<NumberOfFaces;i++)
        {
            Temp= Array.Find(NewList, x => x.Name == FaceList[i].Name);
            if (Temp.Name!= null)
            {  }
            else
            { RemoveButton(FaceList[i]); }
            
        }

    }

    private void CheckForFaceAdded(FaceLink[] NewList, int NumberOfNewFaces)
    {
        //check if a file from the working dir doesnt exist in the old list
        // if so get a button from the object pool and setup up its values
        FaceLink Temp = new FaceLink();
        Temp.Name = null;
        for (int i = 0; i < NumberOfNewFaces; i++)
        {
            Temp = Array.Find(FaceList, x => x.Name == NewList[i].Name);
            if (Temp.Name!=null)
            { }
            else
            { AddButton(NewList[i]); }
            
        }
    }

}
