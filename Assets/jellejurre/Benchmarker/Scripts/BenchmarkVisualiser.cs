using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;

public class BenchmarkVisualiser : MonoBehaviour
{
	public TextAsset textAsset;

	public void Visualise()
	{
		Process process = Process.Start("powershell.exe", " -Command "+ "python.exe Assets/jellejurre/Benchmarker/Scripts/draw.py " + textAsset.name);
	}
}


[CustomEditor(typeof(BenchmarkVisualiser))]
public class BenchmarkVisualiserEditor : Editor
{
	public override void OnInspectorGUI()
	{
		if (target == null)
		{
			return;
		}
		
		BenchmarkVisualiser visualiser = (BenchmarkVisualiser)target;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("textAsset"));

		if (GUILayout.Button("Update Visualisation"))
		{
			visualiser.Visualise();
		}
		serializedObject.ApplyModifiedProperties();

		Texture image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/jellejurre/Benchmarker/Output/Graphs/"+visualiser.textAsset.name+".png");
		if (image != null)
		{
			GUILayout.Box(image);
		}
	}
}
