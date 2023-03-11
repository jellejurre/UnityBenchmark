using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = System.Random;

public class AnyStateToggleActiveBenchmark : BenchmarkTask1d
{
	private Animator animator;
	private float[][] data;
	private int current = 0;
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		animator = gameObject.GetOrAddComponent<Animator>();
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.SetParent(gameObject.transform);
		}
		AnimatorController controller = AnimatorHelpers.SetupAnyStateToggle(iterationNum);
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		data = new float[2][];
		data[0] = new float[iterationNum];
		data[1] = new float[iterationNum];
		Random r = new Random();
		for (int i = 0; i < iterationNum; i++)
		{
			data[0][i] = (float)r.NextDouble();
			data[1][i] = (float)r.NextDouble();
		}
		AssetDatabase.SaveAssets();
		return gameObject;
	}

	public override void RunPlaymode1d(GameObject gameObject, int iterationNum)
	{
		current = (current + 1) % 2;
		for (int i = 0; i < iterationNum; i++)
		{
			animator.SetFloat(i.ToString(), data[current][i]);
		}
	}
	
	public override string GetParameterName()
	{
		return "Layers";
	}

	public override string GetName()
	{
		return "AnyStateToggleActive";
	}

	public override string GetDescription()
	{
		return "Benchmarks constantly changing any state toggle layer count on humanoid avatar";
	}
}