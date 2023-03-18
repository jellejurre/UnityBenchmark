using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = System.Random;

public class LayerFaceTrackingActive : BenchmarkTask1d
{
	private Animator[] animators;
	private int[] hashes;
	private float[][] data;
	private bool[] types;
	private bool[] controlled;
	private int current = 0;
	private bool initialized;
	
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject rootObject = new GameObject("test");
		animators = new Animator[iterationNum];
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject gameObject = Instantiate(prefab);
			animators[i] = gameObject.GetOrAddComponent<Animator>();
			gameObject.transform.SetParent(rootObject.transform);
			AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>($"Assets/jellejurre/Benchmarker/Assets/Generated/FaceTracking/FT.controller");
			animators[i].runtimeAnimatorController = controller;
			animators[i].cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}

		int length = animators[0].parameters.Length;
		data = new float[100][];
		Random r = new Random();
		for (int j = 0; j < 100; j++)
		{
			data[j] = new float[length];
			for (int i = 0; i < length; i++)
			{
				data[j][i] = (r.NextDouble() > 0.5) ? 0 : 1;
			}
		}

		types = new bool[length];
		controlled = new bool[length];
		for(int i = 0; i < length; i++)
		{
			types[i] = animators[0].parameters[i].type == AnimatorControllerParameterType.Float;
		}

		hashes = new int[length];
		for (int i = 0; i < length; i++)
		{
			hashes[i] = Animator.StringToHash(animators[0].parameters[i].name);
		}
		
		AssetDatabase.SaveAssets();
		return rootObject;
	}
	
	public override void RunPlaymode1d(GameObject gameObject, int iterationNum)
	{
		if (!initialized)
		{
			for (int j = 0; j < animators[0].parameterCount; j++)
			{
				controlled[j] = animators[0].IsParameterControlledByCurve(hashes[j]);
				if (animators[0].parameters[j].name.Contains("Enable") ||
				    animators[0].parameters[j].name.Contains("Active"))
				{
					controlled[j] = true;
					for (int i = 0; i < animators.Length; i++)
					{
						animators[i].SetBool(hashes[j], true);
					}
				}
			}
			initialized = true;
		}
		current = (current + 1) % 100;
		int currentAnimator = current % animators.Length;
		int length = animators[currentAnimator].parameterCount;
		Animator animator = animators[currentAnimator];
		for (int j = 0; j < length; j++)
		{
			if (controlled[j])
			{
				continue;
			}
			if (types[j])
			{
				animator.SetFloat(hashes[j], data[current][j]);
			}
			else
			{
				animator.SetBool(hashes[j], data[current][j] == 1.0f);
			}
		}
	}
	
	public override string GetParameterName()
	{
		return "Animators";
	}
	
	public override string GetName()
	{
		return "LayerFTActive";
	}

	public override string GetDescription()
	{
		return "Benchmarks active layer-based facetracking on humanoid avatars";
	}
}