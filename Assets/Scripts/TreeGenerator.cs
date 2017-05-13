using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TreeGenerator : MonoBehaviour {

	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;

	private Mesh treeMesh;

	public Shape shape;

	private void OnEnable ()
	{
		Debug.Log("TreeGenerator.OnEnable");
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
	}

	//[ContextMenu("Generate")]
	public void Generate()
	{
		if (treeMesh)
			DestroyImmediate(treeMesh);

		treeMesh = new Mesh {name = "treeMesh"};
		Tree tree = new Tree();
		tree.GenerateForTesting();

		treeMesh.SetVertices(tree.vertices);
		treeMesh.SetIndices(tree.indices.ToArray(), MeshTopology.Triangles, 0);
		treeMesh.UploadMeshData(markNoLogerReadable: false);
		treeMesh.Optimize();
		treeMesh.RecalculateNormals();
		meshFilter.sharedMesh = treeMesh;
//		GenerateBlabla();
	}


	public void GenerateBlabla()
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> indices = new List<int>();

		for (int i = 0; i < 3; i++)
		{
			vertices.Add(new Vector3(0,0.5f*i,0.4f-0.1f*i));
			vertices.Add(new Vector3(0.4f - 0.1f* i,0.5f*i,-0.3f+ 0.1f*i));
			vertices.Add(new Vector3(-0.4f + 0.1f* i,0.5f*i,-0.3f+ 0.1f*i));
			if (i == 1)
			{
				vertices.Add(new Vector3(0,0.5f*i,0.4f-0.1f*i));
				vertices.Add(new Vector3(0.4f - 0.1f* i,0.5f*i,-0.3f+ 0.1f*i));
				vertices.Add(new Vector3(-0.4f + 0.1f* i,0.5f*i,-0.3f+ 0.1f*i));
			}
		}
		indices.Add(0);
		indices.Add(4);
		indices.Add(3);
		indices.Add(0);
		indices.Add(1);
		indices.Add(4);

		indices.Add(1);
		indices.Add(5);
		indices.Add(4);
		indices.Add(1);
		indices.Add(2);
		indices.Add(5);

		indices.Add(2);
		indices.Add(3);
		indices.Add(5);
		indices.Add(2);
		indices.Add(0);
		indices.Add(3);

		indices.Add(6);
		indices.Add(10);
		indices.Add(9);
		indices.Add(6);
		indices.Add(7);
		indices.Add(10);

		indices.Add(7);
		indices.Add(11);
		indices.Add(10);
		indices.Add(7);
		indices.Add(8);
		indices.Add(11);

		indices.Add(8);
		indices.Add(9);
		indices.Add(11);
		indices.Add(8);
		indices.Add(6);
		indices.Add(9);

		if (treeMesh)
			DestroyImmediate(treeMesh);

		treeMesh = new Mesh {name = "treeMesh"};

		treeMesh.SetVertices(vertices);
		treeMesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
		treeMesh.UploadMeshData(markNoLogerReadable: false);
		treeMesh.Optimize();
		treeMesh.RecalculateNormals();
		meshFilter.sharedMesh = treeMesh;
	}
}
