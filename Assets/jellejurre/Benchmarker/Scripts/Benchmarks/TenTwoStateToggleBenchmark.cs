using System;
using UnityEditor.Animations;
using UnityEngine;

public class TenTwoStateToggleBenchmark : BenchmarkTask
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
				GameObject toggleObject = new GameObject("test"+ j.ToString());
				toggleObject.transform.SetParent(gameObject.transform);
				controller.AddParameter(j.ToString(), AnimatorControllerParameterType.Bool);
				AnimatorControllerLayer layer = new AnimatorControllerLayer();
				layer.name = j.ToString();
				AnimatorStateMachine stateMachine = new AnimatorStateMachine();
				AnimatorState onState = new AnimatorState();
				AnimatorState offState = new AnimatorState();
				onState.name = j.ToString() + "on";
				offState.name = j.ToString() + "off";
				AnimationClip[] anims = AnimationHelper.CreateTwoStateToggle(gameObject.transform, toggleObject.transform);
				onState.motion = anims[0];
				offState.motion = anims[1];
				stateMachine.AddState(onState, Vector3.one);
				stateMachine.AddState(offState, Vector3.one);
				AnimatorStateTransition onToOffTransition = new AnimatorStateTransition();
				AnimatorStateTransition offToOnTransition = new AnimatorStateTransition();
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
				onToOffCondition.parameter = j.ToString();
				offToOnCondition.parameter = j.ToString();
				onToOffTransition.conditions = new AnimatorCondition[] { onToOffCondition };
				offToOnTransition.conditions = new AnimatorCondition[] { offToOnCondition };
				onState.AddTransition(onToOffTransition);
				offState.AddTransition(offToOnTransition);
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
		return "Ten-TwoStateToggle";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks two state toggle layer count on ten humanoid avatars";
	}
}