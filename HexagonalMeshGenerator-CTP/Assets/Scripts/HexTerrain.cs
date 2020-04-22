using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexTerrain : MonoBehaviour {

	public bool useCollider, useColors, useUVCoordinates;

	[NonSerialized] List<Vector3> vertices;
	[NonSerialized] List<Color> colors;

	[NonSerialized] List<Vector2> uvs;
	[NonSerialized] List<int> triangles;

	Mesh hexTerrain;
	MeshCollider meshCollider;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexTerrain = new Mesh();
		if (useCollider) {
			meshCollider = gameObject.AddComponent<MeshCollider>();
		}
		hexTerrain.name = "Hex Terrain";
	}

	public void Clear () {
		hexTerrain.Clear();
		vertices = ListPool<Vector3>.Get();
		if (useColors) {
			colors = ListPool<Color>.Get();
		}
		triangles = ListPool<int>.Get();
	}

	public void Apply () {
		hexTerrain.SetVertices(vertices);
		ListPool<Vector3>.Add(vertices);
		if (useColors) {
			hexTerrain.SetColors(colors);
			ListPool<Color>.Add(colors);
		}
		hexTerrain.SetTriangles(triangles, 0);
		ListPool<int>.Add(triangles);
		hexTerrain.RecalculateNormals();
		if (useCollider) {
			meshCollider.sharedMesh = hexTerrain;
		}
	}

	public void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(HexData.AddRealismNoiseToBaseMesh(v1));
		vertices.Add(HexData.AddRealismNoiseToBaseMesh(v2));
		vertices.Add(HexData.AddRealismNoiseToBaseMesh(v3));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	public void AddTriangleUnperturbed (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	public void AddTriangleColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	public void AddTriangleColor (Color c1, Color c2, Color c3) {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}

	public void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
		int vertexIndex = vertices.Count;
		vertices.Add(HexData.AddRealismNoiseToBaseMesh(v1));
		vertices.Add(HexData.AddRealismNoiseToBaseMesh(v2));
		vertices.Add(HexData.AddRealismNoiseToBaseMesh(v3));
		vertices.Add(HexData.AddRealismNoiseToBaseMesh(v4));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	public void AddQuadColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	public void AddQuadColor (Color c1, Color c2) {
		colors.Add(c1);
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c2);
	}

	public void AddQuadColour (Color c1, Color c2, Color c3, Color c4) {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}
}