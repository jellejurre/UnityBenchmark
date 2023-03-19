using System;
using UnityEditor.Animations;
using UnityEngine;

public class EmptyLayerBenchmark : BenchmarkTask
{
	public override GameObject PrepareIteration(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		AnimatorController controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[iterationNum];
		for (int i = 0; i < layers.Length; i++)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = i.ToString();
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			layer.stateMachine = stateMachine;
			layers[i] = layer;
		}
		controller.layers = layers;
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		return gameObject;
	}

	public override string GetName()
	{
		return "EmptyLayer";
	}

	public override string GetDescription()
	{
		return "Benchmarks empty layer count on humanoid avatar";
	}
}