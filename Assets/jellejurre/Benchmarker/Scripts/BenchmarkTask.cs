using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class BenchmarkTask : ScriptableObject
{
	public BenchmarkTaskGroup group;
	[SerializeField] public float initializationTime;
	[SerializeField] public float benchmarkTime;
	[SerializeField] public int iterationCount;
	[SerializeField] public float baseNum;
	[SerializeField] public int startVal;
	[SerializeField] public GameObject prefab;
	public abstract GameObject PrepareIteration(GameObject prefab, int iterationNum);

	public abstract string GetName();

	public abstract string GetDescription();

	public abstract string[] FormatDebug(BenchmarkData data);

	public abstract string[] FormatResults(BenchmarkData data);

	public abstract int GetMaxIterations();

	public abstract void Visualise();


	public void VisualiseFloatField(ref float floatField, string text, int width = 0)
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
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}
	}
	public void VisualiseIntField(ref int intField, string text, int width = 0)
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
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}
	}
}
