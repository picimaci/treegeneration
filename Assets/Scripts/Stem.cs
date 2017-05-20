using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Stem
{
    public int levelOfRecursion;

    public int segCount;
    public int numberOfSectors;
    public Vector3 basePoint;
    public Quaternion baseRotation;
    public List<Segment> segments;
    public float radius;
    public float length;
    public float taper;
    public float splitFactor;

    public Vector3 splitAngle;
    public Vector3 splitAngleVariation;
    public Vector3 curveAngle;
    public Vector3 curveAngleVariation;

    protected Vector3 curveAnglePerSegment;
    protected Vector3 curveAngleVariationPerSegment;
    protected float radiusReduceStep;

    public Vector3 branchingAngle;
    public Vector3 branchingAngleVariation;

    public float branchingPoint;
    public float branchingFactor;

    public Tree tree;

    public List<Vector3> vertices;
    public List<int> indices;

    public void GenerateStem()
    {
        segments = new List<Segment>();
        vertices = new List<Vector3>();
        indices = new List<int>();

        curveAnglePerSegment = curveAngle / segCount;
        curveAngleVariationPerSegment = curveAngleVariation / segCount;
        radiusReduceStep = radius * (1 - taper) / segCount;

        GenerateSpecifics();
        GenerateSegments(1, null, curveAnglePerSegment, curveAngleVariationPerSegment, false);
        GenerateStemMesh();
        GenerateBranches();
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
                segment.bottomOffset = segment.parent.topOffset + numberOfSectors;
                segment.topOffset = vertices.Count - numberOfSectors;
            }

            segment.GenerateSegmentMeshWithSplits();
            if (iter == 0)
            {
                vertices.AddRange(segment.bottomVertices);
            }
            vertices.AddRange(segment.topVertices);

            indices.AddRange(segment.indices);
            iter++;
        }
    }

    public abstract void GenerateSpecifics();
    public abstract void GenerateSegments(int segNumber, Segment parent, Vector3 angle, Vector3 angleVariation, bool prevSplit);
    public abstract void GenerateBranches();
    public abstract void GenerateBranches(Segment segment);
}
