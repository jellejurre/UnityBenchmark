﻿using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

public class AnimatorHelpers
{
	private static string controllerPath = "Assets/jellejurre/Benchmarker/Assets/Generated/Controllers/";
	public static AnimatorController SetupAnyStateToggle(int layerCount)
	{
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + "AnyState/" + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AnimatorControllerParameter[] parameters = new AnimatorControllerParameter[layers.Length];
		AnimatorStateTransition[] transitions = new AnimatorStateTransition[layers.Length * 2];
		AddParameters(controller, layerCount);
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			AnimatorState onState = new AnimatorState();
			AnimatorState offState = new AnimatorState();
			onState.name = index + "on";
			offState.name = index + "off";
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			onState.motion = anims[0];
			offState.motion = anims[1];
			stateMachine.AddState(onState, Vector3.one);
			stateMachine.AddState(offState, Vector3.one);
			AnimatorStateTransition onToOffTransition = stateMachine.AddAnyStateTransition(offState);
			AnimatorStateTransition offToOnTransition = stateMachine.AddAnyStateTransition(onState);
			transitions[j * 2] = onToOffTransition;
			transitions[j * 2 + 1] = offToOnTransition;
			onToOffTransition.exitTime = 1;
			offToOnTransition.exitTime = 1;
			onToOffTransition.hasExitTime = true;
			offToOnTransition.hasExitTime = true;
			onToOffTransition.destinationState = offState;
			offToOnTransition.destinationState = onState;
			onToOffTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, j.ToString());
			offToOnTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, j.ToString());
			layer.stateMachine = stateMachine;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + "AnyState/" + layerCount + ".controller");
		foreach (var animatorControllerLayer in controller.layers)
		{
			animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(animatorControllerLayer.stateMachine, controller);
			foreach (var childAnimatorState in animatorControllerLayer.stateMachine.states)
			{
				childAnimatorState.state.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(childAnimatorState.state, controller);
			}
		}
		foreach (var animatorStateTransition in transitions)
		{
			animatorStateTransition.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(animatorStateTransition, controller);
		}
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	public static AnimatorController SetupAnyStateToggle(int layerCount, int stateCount)
	{
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + $"AnyState/{layerCount}_{stateCount}.controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AddParameters(controller, layerCount);
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			layer.stateMachine = stateMachine;
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			AnimatorStateTransition[] anyStates = new AnimatorStateTransition[stateCount];
			ChildAnimatorState[] states = new ChildAnimatorState[stateCount];
			for (int i = 0; i < stateCount; i++)
			{
				AnimatorState secondState = new AnimatorState();
				secondState.name = "state" + i;
				secondState.writeDefaultValues = false;
				secondState.motion = anims[i%2];
				var onTransition = new AnimatorStateTransition();
				onTransition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Greater,
						threshold = i / ((float)stateCount),
						parameter = j.ToString()
					},
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Less,
						threshold = i+1 / ((float)stateCount),
						parameter = j.ToString()
					}
				};
				onTransition.destinationState = secondState;
				anyStates[i] = onTransition;
				states[i] = new ChildAnimatorState(){state = secondState, position = Vector3.one};
				onTransition.canTransitionToSelf = true;
			}
			layer.stateMachine.anyStateTransitions = anyStates;
			layer.stateMachine.states = states;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + $"AnyState/{layerCount}_{stateCount}.controller");
		foreach (var animatorControllerLayer in controller.layers)
		{
			animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(animatorControllerLayer.stateMachine, controller);
			foreach (var childAnimatorState in animatorControllerLayer.stateMachine.states)
			{
				childAnimatorState.state.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(childAnimatorState.state, controller);
			}
			foreach (var anyTransition in animatorControllerLayer.stateMachine.anyStateTransitions)
			{
				anyTransition.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(anyTransition, controller);
			}
		}
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	public static AnimatorController SetupManyStateToggle(int layerCount, int stateCount)
	{
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + $"ManyState/{layerCount}_{stateCount}.controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AnimatorStateTransition[] transitions = new AnimatorStateTransition[layers.Length * stateCount * 3];
		AddParameters(controller, layerCount);
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			layer.stateMachine = stateMachine;
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			AnimatorState firstState = new AnimatorState();
			firstState.name = "state1";
			firstState.writeDefaultValues = false;
			firstState.motion = anims[0];
			ChildAnimatorState[] states = new ChildAnimatorState[stateCount];
			for (int i = 0; i < stateCount; i++)
			{
				AnimatorState secondState = new AnimatorState();
				secondState.name = "state" + i;
				secondState.writeDefaultValues = false;
				secondState.motion = anims[1];
				AnimatorStateTransition onTransition = new AnimatorStateTransition();
				onTransition.destinationState = secondState;
				firstState.transitions = new[] { onTransition };
				AnimatorStateTransition offTransition = new AnimatorStateTransition();
				AnimatorStateTransition off2Transition = new AnimatorStateTransition();
				offTransition.destinationState = firstState;
				off2Transition.destinationState = firstState;
				secondState.transitions = new[] { off2Transition, offTransition };
				transitions[3 * (j * stateCount + i)] = onTransition;
				transitions[3 * (j * stateCount + i) + 1] = offTransition;
				transitions[3 * (j * stateCount + i) + 2] = off2Transition;
				onTransition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Greater, threshold = i / ((float)stateCount),
						parameter = i.ToString()
					},
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Less, threshold = (1+i) / ((float)stateCount),
						parameter = i.ToString()
					}
				};
				offTransition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Less, threshold = i / ((float)stateCount), parameter = i.ToString()
					}
				};
				off2Transition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Greater, threshold = (i + 1) / (float)stateCount,
						parameter = i.ToString()
					}
				};
				states[i] = new ChildAnimatorState(){state = secondState, position = Vector3.one};
			}

			layer.stateMachine.states = states;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller,  $"ManyState/{layerCount}_{stateCount}.controller");
		foreach (var animatorControllerLayer in controller.layers)
		{
			animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(animatorControllerLayer.stateMachine, controller);
			foreach (var childAnimatorState in animatorControllerLayer.stateMachine.states)
			{
				childAnimatorState.state.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(childAnimatorState.state, controller);
			}
		}
		foreach (var animatorStateTransition in transitions)
		{
			animatorStateTransition.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(animatorStateTransition, controller);
		}
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	public static AnimatorController SetupTwoToggles(int layerCount)
	{
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + "TwoToggle/" + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		
		AssetDatabase.StartAssetEditing();
		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AddParameters(controller, layerCount);
		AnimatorStateTransition[] transitions = new AnimatorStateTransition[layers.Length * 2];
		for (int j = 0; j < layers.Length; j++)
		{
			string index = j.ToString();
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			layer.name = index;
			AnimatorStateMachine stateMachine = new AnimatorStateMachine();
			AnimatorState onState = new AnimatorState();
			AnimatorState offState = new AnimatorState();
			onState.name = index + "on";
			offState.name = index + "off";
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			onState.motion = anims[0];
			offState.motion = anims[1];
			stateMachine.AddState(onState, Vector3.one);
			stateMachine.AddState(offState, Vector3.one);
			AnimatorStateTransition onToOffTransition = new AnimatorStateTransition();
			AnimatorStateTransition offToOnTransition = new AnimatorStateTransition();
			transitions[j * 2] = onToOffTransition;
			transitions[j * 2 + 1] = offToOnTransition;
			onToOffTransition.exitTime = 1;
			offToOnTransition.exitTime = 1;
			onToOffTransition.hasExitTime = true;
			offToOnTransition.hasExitTime = true;
			onToOffTransition.destinationState = offState;
			offToOnTransition.destinationState = onState;
			onToOffTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, j.ToString());
			offToOnTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, j.ToString());
			onState.AddTransition(onToOffTransition);
			offState.AddTransition(offToOnTransition);
			layer.stateMachine = stateMachine;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + "TwoToggle/" + layerCount + ".controller");
		foreach (var animatorControllerLayer in controller.layers)
		{
			animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(animatorControllerLayer.stateMachine, controller);
			foreach (var childAnimatorState in animatorControllerLayer.stateMachine.states)
			{
				childAnimatorState.state.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(childAnimatorState.state, controller);
			}
		}
		foreach (var animatorStateTransition in transitions)
		{
			animatorStateTransition.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(animatorStateTransition, controller);
		}
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}

	public static void RandomiseParameters(AnimatorController controller)
	{
		AnimatorControllerParameter[] parameters = controller.parameters;
		foreach (var animatorControllerParameter in parameters)
		{
			animatorControllerParameter.defaultFloat = Random.value;
		}
		controller.parameters = parameters;
	}

	public static void AddParameters(AnimatorController controller, int count)
	{
		AnimatorControllerParameter[] parameters = new AnimatorControllerParameter[count];
		for (int j = 0; j < count; j++)
		{
			string index = j.ToString();
			AnimatorControllerParameter parameter = new AnimatorControllerParameter();
			parameter.type = AnimatorControllerParameterType.Float;
			parameter.name = index;
			parameters[j] = parameter;
		}

		controller.parameters = parameters;
	}
}