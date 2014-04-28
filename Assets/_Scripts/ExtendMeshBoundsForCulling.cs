using UnityEngine;
using System.Collections;

public class ExtendMeshBoundsForCulling : MonoBehaviour {
	
	MeshFilter[] meshList;

	// Use this for initialization
	void Start () {
		meshList = GetComponentsInChildren<MeshFilter>();
		Transform camTransform = Camera.main.transform;
		float distToCenter = (Camera.main.farClipPlane - Camera.main.nearClipPlane) / 2.0f;
		Vector3 center = camTransform.position + camTransform.forward * distToCenter;
		float extremeBound = 3000.0f;
		foreach (MeshFilter mesh in meshList){
			mesh.sharedMesh.bounds =  new Bounds(center,  Vector3.one * extremeBound);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
