using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraintActive2SourcesBenchmark : BenchmarkTask1d
{
	private GameObject[] sources;
	private float iter;
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		sources = new GameObject[iterationNum];
		iter = iterationNum;
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			GameObject sourceChild = new GameObject("source"+ i.ToString());
			sourceChild.transform.parent = gameObject.transform;
			GameObject source2Child = new GameObject("secondSource"+ i.ToString());
			source2Child.transform.parent = gameObject.transform;
			sources[i] = sourceChild;
			for (int j = 0; j < 100; j++){
				GameObject lag = new GameObject("lag"+ i.ToString() + j.ToString());
				lag.transform.parent = toggleObject.transform;
			}
			ParentConstraint constraint = toggleObject.AddComponent<ParentConstraint>();
			ConstraintSource source = new ConstraintSource();
			source.weight = 1;
			source.sourceTransform = sourceChild.transform;
			ConstraintSource source2 = new ConstraintSource();
			source2.weight = 0.5f;
			source2.sourceTransform = source2Child.transform;
			constraint.AddSource(source);
			constraint.AddSource(source2);
			constraint.enabled = true;
			constraint.constraintActive = true;
		}
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Active Parent Constraints 2 Sources";
	}
	
	public override string GetName()
	{
		return "Active2SourcesParentConstraint";
	}

	private int j = 0;
	public override void RunPlaymode1d(GameObject prefab, int value)
	{
		j++;
		for (int i = 0; i < iter; i++)
		{
			sources[i].transform.localPosition = new Vector3(Mathf.Sin(i + j), 0, 0);
			sources[i].transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Sin(i + j), 0, 0));
		}
	}

	public override string GetDescription()
	{
		return "Benchmarks active 2 source parent constraint count on avatar";
	}
}