using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Core;
using VRC.Dynamics;
using Object = UnityEngine.Object;

public class BenchmarkRepository
{
	public static string benchmarkLocation = "Assets/jellejurre/Benchmarker/Benchmarks/";
	private static List<BenchmarkTaskGroup> benchmarkTasks;

	public static List<BenchmarkTaskGroup> BenchmarkTasks
	{
		get
		{
			if (benchmarkTasks == null)
			{
				SetupBenchmarks();
			}
			return benchmarkTasks;
		}
		set => benchmarkTasks = value;
	}

	static void SetupBenchmarks()
	{
		benchmarkTasks = new List<BenchmarkTaskGroup>();
		BenchmarkTask[] test = new BenchmarkTask[]
		{
			GetOrCreate<TestBenchmark>("TestBenchmark.asset"),
			GetOrCreate<TestBenchmarkActive>("TestActiveBenchmark.asset"),
			GetOrCreate<TestBenchmarkActive2d>("TestActive2dBenchmark.asset")
		};
		benchmarkTasks.Add(new BenchmarkTaskGroup(test, "TestGroup"));

		BenchmarkTask[] animatorTypes = new BenchmarkTask[] {
			GetOrCreate<NoAvatarLayerBenchmark>("NoRigLayerBenchmark.asset"),
			GetOrCreate<GenericAvatarLayerBenchmark>("GenericLayerBenchmark.asset"),
			GetOrCreate<EmptyLayerBenchmark>("EmptyLayerBenchmark.asset")
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(animatorTypes, "AnimatorTypes"));

		BenchmarkTask[] LayerSetups = new BenchmarkTask[]
		{
			GetOrCreate<AnimatorCountBenchmark>("AnimatorCountBenchmark.asset"),
			GetOrCreate<SingleStateLayerBenchmark>("SingleStateLayerBenchmark.asset"),
			GetOrCreate<SingleStateInactiveLayerBenchmark>("SingleInactiveStateLayerBenchmark.asset"),
			GetOrCreate<ManyStatesConnectedBenchmark>("ManyStatesConnectedBenchmark.asset"),
			GetOrCreate<ManyStatesConnectedBoolsBenchmark>("ManyStatesConnectedBoolsBenchmark.asset"),
			GetOrCreate<ManyStatesDisconnectedBenchmark>("ManyStatesDisconnectedBenchmark.asset"),
			GetOrCreate<TwoStateToggleBenchmark>("TwoStateToggleBenchmark.asset"),
			GetOrCreate<TwoStateToggleBoolBenchmark>("TwoStateToggleBoolBenchmark.asset"),
			GetOrCreate<TwoStateToggleWDOffBenchmark>("TwoStateToggleWDOffBenchmark.asset"),
			GetOrCreate<TwoStateToggleActiveBenchmark>("TwoStateToggleActiveBenchmark.asset"),
			GetOrCreate<TwoStateSubToggleBenchmark>("TwoSubStateToggleBenchmark.asset"),
			GetOrCreate<TwoStateToggle2dBenchmark>("TwoStateToggle2dBenchmark.asset"),
			GetOrCreate<TwoStateToggleActive2dBenchmark>("TwoStateToggleActive2dBenchmark.asset"),
			GetOrCreate<BigTwoStateToggle2dBenchmark>("BigTwoStateToggle2dBenchmark.asset"),
			GetOrCreate<BigTwoStateToggleActive2dBenchmark>("BigTwoStateToggleActive2dBenchmark.asset"),
			GetOrCreate<ManyStateLayerState2dBenchmark>("ManyStateLayerState2dBenchmark.asset"),
			GetOrCreate<ManyStateLayerStateDelayed2dBenchmark>("ManyStateLayerStateDelayed2dBenchmark.asset")
		};

		benchmarkTasks.Add(new BenchmarkTaskGroup(LayerSetups, "NonAnyStateLayers"));

		BenchmarkTask[] AnyStateSetups = new BenchmarkTask[]
		{
			GetOrCreate<OneAnyStateBenchmark>("AnyStateStateBenchmark.asset"),
			GetOrCreate<AnyStateToggleBenchmark>("AnyStateToggleBenchmark.asset"),
			GetOrCreate<AnyStateToggleWDOffBenchmark>("AnyStateToggleWDOffBenchmark.asset"),
			GetOrCreate<AnyStateToggleActiveBenchmark>("AnyStateToggleActiveBenchmark.asset"),
			GetOrCreate<AnyStateSelfToggleBenchmark>("AnyStateSelfToggleBenchmark.asset"),
			GetOrCreate<AnyStateSelfToggleActiveBenchmark>("AnyStateSelfToggleActiveBenchmark.asset"),
			GetOrCreate<AnyStateEdgeCaseBenchmark>("AnyStateEdgeCaseBenchmark.asset"),
			GetOrCreate<AnyStateToggle2dBenchmark>("AnyStateToggle2dBenchmark.asset"),
			GetOrCreate<AnyStateToggle2dActiveBenchmark>("AnyStateToggle2dActiveBenchmark.asset"),
			GetOrCreate<AnyStateLayerState2dBenchmark>("AnyStateLayerState2dBenchmark.asset")
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(AnyStateSetups, "AnyStateLayers"));

		BenchmarkTask[] DirectBlendTreeSetups = new BenchmarkTask[]
		{
			GetOrCreate<DirectBlendTreeToggleBenchmark>("DirectBlendTreeToggleBenchmark.asset"),
			GetOrCreate<DirectBlendTreeNestingBenchmark>("DirectBlendTreeNestingBenchmark.asset"),
			GetOrCreate<DirectBlendTreeSingleBenchmark>("DirectBlendTreeSingleBenchmark.asset"),
			GetOrCreate<DirectBlendTreeDefaultBenchmark>("DirectBlendTreeDefaultBenchmark.asset"),
			GetOrCreate<DBTDefaultAnimBenchmark>("DBTDefaultAnimBenchmark.asset"),
			GetOrCreate<DirectBlendTreeToggle2dBenchmark>("DirectBlendTreeToggle2dBenchmark.asset"),
			GetOrCreate<DirectBlendTreeActiveBenchmark>("DirectBlendTreeActiveBenchmark.asset"),
			GetOrCreate<DirectBlendTreeSingleActiveBenchmark>("DirectBlendTreeSingleActiveBenchmark.asset"),
			GetOrCreate<DirectBlendTreeDefaultActiveBenchmark>("DirectBlendTreeDefaultActiveBenchmark.asset"),
			GetOrCreate<DBTDefaultAnimActiveBenchmark>("DBTDefaultAnimActiveBenchmark.asset"),
			GetOrCreate<DirectBlendTreeActive2dBenchmark>("DirectBlendTreeActive2dBenchmark.asset")
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(DirectBlendTreeSetups, "DirectBlendTree"));

		BenchmarkTask[] Behaviours = new BenchmarkTask[]
		{
			GetOrCreate<LayerControlBenchmark>("LayerControlBenchmark.asset"),
			GetOrCreate<LocomotionControlBenchmark>("LocomotionControlBenchmark.asset"),
			GetOrCreate<ParameterDriverBenchmark>("ParameterDriverBenchmark.asset"),
			GetOrCreate<ParameterDriverActiveBenchmark>("ParameterDriverActiveBenchmark.asset"),
			GetOrCreate<PlayableLayerControlBenchmark>("PlayableLayerControlBenchmark.asset"),
			GetOrCreate<TempPoseBenchmark>("TempPoseBenchmark.asset"),
			GetOrCreate<TrackingControlBenchmark>("TrackingControlBenchmark.asset"),
			GetOrCreate<SingleLayerControlBenchmark>("SingleLayerControlBenchmark.asset"),
			GetOrCreate<SingleLocomotionControlBenchmark>("SingleLocomotionControlBenchmark.asset"),
			GetOrCreate<SingleParameterDriverBenchmark>("SingleParameterDriverBenchmark.asset"),
			GetOrCreate<SinglePlayableLayerControlBenchmark>("SinglePlayableLayerControlBenchmark.asset"),
			GetOrCreate<SingleLayerControlBenchmark>("SingleLayerControlBenchmark.asset"),
			GetOrCreate<SingleTrackingControlBenchmark>("SingleTrackingControlBenchmark.asset"),
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(Behaviours, "Behaviours"));
		
		BenchmarkTask[] Constraints = new BenchmarkTask[]
		{
			GetOrCreate<AimConstraintBenchmark>("AimConstraintBenchmark.asset"),
			GetOrCreate<LookAtConstraintBenchmark>("LookAtConstraintBenchmark.asset"),
			GetOrCreate<PositionConstraintBenchmark>("PositionConstraintBenchmark.asset"),
			GetOrCreate<RotationConstraintBenchmark>("RotationConstraintBenchmark.asset"),
			GetOrCreate<ScaleConstraintBenchmark>("ScaleConstraintBenchmark.asset"),
			GetOrCreate<ParentConstraintBenchmark>("ParentConstraintBenchmark.asset"),
			GetOrCreate<ParentConstraintSourcesBenchmark>("ParentConstraintSourcesBenchmark.asset"),
			GetOrCreate<ParentConstraint2dBenchmark>("ParentConstraint2dBenchmark.asset"),
			GetOrCreate<ParentConstraintOffBenchmark>("ParentConstraintOffBenchmark.asset"),
			GetOrCreate<ParentConstraintDisabledBenchmark>("ParentConstraintDisabledBenchmark.asset"),
			GetOrCreate<ParentConstraintZeroBenchmark>("ParentConstraintZeroBenchmark.asset"),
			GetOrCreate<MixedConstraintBenchmark>("MixedConstraintBenchmark.asset"),
		};
		benchmarkTasks.Add(new BenchmarkTaskGroup(Constraints, "Constraints"));

		BenchmarkTask[] Contacts = new BenchmarkTask[]
		{
			GetOrCreate<ContactSenderBenchmark>("ContactSenderBenchmark.asset"),
			GetOrCreate<ContactReceiverBenchmark>("ContactReceiverBenchmark.asset"),
			GetOrCreate<ContactComboSeperatedBenchmark>("ContactComboSeperatedBenchmark.asset"),
			GetOrCreate<ContactComboTogetherBenchmark>("ContactComboTogetherBenchmark.asset"),
			GetOrCreate<ContactComboSameNameBenchmark>("ContactComboSameNameBenchmark.asset"),
			GetOrCreate<ContactComboActiveBenchmark>("ContactComboActiveBenchmark.asset"),
			GetOrCreate<ContactComboActiveCapsuleBenchmark>("ContactComboActiveCapsuleBenchmark.asset"),
			GetOrCreate<ContactComboActiveProximityBenchmark>("ContactComboActiveProximityBenchmark.asset")
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(Contacts, "Contacts"));

		BenchmarkTask[] FaceTracking = new BenchmarkTask[]
		{
			GetOrCreate<BlendTreeFaceTracking>("BlendTreeFaceTracking.asset"),
			GetOrCreate<BlendTreeFaceTrackingActive>("BlendTreeFaceTrackingActive.asset"),
			GetOrCreate<LayerFaceTracking>("LayerFaceTracking.asset"),
			GetOrCreate<LayerFaceTrackingActive>("LayerFaceTrackingActive.asset"),
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(FaceTracking, "FaceTracking"));

		BenchmarkTask[] Audio = new BenchmarkTask[]
		{
			GetOrCreate<AudioSourceBenchmark>("AudioSourceBenchmark.asset"),
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(Audio, "AudioSources"));
		
		BenchmarkTask[] Cloth = new BenchmarkTask[]
		{
			GetOrCreate<ClothVertexCountInactiveBenchmark>("ClothVertexCountInactiveBenchmark.asset"),
			GetOrCreate<ClothVertexCountActiveBenchmark>("ClothVertexCountActiveBenchmark.asset"),
			GetOrCreate<ClothVertexCountMeshCountBenchmark>("ClothVertexCountMeshCountBenchmark.asset"),
			GetOrCreate<ClothColliderCountActiveBenchmark>("ClothColliderCountActiveBenchmark.asset"),
			GetOrCreate<ClothCapsuleCountActiveBenchmark>("ClothCapsuleCountActiveBenchmark.asset")
		};
		
		benchmarkTasks.Add(new BenchmarkTaskGroup(Cloth, "Cloth"));
		
		// BenchmarkTask[] Physbones = new BenchmarkTask[]
		// {
		// 	GetOrCreate<PhysboneChildCount>("PhysboneChildCount.asset"),
		// 	GetOrCreate<ForkedPhysboneChildCount>("ForkedPhysboneChildCount.asset"),
		// 	GetOrCreate<PhysboneColliderCount>("PhysboneColliderCount.asset"),
		// 	GetOrCreate<PhysboneChildCountAnimated>("PhysboneChildCountAnimated.asset"),
		// 	GetOrCreate<PhysboneChildCountDepth>("PhysboneChildCountDepth.asset")
		// };
		//
		// benchmarkTasks.Add(new BenchmarkTaskGroup(Physbones,"Physbones"));
	}

	public static BenchmarkTask GetNext(BenchmarkTask current)
	{
		if (current == null)
		{
			return benchmarkTasks.First().tasks.First();
		}

		for (int i = 0; i < benchmarkTasks.Count; i++)
		{
			int index = Array.IndexOf(benchmarkTasks[i].tasks, current);
			if (index != -1)
			{
				if (benchmarkTasks[i].tasks.Length == index + 1)
				{
					if (benchmarkTasks.Count == i + 1)
					{
						return null;
					}
					else
					{
						return benchmarkTasks[i + 1].tasks.First();
					}
				}
				return benchmarkTasks[i].tasks[index + 1];

			}
		}
		return null;
	}
	
	static T GetOrCreate<T>(string location) where T : ScriptableObject
	{
		T benchmark = AssetDatabase.LoadAssetAtPath<T>(benchmarkLocation + location);
		if (benchmark == null)
		{
			Debug.Log("Creating benchmark at: " + location);
			benchmark = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(benchmark, benchmarkLocation + location);
			AssetDatabase.SaveAssets();
		}

		return benchmark;
	}
}
