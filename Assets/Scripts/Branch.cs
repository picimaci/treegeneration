using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Branch : Stem
{

    public Vector3 curveBackAngle;
    public Vector3 curveBackAngleVariation;
    protected Vector3 curveBackAnglePerSegment;
    protected Vector3 curveBackAngleVariationPerSegment;

    public override void GenerateSpecifics()
    {
        curveBackAnglePerSegment = curveBackAngle / segCount;
        curveBackAngleVariationPerSegment = curveBackAngleVariation / segCount;
    }

    public override void GenerateSegments(int segNumber, Segment parent, Vector3 angle, Vector3 angleVariation, bool prevSplit)
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

    public void GenerateSplitting(Segment segment, int segNumber, float currentSplitProbability, Vector3 currentCurveAngle, Vector3 currentCurveAngleVariation)
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

    public void SetSegmentProperties(int segNumber, Segment segment, Segment parent, Vector3 angle, Vector3 angleVariation, bool prevSplit)
    {
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
    }

    public override void GenerateBranches()
    {
        float nextRand = (float) tree.random.NextDouble();
        int iter = 0;

        foreach (var segment in segments)
        {
            if (levelOfRecursion < tree.recursionDepth && iter > segCount * branchingPoint && nextRand < branchingFactor)
            {
                GenerateBranches(segment);
            }
            iter++;
        }
    }

    public override void GenerateBranches(Segment segment)
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

        branch.levelOfRecursion = levelOfRecursion + 1;

        segment.childBranches.Add(branch);

        branch.GenerateStem();
    }
}
