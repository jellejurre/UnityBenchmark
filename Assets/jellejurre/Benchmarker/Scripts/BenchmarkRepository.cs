using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.Core;
using VRC.Dynamics;
using Object = UnityEngine.Object;

public class BenchmarkRepository
{
	public static string benchmarkLocation = "Assets/jellejurre/Benchmarker/Benchmarks/";
	private static List<BenchmarkTaskGroup> benchmarkTasks;

	public static List<BenchmarkTaskGroup> BenchmarkTasks
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
		benchmarkTasks = new List<BenchmarkTaskGroup>();
		BenchmarkTask[] test = new[] { GetOrCreate<TestBenchmark>("TestBenchmark.asset") };
		benchmarkTasks.Add(new BenchmarkTaskGroup(test, "TestGroup"));

		BenchmarkTask[] animatorTypes = new BenchmarkTask[] {
			GetOrCreate<NoAvatarLayerBenchmark>("NoRigLayerBenchmark.asset"),
			GetOrCreate<GenericAvatarLayerBenchmark>("GenericLayerBenchmark.asset"),
			GetOrCreate<EmptyLayerBenchmark>("EmptyLayerBenchmark.asset")
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(animatorTypes, "AnimatorTypes"));

		BenchmarkTask[] LayerSetups = new BenchmarkTask[]
		{
			GetOrCreate<AnimatorCountBenchmark>("AnimatorCountBenchmark.asset"),
			GetOrCreate<SingleStateLayerBenchmark>("SingleStateLayerBenchmark.asset"),
			GetOrCreate<SingleStateInactiveLayerBenchmark>("SingleInactiveStateLayerBenchmark.asset"),
			GetOrCreate<TwoStateToggleBenchmark>("TwoStateToggleBenchmark.asset"),
			GetOrCreate<TwoStateToggle2dBenchmark>("TwoStateToggle2dBenchmark.asset"),
			GetOrCreate<ManyStateLayerState2dBenchmark>("ManyStateLayerState2dBenchmark.asset"),
			GetOrCreate<AnyStateToggleBenchmark>("AnyStateToggleBenchmark.asset"),
			GetOrCreate<AnyStateToggle2dBenchmark>("AnyStateToggle2dBenchmark.asset"),
			GetOrCreate<AnyStateLayerState2dBenchmark>("AnyStateLayerState2dBenchmark.asset")
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(LayerSetups, "LayerSetups"));

		BenchmarkTask[] Constraints = new BenchmarkTask[]
		{
			GetOrCreate<ParentConstraintBenchmark>("ParentConstraintBenchmark.asset"),
			GetOrCreate<ParentConstraintSourcesBenchmark>("ParentConstraintSourcesBenchmark.asset"),
			GetOrCreate<ParentConstraint2dBenchmark>("ParentConstraint2dBenchmark.asset")
		};
		benchmarkTasks.Add(new BenchmarkTaskGroup(Constraints, "ParentConstraints"));

		BenchmarkTask[] Contacts = new BenchmarkTask[]
		{
			GetOrCreate<ContactSenderBenchmark>("ContactSenderBenchmark.asset"),
			GetOrCreate<ContactReceiverBenchmark>("ContactReceiverBenchmark.asset")
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(Contacts, "Contacts"));
		// benchmarkTasks.Add(GetOrCreate<ContactSendReceiv2dBenchmark>("ContactSendReceiv2dBenchmark.asset"));
	}

	public static BenchmarkTask GetNext(BenchmarkTask current)
	{
		if (current == null)
		{
			return benchmarkTasks.First().tasks.First();
		}

		for (int i = 0; i < benchmarkTasks.Count; i++)
		{
			int index = Array.IndexOf(benchmarkTasks[i].tasks, current);
			if (index != -1)
			{
				if (benchmarkTasks[i].tasks.Length == index + 1)
				{
					if (benchmarkTasks.Count == i + 1)
					{
						return null;
					}
					else
					{
						return benchmarkTasks[i + 1].tasks.First();
					}
				}
				return benchmarkTasks[i].tasks[index + 1];

			}
		}
		return null;
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
