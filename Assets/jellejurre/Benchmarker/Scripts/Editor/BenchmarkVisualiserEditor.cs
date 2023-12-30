using System;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
			Visualise(visualiser.textAsset, visualiser.drawFittedLine, visualiser.visualisePercentage);
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
    
	public static void Visualise(TextAsset textAsset, bool drawFittedLine, float visualisePercentage)
	{
		string group = AssetDatabase.GetAssetPath(textAsset).Split('/')[ AssetDatabase.GetAssetPath(textAsset).Split('/').Length-2];
		Process.Start("python.exe", "Assets/jellejurre/Benchmarker/Scripts/Runtime/draw.py " + textAsset.name + " " + group + " " + (drawFittedLine ? "True" : "False") + " " + visualisePercentage);
	}
	
	
	public static void VisualiseFloatField(BenchmarkTask task, ref float floatField, string text, int width = 0)
	{
		float oldBenchmarkTime = floatField;
		float newBenchmarkTime;
		BenchmarkerEditor.FitLabel(text);
		GUILayoutOption[] options = new[] { GUILayout.Width(width) };
		if (width == 0)
		{
			options = Array.Empty<GUILayoutOption>();
		}
		if (!float.TryParse(EditorGUILayout.TextField(floatField.ToString(), options), out newBenchmarkTime))
		{
			floatField = oldBenchmarkTime;
		}
		else
		{
			if (newBenchmarkTime != oldBenchmarkTime)
			{
				floatField = newBenchmarkTime;
				EditorUtility.SetDirty(task);
				AssetDatabase.SaveAssets();
			}
		}
	}
	public static void VisualiseIntField(BenchmarkTask task, ref int intField, string text, int width = 0)
	{
		int oldBenchmarkTime = intField;
		int newBenchmarkTime;
		BenchmarkerEditor.FitLabel(text);
		GUILayoutOption[] options = new[] { GUILayout.Width(width) };
		if (width == 0)
		{
			options = Array.Empty<GUILayoutOption>();
		}
		if (!int.TryParse(EditorGUILayout.TextField(intField.ToString(), options), out newBenchmarkTime))
		{
			intField = oldBenchmarkTime;
		}
		else
		{
			if (newBenchmarkTime != oldBenchmarkTime)
			{
				intField = newBenchmarkTime;
				EditorUtility.SetDirty(task);
				AssetDatabase.SaveAssets();
			}
		}
	}
	
	public static void Visualise1D(BenchmarkTask task)
	{
		using (new EditorGUILayout.VerticalScope(GUI.skin.box))
		{
			GUILayout.BeginHorizontal();
			using (new BenchmarkerEditor.FieldWidth(10f))
			{
				BenchmarkerEditor.FitLabel("Name: " + task.GetName());
				GUILayout.Space(10f);
				VisualiseFloatField(task, ref task.initializationTime, "Initialization Time");
				GUILayout.Space(5f);
				VisualiseFloatField(task, ref task.benchmarkTime, "Benchmark Time");
				GUILayout.Space(5f);
				VisualiseIntField(task, ref task.iterationCount, "Iteration Count");
				if (GUILayout.Button("Run"))
				{
					BenchmarkManager.LoadSceneAndRun(task);
				}
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			using (new BenchmarkerEditor.FieldWidth(20f))
			{
				VisualiseFloatField(task, ref task.baseNum, "Exponential Base", 50);
				GUILayout.Space(10f);
				VisualiseIntField(task, ref task.startVal, "Starting Value", 50);
				GUILayout.Space(10f);
				BenchmarkerEditor.FitLabel("Prefab");
				GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(task.prefab, typeof(GameObject), false, Array.Empty<GUILayoutOption>());
				if (newPrefab != task.prefab)
				{
					task.prefab = newPrefab;
					EditorUtility.SetDirty(task);
					AssetDatabase.SaveAssets();
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			BenchmarkerEditor.FitLabel(task.GetDescription());
			GUILayout.EndHorizontal();
		}
	}
	
	public static void Visualise2D(BenchmarkTask2d task)
	{
		GUILayout.BeginVertical(GUI.skin.box);
		GUILayout.BeginHorizontal();
		using (new BenchmarkerEditor.FieldWidth(10f))
		{
			BenchmarkerEditor.FitLabel("Name: " + task.GetName());
			GUILayout.Space(10f);
			VisualiseFloatField(task, ref task.initializationTime, "Initialization Time");
			GUILayout.Space(5f);
			VisualiseFloatField(task, ref task.benchmarkTime, "Benchmark Time");
			GUILayout.Space(5f);
			VisualiseIntField(task, ref task.iterationCount, "Iteration Count");
			if (GUILayout.Button("Run"))
			{
				BenchmarkManager.LoadSceneAndRun(task);
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		using (new BenchmarkerEditor.FieldWidth(20f))
		{
			VisualiseFloatField(task, ref task.baseNum, "Exponential Base", 50);
			GUILayout.Space(10f);
			VisualiseIntField(task, ref task.startVal, "Starting Value", 50);
			GUILayout.Space(10f);
			BenchmarkerEditor.FitLabel("Prefab");
			GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(task.prefab, typeof(GameObject), false, Array.Empty<GUILayoutOption>());
			if (newPrefab != task.prefab)
			{
				task.prefab = newPrefab;
				EditorUtility.SetDirty(task);
				AssetDatabase.SaveAssets();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		using (new BenchmarkerEditor.FieldWidth(20f))
		{
			VisualiseFloatField(task, ref task.baseNum2, "Exponential Base 2", 50);
			GUILayout.Space(10f);
			VisualiseIntField(task, ref task.startVal2, "Starting Value 2", 50);
			GUILayout.Space(10f);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		BenchmarkerEditor.FitLabel(task.GetDescription());
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	public static void VisualiseGroup(BenchmarkTaskGroup group)
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Run all benchmarks in this category"))
		{
			BenchmarkManager.RunCategory(group);
		}
		GUILayout.EndHorizontal();
		foreach (var benchmarkTask in group.tasks)
		{
			if (benchmarkTask is BenchmarkTask1d task1d)
			{
				Visualise1D(task1d);
			}

			if (benchmarkTask is BenchmarkTask2d task2d)
			{
				Visualise2D(task2d);
			}
		}
	}
}