using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class TreeGenerator : MonoBehaviour {

	private MeshRenderer trunkMeshRenderer;
	private MeshRenderer branchMeshRenderer;
	private MeshRenderer leafStemMeshRenderer;
	private MeshRenderer leafMeshRenderer;
	private MeshFilter trunkMeshFilter;
	private MeshFilter branchMeshFilter;
	private MeshFilter leafStemMeshFilter;
	private MeshFilter leafMeshFilter;

	private Mesh trunkMesh;
	private Mesh branchMesh;
	private Mesh leafStemMesh;
	private Mesh leafMesh;

	[Header("General Tree Params")]
	public Material treeMaterial;
	public Material leafMaterial;
	public int segCount = 7;
	public int numberOfSectors = 4;
	public int recursionDepth = 1;
	public float radius = 0.5f;
	public float childrenParentRatio = 0.8f;

	[Header("Stem Params")]
	public int branchNumber = 2;
	public float branchingRotationY = 30;
	public float branchingAngle = 30;
	public float branchingAngleVariation = 10;

	[Header("LeafStem and Leaf Params")]
	//public Texture leafTexture;
	public float leafStemPoint = 0.5f;
	public int leafStemNodesPerSegment = 3;
	public int leafStemsPerNode = 2;
	public float leafStemYRotation = 90;
	public float leafStemAngle = 90;
	public float leafStemCurve = -30;
	public float leafStemCurveVariation = 5;
	public float leafStemCurveBack = 0;
	public float leafStemCurveBackVariation = 5;
	public float leavesPerNode = 2;
	public float leafYRotation = 0;
	public float leafStemRadius = 0.05f;
	public float leafStemLength = 0.3f;
	public float leafStemSplitFactor = 0.5f;
	public float leafStemSplitAngle = 30;
	public float leafStemSplitAngleVariation = 5;
	public int leafStemSegCount = 4;
	public int leafNodePerSegment = 1;
	public float leafAngle = 90;
	public float leafRotationXAngle = 90;
	public float leafWidth = 0.1f;
	public float leafLength = 0.1f;

	[Header("Trunk Params")]
	public float flare = 2;
	public float trunkTaper = 0.7f;
	public float trunkSplitAngle = 15;
	public float trunkSplitAngleVariation = 5;
	public float trunkSplitPoint = 0.5f;
	public float trunkSplitProbability = 0.5f;
	public float trunkCurveAngle = 0;
	public float trunkCurveAngleVariation = 15;
	public float trunkBranchingPoint = 0.7f;
	public float trunkBranchingProbability = 0.5f;
	public float trunkWidthLengthRatio = 20;

	[Header("Branch Params")]
	public float branchTaper = 0.9f;
	public float branchSplitAngle = 15;
	public float branchSplitAngleVariation = 5;
	public float branchSplitProbability = 0.3f;
	public float branchBranchingPoint = 0;
	public float branchingProbabilityForBranch = 0.5f;
	public float branchCurveAngle = 45;
	public float branchCurveAngleVariation = 15;
	public float curveBackAngle = -10;
	public float curveBackAngleVariation = 5;
	public float branchWidthLengthRatio = 30;

	public Shape shape = Shape.Apple;

	private void OnEnable ()
	{
		Debug.Log("TreeGenerator.OnEnable");

		Transform trunkTr = transform.Find("Trunk");
		if (!trunkTr)
		{
			GameObject trunkGO = new GameObject("Trunk");
			trunkTr = trunkGO.transform;
			trunkTr.parent = transform;
			trunkTr.localPosition = Vector3.zero;
			trunkTr.localRotation = Quaternion.identity;
		}
		trunkMeshRenderer = trunkTr.GetComponent<MeshRenderer>();
		if (!trunkMeshRenderer)
			trunkMeshRenderer = trunkTr.gameObject.AddComponent<MeshRenderer>();
		trunkMeshFilter = trunkTr.GetComponent<MeshFilter>();
		if (!trunkMeshFilter)
			trunkMeshFilter = trunkTr.gameObject.AddComponent<MeshFilter>();

		Transform branchTr = transform.Find("Branch");
		if (!branchTr)
		{
			GameObject branchGO = new GameObject("Branch");
			branchTr = branchGO.transform;
			branchTr.parent = transform;
			branchTr.localPosition = Vector3.zero;
			branchTr.localRotation = Quaternion.identity;
		}
		branchMeshRenderer = branchTr.GetComponent<MeshRenderer>();
		if (!branchMeshRenderer)
			branchMeshRenderer = branchTr.gameObject.AddComponent<MeshRenderer>();
		branchMeshFilter = branchTr.GetComponent<MeshFilter>();
		if (!branchMeshFilter)
			branchMeshFilter = branchTr.gameObject.AddComponent<MeshFilter>();

		Transform leafStemTr = transform.Find("LeafStem");
		if (!leafStemTr)
		{
			GameObject leafStemGO = new GameObject("LeafStem");
			leafStemTr = leafStemGO.transform;
			leafStemTr.parent = transform;
			leafStemTr.localPosition = Vector3.zero;
			leafStemTr.localRotation = Quaternion.identity;
		}
		leafStemMeshRenderer = leafStemTr.GetComponent<MeshRenderer>();
		if (!leafStemMeshRenderer)
			leafStemMeshRenderer = leafStemTr.gameObject.AddComponent<MeshRenderer>();
		leafStemMeshFilter = leafStemTr.GetComponent<MeshFilter>();
		if (!leafStemMeshFilter)
			leafStemMeshFilter = leafStemTr.gameObject.AddComponent<MeshFilter>();

		Transform leafTr = transform.Find("Leaf");
		if (!leafTr)
		{
			GameObject leafGO = new GameObject("Leaf");
			leafTr = leafGO.transform;
			leafTr.parent = transform;
			leafTr.localPosition = Vector3.zero;
			leafTr.localRotation = Quaternion.identity;
		}
		leafMeshRenderer = leafTr.GetComponent<MeshRenderer>();
		if (!leafMeshRenderer)
			leafMeshRenderer = leafTr.gameObject.AddComponent<MeshRenderer>();
		leafMeshFilter = leafTr.GetComponent<MeshFilter>();
		if (!leafMeshFilter)
			leafMeshFilter = leafTr.gameObject.AddComponent<MeshFilter>();
	}

	[ContextMenu("Generate")]
	public void Generate()
	{
		if (trunkMesh)
			DestroyImmediate(trunkMesh);
		trunkMesh = new Mesh {name = "trunkMesh"};

		if(branchMesh)
			DestroyImmediate(branchMesh);
		branchMesh = new Mesh {name = "branchMesh"};

		if(leafStemMesh)
			DestroyImmediate(leafStemMesh);
		leafStemMesh = new Mesh {name = "leafStemMesh"};

		if(leafMesh)
			DestroyImmediate(leafMesh);
		leafMesh = new Mesh {name = "leafMesh"};

		Tree tree = new Tree();
		tree.childParentRatio = childrenParentRatio;
		tree.widthLengthRatio = trunkWidthLengthRatio;
		tree.recursionDepth = recursionDepth;
		tree.leafStemPoint = leafStemPoint;
		tree.leafStemAngle = leafStemAngle;
		tree.leafStemNodesPerSegment = leafStemNodesPerSegment;
		tree.leafStemsPerNode = leafStemsPerNode;
		tree.leavesPerNode = leavesPerNode;
		tree.leafYRotation = leafYRotation;
		tree.leafStemYRotation = leafStemYRotation;
		tree.leafStemCurve = leafStemCurve;
		tree.leafStemCurveVariation = leafStemCurveVariation;
		tree.leafStemCurveBack = leafStemCurveBack;
		tree.leafStemCurveBackVariation = leafStemCurveBackVariation;
		tree.leafStemLength = leafStemLength;
		tree.leafStemRadius = leafStemRadius;
		tree.leafStemSplitFactor = leafStemSplitFactor;
		tree.leafStemSplitAngle = leafStemSplitAngle;
		tree.leafStemSplitAngleVariation = leafStemSplitAngleVariation;
		tree.leafStemSegCount = leafStemSegCount;
		tree.leafNodePerSegment = leafNodePerSegment;
		tree.leafAngle = leafAngle;
		tree.leafRotationXAngle = leafRotationXAngle;
		tree.leafWidth = leafWidth;
		tree.leafLength = leafLength;

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
		trunk.branchTaper = branchTaper;
		trunk.branchWidthLengthRatio = branchWidthLengthRatio;
		trunk.curveAngle = trunkCurveAngle;
		trunk.curveAngleVariation = trunkCurveAngleVariation;
		trunk.segCount = segCount;
		trunk.numberOfSectors = numberOfSectors;
		trunk.splitFactor = trunkSplitProbability;
		trunk.splitAngle = trunkSplitAngle;
		trunk.baseSplitPoint = trunkSplitPoint;
		trunk.splitAngleVariation = trunkSplitAngleVariation;
		trunk.radius = radius;
		trunk.widthLengthRatio = trunkWidthLengthRatio;
		trunk.length = radius * trunkWidthLengthRatio;
		trunk.taper = trunkTaper;
		trunk.curveAngleForBranch = branchCurveAngle;
		trunk.curveAngleVariationForBranch = branchCurveAngleVariation;

		trunk.levelOfRecursion = 0;

		trunk.tree = tree;

		tree.trunk = trunk;
		tree.random = new System.Random();

		tree.GenerateTreeUsingEditorParams();

		trunkMesh.SetVertices(tree.trunkVertices);
		trunkMesh.SetIndices(tree.trunkIndices.ToArray(), MeshTopology.Triangles, 0);
		trunkMesh.SetUVs(0, tree.trunkUvs);
		trunkMesh.UploadMeshData(markNoLogerReadable: false);
		trunkMesh.Optimize();
		trunkMesh.RecalculateNormals();
		trunkMeshFilter.sharedMesh = trunkMesh;
		trunkMeshRenderer.sharedMaterial = treeMaterial;

		branchMesh.SetVertices(tree.branchVertices);
		branchMesh.SetIndices(tree.branchIndices.ToArray(), MeshTopology.Triangles, 0);
		branchMesh.SetUVs(0, tree.branchUvs);
		branchMesh.UploadMeshData(markNoLogerReadable: false);
		branchMesh.Optimize();
		branchMesh.RecalculateNormals();
		branchMeshFilter.sharedMesh = branchMesh;
		branchMeshRenderer.sharedMaterial = treeMaterial;

		leafStemMesh.SetVertices(tree.leafStemVertices);
		leafStemMesh.SetIndices(tree.leafStemIndices.ToArray(), MeshTopology.Triangles, 0);
		leafStemMesh.SetUVs(0, tree.leafStemUvs);
		leafStemMesh.UploadMeshData(markNoLogerReadable: false);
		leafStemMesh.Optimize();
		leafStemMesh.RecalculateNormals();
		leafStemMeshFilter.sharedMesh = leafStemMesh;
		leafStemMeshRenderer.sharedMaterial = treeMaterial;

		leafMesh.SetVertices(tree.leafVertices);
		leafMesh.SetIndices(tree.leafIndices.ToArray(), MeshTopology.Triangles, 0);
		leafMesh.SetUVs(0, tree.leafUvs);
		leafMesh.UploadMeshData(markNoLogerReadable: false);
		leafMesh.Optimize();
		leafMesh.RecalculateNormals();
		leafMeshFilter.sharedMesh = leafMesh;
		leafMeshRenderer.sharedMaterial = leafMaterial;
	}
}
