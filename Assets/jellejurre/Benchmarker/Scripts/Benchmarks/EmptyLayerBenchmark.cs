using System;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDKBase.Validation.Performance;
using Random = UnityEngine.Random;

public class EmptyLayerBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		AnimatorController controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[iterationNum];
		AnimatorControllerParameter[] parameters = new AnimatorControllerParameter[iterationNum];
		for (int i = 0; i < layers.Length; i++)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = i.ToString();
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			layer.stateMachine = stateMachine;
			layers[i] = layer;
			AnimatorControllerParameter parameter = new AnimatorControllerParameter();
			parameter.name = i.ToString();
			parameter.type = AnimatorControllerParameterType.Float;
			parameter.defaultFloat = Random.value;
			parameters[i] = parameter;
		}
		controller.layers = layers;
		controller.parameters = parameters;
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
		return "EmptyLayer";
	}

	public override string GetDescription()
	{
		return "Benchmarks empty layer count on humanoid avatar (note: unity cache)";
	}
}