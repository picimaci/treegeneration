using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class Branch : Stem
{
    public float curveBackAngle;
    public float curveBackAngleVariation;
    protected float curveBackAnglePerSegment;
    protected float curveBackAngleVariationPerSegment;

    public override void GenerateSpecifics()
    {
        curveBackAnglePerSegment = curveBackAngle / segCount;
        curveBackAngleVariationPerSegment = curveBackAngleVariation / segCount;
    }

    public override void GenerateSegments(int segNumber, Segment parent, float angle, float angleVariation, bool prevSplit)
    {
        float adjustedSplitProbability = splitFactor;

        Segment segment = new Segment();
        SetSegmentProperties(segNumber, segment, parent, angle, angleVariation, prevSplit);
        segment.GenerateSegment();
        segments.Add(segment);

        if (prevSplit)
        {
            adjustedSplitProbability *= 0.5f;
        }

        if (segNumber < segCount)
        {
            GenerateNextSegments(segment, segNumber, adjustedSplitProbability);
        }
        else
        {
            segment.lastSegment = true;
        }
    }

    public void GenerateNextSegments(Segment segment, int segNumber, float split)
    {
        if (segNumber < segCount / 2.0f)
        {
            GenerateSplitting(segment, segNumber, split, curveAnglePerSegment, curveAngleVariationPerSegment);
        }
        else
        {
            GenerateSplitting(segment, segNumber, split, curveBackAnglePerSegment, curveBackAngleVariationPerSegment);
        }
    }

    public void GenerateSplitting(Segment segment, int segNumber, float currentSplitProbability, float currentCurveAngle, float currentCurveAngleVariation)
    {
        float nextRand = (float)tree.random.NextDouble();
        if (nextRand < currentSplitProbability)
        {
            GenerateSegments(segNumber + 1, segment, -splitAngle - currentCurveAngle,
                splitAngleVariation + currentCurveAngleVariation, true);
            GenerateSegments(segNumber + 1, segment, splitAngle + currentCurveAngle,
                splitAngleVariation + currentCurveAngleVariation, true);
        }
        else
        {
            GenerateSegments(segNumber + 1, segment, currentCurveAngle, currentCurveAngleVariation, false);
        }
    }

    public void SetSegmentProperties(int segNumber, Segment segment, Segment parent, float angle, float angleVariation, bool prevSplit)
    {
        segment.numberOfSectors = numberOfSectors;
        segment.length = length / segCount;

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
            float rotateY = Random.Range(-15, 15);
            float rotateZ = angle + Random.Range(-angleVariation, angleVariation);
            segment.topRotation = segment.bottomRotation * Quaternion.Euler(0, rotateY, rotateZ);
        }
        segment.topRadius = segment.bottomRadius - radiusReduceStep;
    }

    public override void GenerateBranches(Segment segment, float yRotationOffset)
    {
        int currentBranchNumber = Random.Range(0,branchNumber);
        float rotationY = 360.0f / currentBranchNumber;
        float rotationZ = branchingAngle + Random.Range(-1, 1) * branchingAngleVariation;
        for (int i = 0; i < currentBranchNumber; i++)
        {
            Branch branch = new Branch();
            branch.tree = tree;
            branch.segCount = segCount;
            branch.numberOfSectors = numberOfSectors;
            branch.baseRotation = segment.topRotation * Quaternion.Euler(0, yRotationOffset + i * rotationY, rotationZ);
            branch.basePoint = segment.bottom;
            branch.radius = segment.bottomRadius * tree.childParentRatio;
            branch.length = branch.radius * tree.widthLengthRatio;
            branch.taper = taper;
            branch.splitFactor = splitFactor;
            branch.curveAngle = curveAngle;
            branch.curveAngleVariation = curveAngleVariation;
            branch.curveBackAngle = curveBackAngle;
            branch.curveBackAngleVariation = curveBackAngleVariation;
            branch.splitAngle = splitAngle;
            branch.splitAngleVariation = splitAngleVariation;
            branch.branchNumber = branchNumber;
            branch.branchingPoint = branchingPoint;
            branch.branchingAngle = branchingAngle;
            branch.branchingAngleVariation = branchingAngleVariation;
            branch.branchingFactor = branchingFactor;
            branch.branchingRotationY = branchingRotationY;

            branch.levelOfRecursion = levelOfRecursion + 1;

            segment.childBranches.Add(branch);

            branch.GenerateStem();
        }
    }
}
