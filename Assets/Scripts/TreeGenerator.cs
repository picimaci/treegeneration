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

	private MeshRenderer treeMeshRenderer;
	private MeshRenderer leafMeshRenderer;
	private MeshFilter treeMeshFilter;
	private MeshFilter leafMeshFilter;

	private Mesh treeMesh;
	private Mesh leafMesh;

	[Header("General Tree Params")]
	public Material treeMaterial;
	public Material leafMaterial;
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

	[Header("LeafStem and Leaf Params")]
	//public Texture leafTexture;
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

		Transform treeTr = transform.Find("Tree");
		if (!treeTr)
		{
			GameObject treeGO = new GameObject("Tree");
			treeTr = treeGO.transform;
			treeTr.parent = transform;
			treeTr.localPosition = Vector3.zero;
			treeTr.localRotation = Quaternion.identity;
		}
		treeMeshRenderer = treeTr.GetComponent<MeshRenderer>();
		if (!treeMeshRenderer)
			treeMeshRenderer = treeTr.gameObject.AddComponent<MeshRenderer>();
		treeMeshFilter = treeTr.GetComponent<MeshFilter>();
		if (!treeMeshFilter)
			treeMeshFilter = treeTr.gameObject.AddComponent<MeshFilter>();

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
		if (treeMesh)
			DestroyImmediate(treeMesh);
		treeMesh = new Mesh {name = "treeMesh"};

		if(leafMesh)
			DestroyImmediate(leafMesh);
		leafMesh = new Mesh {name = "leafMesh"};

		Tree tree = new Tree();
		tree.childParentRatio = childrenParentRatio;
		tree.widthLengthRatio = widthLengthRatio;
		tree.recursionDepth = recursionDepth;
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
		treeMeshFilter.sharedMesh = treeMesh;
		treeMeshRenderer.sharedMaterial = treeMaterial;

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
