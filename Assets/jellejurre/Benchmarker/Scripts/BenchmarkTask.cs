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
	public abstract GameObject PrepareIteration(GameObject prefab, int iteration);

	public abstract string GetName();
}
