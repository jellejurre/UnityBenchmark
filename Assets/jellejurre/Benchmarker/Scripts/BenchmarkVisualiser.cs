using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;

public class BenchmarkVisualiser : MonoBehaviour
{
	public TextAsset textAsset;

	public void Visualise()
	{
		Process.Start("python.exe", "Assets/jellejurre/Benchmarker/Scripts/draw.py " + textAsset.name);
	}
}


[CustomEditor(typeof(BenchmarkVisualiser))]
public class BenchmarkVisualiserEditor : Editor
{
	private int inputNumber;
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
		
		GUILayout.BeginHorizontal();
		TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/jellejurre/Benchmarker/Output/Data/"+visualiser.textAsset.name+".txt");
		if (text != null)
		{
			int oldNumber = inputNumber;
			int newNumber;
			FitLabel("Iteration Count");
			if (int.TryParse(EditorGUILayout.TextField(inputNumber.ToString()), out newNumber))
			{
				if (oldNumber != newNumber)
				{
					inputNumber = newNumber;
				}
			}

			GUILayout.Space(10);
			FitLabel("Ms lag caused: " + GetMSLag(text.text.Split(',').Select(x => double.Parse(x)).ToArray(), inputNumber));
		}
		GUILayout.EndHorizontal();
	}

	public static string GetMSLag(double[] vals, int input)
	{
		double val = (vals[0] * Math.Pow(input, 2) + vals[1] * input + vals[2] - 0.002f)*1000;
		return ((decimal)val).ToString();
	}
	
	public static void FitLabel(string text)
	{
		float width = EditorStyles.label.CalcSize(new GUIContent(text)).x + 10;
		GUILayout.Label(text, GUILayout.Width(width));
	}
}
