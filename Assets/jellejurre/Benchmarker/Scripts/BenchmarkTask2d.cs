using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
public abstract class BenchmarkTask2d  : BenchmarkTask {
	
	[SerializeField] public float baseNum2;
	[SerializeField] public int startVal2;
	
	public override int GetMaxIterations()
	{
		return iterationCount * iterationCount;
	}

	public override GameObject PrepareIteration(GameObject prefab, int iterationNum)
	{
		return PrepareIteration2d(prefab, 
			(int)(startVal * Math.Pow(baseNum, iterationNum%iterationCount)),
			(int)(startVal2 * Math.Pow(baseNum2, iterationNum/iterationCount)));
	}

	public override string[] FormatDebug(BenchmarkData data)
	{
		string[] lines = new string[data.frameTimes.Length];
		string[] names = GetParameterNames();
		for (int i = 0; i < data.frameTimes.Length; i++)
		{
			lines[i] = ($"Iteration: {i}, {names[0]}: {(int)(startVal * Math.Pow(baseNum, i%iterationCount))}, {names[1]}:{(int)(startVal2 * Math.Pow(baseNum2, i/iterationCount))}, Average FrameTime: {data.frameTimes[i].Average()}");
		}
		return lines;
	}
	
	public override string[] FormatResults(BenchmarkData data)
	{
		string[] lines = new string[data.frameTimes.Length + 1];
		string[] names = GetParameterNames();
		lines[0] = string.Join(",", names);
		for (int i = 0; i < data.frameTimes.Length; i++)
		{
			lines[i + 1] = $"{(int)(startVal * Math.Pow(baseNum, i % iterationCount))},{(int)(startVal2 * Math.Pow(baseNum2, i / iterationCount))},{data.frameTimes[i].Average()}";
		}
		return lines;
	}

	public abstract string[] GetParameterNames();
	
	public abstract GameObject PrepareIteration2d(GameObject prefab, int value1, int value2);

	public override void Visualise()
	{
		BenchmarkTask2d task = this;
		GUILayout.BeginVertical(GUI.skin.box);
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
				AssetDatabase.SaveAssets();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		using (new BenchmarkerEditor.FieldWidth(20f))
		{
			VisualiseFloatField(ref task.baseNum2, "Exponential Base 2", 50);
			GUILayout.Space(10f);
			VisualiseIntField(ref task.startVal2, "Starting Value 2", 50);
			GUILayout.Space(10f);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		BenchmarkerEditor.FitLabel(task.GetDescription());
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
}