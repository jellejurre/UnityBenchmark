using System;
using UnityEditor.Animations;
using UnityEngine;

public class NoAvatarLayerBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		AnimatorController controller = AnimatorHelpers.SetupTwoToggles(iterationNum);
		animator.runtimeAnimatorController = controller;
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		animator.avatar = null;
		return gameObject;
	}

	public override string GetParameterName()
	{
		return "Layers";
	}
	
	public override string GetName()
	{
		return "NoAvatar";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks two state toggle layer count on avatar";
	}
}