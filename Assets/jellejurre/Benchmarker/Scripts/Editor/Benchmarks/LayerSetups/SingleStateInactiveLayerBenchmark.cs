using System;
using UnityEditor.Animations;
using UnityEngine;

public class SingleStateInactiveLayerBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		AnimatorController controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[iterationNum];
		AnimatorHelpers.AddParameters(controller, iterationNum);
		for (int i = 0; i < layers.Length; i++)
		{
			GameObject toggleObject = new GameObject("test"+i);
			toggleObject.transform.SetParent(gameObject.transform);
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = i.ToString();
			layer.defaultWeight = 1;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			AnimatorState state = new AnimatorState();
			AnimationClip offAnim = AnimationHelper.GetOrCreateOneStateToggle("test"+i, i);
			state.motion = offAnim;
			stateMachine.AddState(state, Vector3.one);
			AnimatorState emptystate = new AnimatorState();
			stateMachine.AddState(emptystate, Vector3.one);
			AnimatorStateTransition transition = new AnimatorStateTransition();
			transition.destinationState = emptystate;
			AnimatorCondition condition = new AnimatorCondition();
			condition.mode = AnimatorConditionMode.Greater;
			condition.parameter = i.ToString();
			condition.threshold = -1;
			state.AddTransition(transition);
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
		return "InactiveStateLayer";
	}

	public override string GetDescription()
	{
		return "Benchmarks inactive single state layer count on humanoid avatar";
	}
}