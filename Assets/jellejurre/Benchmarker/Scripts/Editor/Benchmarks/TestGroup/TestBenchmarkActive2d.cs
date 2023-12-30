using System;
using UnityEditor.Animations;
using UnityEngine;
using Random = System.Random;

public class TestBenchmarkActive2d : BenchmarkTask2d
{
	private Animator[] animators;
	private float[][] data;
	private int[] hashes;
	private int current = 0;
	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum, int iterationNum2)
	{
		var rootObject = new GameObject();
		animators = new Animator[iterationNum2];
		for (int i = 0; i < iterationNum2; i++)
		{
			var gameObject = Instantiate(prefab, rootObject.transform);
			Animator animator = gameObject.GetOrAddComponent<Animator>();
			for (int j = 0; j < iterationNum; j++)
			{
				GameObject toggleObject = new GameObject("test" + j.ToString());
				toggleObject.transform.SetParent(gameObject.transform);
			}
			AnimatorController controller = new AnimatorController();
			AnimatorHelpers.AddParameters(controller, iterationNum);
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.stateMachine = new AnimatorStateMachine();
			controller.layers = new[] { layer };
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

		hashes = new int[iterationNum];
		for (int i = 0; i < iterationNum; i++)
		{
			hashes[i] = Animator.StringToHash(i.ToString());
		}
		return rootObject;
	}
	
	public override void RunPlaymode2d(GameObject gameObject, int iterationNum, int iterationNum2)
	{
		current = (current + 1) % 100;
		int currentAnimator = current % animators.Length;
		Animator animator = animators[currentAnimator];
		for (int i = 0; i < iterationNum; i++)
		{
			if (i > 500) continue;
			animator.SetFloat(hashes[i], data[current][i]);
		}
	}
	
	public override string[] GetParameterNames()
	{
		return new [] {"Layer", "Controllers"};
	}

	public override string GetName()
	{
		return "TestActiveBenchmark2d";
	}
	
	public override string GetDescription()
	{
		return "Test 2d benchmark which runs the toggle script";
	}
}