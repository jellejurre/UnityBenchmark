using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Avatars.Components;

public class AudioSourceBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject audioObject = new GameObject("test"+ i.ToString());
			audioObject.transform.parent = gameObject.transform;
			audioObject.AddComponent<VRCSpatialAudioSource>();
			AudioSource source = audioObject.GetComponent<AudioSource>();
			source.clip = AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/jellejurre/Benchmarker/Assets/Files/bat funny {Math.Min(i, 64)}.mp3");
			source.loop = true;
			source.volume = 0.01f;
		}
		gameObject.SetActive(false);
		gameObject.SetActive(true);
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Audio Sources";
	}
	
	public override string GetName()
	{
		return "AudioSource";
	}

	public override string GetDescription()
	{
		return "Benchmarks audio source count on avatar";
	}
}