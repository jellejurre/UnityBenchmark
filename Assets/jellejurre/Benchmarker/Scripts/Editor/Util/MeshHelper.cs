using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public class MeshHelper
{
	public static string meshPath = "Assets/jellejurre/Benchmarker/Assets/Generated/Meshes/";

	public static GameObject GetSquareMesh(int sideLength, int materials = 1, int blendshapes = 0, bool targetSingleVertex = false, int bones = 0)
	{
		string path = "Square/";
		AnimatorHelpers.ReadyPath(meshPath + path);
		string assetPath = meshPath + path + sideLength + $"-{materials}-{blendshapes}-{(targetSingleVertex ? 't' : 'f')}-{bones}.fbx";
		GameObject oldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
		if (oldPrefab != null)
		{
			return oldPrefab;
		}
		GameObject meshObj = new GameObject();
		SkinnedMeshRenderer meshRenderer = meshObj.AddComponent<SkinnedMeshRenderer>();
		Mesh mesh = GenerateSquareMesh(sideLength, materials, blendshapes, targetSingleVertex, bones);
		meshRenderer.materials = Util.CreateArr((i) => new Material(Shader.Find("Standard")), materials);
		for (var i = 0; i < meshRenderer.materials.Length; i++)
		{
			meshRenderer.materials[i].name = "Material" + i;
			meshRenderer.materials[i].color = new Color(i / (float)materials, 0, 0, 1);
		}

		Transform[] boneArr = new Transform[bones];
		if (bones > 0)
		{
			for (int i = 0; i < bones; i++)
			{
				boneArr[i] = new GameObject($"bone{i}").transform;
				boneArr[i].transform.parent = meshObj.transform;
			}

			Matrix4x4[] bindposes = mesh.bindposes;
			for (var index = 0; index < boneArr.Length; index++)
			{
				bindposes[index] = boneArr[index].worldToLocalMatrix * meshObj.transform.localToWorldMatrix;
			}

			mesh.bindposes = bindposes;
			
			meshRenderer.bones = boneArr;
		}
		meshRenderer.sharedMesh = mesh; 

		ModelExporter.ExportObjects(assetPath, new Object[] {meshObj});
		GameObject.DestroyImmediate(meshObj);
		AssetDatabase.Refresh();
		ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
		importer.optimizeMeshVertices = false;
		importer.optimizeMeshPolygons = false;
		importer.isReadable = true;
		string pName = "legacyComputeAllNormalsFromSmoothingGroupsWhenMeshHasBlendShapes";
		PropertyInfo prop = importer.GetType().GetProperty(pName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		prop.SetValue(importer, true);
		AssetDatabase.SaveAssets();
		AssetDatabase.ImportAsset(assetPath);
		importer.isReadable = false;
		AssetDatabase.SaveAssets();
		AssetDatabase.ImportAsset(assetPath);
		importer.isReadable = true;
		AssetDatabase.SaveAssets();
		AssetDatabase.ImportAsset(assetPath);
		AssetDatabase.Refresh();
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
		return prefab;
	}
	
	private static Mesh GenerateSquareMesh(int sideLength, int submeshes = 1, int blendshapes = 0, bool targetSingleVertex = false, int bones = 0)
	{
		Mesh mesh = new Mesh();
		mesh.indexFormat = IndexFormat.UInt32;
		int pointCount = sideLength * sideLength;
		int quadsPerSubMesh = (sideLength - 1) * (sideLength - 1) / submeshes;
		Vector3[] vertices = new Vector3[pointCount];
		for (int i = 0; i < pointCount; i++)
		{
			int x = i % sideLength;
			int y = (int)Math.Floor((double)i / sideLength);
			vertices[i] = new Vector3((float)x/sideLength, (float)y/sideLength, 0);
		}
		mesh.vertices = vertices;
		int[][] triangles = Util.CreateArr(x => new int[quadsPerSubMesh * 6], submeshes);
		var ind = GetSidelengthFunc(sideLength);   
		for (int x = 0; x < sideLength - 1; x++)
		{
			for (int y = 0; y < sideLength - 1; y++)
			{
				int triangleCount = (y * (sideLength - 1) + x);
				int triangleIndex = triangleCount * 6;
				int submesh = Math.Min(submeshes, triangleCount / quadsPerSubMesh);
				
				if(submesh == submeshes) continue;
				triangles[submesh][triangleIndex % (quadsPerSubMesh * 6)] = ind(x, y + 1);
				triangles[submesh][(triangleIndex + 1) % (quadsPerSubMesh * 6)] = ind(x+1, y);
				triangles[submesh][(triangleIndex + 2) % (quadsPerSubMesh * 6)] = ind(x, y);
				triangles[submesh][(triangleIndex + 3) % (quadsPerSubMesh * 6)] = ind(x, y + 1);
				triangles[submesh][(triangleIndex + 4) % (quadsPerSubMesh * 6)] = ind(x + 1, y + 1);
				triangles[submesh][(triangleIndex + 5) % (quadsPerSubMesh * 6)] = ind(x + 1, y); 
			}	
		}
		mesh.normals = Enumerable.Repeat(Vector3.forward, sideLength * sideLength).ToArray();
		mesh.subMeshCount = submeshes;
		for (var i = 0; i < triangles.Length; i++)
		{
			mesh.SetTriangles(triangles[i], i);
		}

		if (blendshapes > 0)
		{
			Vector3[][] blendshapeOffsets = Util.CreateArr((x) => new Vector3[pointCount], blendshapes);
			int verticesPerBlendshape = pointCount / blendshapes;
			for (int i = 0; i < pointCount; i++)
			{
				int index = i / verticesPerBlendshape;
				if (targetSingleVertex && i % verticesPerBlendshape != 0) continue;
				blendshapeOffsets[index][i] = Vector3.forward;
			}

			Vector3[] nullOffsets = new Vector3[pointCount];
			
			for (var i = 0; i < blendshapeOffsets.Length; i++)
			{
				mesh.AddBlendShapeFrame("Shape" + i, 1, blendshapeOffsets[i], nullOffsets, nullOffsets);
			}
		}
		
		mesh.RecalculateNormals();

		if (bones > 0)
		{
			BoneWeight[] boneWeights = new BoneWeight[pointCount];
			int verticesPerBone = pointCount / bones;
			for (int i = 0; i < pointCount; i++)
			{
				boneWeights[i].boneIndex0 = i / verticesPerBone;
				boneWeights[i].weight0 = 1;
				boneWeights[i].boneIndex1 = i / verticesPerBone;
				boneWeights[i].weight1 = 1;
				boneWeights[i].boneIndex2 = i / verticesPerBone;
				boneWeights[i].weight2 = 1;
				boneWeights[i].boneIndex3 = i / verticesPerBone;
				boneWeights[i].weight3 = 1;
			}

			mesh.boneWeights = boneWeights;
			mesh.bindposes = Util.CreateArr(x => Matrix4x4.identity, bones);
		}
		
		return mesh;
	}

	public static void ProcessCloth(Cloth cloth, int sideLength)
	{
		cloth.stretchingStiffness = 0.9f;
		cloth.damping = 0.5f;
		cloth.bendingStiffness = 0.9f;
		ClothSkinningCoefficient[] newConstraints = Enumerable
			.Repeat(new ClothSkinningCoefficient(){collisionSphereDistance = 0.1f, maxDistance = 1f}
				, sideLength * sideLength).ToArray();//new ClothSkinningCoefficient[sideLength * sideLength];
		newConstraints[sideLength * (sideLength-1)].maxDistance = 0f;
		newConstraints[sideLength * (sideLength-1) + (sideLength - 1)].maxDistance = 0f;
		cloth.coefficients = newConstraints;
	}

	private static Func<int, int, int> GetSidelengthFunc(int sideLength)
	{
		return (x, y) => y * sideLength + x;
	}
}