using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

public class SingleTrackingControlBenchmark : BenchmarkTask1d
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
		
		VRCAnimatorTrackingControl behaviour = ScriptableObject.CreateInstance<VRCAnimatorTrackingControl>();
		behaviour.trackingEyes = VRC_AnimatorTrackingControl.TrackingType.Animation;
		behaviour.trackingRightHand = VRC_AnimatorTrackingControl.TrackingType.Tracking;
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
		return "SingleTrackingControl";
	}

	public override string GetDescription()
	{
		return "Benchmarks two state toggle layer count with one tracking control on humanoid avatar";
	}
}