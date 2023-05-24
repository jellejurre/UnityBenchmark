using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.PhysBone.Components;

public class PhysboneChildCountDepth : BenchmarkTask2d
{
	public override string GetName()
	{
		return "PhysboneChildCountDepth";
	}

	public override string GetDescription()
	{
		return "Benchmarks direct active Physbone child count vs depth";
	}

	public override string[] GetParameterNames()
	{
		return new [] {"PhysboneTransforms", "PhysboneDepth"};
	}
	
	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum1, int iterationNum2)
	{
		GameObject root = Instantiate(prefab);
		for (int i = 0; i < iterationNum1/64; i++)
		{
			GameObject physbone = PhysboneHelper.AddPhysbone(root, depth: iterationNum2);
			physbone.transform.localPosition = new Vector3(0, 0, 1 + (float)i / iterationNum1);
		}
		return root;
	}


	public int i = 0;
	public override void RunPlaymode2d(GameObject prefab, int value, int value2)
	{
		i++;
		prefab.transform.position = new Vector3(0, Mathf.Sin(i/100f), 0);
	}
}