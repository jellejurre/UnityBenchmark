using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

public class PhysboneChildCount : BenchmarkTask1d
{
	public override string GetName()
	{
		return "PhysboneChildCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks direct active Physbone child count";
	}

	public override string GetParameterName()
	{
		return "PhysboneTransforms";
	}
	
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject root = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject physbone = PhysboneHelper.AddPhysbone(root, depth:1, amount:1);
			physbone.transform.localPosition = new Vector3(0, 0, 1 + (float)i / iterationNum);
		}
		return root;
	}


	public int i = 0;
	public override void RunPlaymode1d(GameObject prefab, int value)
	{
		i++;
		prefab.transform.position = new Vector3(0, Mathf.Sin(i/100f), 0);
	}
}