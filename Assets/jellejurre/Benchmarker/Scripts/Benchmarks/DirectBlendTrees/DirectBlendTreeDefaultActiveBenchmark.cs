using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = System.Random;

public class DirectBlendTreeDefaultActiveBenchmark : BenchmarkTask1d
{
	private Animator animator;
	private float[][] data;
	private int[] hashes;
	private int current = 0;
	
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		animator = gameObject.GetOrAddComponent<Animator>();
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test" + i.ToString());
			toggleObject.transform.SetParent(gameObject.transform);
		}
		AnimatorController controller = AnimatorHelpers.SetupSingleDirectBlendTree(iterationNum, true);
		data = new float[100][];
		
		Random r = new Random();
		for (int j = 0; j < 100; j++)
		{
			data[j] = new float[iterationNum];
			for (int i = 0; i < iterationNum; i++)
			{
				data[j][i] = (r.NextDouble() > 0.5) ? 0 : 1;
			}
		}
		
		hashes = new int[iterationNum];
		for (int i = 0; i < iterationNum; i++)
		{
			hashes[i] = Animator.StringToHash(i.ToString());
		}

		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override void RunPlaymode1d(GameObject gameObject, int iterationNum)
	{
		current = (current + 1) % 100;
		for (int i = 0; i < iterationNum; i++)
		{
			if (i > 500) continue;
			animator.SetFloat(hashes[i], data[current][i]);
		}
	}
	
	public override string GetParameterName()
	{
		return "Toggles";
	}
	
	public override string GetName()
	{
		return "ActiveDBTDefaultToggle";
	}

	public override string GetDescription()
	{
		return "Benchmarks active blend tree single toggle count with defaults layer on humanoid avatar";
	}
}