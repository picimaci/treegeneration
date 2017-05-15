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

	public int segCount = 7;
	public int numberOfSectors = 4;

	public int recursionDepth = 1;
	public float radius = 0.5f;
	public float flare = 2;
	public float taper = 0.7f;
	public float widthLengthRatio = 16;
	public float childrenParentRatio = 0.8f;

	public Vector3 splitAngle = new Vector3(0,45,30);
	public Vector3 splitAngleVariation = new Vector3(0,-90, -60);
	public float splitPoint = 0.5f;
	public float splitProbability = 0.5f;

	public float splitProbabilityForBranch = 0.3f;

	public Vector3 branchingAngle = new Vector3(0,180,45);
	public Vector3 branchingAngleVariation = new Vector3(0,-360,-90);
	public float branchingPoint = 0.7f;
	public float branchingProbability = 0.5f;

	public Vector3 curveAngle = new Vector3(5,5,5);
	public Vector3 curveAngleVariation = new Vector3(-10,-10,-10);

	public Vector3 curveBackAngle = new Vector3(-10,-10,0);
	public Vector3 curveBackAngleVariation = new Vector3(5,5,0);

	public Shape shape = Shape.Apple;

	private void OnEnable ()
	{
		Debug.Log("TreeGenerator.OnEnable");
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
	}

	[ContextMenu("Generate")]
	public void Generate()
	{
		if (treeMesh)
			DestroyImmediate(treeMesh);

		treeMesh = new Mesh {name = "treeMesh"};
		Tree tree = new Tree();
		tree.childParentRatio = childrenParentRatio;
		tree.widthLengthRatio = widthLengthRatio;
		tree.recursionDepth = recursionDepth;

		Trunk trunk = new Trunk();
		trunk.flare = flare;
		trunk.basePoint = new Vector3(0,0,0);
		trunk.baseRotation = Quaternion.Euler(0, 0, 0);
		trunk.branchingAngle = branchingAngle;
		trunk.branchingAngleVariation = branchingAngleVariation;
		trunk.branchingFactor = branchingProbability;
		trunk.branchingPoint = branchingPoint;
		trunk.curveAngle = curveAngle;
		trunk.curveAngleVariation = curveAngleVariation;
		trunk.segCount = segCount;
		trunk.numberOfSectors = numberOfSectors;
		trunk.split = splitProbability;
		trunk.splitAngle = splitAngle;
		trunk.baseSplitPoint = splitPoint;
		trunk.splitAngleVariation = splitAngleVariation;
		trunk.radius = radius;
		trunk.length = radius * widthLengthRatio;
		trunk.taper = taper;
		trunk.curveBackAngleForBranch = curveBackAngle;
		trunk.curveBackAngleVariationForBranch = curveBackAngleVariation;
		trunk.splitForBranch = splitProbabilityForBranch;

		trunk.levelOfRecursion = 0;

		trunk.tree = tree;

		tree.trunk = trunk;
		tree.random = new System.Random();

		tree.GenerateTreeUsingEditorParams();

		treeMesh.SetVertices(tree.vertices);
		treeMesh.SetIndices(tree.indices.ToArray(), MeshTopology.Triangles, 0);
		treeMesh.UploadMeshData(markNoLogerReadable: false);
		treeMesh.Optimize();
		treeMesh.RecalculateNormals();
		meshFilter.sharedMesh = treeMesh;
	}
}
