using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BenchmarkInterface : MonoBehaviour
{
	[SerializeField]
	public List<BenchmarkTask> tasks;
}

[CustomEditor(typeof(BenchmarkInterface))]
public class BenchmarkerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		if (target == null)
		{
			return;
		}
		
		if (!EditorApplication.isPlaying)
		{
			GUILayout.Label("Please enter play mode to start benchmark mode.");
			return;
		}

		BenchmarkInterface benchmarkInterface = (BenchmarkInterface)target;

		if (benchmarkInterface.tasks == null)
		{
			benchmarkInterface.tasks = BenchmarkRepository.BenchmarkTasks;
			serializedObject.Update();
		}
		
		for (var i = 0; i < BenchmarkRepository.BenchmarkTasks.Count; i++)
		{
			var task = BenchmarkRepository.BenchmarkTasks[i];
			GUILayout.BeginVertical(GUI.skin.box);
			GUILayout.BeginHorizontal();
			using (new FieldWidth(10f))
			{
				FitLabel("Name: " + task.GetName());
				GUILayout.Space(10f);
				FitLabel("Initialization Time");
				float oldInitTime = task.initializationTime;
				float newInitTime;
				if (!float.TryParse(
					    EditorGUILayout.TextField(task.initializationTime.ToString()),
					    out newInitTime))
				{
					task.initializationTime = oldInitTime;
				}
				else
				{
					if (newInitTime != oldInitTime)
					{
						task.initializationTime = newInitTime;
						EditorUtility.SetDirty(task);
						AssetDatabase.SaveAssets();
					}
				}
				GUILayout.Space(5f);

				float oldBenchmarkTime = task.benchmarkTime;
				float newBenchmarkTime;
				FitLabel("Benchmark Time");
				if (!float.TryParse(
					    EditorGUILayout.TextField(task.benchmarkTime.ToString()),
					    out newBenchmarkTime))
				{
					task.benchmarkTime = oldBenchmarkTime;
				}
				else
				{
					if (newBenchmarkTime != oldBenchmarkTime)
					{
						task.benchmarkTime = newBenchmarkTime;
						EditorUtility.SetDirty(task);
						AssetDatabase.SaveAssets();
					}
				}
				GUILayout.Space(5f);

				int oldIterationCount = task.iterationCount;
				int newIterationCount;
				FitLabel("Iteration Count");
				if (!int.TryParse(
					    EditorGUILayout.TextField(task.iterationCount.ToString()), out newIterationCount))
				{
					task.iterationCount = oldIterationCount;
				}
				else
				{
					if (newIterationCount != oldIterationCount)
					{
						task.iterationCount = newIterationCount;
						EditorUtility.SetDirty(task);
						AssetDatabase.SaveAssets();
					}
				}

				if (GUILayout.Button("Run"))
				{
					BenchmarkManager.Run(task);
				}
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			using (new FieldWidth(20f))
			{
				float oldBase = task.baseNum;
				float newBase;
				FitLabel("Exponential Base");
				if (!float.TryParse(
					    EditorGUILayout.TextField(task.baseNum.ToString(), new []{ GUILayout.Width(50f)}), out newBase))
				{
					task.baseNum = oldBase;
				}
				else
				{
					if (newBase != oldBase)
					{
						task.baseNum = newBase;
						EditorUtility.SetDirty(task);
						AssetDatabase.SaveAssets();
					}
				}
				
				GUILayout.Space(10f);

				int oldStartVal = task.startVal;
				int newStartVal;
				FitLabel("Starting Value");
				if (!int.TryParse(EditorGUILayout.TextField(task.startVal.ToString(), new []{ GUILayout.Width(50f)}), out newStartVal))
				{
					task.startVal = oldStartVal;
				}
				else
				{
					if (newStartVal != oldStartVal)
					{
						task.startVal = newStartVal;
						EditorUtility.SetDirty(task);
						AssetDatabase.SaveAssets();
					}
				}
				
				GUILayout.Space(10f);
				FitLabel("Prefab");

				GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(task.prefab, typeof(GameObject), false, Array.Empty<GUILayoutOption>());
				if (newPrefab != task.prefab)
				{
					task.prefab = newPrefab;
					AssetDatabase.SaveAssets();
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			FitLabel(task.GetDescription());
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
	}
	
	public static void FitLabel(string text)
	{
		float width = EditorStyles.label.CalcSize(new GUIContent(text)).x + 10;
		GUILayout.Label(text, GUILayout.Width(width));
	}

		public class LabelWidth : IDisposable
	{
		public float originalValue = EditorGUIUtility.labelWidth;
		public LabelWidth(float newWidth)
		{
			EditorGUIUtility.labelWidth = newWidth;
		}
		public void Dispose()
		{
			EditorGUIUtility.labelWidth = originalValue;
		}
	}
	
	public class FieldWidth : IDisposable
	{
		public float originalValue = EditorGUIUtility.fieldWidth;
		public FieldWidth(float newWidth)
		{
			EditorGUIUtility.fieldWidth = newWidth;
		}
		public void Dispose()
		{
			EditorGUIUtility.fieldWidth = originalValue;
		}
	}
}
