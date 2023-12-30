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

	public static GameObject GetSquareMesh(int sideLength)
	{
		string path = "Square/";
		AnimatorHelpers.ReadyPath(meshPath + path);
		GameObject oldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(meshPath + path + sideLength + ".fbx");
		if (oldPrefab != null)
		{
			return oldPrefab;
		}
		GameObject meshObj = new GameObject();
		MeshFilter mf = meshObj.AddComponent<MeshFilter>();
		SkinnedMeshRenderer meshRenderer = meshObj.AddComponent<SkinnedMeshRenderer>();
		Mesh mesh = GenerateSquareMesh(sideLength);
		mf.mesh = mesh;
		meshRenderer.sharedMesh = mesh;
		meshRenderer.material = new Material(Shader.Find("Standard"));
		ModelExporter.ExportObjects(meshPath + path + sideLength + ".fbx", new Object[] {meshObj});
		GameObject.DestroyImmediate(meshObj);
		AssetDatabase.Refresh();
		ModelImporter importer = AssetImporter.GetAtPath(meshPath + path + sideLength + ".fbx") as ModelImporter;
		importer.optimizeMeshVertices = false;
		importer.optimizeMeshPolygons = false;
		importer.isReadable = true;
		string pName = "legacyComputeAllNormalsFromSmoothingGroupsWhenMeshHasBlendShapes";
		PropertyInfo prop = importer.GetType().GetProperty(pName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		prop.SetValue(importer, true);
		AssetDatabase.SaveAssets();
		AssetDatabase.ImportAsset(meshPath + path + sideLength + ".fbx");
		importer.isReadable = false;
		AssetDatabase.SaveAssets();
		AssetDatabase.ImportAsset(meshPath + path + sideLength + ".fbx");
		importer.isReadable = true;
		AssetDatabase.SaveAssets();
		AssetDatabase.ImportAsset(meshPath + path + sideLength + ".fbx");
		AssetDatabase.Refresh();
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(meshPath + path + sideLength + ".fbx");
		return prefab;
	}
	private static Mesh GenerateSquareMesh(int sideLength)
	{
		Mesh mesh = new Mesh();
		mesh.indexFormat = IndexFormat.UInt32;
		int pointCount = sideLength * sideLength;
		Vector3[] vertices = new Vector3[pointCount];
		for (int i = 0; i < pointCount; i++)
		{
			int x = i % sideLength;
			int y = (int)Math.Floor((double)i / sideLength);
			vertices[i] = new Vector3((float)x/sideLength, (float)y/sideLength, 0);
		}
		mesh.vertices = vertices;
		int[] triangles = new int[(sideLength - 1) * (sideLength - 1) * 6];
		var ind = GetSidelengthFunc(sideLength);
		for (int x = 0; x < sideLength - 1; x++)
		{
			for (int y = 0; y < sideLength - 1; y++)
			{
				int triangleIndex = (y * (sideLength - 1) + x) * 6;
				triangles[triangleIndex] = ind(x, y + 1);
				triangles[triangleIndex + 1] = ind(x+1, y);
				triangles[triangleIndex + 2] = ind(x, y);
				triangles[triangleIndex + 3] = ind(x, y + 1);
				triangles[triangleIndex + 4] = ind(x + 1, y + 1);
				triangles[triangleIndex + 5] = ind(x + 1, y); }	
		}
		mesh.triangles = triangles;
		mesh.normals = Enumerable.Repeat(Vector3.forward, sideLength * sideLength).ToArray();
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