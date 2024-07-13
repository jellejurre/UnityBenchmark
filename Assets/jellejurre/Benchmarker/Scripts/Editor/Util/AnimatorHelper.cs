using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static ControllerGenerationMethods;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using Random = UnityEngine.Random;

public class AnimatorHelpers
{
	public static string controllerPath = "Assets/jellejurre/Benchmarker/Assets/Generated/Controllers/";

	public static void ReadyPath(string folderPath)
	{
		folderPath = folderPath.Substring(0 , folderPath.Length - 1);
		if (System.IO.Directory.Exists(folderPath)) return;
		System.IO.Directory.CreateDirectory(controllerPath.Substring(0, controllerPath.Length - 1));
		AssetDatabase.ImportAsset(controllerPath.Substring(0, controllerPath.Length - 1));
		System.IO.Directory.CreateDirectory(folderPath);
		AssetDatabase.ImportAsset(folderPath);
	}
	
	#region AnyState
	public static AnimatorController SetupAnyStateToggle(int layerCount, bool canTransitionToSelf = false, bool writeDefaults = true)
	{
		string path = writeDefaults ? canTransitionToSelf ? "AnyStateSelf/" : "AnyState/" : "AnyStateWDOff/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
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
			onState.writeDefaultValues = writeDefaults;
			offState.writeDefaultValues = writeDefaults;
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
		ReadyPath(controllerPath + path);
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
		AssetDatabase.CreateAsset(controller, controllerPath + $"{path}{layerCount}_{stateCount}.controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
		public static AnimatorController SetupAnyStateEdgeCase(int stateCount)
	{
		string path = "AnyStateEC/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + $"{path}/{stateCount}.controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[1];
		AddParameters(controller, stateCount, true);
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
		layer.name = 1.ToString();
		AnimatorStateMachine stateMachine = new AnimatorStateMachine();
		layer.stateMachine = stateMachine;
		AnimatorStateTransition[] anyStates = new AnimatorStateTransition[stateCount];
		ChildAnimatorState[] states = new ChildAnimatorState[stateCount];
		for (int i = 0; i < stateCount; i++)
		{
			AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+i.ToString(), i);
			AnimatorState secondState = new AnimatorState();
			secondState.name = "state" + i;
			secondState.writeDefaultValues = true;
			secondState.motion = anims[i%2];
			var onTransition = new AnimatorStateTransition();
			onTransition.conditions = new[]
			{
				new AnimatorCondition()
				{
					mode = AnimatorConditionMode.If,
					parameter = i.ToString()
				}
			};
			onTransition.hasExitTime = false;
			onTransition.duration = 0;
			onTransition.destinationState = secondState;
			anyStates[i] = onTransition;
			states[i] = new ChildAnimatorState(){state = secondState, position = Vector3.one};
			onTransition.canTransitionToSelf = false;
		}
		layer.stateMachine.anyStateTransitions = anyStates;
		layer.stateMachine.states = states;
		layer.defaultWeight = 1;
		layers[0] = layer;
		controller.layers = layers;
		AssetDatabase.CreateAsset(controller, controllerPath + $"{path}{stateCount}.controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	#endregion

	#region nonAnyState

	public static AnimatorController SetupManyStateToggleDelayed(int layerCount, int stateCount)
	{
		ReadyPath(controllerPath + "ManyStateDelayed/");
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + $"ManyStateDelayed/{layerCount}_{stateCount}.controller");

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
				AnimationClip[] animsDelayed = AnimationHelper.GetOrCreateTwoStateToggleDelayed("test"+j.ToString(), j, i);
				AnimatorState secondState = new AnimatorState();
				secondState.name = "state" + i;
				secondState.writeDefaultValues = false;
				secondState.motion = animsDelayed[1];
				AnimatorStateTransition onTransition = new AnimatorStateTransition();
				onTransition.destinationState = secondState;
				firstState.transitions = new[] { onTransition };
				AnimatorStateTransition offTransition = new AnimatorStateTransition();
				AnimatorStateTransition off2Transition = new AnimatorStateTransition();
				offTransition.destinationState = firstState;
				off2Transition.destinationState = secondState;
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
		AssetDatabase.CreateAsset(controller,  controllerPath + $"ManyStateDelayed/{layerCount}_{stateCount}.controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParameters(controller);
		return controller;
	}
	
	public static AnimatorController SetupManyStateToggle(int layerCount, int stateCount)
	{
		ReadyPath(controllerPath + "ManyState/");
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

	public static AnimatorController SetupTwoToggles(int layerCount, bool writeDefaults = true, bool b = false)
	{
		string path = b ? "TwoToggleBool/" : writeDefaults ? "TwoToggle/" : "TwoToggleWDOff/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = 
			AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParameters(controller);
			return controller;
		}
		
		AssetDatabase.StartAssetEditing();
		controller = new AnimatorController();
		AnimatorControllerLayer[] layers = new AnimatorControllerLayer[layerCount];
		AddParameters(controller, layerCount, b);
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
			onState.writeDefaultValues = writeDefaults;
			offState.writeDefaultValues = writeDefaults;
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
			if (b)
			{
				onToOffTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, j.ToString());
				offToOnTransition.AddCondition(AnimatorConditionMode.If, 0.5f, j.ToString());
			}
			else
			{
				onToOffTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, j.ToString());
				offToOnTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, j.ToString());	
			}
			onState.AddTransition(onToOffTransition);
			offState.AddTransition(offToOnTransition);
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
	
	public static AnimatorController SetupTwoTogglesSubStateMachine(int layerCount)
	{
		ReadyPath(controllerPath + "TwoToggleSub/");
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
	
	#endregion

	#region DirectBlendTrees

	public static AnimatorController SetupDirectBlendTree(int layerCount)
	{
		string path = "DBT/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParametersDBT(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AddParameters(controller, layerCount);
		AnimatorControllerParameter oneParameter = new AnimatorControllerParameter();
		oneParameter.name = "one";
		oneParameter.defaultFloat = 1;
		oneParameter.type = AnimatorControllerParameterType.Float;
		controller.parameters = (new[] { oneParameter }).Concat(controller.parameters).ToArray();
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
        layer.defaultWeight = 1;
        layer.name = "ToggleTree";
        layer.stateMachine = new AnimatorStateMachine();
        AnimatorState treeState = new AnimatorState();
        treeState.name = "ToggleTree";
        BlendTree bigTree = new BlendTree();
        bigTree.name = "ToggleTree";
        bigTree.blendType = BlendTreeType.Direct;
        for (int i = 0; i < layerCount; i++)
        {
            BlendTree child = new BlendTree();
            child.name = i.ToString();
            child.blendType = BlendTreeType.Simple1D;
            child.blendParameter = i.ToString();
            AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+i.ToString(), i);
            child.AddChild(anims[1]);
            child.AddChild(anims[0]);
            child.hideFlags = HideFlags.HideInHierarchy;
            bigTree.AddChild(child);
        }
        ChildMotion[] childMotions = bigTree.children;
        for (var i = 0; i < childMotions.Length; i++)
        {
            childMotions[i].directBlendParameter = "one";
        }
        bigTree.children = childMotions;
        treeState.motion = bigTree;
        layer.stateMachine.AddState(treeState, Vector3.one);
		controller.layers = new []{layer};
		AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParametersDBT(controller);
		return controller;
	}
	
	public static AnimatorController SetupExponentialBlendTree(int layerCount)
	{
		string path = "DBTExp/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		controller.parameters = Enumerable.Range(0, layerCount)
			.Select(x => GenerateFloatParameter(x.ToString()))
			.Concat(Enumerable.Range(0, layerCount).Select(x => GenerateFloatParameter("Output"+x.ToString())))
			.Append(GenerateFloatParameter("SmoothAmount", defaultFloat: 0.9f))
			.Append(GenerateFloatParameter("one", 1))
			.ToArray();

		List<BlendTree> trees = new List<BlendTree>();
		ReadyPath(AnimationHelper.animationPath + "DBT/");

		for (int i = 0; i < layerCount; i++)
		{
			string onName = AnimationHelper.animationPath + "DBT/GameObjectOn" + i + "Output" + i + ".anim";
			string offName = AnimationHelper.animationPath + "DBT/GameObjectOff" + i + "Output" + i + ".anim";
			AnimationClip onClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(onName);
			AnimationClip offClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(offName);

			if (onClip == null)
			{
				onClip = GenerateClip($"Output{i}On");
				AddCurve(onClip, "test" + i, typeof(GameObject), "m_IsActive", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 1f), GenerateKeyFrame(time: 0.01666667f, value: 1f) }));
				AddCurve(onClip, "", typeof(UnityEngine.Animator), "Output" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 1f), GenerateKeyFrame(time: 0.01666667f, value: 1f) }));

				offClip = GenerateClip($"Output{i}Off");
				AddCurve(offClip, "test" + i, typeof(GameObject), "m_IsActive", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 0f), GenerateKeyFrame(time: 0.01666667f, value: 0) }));
				AddCurve(offClip, "", typeof(UnityEngine.Animator), "Output" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 0f), GenerateKeyFrame(time: 0.01666667f, value: 0f)}));

				AssetDatabase.CreateAsset(onClip, onName);
				AssetDatabase.CreateAsset(offClip, offName);
			}

			BlendTree TreeSmoothedValueValue = GenerateBlendTree("Smoothed Value = Value", BlendTreeType.Simple1D, blendParameter: i.ToString(), maxThreshold: 1, minThreshold: 0);
			TreeSmoothedValueValue.children = new ChildMotion[] {
				GenerateChildMotion(offClip,  threshold: 0f, directBlendParameter: "Value"), 
				GenerateChildMotion(onClip,  threshold: 1f, directBlendParameter: "Value")
			};

			BlendTree TreeSmoothedValueSmoothedValue = GenerateBlendTree("Smoothed Value = Smoothed Value", BlendTreeType.Simple1D, blendParameter: "Output" + i, maxThreshold: 1, minThreshold: 0);

			TreeSmoothedValueSmoothedValue.children = new ChildMotion[] {
				GenerateChildMotion(offClip,  threshold: 0f, directBlendParameter: "Value"), 
				GenerateChildMotion(onClip,  threshold: 1f, directBlendParameter: "Value")
			};

			BlendTree TreeExponentialSmoothing = GenerateBlendTree("Exponential Smoothing" + i, BlendTreeType.Simple1D, blendParameter: "SmoothAmount", maxThreshold: 1f); 
			TreeExponentialSmoothing.children = new ChildMotion[] {
				GenerateChildMotion(TreeSmoothedValueValue,  threshold: 0f), 
				GenerateChildMotion(TreeSmoothedValueSmoothedValue,  threshold: 1f)
			};
			trees.Add(TreeExponentialSmoothing);
		}
		
		BlendTree BigBlendTree = GenerateBlendTree("Parent Blend Tree", BlendTreeType.Direct);
		BigBlendTree.children = trees.Select(x => GenerateChildMotion(x, directBlendParameter: "one")).ToArray();
		
		AnimatorState StateExponentialSmoothing = GenerateState("Exponential Smoothing", motion: BigBlendTree);
		
		ChildAnimatorState[] states = new ChildAnimatorState[] {
			GenerateChildState(new Vector3(280f, 120f, 0f), StateExponentialSmoothing)
		};

		AnimatorStateMachine StateMachineExponentialSmoothingLayer = GenerateStateMachine("Exponential Smoothing Layer", new Vector3(50f, 20f, 0f), new Vector3(50f, 120f, 0f), new Vector3(800f, 120f, 0f), states: states, defaultState: StateExponentialSmoothing);
		AnimatorControllerLayer ExponentialSmoothingLayer = GenerateLayer("Exponential Smoothing Layer", StateMachineExponentialSmoothingLayer);
        
		controller.layers = new []{ExponentialSmoothingLayer};
		AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		return controller;
	}
	
	
	public static AnimatorController SetupLinearBlendTree(int layerCount)
	{
		string path = "DBTLinear/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		controller.parameters = Enumerable.Range(0, layerCount)
			.Select(x => GenerateFloatParameter(x.ToString()))
			.Concat(Enumerable.Range(0, layerCount).Select(x => GenerateFloatParameter("Output"+x.ToString())))
			.Concat(Enumerable.Range(0, layerCount).Select(x => GenerateFloatParameter("InputOutputDelta"+x.ToString())))
			.Append(GenerateFloatParameter("One", 1))
			.Append(GenerateFloatParameter("StepSize", defaultFloat: 0.05f))
			.Append(GenerateFloatParameter("Time"))
			.Append(GenerateFloatParameter("LastTime"))
			.Append(GenerateFloatParameter("FrameTime"))
			.ToArray();

		
		ReadyPath(AnimationHelper.animationPath + "DBT/");
		string timeName = AnimationHelper.animationPath + "DBT/Time.anim";
		string time1Name = AnimationHelper.animationPath + "DBT/Time1.anim";
		string timeMinus1Name = AnimationHelper.animationPath + "DBT/Time-1.anim";
		string lastTime1Name = AnimationHelper.animationPath + "DBT/LastTime1.anim";
		AnimationClip timeClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(timeName);
		AnimationClip time1Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(time1Name);
		AnimationClip timeMinus1Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(timeMinus1Name);
		AnimationClip lastTime1Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(lastTime1Name);

		if (timeClip == null)
		{
			timeClip = GenerateClip("Time");
			AddCurve(timeClip, "", typeof(UnityEngine.Animator), "Time", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(outTangent: 1f), GenerateKeyFrame(time: 20000f, value: 20000f, inTangent: 1f) }));

			time1Clip = GenerateClip("FrameTime1");
			AddCurve(time1Clip, "", typeof(UnityEngine.Animator), "FrameTime", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 1f), GenerateKeyFrame(time: 0.01666667f, value: 1f) }));

			timeMinus1Clip = GenerateClip("FrameTime-1");
			AddCurve(timeMinus1Clip, "", typeof(UnityEngine.Animator), "FrameTime", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: -1f), GenerateKeyFrame(time: 0.01666667f, value: -1f) }));

			lastTime1Clip = GenerateClip("LastTime1");
			AddCurve(lastTime1Clip, "", typeof(UnityEngine.Animator), "LastTime", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 1f), GenerateKeyFrame(time: 0.01666667f, value: 1f) }));
			
			AssetDatabase.CreateAsset(timeClip, timeName);
			AssetDatabase.CreateAsset(time1Clip, time1Name);
			AssetDatabase.CreateAsset(timeMinus1Clip, timeMinus1Name);
			AssetDatabase.CreateAsset(lastTime1Clip, lastTime1Name);
		}


		AnimatorState StateTime = GenerateState("Time", writeDefaultValues: true, motion: timeClip);

		ChildAnimatorState[] timeStates = new ChildAnimatorState[] {
			GenerateChildState(new Vector3(310f, 110f, 0f), StateTime)
		};
		AnimatorStateMachine StateMachineTime = GenerateStateMachine("Time", new Vector3(50f, 20f, 0f), new Vector3(50f, 120f, 0f), new Vector3(800f, 120f, 0f), states: timeStates, defaultState: StateTime);
		AnimatorControllerLayer LayerTime = GenerateLayer("Time", StateMachineTime, defaultWeight: 0f);
		
		
		BlendTree TreeDeltaTime = GenerateBlendTree("DeltaTime", BlendTreeType.Direct, blendParameter: "One", blendParameterY: "One", maxThreshold: 1f);
		TreeDeltaTime.children = new ChildMotion[] {
			GenerateChildMotion(time1Clip,  directBlendParameter: "Time"), 
			GenerateChildMotion(timeMinus1Clip,  threshold: 1f, directBlendParameter: "LastTime")
		};
		
		
		List<BlendTree> trees = new List<BlendTree>();
		for (int i = 0; i < layerCount; i++)
		{
			string IODMinus100Name = AnimationHelper.animationPath + $"DBT/InputOutputDelta-100-{i}.anim";
			string IOD100Name = AnimationHelper.animationPath + $"DBT/InputOutputDelta100-{i}.anim";;
			string output0Name = AnimationHelper.animationPath + $"DBT/OutputLinear0-{i}.anim";;
			string output1Name = AnimationHelper.animationPath + $"DBT/OutputLinear1-{i}.anim";;
			string outputMinus1Name = AnimationHelper.animationPath + $"DBT/OutputLinear-1-{i}.anim";;
			string output100Name = AnimationHelper.animationPath + $"DBT/OutputLinear100-{i}.anim";;
			string outputMinus100Name = AnimationHelper.animationPath + $"DBT/OutputLinear-100-{i}.anim";;

			AnimationClip IODMinus100Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(IODMinus100Name);
			AnimationClip IOD100Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(IOD100Name);
			AnimationClip output0Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(output0Name);
			AnimationClip output1Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(output1Name);
			AnimationClip outputMinus1Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputMinus1Name);
			AnimationClip output100Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(output100Name);
			AnimationClip outputMinus100Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputMinus100Name);

			if (IODMinus100Clip == null)
			{
				IODMinus100Clip = GenerateClip("InputOutputDelta-100");
				AddCurve(IODMinus100Clip, "", typeof(UnityEngine.Animator), "InputOutputDelta" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: -100f), GenerateKeyFrame(time: 0.01666667f, value: -100f) }));

				IOD100Clip = GenerateClip("InputOutputDelta100");
				AddCurve(IOD100Clip, "", typeof(UnityEngine.Animator), "InputOutputDelta" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 100f), GenerateKeyFrame(time: 0.01666667f, value: 100f) }));

				outputMinus100Clip = GenerateClip("Output-100");
				AddCurve(outputMinus100Clip, "", typeof(UnityEngine.Animator), "Output" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: -100f), GenerateKeyFrame(time: 0.01666667f, value: -100f) }));
				AddCurve(outputMinus100Clip, "test" + i, typeof(UnityEngine.GameObject), "m_IsActive", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: -100f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity), GenerateKeyFrame(time: 0.01666667f, value: -100f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity) }));

				output100Clip = GenerateClip("Output100");
				AddCurve(output100Clip, "", typeof(UnityEngine.Animator), "Output" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 100f), GenerateKeyFrame(time: 0.01666667f, value: 100f) }));
				AddCurve(output100Clip, "test" + i, typeof(UnityEngine.GameObject), "m_IsActive", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 100f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity), GenerateKeyFrame(time: 0.01666667f, value: 100f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity) }));


				outputMinus1Clip = GenerateClip("Output-1");
				AddCurve(outputMinus1Clip, "", typeof(UnityEngine.Animator), "Output" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: -1f), GenerateKeyFrame(time: 0.01666667f, value: -1f) }));
				AddCurve(outputMinus1Clip, "test" + i, typeof(UnityEngine.GameObject), "m_IsActive", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: -1f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity), GenerateKeyFrame(time: 0.01666667f, value: -1f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity) }));

				output0Clip = GenerateClip("Output0");
				AddCurve(output0Clip, "", typeof(UnityEngine.Animator), "Output" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(inWeight: 0.3333333f, outWeight: 0.3333333f), GenerateKeyFrame(time: 0.01666667f) }));
				AddCurve(output0Clip, "test" + i, typeof(UnityEngine.GameObject), "m_IsActive", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity), GenerateKeyFrame(time: 0.01666667f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity) }));

				output1Clip = GenerateClip("Output1");
				AddCurve(output1Clip, "", typeof(UnityEngine.Animator), "Output" + i, GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 1f), GenerateKeyFrame(time: 0.01666667f, value: 1f) }));
				AddCurve(output1Clip, "test" + i, typeof(UnityEngine.GameObject), "m_IsActive", GenerateCurve(keys: new Keyframe[] { GenerateKeyFrame(value: 1f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity), GenerateKeyFrame(time: 0.01666667f, value: 1f, inTangent: float.PositiveInfinity, outTangent: float.PositiveInfinity) }));

				
				AssetDatabase.CreateAsset(IODMinus100Clip, IODMinus100Name);
				AssetDatabase.CreateAsset(IOD100Clip, IOD100Name);
				AssetDatabase.CreateAsset(output0Clip, output0Name);
				AssetDatabase.CreateAsset(output1Clip, output1Name);
				AssetDatabase.CreateAsset(outputMinus1Clip, outputMinus1Name);
				AssetDatabase.CreateAsset(output100Clip, output100Name);
				AssetDatabase.CreateAsset(outputMinus100Clip, outputMinus100Name);
			}
			
			BlendTree TreeDeltaInput = GenerateBlendTree("Delta = Input", BlendTreeType.Simple1D, blendParameter: i.ToString(), maxThreshold: 100f, minThreshold: -100f, useAutomaticThresholds: false);
			TreeDeltaInput.children = new ChildMotion[] {
				GenerateChildMotion(IODMinus100Clip,  threshold: -100f, directBlendParameter: "One"), 
				GenerateChildMotion(IOD100Clip,  threshold: 100f, directBlendParameter: "One")
			};
			trees.Add(TreeDeltaInput);
			
			BlendTree TreeDeltaOutput = GenerateBlendTree("Delta = -Output" + i, BlendTreeType.Simple1D, blendParameter: "Output" + i, maxThreshold: 100f, minThreshold: -100f, useAutomaticThresholds: false);
			TreeDeltaOutput.children = new ChildMotion[] { GenerateChildMotion(IOD100Clip,  threshold: -100f, directBlendParameter: "One"), GenerateChildMotion(IODMinus100Clip,  threshold: 100f, directBlendParameter: "One")};
			trees.Add(TreeDeltaOutput);
			
			BlendTree TreeOutputOutput = GenerateBlendTree("Output = Output", BlendTreeType.Simple1D, blendParameter: "Output" + i, maxThreshold: 100f, minThreshold: -100f, useAutomaticThresholds: false);
			TreeOutputOutput.children = new ChildMotion[] { GenerateChildMotion(outputMinus100Clip,  threshold: -100f, directBlendParameter: "One"), GenerateChildMotion(output100Clip,  threshold: 100f, directBlendParameter: "One") };
			trees.Add(TreeOutputOutput);


			BlendTree TreeLinearBlend = GenerateBlendTree("Linear Blend", BlendTreeType.Simple1D, blendParameter: "InputOutputDelta" + i, maxThreshold: 0.1f, minThreshold: -0.1f, useAutomaticThresholds: false);
			TreeLinearBlend.children = new ChildMotion[] {
				GenerateChildMotion(outputMinus1Clip,  threshold: -0.1f, directBlendParameter: "One"), 
				GenerateChildMotion(output0Clip,  directBlendParameter: "One"), 
				GenerateChildMotion(output1Clip,  threshold: 0.1f, directBlendParameter: "One")
			};
			trees.Add(TreeLinearBlend);
		}
		
		
		BlendTree TreeLinearSmoothing = GenerateBlendTree("Linear Smoothing", BlendTreeType.Direct, blendParameter: "Smooth Amount", blendParameterY: "Value", maxThreshold: 1f, minThreshold: 0.2857143f);

		TreeLinearSmoothing.children = new ChildMotion[] {
			GenerateChildMotion(TreeDeltaTime, directBlendParameter: "StepSize"), 
			GenerateChildMotion(lastTime1Clip, directBlendParameter: "Time"), 
		}.Concat(trees.Select((tree, index) => GenerateChildMotion(tree, directBlendParameter: index % 4 == 3 ? "FrameTime" : "One"))).ToArray();
		
		AnimatorState StateLinearSmoothingWDOn = GenerateState("Linear Smoothing (WD On)", writeDefaultValues: true, motion: TreeLinearSmoothing);


		ChildAnimatorState[] states = new ChildAnimatorState[] {
			GenerateChildState(new Vector3(280f, 120f, 0f), StateLinearSmoothingWDOn)
		};
		
		AnimatorStateMachine StateMachineLinearSmoothingLayer = GenerateStateMachine("Linear Smoothing Layer", new Vector3(50f, 20f, 0f), new Vector3(50f, 120f, 0f), new Vector3(800f, 120f, 0f), states: states, defaultState: StateLinearSmoothingWDOn);
		AnimatorControllerLayer LinearSmoothingLayer = GenerateLayer("Linear Smoothing Layer", StateMachineLinearSmoothingLayer);
        
		controller.layers = new []{LayerTime, LinearSmoothingLayer};
		AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		return controller;
	}
	
	
	
		public static AnimatorController SetupNestedBlendTree(int splitCount, int maxAnims)
	{
		string path = "DBTNested/";
		ReadyPath(controllerPath + path);
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + splitCount + ".controller");

		if (controller != null)
		{
			RandomiseParametersDBT(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AddParameters(controller, maxAnims);
		AnimatorControllerParameter oneParameter = new AnimatorControllerParameter();
		oneParameter.name = "one";
		oneParameter.defaultFloat = 1;
		oneParameter.type = AnimatorControllerParameterType.Float;
		controller.parameters = (new[] { oneParameter }).Concat(controller.parameters).ToArray();
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
        layer.defaultWeight = 1;
        layer.name = "ToggleTree";
        layer.stateMachine = new AnimatorStateMachine();
        AnimatorState treeState = new AnimatorState();
        treeState.name = "ToggleTree";
        int animIndex = 0;
        Motion motion = RecurseBlendTree(splitCount, (int) (maxAnims/(Math.Pow(2, splitCount))), ref animIndex);
        treeState.motion = motion;
        layer.stateMachine.AddState(treeState, Vector3.one);
		controller.layers = new []{layer};
		AssetDatabase.CreateAsset(controller, controllerPath + path + splitCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParametersDBT(controller);
		return controller;
	}

		public static Motion RecurseBlendTree(int layerCount, int treesPerLayer, ref int currentAnim)
		{
			if (layerCount == 0)
			{
				BlendTree child = new BlendTree();
				child.name = currentAnim.ToString();
				child.blendType = BlendTreeType.Simple1D;
				child.blendParameter = currentAnim.ToString();
				AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+currentAnim.ToString(), currentAnim);
				child.AddChild(anims[1]);
				child.AddChild(anims[0]);
				child.hideFlags = HideFlags.HideInHierarchy;
				currentAnim++;
				return child;
			}
			BlendTree bigTree = new BlendTree();
			bigTree.name = "tree";
			bigTree.blendType = BlendTreeType.Direct;
			int amount = (layerCount == 1) ? treesPerLayer : 2;
			for (int i = 0; i < amount; i++)
			{
				bigTree.AddChild(RecurseBlendTree(layerCount - 1, treesPerLayer, ref currentAnim));
			}
			ChildMotion[] childMotions = bigTree.children;
			for (var i = 0; i < childMotions.Length; i++)
			{
				childMotions[i].directBlendParameter = "one";
				childMotions[i].motion.hideFlags = HideFlags.HideInHierarchy;
			}
			bigTree.children = childMotions;
			return bigTree;
		}
	
	
	public static AnimatorController SetupSingleDirectBlendTree(int layerCount, bool defaultsLayer = false, bool singleAnim = false)
	{
		string path = singleAnim ? "DBT-Single-Anim/" : defaultsLayer ? "DBT-Single-Defaults/" : "DBT-Single/";
		ReadyPath(controllerPath + "TwoToggleSub/");
		AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + path + layerCount + ".controller");

		if (controller != null)
		{
			RandomiseParametersDBT(controller);
			return controller;
		}
		AssetDatabase.StartAssetEditing();

		controller = new AnimatorController();
		AddParameters(controller, layerCount);
		AnimatorControllerParameter oneParameter = new AnimatorControllerParameter();
		oneParameter.name = "one";
		oneParameter.defaultFloat = 1;
		oneParameter.type = AnimatorControllerParameterType.Float;
		controller.parameters = (new[] { oneParameter }).Concat(controller.parameters).ToArray();
		// DefaultsLayer
		AnimatorControllerLayer defLayer = new AnimatorControllerLayer();
		if (defaultsLayer)
		{
			defLayer.defaultWeight = 1;
			defLayer.name = "ToggleTree";
			defLayer.stateMachine = new AnimatorStateMachine();
			if (singleAnim)
			{
				AnimatorState defState = new AnimatorState();
				defState.motion = AnimationHelper.GetOrCreateBigOnToggle("test", layerCount);
				defLayer.stateMachine.AddState(defState, Vector3.one);
			}
			else
			{
				AnimatorState defTreeState = new AnimatorState();
				defTreeState.name = "ToggleTree";
				BlendTree defTree = new BlendTree();
				defTree.name = "ToggleTree";
				defTree.blendType = BlendTreeType.Direct;
				for (int i = 0; i < layerCount; i++)
				{
					AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test" + i.ToString(), i);
					defTree.AddChild(anims[0]);
				}

				ChildMotion[] defChildMotions = defTree.children;
				for (var i = 0; i < defChildMotions.Length; i++)
				{
					defChildMotions[i].directBlendParameter = "one";
				}

				defTree.children = defChildMotions;
				defTreeState.motion = defTree;
				defLayer.stateMachine.AddState(defTreeState, Vector3.one);
			}
		}
		// TreeLayer
		AnimatorControllerLayer layer = new AnimatorControllerLayer();
        layer.defaultWeight = 1;
        layer.name = "ToggleTree";
        layer.stateMachine = new AnimatorStateMachine();
        AnimatorState treeState = new AnimatorState();
        treeState.name = "ToggleTree";
        BlendTree bigTree = new BlendTree();
        bigTree.name = "ToggleTree";
        bigTree.blendType = BlendTreeType.Direct;
        for (int i = 0; i < layerCount; i++)
        {
	        AnimationClip[] anims = AnimationHelper.GetOrCreateTwoStateToggle("test"+i.ToString(), i);
	        bigTree.AddChild(anims[1]);
        }
        ChildMotion[] childMotions = bigTree.children;
        for (var i = 0; i < childMotions.Length; i++)
        {
            childMotions[i].directBlendParameter = i.ToString();
        }
        bigTree.children = childMotions;
        treeState.motion = bigTree;
        layer.stateMachine.AddState(treeState, Vector3.one);
        if (defaultsLayer)
        {
	        controller.layers = new[] { defLayer, layer };
        }
        else
        {
	        controller.layers = new []{layer};
        }

        AssetDatabase.CreateAsset(controller, controllerPath + path + layerCount + ".controller");
		SerializeController(controller);
		AssetDatabase.StopAssetEditing();
		RandomiseParametersDBT(controller);
		return controller;
	}

	#endregion
	
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
			if (childAnimatorState.state.motion is BlendTree tree)
			{
				Queue<BlendTree> trees = new Queue<BlendTree>();
				trees.Enqueue(tree);
				while (trees.Count>0)
				{
					tree = trees.Dequeue();
					AssetDatabase.RemoveObjectFromAsset(tree);
					AssetDatabase.AddObjectToAsset(tree, controller);
					tree.hideFlags = HideFlags.HideInHierarchy;

					foreach (var childMotion in tree.children)
					{
						if (childMotion.motion is BlendTree childTree)
						{
							trees.Enqueue(childTree);
						}
					}
				}
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
	
	public static void RandomiseParametersDBT(AnimatorController controller)
	{
		AnimatorControllerParameter[] parameters = controller.parameters;
		foreach (var animatorControllerParameter in parameters)
		{
			animatorControllerParameter.defaultFloat = (Random.value > 0.5) ? 0 : 1;
			if (animatorControllerParameter.name == "one")
			{
				animatorControllerParameter.defaultFloat = 1;
			}
		}
		controller.parameters = parameters;
	}

	public static void AddParameters(AnimatorController controller, int count, bool b = false)
	{
		AnimatorControllerParameter[] parameters = new AnimatorControllerParameter[count];
		for (int j = 0; j < count; j++)
		{
			string index = j.ToString();
			AnimatorControllerParameter parameter = new AnimatorControllerParameter();
			parameter.type = b ? AnimatorControllerParameterType.Bool : AnimatorControllerParameterType.Float;
			parameter.name = index;
			parameters[j] = parameter;
		}

		controller.parameters = parameters;
	}
	
	
}