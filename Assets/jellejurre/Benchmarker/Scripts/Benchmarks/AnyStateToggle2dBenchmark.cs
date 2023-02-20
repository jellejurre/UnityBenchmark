using UnityEditor.Animations;
using UnityEngine;

public class AnyStateToggle2dBenchmark : BenchmarkTask2d
{
	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum, int iterationNum2)
	{
		var rootObject = new GameObject();
		for (int i = 0; i < iterationNum2; i++)
		{
			var gameObject = Instantiate(prefab, rootObject.transform);
			Animator animator = gameObject.GetOrAddComponent<Animator>();
			AnimatorController controller = new AnimatorController();
			AnimatorControllerLayer[] layers = new AnimatorControllerLayer[iterationNum];
			AnimatorControllerParameter[] parameters = new AnimatorControllerParameter[layers.Length];
			for (int j = 0; j < layers.Length; j++)
			{
				string index = i.ToString() + j.ToString();
				AnimatorControllerParameter parameter = new AnimatorControllerParameter();
				parameter.type = AnimatorControllerParameterType.Float;
				parameter.name = index;
				parameter.defaultFloat = (i*0.4f + j*0.7f) % 1.0f;
				parameters[j] = parameter;
			}
			controller.parameters = parameters;
			for (int j = 0; j < layers.Length; j++)
			{
				string index = i.ToString() + j.ToString();
				GameObject toggleObject = new GameObject("test"+ j.ToString());
				toggleObject.transform.SetParent(gameObject.transform);
				AnimatorControllerLayer layer = new AnimatorControllerLayer();
				layer.name = index;
				AnimatorStateMachine stateMachine = new AnimatorStateMachine();
				AnimatorState onState = new AnimatorState();
				AnimatorState offState = new AnimatorState();
				onState.name = index + "on";
				offState.name = index + "off";
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
				onToOffCondition.mode = AnimatorConditionMode.Greater;
				offToOnCondition.mode = AnimatorConditionMode.Less;
				onToOffCondition.threshold = 0.5f;
				offToOnCondition.threshold = 0.5f;
				onToOffCondition.parameter = index;
				offToOnCondition.parameter = index;
				onToOffTransition.conditions = new AnimatorCondition[] { onToOffCondition };
				offToOnTransition.conditions = new AnimatorCondition[] { offToOnCondition };
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
	
	public override string[] GetParameterNames()
	{
		return new [] {"Layers", "Controllers"};
	}
	

	public override string GetName()
	{
		return "AnyStateToggle2d";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks any state toggle layer count on numerous humanoid avatars.\nFirst number is layer amount, second number is controller amount.";
	}
}