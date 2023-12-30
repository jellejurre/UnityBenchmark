using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

public class ParameterDriverBenchmark : BenchmarkTask1d
{

	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test" + i.ToString());
			toggleObject.transform.SetParent(gameObject.transform);
		}
		AnimatorController controller = AnimatorHelpers.SetupTwoToggles(iterationNum);
		AnimatorControllerLayer[] layers = controller.layers;
		
		for (var i = 0; i < controller.layers.Length; i++)
		{
			VRCAvatarParameterDriver behaviour = ScriptableObject.CreateInstance<VRCAvatarParameterDriver>();
			behaviour.parameters = new List<VRC_AvatarParameterDriver.Parameter>()
			{
				new VRC_AvatarParameterDriver.Parameter()
				{
					name = 0.ToString(),
					source = 0.ToString(),
					type = VRC_AvatarParameterDriver.ChangeType.Set
				}
			};
			layers[i].stateMachine.states[0].state.behaviours = new[] { behaviour };
		}

		controller.layers = layers;

		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Layers";
	}
	
	public override string GetName()
	{
		return "ParameterDriverBenchmark";
	}

	public override string GetDescription()
	{
		return "Benchmarks two state toggle layer count with parameter driver on humanoid avatar";
	}
}