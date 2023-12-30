using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

public class SingleLayerControlBenchmark : BenchmarkTask1d
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
		
		VRCAnimatorLayerControl behaviour = ScriptableObject.CreateInstance<VRCAnimatorLayerControl>();
		behaviour.playable = VRC_AnimatorLayerControl.BlendableLayer.FX;
		behaviour.layer = 0;
		behaviour.blendDuration = 1;
		behaviour.goalWeight = 0.9f;
		layers[0].stateMachine.states[0].state.behaviours = new[] { behaviour };

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
		return "SingleLayerControlBenchmark";
	}

	public override string GetDescription()
	{
		return "Benchmarks two state toggle layer count with one layer control on humanoid avatar";
	}
}