using System;
using UnityEditor.Animations;
using UnityEngine;

public class TestBenchmark : BenchmarkTask1d
{
	
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Nothing";
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