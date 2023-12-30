using System;
using System.Linq;
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
}
