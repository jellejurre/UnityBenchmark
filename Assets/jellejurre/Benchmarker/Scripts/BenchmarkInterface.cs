using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BenchmarkInterface : MonoBehaviour
{ }

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

		// if (benchmarkInterface.tasks == null)
		// {
		// 	benchmarkInterface.tasks = BenchmarkRepository.BenchmarkTasks;
		// 	serializedObject.Update();
		// }

		bool runAll = GUILayout.Button("Run All Benchmarks");
		if (runAll)
		{
			BenchmarkManager.RunAll();
		}

		for (var i = 0; i < BenchmarkRepository.BenchmarkTasks.Count; i++)
		{
			var taskGroup = BenchmarkRepository.BenchmarkTasks[i];
			taskGroup.shown = EditorGUILayout.Foldout(taskGroup.shown, taskGroup.name);
			if (taskGroup.shown)
			{
				taskGroup.Visualise();
			}
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
