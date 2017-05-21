using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions.Comparers;

public abstract class Stem
{
    public int levelOfRecursion;
    public float widthLengthRatio;

    public int segCount;
    public int numberOfSectors;
    public Vector3 basePoint;
    public Quaternion baseRotation;
    public List<Segment> segments;
    public float radius;
    public float length;
    public float taper;
    public float splitFactor;

    public float splitAngle;
    public float splitAngleVariation;
    public float curveAngle;
    public float curveAngleVariation;

    protected float curveAnglePerSegment;
    protected float curveAngleVariationPerSegment;
    protected float radiusReduceStep;

    public int branchNumber;
    public float branchingRotationY;

    public float branchingAngle;
    public float branchingAngleVariation;

    public float branchingPoint;
    public float branchingFactor;

    public float beginningRotation;

    public Tree tree;

    public List<Vector3> vertices;
    public List<int> indices;
    public List<Vector2> uvs;

    public void GenerateStem()
    {
        segments = new List<Segment>();
        vertices = new List<Vector3>();
        indices = new List<int>();
        uvs = new List<Vector2>();

        curveAnglePerSegment = curveAngle / segCount;
        curveAngleVariationPerSegment = curveAngleVariation / segCount;
        radiusReduceStep = radius * (1 - taper) / segCount;

        GenerateSpecifics();
        GenerateSegments(1, null, curveAnglePerSegment, curveAngleVariationPerSegment, false);
        GenerateStemMesh();
        if (levelOfRecursion < tree.recursionDepth)
        {
            GenerateBranches();
        }
        else if (levelOfRecursion == tree.recursionDepth)
        {
            GenerateLeafStems();
        }
        else if (levelOfRecursion == tree.recursionDepth + 1)
        {
            GenerateLeaves();
        }
    }

    public void GenerateStemMesh()
    {
        int iter = 0;

        foreach (var segment in segments)
        {
            if (iter == 0)
            {
                segment.bottomOffset = 0;
                segment.topOffset = 0;
            }
            else
            {
                segment.bottomOffset = vertices.Count;
                segment.topOffset = vertices.Count;
            }

            segment.GenerateSegmentMeshWithSplits();

            vertices.AddRange(segment.bottomVertices);
            vertices.AddRange(segment.topVertices);

            indices.AddRange(segment.indices);
            uvs.AddRange(segment.bottomUvs);
            uvs.AddRange(segment.topUvs);
            iter++;
        }
    }

    public abstract void GenerateSpecifics();
    public abstract void GenerateSegments(int segNumber, Segment parent, float angle, float angleVariation, bool prevSplit);

    public void GenerateBranches()
    {
        float nextRand = (float) tree.random.NextDouble();
        int iter = 0;

        foreach (var segment in segments)
        {
            if (iter > segCount * branchingPoint && nextRand < branchingFactor)
            {
                GenerateBranches(segment, iter * branchingRotationY);
            }
            iter++;
        }
    }

    public abstract void GenerateBranches(Segment segment, float yRotationOffset);

    public void GenerateLeafStems()
    {
        float rotationYOffset = 0;
        int iter = 0;
        foreach(var segment in segments)
        {
            if ((float) segment.segNumber / segCount > tree.leafStemPoint)
            {
                Vector3 step = (segment.top - segment.bottom) / tree.leafStemNodesPerSegment;
                for (int i = 0; i < tree.leafStemNodesPerSegment; i++)
                {
                    Vector3 stemBasePoint = segment.bottom + i * step;
                    float rotationY = 360.0f / tree.leafStemsPerNode;
                    for (int j = 0; j < tree.leafStemsPerNode; j++)
                    {
                        LeafStem leafStem = new LeafStem();
                        leafStem.tree = tree;
                        leafStem.curveAngle = tree.leafStemCurve;
                        leafStem.curveAngleVariation = tree.leafStemCurveVariation;
                        leafStem.curveBackAngle = tree.leafStemCurveBack;
                        leafStem.curveBackAngleVariation = tree.leafStemCurveBackVariation;
                        leafStem.splitFactor = tree.leafStemSplitFactor;
                        leafStem.splitAngle = tree.leafStemSplitAngle;
                        leafStem.splitAngleVariation = tree.leafStemSplitAngleVariation;
                        leafStem.radius = tree.leafStemRadius;
                        leafStem.length = tree.leafStemLength;
                        leafStem.taper = 1.0f;
                        leafStem.segCount = tree.leafStemSegCount;
                        leafStem.numberOfSectors = numberOfSectors;
                        leafStem.basePoint = stemBasePoint;
                        leafStem.baseRotation = segment.topRotation *
                                                Quaternion.Euler(0, rotationYOffset + j * rotationY,
                                                    tree.leafStemAngle);
                        leafStem.levelOfRecursion = levelOfRecursion + 1;

                        leafStem.GenerateStem();
                        segment.childLeafStems.Add(leafStem);
                    }
                    rotationYOffset += tree.leafStemYRotation;
                }
            }
            iter++;
        }
    }

    public abstract void GenerateLeaves();
}
