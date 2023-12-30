using UnityEditor.Animations;
using UnityEngine;
using Random = System.Random;

public class AnyStateToggle2dActiveBenchmark : BenchmarkTask2d
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
			AnimatorController controller = AnimatorHelpers.SetupAnyStateToggle(iterationNum);
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
		return new [] {"Layers", "Controllers"};
	}
	

	public override string GetName()
	{
		return "AnyStateToggleActive2d";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks active any state toggle layer count on numerous humanoid avatars.\nFirst number is layer amount, second number is controller amount.";
	}
}