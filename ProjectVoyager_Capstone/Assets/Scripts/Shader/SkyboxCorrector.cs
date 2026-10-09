using UnityEngine;

[RequireComponent(typeof(Skybox))]
public class SkyboxCorrector : MonoBehaviour {

    void Start () {
        // Construct a rotation matrix and set it for the shader
        Quaternion rot = Quaternion.Euler (180f, 0f, 0f);
        Matrix4x4 m = Matrix4x4.TRS (Vector3.zero, rot, new Vector3(1,1,1) );
        GetComponent<Skybox>().material.SetMatrix ("_CustomRotation", m);
    }

}