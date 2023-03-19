using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BenchmarkRunner : MonoBehaviour
{
	public BenchmarkTask task;
	public GameObject prefab;
	
	public LinkedList<float>[] data;
	public float totalRunTime;
	public int currentIteration;
	public GameObject currentObject;

	public bool restart = false;

	private void Start()
	{
		this.data = new LinkedList<float>[task.iterationCount];
		for (int i = 0; i < task.iterationCount; i++)
		{
			this.data[i] = new LinkedList<float>();
		}
		SetupIteration();
	}

	private void SetupIteration()
	{
		restart = true;
		if (!(currentObject == null))
		{
			GameObject.DestroyImmediate(currentObject);
		}

		if (prefab == null)
		{
			prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/jellejurre/Benchmarker/Assets/Prefabs/Cube.prefab");
		}

		currentObject = task.PrepareIteration(prefab, task.GetIterationNumber(currentIteration));
	}

	private void Update()
	{
		if (task == null)
		{
			return;
		}

		if (restart)
		{
			restart = false;
			totalRunTime = 0;
		}
		
		totalRunTime += Time.deltaTime;
		if (totalRunTime >= task.initializationTime)
		{
			data[currentIteration].AddLast(Time.deltaTime);
		}
		
		if (totalRunTime >= task.initializationTime + task.benchmarkTime)
		{
			currentIteration++;
			if (currentIteration == task.iterationCount)
			{
				gameObject.SetActive(false);
				BenchmarkManager.ProcessBenchmarkData(new BenchmarkData(data));
			}
			else
			{
				SetupIteration();
			}
		}
	}
}