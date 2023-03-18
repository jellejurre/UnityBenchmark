using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = System.Random;

public class BlendTreeFaceTracking : BenchmarkTask1d
{

	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject rootObject = new GameObject("test");
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject gameObject = Instantiate(prefab);
			Animator animator = gameObject.GetOrAddComponent<Animator>();
			gameObject.transform.SetParent(rootObject.transform);
			AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>($"Assets/jellejurre/Benchmarker/Assets/Generated/FaceTracking/FT_BT.controller");
			animator.runtimeAnimatorController = controller;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
		
		AssetDatabase.SaveAssets();
		return rootObject;
	}
	
	public override string GetParameterName()
	{
		return "Animators";
	}
	
	public override string GetName()
	{
		return "BlendTreeFT";
	}

	public override string GetDescription()
	{
		return "Benchmarks blend-tree-based facetracking on humanoid avatars";
	}
}