using System;
using UnityEditor.Animations;
using UnityEngine;
using Random = System.Random;

public class TestBenchmarkActive : BenchmarkTask1d
{
	private Animator animator;
	private float[][] data;
	private int[] hashes;
	private int current = 0;
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		animator = gameObject.GetOrAddComponent<Animator>();
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

		AnimatorController controller = new AnimatorController();
		AnimatorHelpers.AddParameters(controller, iterationNum);
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
		layer.stateMachine = new AnimatorStateMachine();
		controller.layers = new[] { layer };
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
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
		return "Layers";
	}

	public override string GetName()
	{
		return "TestActiveBenchmark";
	}
	
	public override string GetDescription()
	{
		return "Test benchmark which runs the toggle script";
	}
}