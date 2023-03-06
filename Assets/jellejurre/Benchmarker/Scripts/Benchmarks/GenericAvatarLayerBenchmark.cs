using System;
using UnityEditor.Animations;
using UnityEngine;

public class GenericAvatarLayerBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		AnimatorController controller = AnimatorHelpers.SetupTwoToggles(iterationNum);
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Layers";
	}

	public override string GetName()
	{
		return "GenericRig";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks two state toggle layer count on generic avatar";
	}
}