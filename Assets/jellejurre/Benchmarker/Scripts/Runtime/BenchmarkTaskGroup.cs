using UnityEngine;

public class BenchmarkTaskGroup
{
	public BenchmarkTask[] tasks;
	public string name;
	public bool shown;
	public BenchmarkTaskGroup(BenchmarkTask[] tasks, string name)
	{
		this.tasks = tasks;
		foreach (var benchmarkTask in tasks)
		{
			benchmarkTask.group = this;
		}
		this.name = name;
	}
}