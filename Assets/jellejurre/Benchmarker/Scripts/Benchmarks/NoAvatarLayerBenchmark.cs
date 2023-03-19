using System;
using UnityEditor.Animations;
using UnityEngine;

public class NoAvatarLayerBenchmark : BenchmarkTask
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
			AnimatorState state = new AnimatorState();
			stateMachine.AddState(state, Vector3.one);
			layer.stateMachine = stateMachine;
			layers[i] = layer;
		}
		controller.layers = layers;
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		animator.avatar = null;
		return gameObject;
	}

	public override string GetName()
	{
		return "NoAvatar";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks empty layer count on avatar";
	}
}