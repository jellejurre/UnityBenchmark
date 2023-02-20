﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class BenchmarkManager
{
	public static bool runAll = false;
	
	private static string oldSceneName;
	private static string lastSelectedObjectPath;

	private static BenchmarkTask currentTask;
	private static GameObject currentPrefab;
	static BenchmarkManager()
	{
		SceneManager.activeSceneChanged += RunBenchmark;
		SceneManager.activeSceneChanged += SelectOldObject;
		EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(new EditorBuildSettingsScene("Assets/jellejurre/Benchmarker/Assets/BenchmarkScene.unity", true)).ToArray();
	}

	public static void LoadSceneAndRun(BenchmarkTask task)
	{
		currentTask = task;
		currentPrefab = task.prefab;
		Scene oldScene = SceneManager.GetActiveScene(); 
		oldSceneName = oldScene.name;
		lastSelectedObjectPath = GetGameObjectPath(Selection.activeGameObject);
		if (oldScene.isDirty)
		{
			Debug.LogError("Can't run benchmark with unsaved scene. Please save your scene before running.");
			return;
		}
		try
		{
			SceneManager.LoadScene("BenchmarkScene");
		}
		catch
		{
			SceneManager.LoadScene(oldSceneName, LoadSceneMode.Single);
			throw;
		}
	}

	public static void RunAll()
	{
		runAll = true;
		LoadSceneAndRun(BenchmarkRepository.BenchmarkTasks.First());
	}
	
	public static void RunBenchmark(Scene s1, Scene s2)
	{
		if (s2.name != "BenchmarkScene")
		{
			return;
		}
		GameObject runnerObject = new GameObject("BenchmarkDataCollector");
		BenchmarkRunner runner = runnerObject.AddComponent<BenchmarkRunner>();
		runner.task = currentTask;
		runner.prefab = currentPrefab;
		Selection.activeObject = runnerObject;
	}
	

	public static void ProcessBenchmarkData(BenchmarkData data)
	{
		string[] debug = currentTask.FormatDebug(data);
		foreach (var s in debug)
		{
			Debug.Log(s);
		}
		string[] results = currentTask.FormatResults(data);
		StreamWriter writer = new StreamWriter($"Assets/jellejurre/Benchmarker/Output/{currentTask.GetName()}.txt");
		writer.Write(String.Join("\n", results));
		writer.Flush();
		writer.Close();
		AssetDatabase.SaveAssets();
		SceneManager.LoadScene(oldSceneName, LoadSceneMode.Single);
		if (runAll)
		{
			int index = BenchmarkRepository.BenchmarkTasks.IndexOf(currentTask);
			if (index == BenchmarkRepository.BenchmarkTasks.Count - 1)
			{
				runAll = false;
			}
			else
			{
				LoadSceneAndRun(BenchmarkRepository.BenchmarkTasks[index + 1]);
			}
		}
	}
	
	public static void SelectOldObject(Scene s1, Scene s2)
	{
		if (s2.name == oldSceneName)
		{
			Selection.activeGameObject = GameObject.Find(lastSelectedObjectPath);
		}
	}
	
	public static string GetGameObjectPath(GameObject obj)
	{
		string path = "/" + obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			path = "/" + obj.name + path;
		}
		return path;
	}		
}