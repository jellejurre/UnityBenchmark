using System;
using UnityEditor.Animations;
using UnityEngine;

public class NoRigLayerBenchmark : BenchmarkTask
{
	public override GameObject PrepareIteration(GameObject prefab, int iteration)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		AnimatorController controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[(int)(startVal * Math.Pow(baseNum, iteration))];
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
		animator.avatar = null;
		return gameObject;
	}

	public override string GetName()
	{
		return "NoRig";
	}
}