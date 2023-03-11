using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

[Serializable]
public abstract class BenchmarkTask1d : BenchmarkTask
{
	public override GameObject PrepareIteration(GameObject prefab, int iterationNum)
	{
		return PrepareIteration1d(prefab, (int)(startVal * Math.Pow(baseNum, iterationNum)));
	}
	
	public override void RunPlaymode(GameObject prefab, int iterationNum)
	{
		RunPlaymode1d(prefab, (int)(startVal * Math.Pow(baseNum, iterationNum)));
	}

	public override string[] FormatDebug(BenchmarkData data)
	{
		string[] lines = new string[data.frameTimes.Length];
		string name = GetParameterName(); ;
		for (int i = 0; i < data.frameTimes.Length; i++)
		{
			lines[i] = ($"Iteration: {i}, {name}: {(int)(startVal * Math.Pow(baseNum, i%iterationCount))}, Average FrameTime: {data.frameTimes[i].Average()}");
		}
		return lines;
	}
	
	public override string[] FormatResults(BenchmarkData data)
	{
		string[] lines = new string[data.frameTimes.Length + 1];
		string name = GetParameterName();
		lines[0] = name;
		for (int i = 0; i < data.frameTimes.Length; i++)
		{
			lines[i + 1] = $"{(int)(startVal * Math.Pow(baseNum, i % iterationCount))},{data.frameTimes[i].Average()}";
		}
		return lines;
	}

	public abstract string GetParameterName();
	public abstract GameObject PrepareIteration1d(GameObject prefab, int value);

	public virtual void RunPlaymode1d(GameObject prefab, int value)
	{
		
	}

	public override int GetMaxIterations()
	{
		return iterationCount;
	}

	public override void Visualise()
	{
		BenchmarkTask task = this;
		using (new EditorGUILayout.VerticalScope(GUI.skin.box))
		{
			GUILayout.BeginHorizontal();
			using (new BenchmarkerEditor.FieldWidth(10f))
			{
				BenchmarkerEditor.FitLabel("Name: " + task.GetName());
				GUILayout.Space(10f);
				VisualiseFloatField(ref task.initializationTime, "Initialization Time");
				GUILayout.Space(5f);
				VisualiseFloatField(ref task.benchmarkTime, "Benchmark Time");
				GUILayout.Space(5f);
				VisualiseIntField(ref task.iterationCount, "Iteration Count");
				if (GUILayout.Button("Run"))
				{
					BenchmarkManager.LoadSceneAndRun(task);
				}
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			using (new BenchmarkerEditor.FieldWidth(20f))
			{
				VisualiseFloatField(ref task.baseNum, "Exponential Base", 50);
				GUILayout.Space(10f);
				VisualiseIntField(ref task.startVal, "Starting Value", 50);
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
}
