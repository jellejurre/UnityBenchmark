using System;
using UnityEditor.Animations;
using UnityEngine;

public class SingleStateLayerBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
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
			stateMachine.AddState(new AnimatorState(), Vector3.one);
			layer.stateMachine = stateMachine;
			layers[i] = layer;
		}
		controller.layers = layers;
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
		return "SingleStateLayer";
	}

	public override string GetDescription()
	{
		return "Benchmarks single state layer count on humanoid avatar";
	}
}