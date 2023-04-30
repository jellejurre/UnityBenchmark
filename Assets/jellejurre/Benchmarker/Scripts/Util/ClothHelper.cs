using System;
using UnityEditor;
using UnityEngine;

public class ClothHelper : MonoBehaviour
{
	public Cloth cloth;
	public SkinnedMeshRenderer renderer;
	public int vertexCount;

	public void GenerateMesh()
	{
		GameObject meshObj = MeshHelper.GetSquareMesh(vertexCount);
	}

	public void SetupCloth()
	{
		MeshHelper.ProcessCloth(cloth, vertexCount);
	}

}


[CustomEditor(typeof(ClothHelper))]
public class ClothHelperEditor : Editor
{

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (target == null)
		{
			return;
		}

		ClothHelper validator = (ClothHelper) target;
		if (GUILayout.Button("SetupCloth"))
		{
			validator.SetupCloth();
		}
		
		if (GUILayout.Button("GenerateMesh"))
		{
			validator.GenerateMesh();
		}
	}

}