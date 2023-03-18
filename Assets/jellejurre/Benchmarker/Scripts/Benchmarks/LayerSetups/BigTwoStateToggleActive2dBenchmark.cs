using System;
using UnityEditor.Animations;
using UnityEngine;
using Random = System.Random;

public class BigTwoStateToggleActive2dBenchmark : BenchmarkTask2d
{
	private Animator[] animators;
	private float[][] data;
	private float[] bigData;
	private int[] hashes;
	private int current = 0;
	private bool cycle = false;
	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum, int iterationNum2)
	{
		var rootObject = new GameObject();
		animators = new Animator[iterationNum2 + 1];
		for (int i = 0; i < iterationNum2 + 1; i++)
		{
			int innerval = iterationNum;
			if (i == iterationNum2)
			{
				innerval = 534;
			}
			var gameObject = Instantiate(prefab, rootObject.transform);
			Animator animator = gameObject.GetOrAddComponent<Animator>();
			for (int j = 0; j < innerval; j++)
			{
				GameObject toggleObject = new GameObject("test" + j.ToString());
				toggleObject.transform.SetParent(gameObject.transform);
			}
			AnimatorController controller = AnimatorHelpers.SetupTwoToggles(innerval);
			animator.runtimeAnimatorController = controller;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animators[i] = animator;
		}		
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

		bigData = new float[534];
		for (int i = 0; i < 534; i++)
		{
			bigData[i] = (r.NextDouble() > 0.5) ? 0 : 1;
		}
		
		hashes = new int[Math.Max(534, iterationNum)];
		for (int i = 0; i < Math.Max(534, iterationNum); i++)
		{
			hashes[i] = Animator.StringToHash(i.ToString());
		}
		return rootObject;
	}
	
	public override void RunPlaymode2d(GameObject gameObject, int iterationNum, int iterationNum2)
	{
		cycle = !cycle;
		current = (current + 1) % 100;
		int currentAnimator = current % animators.Length;
		Animator animator = animators[currentAnimator];
		if (currentAnimator == iterationNum2)
		{
			for (int i = 0; i < 534; i++)
			{
				if (i > 500) continue;
				animator.SetFloat(hashes[i], cycle ? bigData[i] : 1-bigData[i]);
			}		
		}
		else
		{
			for (int i = 0; i < iterationNum; i++)
			{
				if (i > 500) continue;
				animator.SetFloat(hashes[i], data[current][i]);
			}
		}
	}
	
	public override string[] GetParameterNames()
	{
		return new [] {"Layer", "Controllers"};
	}

	public override string GetName()
	{
		return "BigTwoStateActive2d";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks active two state toggle layer count on numerous humanoid avatars with one big avatar in the lobby.\nFirst number is layer amount, second number is controller amount.";
	}
}