using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;

public class AnyStateToggleBenchmark : BenchmarkTask
{
	public override GameObject PrepareIteration(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		AnimatorController controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[iterationNum];
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.SetParent(gameObject.transform);
			controller.AddParameter(i.ToString(), AnimatorControllerParameterType.Bool);
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = i.ToString();
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			AnimatorState onState = new AnimatorState();
			AnimatorState offState = new AnimatorState();
			onState.name = i.ToString() + "on";
			offState.name = i.ToString() + "off";
			AnimationClip[] anims = AnimationHelper.CreateTwoStateToggle(gameObject.transform, toggleObject.transform);
			onState.motion = anims[0];
			offState.motion = anims[1];
			stateMachine.AddState(onState, Vector3.one);
			stateMachine.AddState(offState, Vector3.one);
			AnimatorStateTransition onToOffTransition = stateMachine.AddAnyStateTransition(offState);
			AnimatorStateTransition offToOnTransition = stateMachine.AddAnyStateTransition(onState);
			onToOffTransition.exitTime = 1;
			offToOnTransition.exitTime = 1;
			onToOffTransition.hasExitTime = true;
			offToOnTransition.hasExitTime = true;
			onToOffTransition.destinationState = offState;
			offToOnTransition.destinationState = onState;
			AnimatorCondition onToOffCondition = new AnimatorCondition();
			AnimatorCondition offToOnCondition = new AnimatorCondition();
			onToOffCondition.mode = AnimatorConditionMode.IfNot;
			offToOnCondition.mode = AnimatorConditionMode.If;
			onToOffCondition.parameter = i.ToString();
			offToOnCondition.parameter = i.ToString();
			onToOffTransition.conditions = new AnimatorCondition[] { onToOffCondition };
			offToOnTransition.conditions = new AnimatorCondition[] { offToOnCondition };
			layer.stateMachine = stateMachine;
			layer.defaultWeight = 1;
			layers[i] = layer;
		}
		controller.layers = layers;
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		AssetDatabase.SaveAssets();
		return gameObject;
	}

	public override string GetName()
	{
		return "AnyStateToggle";
	}

	public override string GetDescription()
	{
		return "Benchmarks any state toggle layer count on humanoid avatar";
	}
}