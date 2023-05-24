using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

public class ForkedPhysboneChildCount : BenchmarkTask1d
{
	public override string GetName()
	{
		return "ForkedPBChildCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks direct active forked Physbone child count";
	}

	public override string GetParameterName()
	{
		return "PhysboneTransforms";
	}
	
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject root = Instantiate(prefab);
		for (int i = 0; i < iterationNum/16; i++)
		{
			GameObject physbone = PhysboneHelper.AddForkedPhysbone(root, depth:4);
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