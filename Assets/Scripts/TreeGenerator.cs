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

	public float splitAngle = 15;
	public float splitAngleVariation = 5;
	public float splitPoint = 0.5f;
	public float splitProbability = 0.5f;

	public float splitProbabilityForBranch = 0.3f;

	public int branchNumber = 2;
	public float branchingRotationY = 30;
	public float branchingAngle = 30;
	public float branchingAngleVariation = 10;
	public float branchingPoint = 0.7f;
	public float branchingPointForBranch = 0;
	public float branchingProbability = 0.5f;
	public float branchingProbabilityForBranch = 0.5f;

	public float trunkCurveAngle = 0;
	public float trunkCurveAngleVariation = 15;

	public float curveAngle = 45;
	public float curveAngleVariation = 15;

	public float curveBackAngle = -10;
	public float curveBackAngleVariation = 5;

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
		trunk.branchNumber = branchNumber;
		trunk.branchingRotationY = branchingRotationY;
		trunk.branchingAngle = branchingAngle;
		trunk.branchingAngleVariation = branchingAngleVariation;
		trunk.branchingFactor = branchingProbability;
		trunk.branchingFactorForBranch = branchingProbabilityForBranch;
		trunk.branchingPoint = branchingPoint;
		trunk.branchingPointForBranch = branchingPointForBranch;
		trunk.curveAngle = trunkCurveAngle;
		trunk.curveAngleVariation = trunkCurveAngleVariation;
		trunk.segCount = segCount;
		trunk.numberOfSectors = numberOfSectors;
		trunk.splitFactor = splitProbability;
		trunk.splitAngle = splitAngle;
		trunk.baseSplitPoint = splitPoint;
		trunk.splitAngleVariation = splitAngleVariation;
		trunk.radius = radius;
		trunk.length = radius * widthLengthRatio;
		trunk.taper = taper;
		trunk.curveBackAngleForBranch = curveBackAngle;
		trunk.curveBackAngleVariationForBranch = curveBackAngleVariation;
		trunk.splitForBranch = splitProbabilityForBranch;
		trunk.curveAngleForBranch = curveAngle;
		trunk.curveAngleVariationForBranch = curveAngleVariation;

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
