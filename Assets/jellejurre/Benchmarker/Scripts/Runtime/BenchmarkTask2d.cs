using System;
using System.Linq;
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
	
	public override void RunPlaymode(GameObject prefab, int iterationNum)
	{
		RunPlaymode2d(prefab, 
			(int)(startVal * Math.Pow(baseNum, iterationNum%iterationCount)),
			(int)(startVal2 * Math.Pow(baseNum2, iterationNum/iterationCount)));	}

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

	public virtual void RunPlaymode2d(GameObject gameObject, int iterationNum, int iterationNum2)
	{
		
	}
}