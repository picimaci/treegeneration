using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trunk : Stem
{
    public float baseSplitFactor;
    public float baseSplitPoint;
    public Vector3 baseSplitAngle;
    public Vector3 baseSplitAngleVariation;
    public float flare;

    public Vector3 curveAngleForBranch;
    public Vector3 curveAngleVariationForBranch;
    public Vector3 curveBackAngleForBranch;
    public Vector3 curveBackAngleVariationForBranch;
    public float splitForBranch;


    public override void GenerateSpecifics()
    {
    }

    public override void GenerateSegments(int segNumber, Segment parent, Vector3 angle, Vector3 angleVariation, bool prevSplit)
    {
        float adjustedSplitProbability = splitFactor;
        float adjustedBaseSplitProbability = baseSplitFactor;

        Segment segment = new Segment();
        segments.Add(segment);
        SetSegmentProperties(segNumber, segment, parent, angle, angleVariation, prevSplit);
        segment.GenerateSegment();

        if (prevSplit)
        {
            adjustedSplitProbability *= 0.5f;
            adjustedBaseSplitProbability *= 0.5f;
        }

        if (segNumber < segCount)
        {
            GenerateNextSegments(segment, segNumber, adjustedSplitProbability, adjustedBaseSplitProbability);
        }
    }

    public void SetSegmentProperties(int segNumber, Segment segment, Segment parent, Vector3 angle, Vector3 angleVariation, bool prevSplit)
    {
        segment.numberOfSectors = numberOfSectors;
        segment.length = length / segCount;

        if (segNumber >= segCount - 1)
        {
            segment.length *= 1.5f;
        }
        if (parent == null)
        {
            segment.bottomRadius = radius * flare;
            segment.bottom = basePoint;
            segment.bottomRotation = baseRotation;
            segment.topRotation = baseRotation;
            segment.topRadius = radius;
        }
        else
        {
            segment.bottomRadius = parent.topRadius;
            segment.bottom = parent.top;
            segment.bottomRotation = parent.topRotation;
            segment.parent = parent;
            Vector3 topAngle = angle + (float) tree.random.NextDouble() * angleVariation;
            segment.topRotation = segment.bottomRotation * Quaternion.Euler(topAngle);
            if (prevSplit)
                segment.bottomRotation = segment.topRotation;
            segment.topRadius = segment.bottomRadius - radiusReduceStep;
        }
    }

    public void GenerateNextSegments(Segment segment, int segNumber, float split, float baseSplit)
    {
        if (segNumber < segCount * baseSplitPoint)
        {
            GenerateSplitting(segment, segNumber, baseSplit, baseSplitAngle, baseSplitAngleVariation);
        }
        else
        {
            GenerateSplitting(segment, segNumber, split, splitAngle, splitAngleVariation);
        }
    }

    public void GenerateSplitting(Segment segment, int segNumber, float currentSplitProbability, Vector3 currentSplitAngle, Vector3 currentSplitAngleVariation)
    {
        float nextRand = (float)tree.random.NextDouble();
        if (nextRand < currentSplitProbability)
        {
            GenerateSegments(segNumber + 1, segment, -currentSplitAngle - curveAnglePerSegment,
                currentSplitAngleVariation + curveAngleVariationPerSegment, true);
            GenerateSegments(segNumber + 1, segment, currentSplitAngle + curveAnglePerSegment,
                currentSplitAngleVariation + curveAngleVariationPerSegment, true);
        }
        else
        {
            GenerateSegments(segNumber + 1, segment, curveAnglePerSegment, curveAngleVariationPerSegment, false);
        }
    }

    public override void GenerateBranches()
    {
        float nextRand = (float) tree.random.NextDouble();
        int iter = 0;

        foreach (var segment in segments)
        {
            if (iter > segCount * branchingPoint && nextRand < branchingFactor)
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
        branch.length = length * tree.childParentRatio;
        branch.taper = taper;
        branch.splitFactor = splitForBranch;
        branch.curveAngle = curveAngleForBranch;
        branch.curveAngleVariation = curveAngleVariationForBranch;
        branch.curveBackAngle = curveBackAngleForBranch;
        branch.curveBackAngleVariation = curveBackAngleVariationForBranch;
        branch.splitAngle = splitAngle;
        branch.splitAngleVariation = splitAngleVariation;
        branch.branchingPoint = 0;
        branch.branchingAngle = branchingAngle;
        branch.branchingAngleVariation = branchingAngleVariation;
        branch.branchingFactor = 0.5f;

        branch.levelOfRecursion = 1;

        segment.childBranches.Add(branch);

        branch.GenerateStem();
    }
}