using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using System.IO;
#if !UNITY_EDITOR
using System.Threading.Tasks;
using System;
#endif

public class SampleFaceButton : MonoBehaviour,IInputClickHandler {

    public Button button;
    public Text FaceName;
    public Shader UnlitTexture;
    private FaceLink FaceData;
    private GameObject MainGUI;
    private string Path2FilePng;
    private string Path2FileObj;
    private Mesh FaceMesh;
    private Material textureMat;
    private Vector3 minMesh;
    private Vector3 maxMesh;
    private int ButtonID=-1;
    private GUIActivation GUIActivationHandler;
    //public GameObject PopUpMenu;
#if !UNITY_EDITOR
    private Task<ThreadingReturn> t ;
    private Task<byte[]> tt;
#endif

    public struct ThreadingReturn
    {
        
        public int[] triangles;
        public Vector3[] vertices;
        public Vector2[] uv;
        public Vector3[] normals;
        public Vector3 minMesh;
        public Vector3 maxMesh;
    }

    // Use this for initialization
    void  Start () {
        GUIActivationHandler = MainGUI.GetComponent<GUIActivation>();
    }
    public void SetNull()
    {
        FaceMesh = null;
        textureMat = null;
        FaceData = new FaceLink();
        Path2FilePng = null;
        Path2FileObj = null;
        minMesh = new Vector3();
        maxMesh = new Vector3();
        ButtonID = -1;
    }
    public void SetButtonID(int ID)
    {
        if(ButtonID==-1)
        {
            ButtonID = ID;
        }
    }

#if !UNITY_EDITOR
    public void Setup(FaceLink currentFaceLink)
    {    
        FaceName.text = currentFaceLink.Name;
        FaceData = currentFaceLink;        
        Path2FilePng = currentFaceLink.Path2FilePng;
        Path2FileObj = currentFaceLink.Path2FileObj;
        MainGUI = this.transform.root.gameObject;
        


    }

    public byte[] LoadByte()
    {
        byte[] ByteReturn = System.IO.File.ReadAllBytes(Path2FilePng);
        return ByteReturn;

    }
    public ThreadingReturn LoadTexture()
    {
        ThreadingReturn TR = new ThreadingReturn();
        int NumOfTriangles = 0;
        int NumOfVertices = 0;
        int NumOfVt = 0;
        int NumOfVn = 0;
        int NumOfFace = 0;
        TR.minMesh = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        TR.maxMesh = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        using (StreamReader stream = File.OpenText(Path2FileObj))
        {
            string entireText = stream.ReadToEnd();
            using (StringReader reader = new StringReader(entireText))
            {
                string currentText = reader.ReadLine();
                char[] splitIdentifier = { ' ' };
                string[] brokenString;
                while (currentText != null)
                {
                    if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ")
                        && !currentText.StartsWith("vn "))
                    {
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                    else
                    {
                        currentText = currentText.Trim();                           //Trim the current line
                        brokenString = currentText.Split(splitIdentifier, 50);      //Split the line into an array, separating the original line by blank spaces
                        switch (brokenString[0])
                        {
                            case "v":
                                NumOfVertices++;
                                break;
                            case "vt":
                                NumOfVt++;
                                break;
                            case "vn":
                                NumOfVn++;
                                break;
                            case "f":
                                NumOfFace = NumOfFace + brokenString.Length - 1;
                                NumOfTriangles = NumOfTriangles + 3 * (brokenString.Length - 2); /*brokenString.Length is 3 or greater since a face must have at least
                                                                                      3 vertices.  For each additional vertice, there is an additional
                                                                                      triangle in the mesh (hence this formula).*/
                                break;
                        }
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                }
            }
            entireText = null;
        }

        TR.triangles = new int[NumOfTriangles];
        Vector3[] TempVertices = new Vector3[NumOfVertices];
        Vector2[] TempUv = new Vector2[NumOfVt];
        Vector3[] TempNormals = new Vector3[NumOfVn];
        Vector3[] faceData = new Vector3[NumOfFace];
        using (StreamReader stream = File.OpenText(Path2FileObj))
        {
            string entireText = stream.ReadToEnd();
            using (StringReader reader = new StringReader(entireText))
            {
                string currentText = reader.ReadLine();
                char[] splitIdentifier = { ' ' };
                char[] splitIdentifier2 = { '/' };
                string[] brokenString;
                string[] brokenBrokenString;
                int f = 0;
                int f2 = 0;
                int v = 0;
                NumOfVn = 0;
                NumOfVt = 0;
                int vt1 = 0;
                int vt2 = 0;
                Vector3 temp = new Vector3();
                while (currentText != null)
                {

                    if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ") &&
                        !currentText.StartsWith("vn ") && !currentText.StartsWith("g ") && !currentText.StartsWith("usemtl ") &&
                        !currentText.StartsWith("mtllib ") && !currentText.StartsWith("vt1 ") && !currentText.StartsWith("vt2 ") &&
                        !currentText.StartsWith("vc ") && !currentText.StartsWith("usemap "))
                    {
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                    else
                    {
                        currentText = currentText.Trim();
                        brokenString = currentText.Split(splitIdentifier, 50);
                        switch (brokenString[0])
                        {
                            case "g":
                                break;
                            case "usemtl":
                                break;
                            case "usemap":
                                break;
                            case "mtllib":
                                break;
                            case "v":
                                TempVertices[v].Set(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]), System.Convert.ToSingle(brokenString[3]));

                                if (TempVertices[v].x < TR.minMesh.x)
                                    TR.minMesh.x = TempVertices[v].x;
                                if (TempVertices[v].y < TR.minMesh.y)
                                    TR.minMesh.y = TempVertices[v].y;
                                if (TempVertices[v].z < TR.minMesh.z)
                                    TR.minMesh.z = TempVertices[v].z;

                                if (TempVertices[v].x > TR.maxMesh.x)
                                    TR.maxMesh.x = TempVertices[v].x;
                                if (TempVertices[v].y > TR.maxMesh.y)
                                    TR.maxMesh.y = TempVertices[v].y;
                                if (TempVertices[v].z > TR.maxMesh.z)
                                    TR.maxMesh.z = TempVertices[v].z;
                                v++;
                                break;
                            case "vt":
                                TempUv[NumOfVt].Set(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                                NumOfVt++;
                                break;
                            case "vt1":
                                TempUv[vt1].Set(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                                vt1++;
                                break;
                            case "vt2":
                                TempUv[vt2].Set(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                                vt2++;
                                break;
                            case "vn":
                                TempNormals[NumOfVn].Set(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]), System.Convert.ToSingle(brokenString[3]));
                                NumOfVn++;
                                break;
                            case "vc":
                                break;
                            case "f":

                                int j = 1;
                                List<int> intArray = new List<int>();
                                while (j < brokenString.Length && ("" + brokenString[j]).Length > 0)
                                {

                                    brokenBrokenString = brokenString[j].Split(splitIdentifier2, 3);    //Separate the face into individual components (vert, uv, normal)
                                    temp.x = System.Convert.ToInt32(brokenBrokenString[0]);
                                    if (brokenBrokenString.Length > 1)                                  //Some .obj files skip UV and normal
                                    {
                                        if (brokenBrokenString[1] != "")                                    //Some .obj files skip the uv and not the normal
                                        {
                                            temp.y = System.Convert.ToInt32(brokenBrokenString[1]);
                                        }
                                        temp.z = System.Convert.ToInt32(brokenBrokenString[2]);
                                    }
                                    j++;

                                    faceData[f2] = temp;
                                    intArray.Add(f2);
                                    f2++;
                                }
                                j = 1;
                                while (j + 2 < brokenString.Length)     //Create triangles out of the face data.  There will generally be more than 1 triangle per face.
                                {
                                    TR.triangles[f] = intArray[0];
                                    f++;
                                    TR.triangles[f] = intArray[j];
                                    f++;
                                    TR.triangles[f] = intArray[j + 1];
                                    f++;

                                    j++;
                                }
                                break;
                        }
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");       //Some .obj files insert double spaces, this removes them.
                        }
                    }
                }
            }
            entireText = null;

        }

