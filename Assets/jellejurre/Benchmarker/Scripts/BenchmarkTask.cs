using System;
using UnityEngine;

[Serializable]
public abstract class BenchmarkTask : ScriptableObject
{
	[SerializeField] public float initializationTime;
	[SerializeField] public float benchmarkTime;
	[SerializeField] public int iterationCount;
	[SerializeField] public float baseNum;
	[SerializeField] public int startVal;
	[SerializeField] public GameObject prefab;
	public abstract GameObject PrepareIteration(GameObject prefab, int iterationNum);

	public abstract string GetName();

	public abstract string GetDescription();

	public int GetIterationNumber(int iteration)
	{
		return (int)(startVal * Math.Pow(baseNum, iteration));
	}
}
