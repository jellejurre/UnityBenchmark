using UnityEditor.Animations;
using UnityEngine;

public class DirectBlendTreeToggle2dBenchmark : BenchmarkTask2d
{
	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum, int iterationNum2)
	{
		var rootObject = new GameObject();
		for (int i = 0; i < iterationNum2; i++)
		{
			var gameObject = Instantiate(prefab, rootObject.transform);
			Animator animator = gameObject.GetOrAddComponent<Animator>();
			for (int j = 0; j < iterationNum; j++)
			{
				GameObject toggleObject = new GameObject("test" + j.ToString());
				toggleObject.transform.SetParent(gameObject.transform);
			}
			AnimatorController controller = AnimatorHelpers.SetupDirectBlendTree(iterationNum);
			animator.runtimeAnimatorController = controller;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}

		return rootObject;
	}
	
	public override string[] GetParameterNames()
	{
		return new [] {"Layers", "Controllers"};
	}
	

	public override string GetName()
	{
		return "DBTToggle2d";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks direct blend tree toggle layer count on numerous humanoid avatars.\nFirst number is layer amount, second number is controller amount.";
	}
}