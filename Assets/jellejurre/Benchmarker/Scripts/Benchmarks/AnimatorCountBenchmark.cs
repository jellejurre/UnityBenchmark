using System;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorCountBenchmark : BenchmarkTask
{
	public override GameObject PrepareIteration(GameObject prefab, int iterationNum)
	{
		GameObject root = new GameObject();
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject gameObject = Instantiate(prefab, root.transform);
			Animator animator = gameObject.GetOrAddComponent<Animator>();
			AnimatorController controller = new AnimatorController();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = 0.ToString();
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			AnimatorState state = new AnimatorState();
			stateMachine.AddState(state, Vector3.one);
			layer.stateMachine = stateMachine;
			controller.layers = new [] {layer};
			animator.runtimeAnimatorController = controller;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
		return root;
	}

	public override string GetName()
	{
		return "AnimatorCount";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks animator count with humanoid avatar";
	}
}