using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;

public class TwoStateToggleBoolBenchmark : BenchmarkTask1d
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
		AnimatorController controller = AnimatorHelpers.SetupTwoToggles(iterationNum, false, true);
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
		return "TwoStateBoolToggle";
	}

	public override string GetDescription()
	{
		return "Benchmarks two state bool toggle layer count on humanoid avatar";
	}
}