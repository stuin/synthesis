using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class auxFunctions : RigidNode_Base
{
    public delegate void HandleMesh(int id, BXDAMesh.BXDASubMesh subMesh, Mesh mesh);

    public static void ReadMeshSet(List<BXDAMesh.BXDASubMesh> meshes, HandleMesh handleMesh)
    {
		for (int j = 0; j < meshes.Count; j++)
        {
            BXDAMesh.BXDASubMesh sub = meshes[j];
            //takes all of the required information from the API (the API information is within "sub" above)
            Vector3[] vertices = sub.verts == null ? null : ArrayUtilities.WrapArray<Vector3>(
                delegate(double x, double y, double z)
                {
                    return new Vector3((float) x * 0.01f, (float) y * 0.01f, (float) z * 0.01f);
                }, sub.verts);

            Vector3[] normals = sub.norms == null ? null : ArrayUtilities.WrapArray<Vector3>(
                delegate(double x, double y, double z)
                {
                    return new Vector3((float) x, (float) y, (float) z);
                }, sub.norms);

            Mesh unityMesh = new Mesh();
            unityMesh.vertices = vertices;
            unityMesh.normals = normals;
            unityMesh.uv = new Vector2[vertices.Length];
            unityMesh.subMeshCount = sub.surfaces.Count;
            for (int i = 0; i < sub.surfaces.Count; i++)
            {
                unityMesh.SetTriangles(sub.surfaces[i].indicies, i);
            }
            unityMesh.RecalculateNormals();
            handleMesh(j, sub, unityMesh);
        }
    }
	
	public static Vector3 ConvertV3 (BXDVector3 vector)
	{
        return new Vector3((float) vector.x * 0.01f, (float) vector.y * 0.01f, (float) vector.z * 0.01f);
	}
	
	public static void OrientRobot(List<WheelCollider> wheelcolliders, Transform parent)
	{

		Quaternion q = new Quaternion();
		List<Vector3> wheels = new List<Vector3>();

		foreach (WheelCollider collider in wheelcolliders) 
		{
			wheels.Add(collider.transform.position);

		}
		Vector3 com = UnityRigidNode.TotalCenterOfMass(parent.gameObject);
		Vector3 a = wheels [0] - wheels [1];
		Vector3 b = a;

		for(int i = 2; Mathf.Abs(Vector3.Dot(a,b)-(a.magnitude*b.magnitude)) < .1f; i++) 
			b = wheels[0] - wheels[i];

		Vector3 norm = Vector3.Cross(a,b).normalized;
		//norm.y *= Mathf.Sign (norm.y * com.y);
		//TODO make sure robot never spawns upside down


		q.SetFromToRotation (norm, Vector3.up);
		parent.localRotation *= q;
	}
    public static void IgnoreCollisionDetection(List<MeshCollider> meshColliders)
    {
        for(int i = 0; i < meshColliders.Count; i++)
        {
            for (int j = 0; j < meshColliders.Count; j++)
            {
                if (meshColliders[i] == meshColliders[j])
                    continue;
                Physics.IgnoreCollision(meshColliders[i], meshColliders[j], true);
            }
               
            }
                 

    }
}


