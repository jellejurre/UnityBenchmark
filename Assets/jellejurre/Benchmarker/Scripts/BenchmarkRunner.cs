using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class BenchmarkRunner : MonoBehaviour
{
	public BenchmarkTask task;
	public GameObject prefab;
	
	public LinkedList<float>[] data;
	public float totalRunTime;
	public int currentIteration;
	public GameObject currentObject;
	private Random r;
	private bool restart = false;

	private void Start()
	{
		this.data = new LinkedList<float>[task.GetMaxIterations()];
		for (int i = 0; i < task.GetMaxIterations(); i++)
		{
			this.data[i] = new LinkedList<float>();
		}

		GameObject camera = new GameObject("Camera", new []{typeof(Camera)});
		camera.transform.position = new Vector3(0, 0, -10);
		camera.transform.rotation = Quaternion.Euler(0, 0, 0);
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

		currentObject = task.PrepareIteration(prefab, currentIteration);
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
		
		task.RunPlaymode(currentObject, currentIteration);

		if (totalRunTime >= task.initializationTime + task.benchmarkTime)
		{
			currentIteration++;
			if (currentIteration == task.GetMaxIterations())
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