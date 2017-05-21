using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts;

public class Tree
{
    public System.Random random;

    public int recursionDepth;
    public Shape shape;
    public Vector3 basePoint;
    public float widthLengthRatio;
    public float childParentRatio;

    public Trunk trunk;

    public int leafStemNodesPerSegment;
    public int leafStemsPerNode;
    public float leafStemYRotation;
    public float leafStemAngle;
    public float leafStemCurve;
    public float leafStemCurveVariation;
    public float leafStemCurveBack;
    public float leafStemCurveBackVariation;
    public int leafNodePerSegment;
    public float leavesPerNode;
    public float leafYRotation;
    public float leafStemRadius;
    public float leafStemLength;
    public float leafStemSplitFactor;
    public float leafStemSplitAngle;
    public float leafStemSplitAngleVariation;
    public int leafStemSegCount;
    public float leafAngle;
    public float leafRotationXAngle;
    public float leafWidth;
    public float leafLength;
    public Texture leafTexture;

    public List<Vector3> trunkVertices;
    public List<int> trunkIndices;
    public List<Vector2> trunkUvs;

    public List<Vector3> branchVertices;
    public List<int> branchIndices;
    public List<Vector2> branchUvs;

    public List<Vector3> leafStemVertices;
    public List<int> leafStemIndices;
    public List<Vector2> leafStemUvs;

    public List<Vector3> leafVertices;
    public List<int> leafIndices;
    public List<Vector2> leafUvs;

    public void SetTreeShape(Shape s)
    {
        shape = s;
        switch (shape)
        {
            case Shape.Apple:
                childParentRatio = 0.75f;
                recursionDepth = 2;
                break;
            case Shape.Palm:
                break;
            case Shape.Pine:
                break;
            case Shape.Willow:
                break;
            default:
                Debug.Log("Tree Shape does not exist");
                break;
        }
    }

    public void GenerateTreeUsingEditorParams()
    {
        trunkVertices = new List<Vector3>();
        trunkIndices = new List<int>();
        trunkUvs = new List<Vector2>();

        branchVertices = new List<Vector3>();
        branchIndices = new List<int>();
        branchUvs = new List<Vector2>();

        leafStemVertices = new List<Vector3>();
        leafStemIndices = new List<int>();
        leafStemUvs = new List<Vector2>();

        leafVertices = new List<Vector3>();
        leafIndices = new List<int>();
        leafUvs = new List<Vector2>();

        trunk.GenerateStem();
        GenerateTreeMesh();
    }

    public void GenerateTreeMesh()
    {
        GenerateMeshRecursively(trunk);
    }

    public void GenerateMeshRecursively(Stem stem)
    {
        if (stem.levelOfRecursion == 0)
        {
            trunkVertices.AddRange(trunk.vertices);
            trunkIndices.AddRange(trunk.indices);
            trunkUvs.AddRange(trunk.uvs);
        } else
        if (stem.levelOfRecursion < recursionDepth + 1)
        {
            int offset = branchVertices.Count;
            branchVertices.AddRange(stem.vertices);
            foreach (var index in stem.indices)
            {
                branchIndices.Add(offset + index);
            }
            branchUvs.AddRange(stem.uvs);
        } else
        if (stem.levelOfRecursion == recursionDepth + 1)
        {
            int offset = leafStemVertices.Count;
            leafStemVertices.AddRange(stem.vertices);
            foreach (var index in stem.indices)
            {
                leafStemIndices.Add(offset + index);
            }
            leafStemUvs.AddRange(stem.uvs);
        }

        foreach (var segment in stem.segments)
        {
            foreach (var branch in segment.childBranches)
            {
                GenerateMeshRecursively(branch);
            }
            foreach (var leafStem in segment.childLeafStems)
            {
                GenerateMeshRecursively(leafStem);
            }
            foreach (var leaf in segment.childLeaves)
            {
                AddLeaf(leaf);
            }
        }
    }

    public void AddLeaf(Leaf leaf)
    {
        int indiceOffset = leafVertices.Count;
        leafVertices.AddRange(leaf.vertices);
        foreach (var indice in leaf.indices)
        {
            leafIndices.Add(indiceOffset + indice);
        }
        leafUvs.AddRange(leaf.uvs);
    }
}
