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
		benchmarkTasks.Add(GetOrCreate<AnimatorCountBenchmark>("AnimatorCountBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<NoAvatarLayerBenchmark>("NoRigLayerBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<GenericAvatarLayerBenchmark>("GenericLayerBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<EmptyLayerBenchmark>("EmptyLayerBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<SingleStateLayerBenchmark>("SingleStateLayerBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<TwoStateToggleBenchmark>("TwoStateToggleBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<TwoStateToggle2dBenchmark>("TwoStateToggle2dBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<AnyStateToggleBenchmark>("AnyStateToggleBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<AnyStateToggle2dBenchmark>("AnyStateToggle2dBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<ParentConstraintBenchmark>("ParentConstraintBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<ParentConstraintSourcesBenchmark>("ParentConstraintSourcesBenchmark.asset"));
		benchmarkTasks.Add(GetOrCreate<ParentConstraint2dBenchmark>("ParentConstraint2dBenchmark.asset"));
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
