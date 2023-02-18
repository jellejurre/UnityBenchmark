using System;
using UnityEditor.Animations;
using UnityEngine;

public class TenEmptyLayerBenchmark : BenchmarkTask
{
	public override GameObject PrepareIteration(GameObject prefab, int iterationNum)
	{
		var rootObject = new GameObject();
		for (int i = 0; i < 10; i++)
		{
			var gameObject = Instantiate(prefab, rootObject.transform);
			Animator animator = gameObject.GetOrAddComponent<Animator>();
			AnimatorController controller = new AnimatorController();
			AnimatorControllerLayer[] layers = new AnimatorControllerLayer[iterationNum];
			for (int j = 0; j < layers.Length; j++)
			{
				AnimatorControllerLayer layer = new AnimatorControllerLayer();
				layer.name = j.ToString();
				AnimatorStateMachine stateMachine = new AnimatorStateMachine();
				AnimatorState state = new AnimatorState();
				stateMachine.AddState(state, Vector3.one);
				layer.stateMachine = stateMachine;
				layer.defaultWeight = 1;
				layers[j] = layer;
			}
			controller.layers = layers;
			animator.runtimeAnimatorController = controller;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}

		return rootObject;
	}

	public override string GetName()
	{
		return "TenEmptyLayer";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks empty layer count on ten humanoid avatars";
	}
}