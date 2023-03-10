using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

public class AnimatorHelpers
{
	public static string controllerPath = "Assets/jellejurre/Benchmarker/Assets/Generated/Controllers/";
	public static AnimatorController SetupAnyStateToggle(int layerCount, bool canTransitionToSelf = false)
	{
		string path = canTransitionToSelf ? "AnyStateSelf/" : "AnyState/";
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

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
			onState.writeDefaultValues = true;
			offState.writeDefaultValues = true;
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
			onToOffTransition.canTransitionToSelf = canTransitionToSelf;
			offToOnTransition.canTransitionToSelf = canTransitionToSelf;
			onToOffTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, j.ToString());
			offToOnTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, j.ToString());
			layer.stateMachine = stateMachine;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	public static AnimatorController SetupAnyStateToggle(int layerCount, int stateCount, bool canTransitionToSelf = false)
	{
		string path = canTransitionToSelf ? "AnyStateSelf/" : "AnyState/";
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + $"{path}/{layerCount}_{stateCount}.controller");

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
				secondState.writeDefaultValues = true;
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
				onTransition.canTransitionToSelf = canTransitionToSelf;
			}
			layer.stateMachine.anyStateTransitions = anyStates;
			layer.stateMachine.states = states;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + $"AnyState/{layerCount}_{stateCount}.controller");
		SerializeController(controller);
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
			ChildAnimatorState[] states = new ChildAnimatorState[stateCount + 1];
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
						parameter = j.ToString()
					},
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Less, threshold = (1+i) / ((float)stateCount),
						parameter = j.ToString()
					}
				};
				offTransition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Less, threshold = i / ((float)stateCount), parameter = j.ToString()
					}
				};
				off2Transition.conditions = new[]
				{
					new AnimatorCondition()
					{
						mode = AnimatorConditionMode.Greater, threshold = (i + 1) / (float)stateCount,
						parameter = j.ToString()
					}
				};
				states[i] = new ChildAnimatorState(){state = secondState, position = Vector3.one};
			}

			states[stateCount] =  new ChildAnimatorState(){state = firstState, position = Vector3.one};
			layer.stateMachine.states = states;
			layer.defaultWeight = 1;
			layers[j] = layer;
		}
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller,  controllerPath + $"ManyState/{layerCount}_{stateCount}.controller");
		SerializeController(controller);
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
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	public static AnimatorController SetupTwoTogglesSubStateMachine(int layerCount)
	{
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + "TwoToggleSub/" + layerCount + ".controller");

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
			AnimatorStateMachine subStateMachine = new AnimatorStateMachine();
			stateMachine.AddStateMachine(subStateMachine, new Vector3(1, 1, 1));
			AnimatorState onState = new AnimatorState();
			AnimatorState offState = new AnimatorState();
			onState.name = index + "on";
			offState.name = index + "off";
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+j.ToString(), j);
			onState.motion = anims[0];
			offState.motion = anims[1];
			subStateMachine.AddState(onState, Vector3.one);
			subStateMachine.AddState(offState, Vector3.one);
			AnimatorStateTransition onToOffTransition = new AnimatorStateTransition();
			AnimatorStateTransition offToOnTransition = new AnimatorStateTransition();
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
		AssetDatabase.CreateAsset(controller, controllerPath + "TwoToggleSub/" + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}

	public static void SerializeController(AnimatorController controller)
	{
		foreach (var animatorControllerLayer in controller.layers)
		{
			SerializeStateMachine(controller, animatorControllerLayer.stateMachine);
		}
	}

	public static void SerializeStateMachine(AnimatorController controller, AnimatorStateMachine stateMachine)
	{
		stateMachine.hideFlags = HideFlags.HideInHierarchy;
		AssetDatabase.AddObjectToAsset(stateMachine, controller);
		foreach (var childAnimatorState in stateMachine.states)
		{
			childAnimatorState.state.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(childAnimatorState.state, controller);
			foreach (var animatorStateTransition in childAnimatorState.state.transitions)
			{
				animatorStateTransition.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(animatorStateTransition, controller);
			}
		}
		foreach (var stateMachineAnyStateTransition in stateMachine.anyStateTransitions)
		{
			stateMachineAnyStateTransition.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(stateMachineAnyStateTransition, controller);
		}
		foreach (var childAnimatorStateMachine in stateMachine.stateMachines)
		{
			SerializeStateMachine(controller, childAnimatorStateMachine.stateMachine);
		}
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