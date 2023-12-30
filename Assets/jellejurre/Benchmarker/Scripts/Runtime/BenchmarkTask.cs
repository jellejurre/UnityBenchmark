using System;
using UnityEngine;
using Random = System.Random;

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

	public virtual void RunPlaymode(GameObject prefab, int iterationNum)
	{
		return;
	}
	
	public abstract string GetName();

	public abstract string GetDescription();

	public abstract string[] FormatDebug(BenchmarkData data);

	public abstract string[] FormatResults(BenchmarkData data);

	public abstract int GetMaxIterations();
}
