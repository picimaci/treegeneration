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
	[Header("General Tree Params")]
	public int segCount = 7;
	public int numberOfSectors = 4;
	public int recursionDepth = 1;
	public float radius = 0.5f;
	public float taper = 0.7f;
	public float widthLengthRatio = 16;
	public float childrenParentRatio = 0.8f;

	[Header("Stem Params")]
	public int branchNumber = 2;
	public float branchingRotationY = 30;
	public float branchingAngle = 30;
	public float branchingAngleVariation = 10;
	[Header("Trunk Params")]
	public float flare = 2;
	public float trunkSplitAngle = 15;
	public float trunkSplitAngleVariation = 5;
	public float trunkSplitPoint = 0.5f;
	public float trunkSplitProbability = 0.5f;
	public float trunkCurveAngle = 0;
	public float trunkCurveAngleVariation = 15;
	public float trunkBranchingPoint = 0.7f;
	public float trunkBranchingProbability = 0.5f;
	[Header("Branch Params")]
	public float branchSplitAngle = 15;
	public float branchSplitAngleVariation = 5;
	public float branchSplitProbability = 0.3f;
	public float branchBranchingPoint = 0;
	public float branchingProbabilityForBranch = 0.5f;
	public float branchCurveAngle = 45;
	public float branchCurveAngleVariation = 15;
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
		trunk.branchingFactor = trunkBranchingProbability;
		trunk.branchBranchingFactor = branchingProbabilityForBranch;
		trunk.branchingPoint = trunkBranchingPoint;
		trunk.branchingPointForBranch = branchBranchingPoint;
		trunk.curveBackAngle = curveBackAngle;
		trunk.curveBackAngleVariation = curveBackAngleVariation;
		trunk.branchSplitFactor = branchSplitProbability;
		trunk.branchSplitAngle = branchSplitAngle;
		trunk.branchSplitAngleVariation = branchSplitAngleVariation;
		trunk.curveAngle = trunkCurveAngle;
		trunk.curveAngleVariation = trunkCurveAngleVariation;
		trunk.segCount = segCount;
		trunk.numberOfSectors = numberOfSectors;
		trunk.splitFactor = trunkSplitProbability;
		trunk.splitAngle = trunkSplitAngle;
		trunk.baseSplitPoint = trunkSplitPoint;
		trunk.splitAngleVariation = trunkSplitAngleVariation;
		trunk.radius = radius;
		trunk.length = radius * widthLengthRatio;
		trunk.taper = taper;
		trunk.curveAngleForBranch = branchCurveAngle;
		trunk.curveAngleVariationForBranch = branchCurveAngleVariation;

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
