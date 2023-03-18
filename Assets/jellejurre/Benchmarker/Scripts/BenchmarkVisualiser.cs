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
	public bool drawFittedLine;
	public float visualisePercentage;
	public void Visualise()
	{
		string group = AssetDatabase.GetAssetPath(textAsset).Split('/')[ AssetDatabase.GetAssetPath(textAsset).Split('/').Length-2];
		Process.Start("python.exe", "Assets/jellejurre/Benchmarker/Scripts/draw.py " + textAsset.name + " " + group + " " + (drawFittedLine ? "True" : "False") + " " + visualisePercentage);
	}
}


[CustomEditor(typeof(BenchmarkVisualiser))]
public class BenchmarkVisualiserEditor : Editor
{
	private int inputNumber1;
	private int inputNumber2;
	public override void OnInspectorGUI()
	{
		if (target == null)
		{
			return;
		}
		
		BenchmarkVisualiser visualiser = (BenchmarkVisualiser)target;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("textAsset"));
		visualiser.drawFittedLine = GUILayout.Toggle(visualiser.drawFittedLine, "Visualise Fitted Line");

		visualiser.visualisePercentage = EditorGUILayout.Slider(new GUIContent("Percentage of data to visualize"), visualiser.visualisePercentage, 0, 1, Array.Empty<GUILayoutOption>());
		
		if (GUILayout.Button("Update Visualisation"))
		{
			visualiser.Visualise();
		}
		serializedObject.ApplyModifiedProperties();
		if (visualiser.textAsset == null)
		{
			return;
		}
		string group = AssetDatabase.GetAssetPath(visualiser.textAsset).Split('/')[AssetDatabase.GetAssetPath(visualiser.textAsset).Split('/').Length-2];
		Texture image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/jellejurre/Benchmarker/Output/" + group + "/Graphs/"+visualiser.textAsset.name+".png");
		if (image != null)
		{
			GUILayout.Box(image);
		}
		
		GUILayout.BeginHorizontal();
		TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/jellejurre/Benchmarker/Output/" + group + "/Data/"+visualiser.textAsset.name+".txt");
		if (text != null)
		{
			int oldNumber1 = inputNumber1;
			int newNumber1;
			FitLabel("x Input");
			if (int.TryParse(EditorGUILayout.TextField(inputNumber1.ToString()), out newNumber1))
			{
				if (oldNumber1 != newNumber1)
				{
					inputNumber1 = newNumber1;
				}
			}

			if (text.text.Split(',').Length > 3)
			{
				GUILayout.Space(10);
				int oldNumber2 = inputNumber2;
				int newNumber2;
				FitLabel("y Input");
				if (int.TryParse(EditorGUILayout.TextField(inputNumber2.ToString()), out newNumber2))
				{
					if (oldNumber2 != newNumber2)
					{
						inputNumber2 = newNumber2;
					}
				}
			}
			
			GUILayout.Space(10);
			FitLabel("Ms lag caused: " + GetMSLag(text.text.Split(',').Select(x => double.Parse(x)).ToArray()));
		}
		GUILayout.EndHorizontal();
	}

	public string GetMSLag(double[] vals)
	{
		if (vals.Length == 3)
		{
			double val = (vals[0] * Math.Pow(inputNumber1, 2) + vals[1] * inputNumber1);//+ vals[2] - 0.0018f);
			return ((decimal)val * 1000).ToString();
		}
		else
		{
			int x = inputNumber1;
			int y = inputNumber2;
			double val = vals[1] * y + vals[2] * Math.Pow(y, 2) +
			             vals[3] * x + vals[4] * x * y + vals[5] * x * Math.Pow(y, 2) + 
			             vals[6] * Math.Pow(x, 2) + vals[7] * Math.Pow(x, 2)*y + vals[8] *Math.Pow(x, 2) * Math.Pow(y, 2);
			return ((decimal)val * 1000).ToString();
		}
	}
	
	public static void FitLabel(string text)
	{
		float width = EditorStyles.label.CalcSize(new GUIContent(text)).x + 10;
		GUILayout.Label(text, GUILayout.Width(width));
	}
}
