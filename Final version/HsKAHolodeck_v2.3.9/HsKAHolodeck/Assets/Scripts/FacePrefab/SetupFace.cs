using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class SetupFace : MonoBehaviour {

    public int FaceID { get; set; }
    public void LoadFace(ref Material textureMat,ref Mesh mesh1, ref Vector3 minMesh,ref Vector3 maxMesh)
    {

        this.GetComponent<MeshRenderer>().material = textureMat;
        this.GetComponent<MeshFilter>().sharedMesh = mesh1;
        this.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        this.GetComponent<MeshRenderer>().receiveShadows = false;
        this.GetComponent<MeshRenderer>().motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        this.GetComponent<MeshRenderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        this.GetComponent<MeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        this.GetComponent<BoxCollider>().size = maxMesh - minMesh;
    }


}
