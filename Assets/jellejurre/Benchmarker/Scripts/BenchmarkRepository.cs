using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class BenchmarkRepository
{
	public static string benchmarkLocation = "Assets/jellejurre/Benchmarker/Benchmarks/";
	private static List<BenchmarkTask> benchmarkTasks;

	public static List<BenchmarkTask> BenchmarkTasks
	{
		get
		{
			if (benchmarkTasks == null)
			{
				SetupBenchmarks();
			}
			return benchmarkTasks;
		}
		set => benchmarkTasks = value;
	}

	static void SetupBenchmarks()
	{
		benchmarkTasks = new List<BenchmarkTask>();
		benchmarkTasks.Add(GetOrCreate<TestBenchmark>("TestBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<NoRigLayerBenchmark>("NoRigLayerBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<GenericRigLayerBenchmark>("GenericLayerBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<EmptyLayerBenchmark>("EmptyLayerBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<TenEmptyLayerBenchmark>("TenEmptyLayerBenchmark.asset"));

	}
	
	static T GetOrCreate<T>(string location) where T : ScriptableObject
	{
		T benchmark = AssetDatabase.LoadAssetAtPath<T>(benchmarkLocation + location);
		if (benchmark == null)
		{
			Debug.Log("Creating benchmark at: " + location);
			benchmark = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(benchmark, benchmarkLocation + location);
			AssetDatabase.SaveAssets();
		}

		return benchmark;
	}
}
