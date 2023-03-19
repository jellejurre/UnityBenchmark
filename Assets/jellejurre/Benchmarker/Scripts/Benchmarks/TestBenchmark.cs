using System;
using UnityEditor.Animations;
using UnityEngine;

public class TestBenchmark : BenchmarkTask
{
	public override GameObject PrepareIteration(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		return gameObject;
	}

	public override string GetName()
	{
		return "TestBenchmark";
	}
	
	public override string GetDescription()
	{
		return "Test benchmark which runs nothing";
	}
}