        TR.vertices = new Vector3[faceData.Length];
        TR.uv = new Vector2[faceData.Length];
        TR.normals = new Vector3[faceData.Length];
        /* The following foreach loops through the facedata and assigns the appropriate vertex, uv, or normal
         * for the appropriate Unity mesh array.
         */
        int kk = faceData.Length;
        for (int k = 0; k < kk; k++)
        {
            TR.vertices[k] = TempVertices[(int)faceData[k].x - 1] - (TR.maxMesh + TR.minMesh) / 2;
            if (faceData[k].y >= 1)
                TR.uv[k] = TempUv[(int)faceData[k].y - 1];

            if (faceData[k].z >= 1)
                TR.normals[k] = TempNormals[(int)faceData[k].z - 1];
        }

        TempVertices = null;
        TempUv = null;
        TempNormals = null;
        faceData = null;
        GC.Collect();
        return TR;


    }




    public void OnInputClicked(InputEventData eventData)
    {
        
        if(!SimpleFaceManager.Instance.GetLoadingStateFlag())
        {
            SimpleFaceManager.Instance.SetLoadingStateFlag(true, ButtonID);
            StartCoroutine(LoadData());
        }
        
    }

    public IEnumerator LoadData()
    {

        // "if" to skip loading if it has been done previously
        if (FaceMesh==null && textureMat==null)
        {
            //creates two tasks for loading the mesh (textures) and the .png file (byte)
            t = new Task<ThreadingReturn>(LoadTexture);
            tt = new Task<byte[]>(LoadByte);
            GUIActivationHandler.LoadingScreenFace(true);
            FaceMesh = new Mesh();

            textureMat = new Material(UnlitTexture);

            Texture2D tex = new Texture2D(1, 1);

            t.Start();

            tt.Start();

            //waits till both tasks are done

            while (!(t.IsCompleted == true) || !(tt.IsCompleted == true))
            {
                yield return null;

            }

            ThreadingReturn TR = t.Result;

            byte[] Byte = tt.Result;

            tex.LoadImage(Byte);

            textureMat.mainTexture = tex;

            FaceMesh.vertices = TR.vertices;

            FaceMesh.uv = TR.uv;

            FaceMesh.normals = TR.normals;

            FaceMesh.triangles = TR.triangles;
            minMesh = TR.minMesh;
            maxMesh = TR.maxMesh;
            TR =new ThreadingReturn();
            GUIActivationHandler.LoadingScreenFace(false);
            t = null;
            tt = null;
        }


        GameObject spawnedGameObject = SimpleFaceManager.Instance.GetObject();

        spawnedGameObject.GetComponent<SetupFace>().LoadFace(ref this.textureMat,ref this.FaceMesh,ref this.minMesh,ref this.maxMesh);
        spawnedGameObject.GetComponent<FaceBehavior>().GetNewMaterial();
        
        
        MainGUI.GetComponent<GUIActivation>().DeactivateGUI();
        SimpleFaceManager.Instance.SetLoadingStateFlag(false, ButtonID);

    }
    private Material GetMaterial(Material textureMat3)
    {

        return textureMat3;

    }
#else

    public void OnInputClicked(InputEventData eventData)
    { }
    public void Setup(FaceLink currentFaceLink)
    {
    }
    public IEnumerator LoadTexture()
    {
        yield return null;
    }
#endif
}
