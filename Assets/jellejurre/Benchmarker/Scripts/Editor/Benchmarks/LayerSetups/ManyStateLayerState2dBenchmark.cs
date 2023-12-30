using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;

public class ManyStateLayerState2dBenchmark : BenchmarkTask2d
{

	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum, int iterationNum2)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.SetParent(gameObject.transform);
		}
		AnimatorController controller = AnimatorHelpers.SetupManyStateToggle(iterationNum, iterationNum2);
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string[] GetParameterNames()
	{
		return new[] { "Layers", "States" };
	}
	
	public override string GetName()
	{
		return "ManyLayerVsState";
	}

	public override string GetDescription()
	{
		return "Benchmarks many state toggle state vs layer count on humanoid avatar.\nFirst parameter is Layer count, second parameter is State count.";
	}
}