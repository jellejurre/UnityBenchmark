using UnityEditor.Animations;
using UnityEngine;

public class BigTwoStateToggle2dBenchmark : BenchmarkTask2d
{
	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum, int iterationNum2)
	{
		var rootObject = new GameObject();
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
		}

		return rootObject;
	}
	
	public override string[] GetParameterNames()
	{
		return new [] {"Layers", "Controllers"};
	}
	

	public override string GetName()
	{
		return "BigTwoStateToggle2d";
	}
	
	public override string GetDescription()
	{
		return "Benchmarks two state toggle layer count on numerous humanoid avatars with one big avatar in the lobby.\nFirst number is layer amount, second number is controller amount.";
	}
}