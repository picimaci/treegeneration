using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Branch : Stem
{

    public Vector3 curveBackAngle;
    public Vector3 curveBackAngleVariation;

    public void GenerateForTesting()
    {
        segCount = 5;
        basePoint = new Vector3(0,0,0);
        baseRotation = Quaternion.Euler(-90,0,0);
        taper = 0.75f;
        length = 5;
        splitFactor = 0.5f;
        radius = 0.2f;
        numberOfSectors = 5;
        curveAngle = new Vector3(20,10,0);
        curveAngleVariation = new Vector3(15,5,0);
        splitAngle = new Vector3(0,0,45);
        splitAngleVariation = new Vector3(0,0,15);
        curveBackAngle = new Vector3(-10,-10,0);
        curveBackAngleVariation = new Vector3(5,5,0);

        GenerateBranch();
    }

    public void GenerateBranch()
    {
        segments = new List<Segment>();
        vertices = new List<Vector3>();
        indices = new List<int>();

        GenerateSegments(1, null, curveAngle, curveAngleVariation, false);
        GenerateBranchMesh();
    }

    public void GenerateSegments(int segNumber, Segment parent, Vector3 angle, Vector3 angleVariation, bool prevSplit)
    {
        float radiusReduceStep = radius * (1-taper) / segCount;
        float splitThisShit = splitFactor;

        Segment segment = new Segment();
        segment.numberOfSectors = numberOfSectors;
        segment.length = length / segCount;

        float nextRand = (float) tree.random.NextDouble();

        if (segNumber == 1)
        {
            segment.bottomRadius = radius;
            segment.bottom = basePoint;
            segment.bottomRotation = baseRotation;
            segment.topRotation = baseRotation;
        }
        else
        {
            segment.bottomRadius = parent.topRadius;
            segment.bottom = parent.top;
            segment.bottomRotation = parent.topRotation;
            segment.parent = parent;
            segment.topRotation = segment.bottomRotation * Quaternion.Euler(angle + nextRand * angleVariation);
        }
        segment.topRadius = segment.bottomRadius - radiusReduceStep;

        segment.GenerateSegment();

        segments.Add(segment);

        if (prevSplit)
        {
            splitThisShit *= 0.7f;
        }

        nextRand = (float) tree.random.NextDouble();

        if (levelOfRecursion < tree.recursionDepth && segNumber > segCount * branchingPoint && nextRand < branchingFactor)
        {
            GenerateBranches(segment);
        }

        if (segNumber < segCount)
        {
            nextRand = (float) tree.random.NextDouble();
            //Debug.Log(nextRand);
            if (nextRand < splitThisShit)
            {
                if (segNumber < segCount / 2.0f)
                {
                    GenerateSegments(segNumber + 1, segment, -splitAngle + curveAngle, splitAngleVariation + curveAngleVariation, true);
                    GenerateSegments(segNumber + 1, segment, splitAngle + curveAngle, splitAngleVariation + curveAngleVariation, true);
                }
                else
                {
                    GenerateSegments(segNumber + 1, segment, -splitAngle + curveBackAngle, splitAngleVariation + curveBackAngleVariation, true);
                    GenerateSegments(segNumber + 1, segment, splitAngle + curveBackAngle, splitAngleVariation + curveBackAngleVariation, true);
                }
            }
            else
            {
                if (segNumber < segCount / 2.0f)
                {
                    GenerateSegments(segNumber + 1, segment, curveAngle, curveAngleVariation, false);
                }
                else
                {
                    GenerateSegments(segNumber + 1, segment, curveBackAngle, curveBackAngleVariation, false);
                }
            }
        }
    }

    public void GenerateBranchMesh()
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
                segment.topOffset = iter * numberOfSectors;
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

    public void GenerateBranches(Segment segment)
    {
        Branch branch = new Branch();
        branch.tree = tree;
        branch.segCount = segCount;
        branch.numberOfSectors = numberOfSectors;
        branch.baseRotation = segment.bottomRotation * Quaternion.Euler(branchingAngle + (float)tree.random.NextDouble() * branchingAngleVariation);
        branch.basePoint = segment.bottom;
        branch.radius = segment.bottomRadius * tree.childParentRatio;
        branch.length = branch.radius * tree.widthLengthRatio;
        branch.taper = taper;
        branch.splitFactor = 0f;
        branch.curveAngle = curveAngle;
        branch.curveAngleVariation = curveAngleVariation;
        branch.curveBackAngle = curveBackAngle;
        branch.curveBackAngleVariation = curveBackAngleVariation;
        branch.splitAngle = splitAngle;
        branch.splitAngleVariation = splitAngleVariation;
        branch.branchingPoint = 0;
        branch.branchingAngle = branchingAngle;
        branch.branchingAngleVariation = branchingAngleVariation;
        branch.branchingFactor = branchingFactor;

        segment.branchList.Add(branch);

        branch.GenerateBranch();
    }
}
