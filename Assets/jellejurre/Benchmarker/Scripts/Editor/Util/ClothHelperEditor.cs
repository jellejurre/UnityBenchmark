using System;
using UnityEditor;
using UnityEngine;

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
			MeshHelper.ProcessCloth(validator.cloth, validator.vertexCount);
		}
		
		if (GUILayout.Button("GenerateMesh"))
		{
			GameObject meshObj = MeshHelper.GetSquareMesh(validator.vertexCount);
		}
	}

}