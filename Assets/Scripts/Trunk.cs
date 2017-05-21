using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class Trunk : Stem
{
    public float baseSplitFactor;
    public float baseSplitPoint;
    public float baseSplitAngle;
    public float baseSplitAngleVariation;
    public float flare;

    public float curveAngleForBranch;
    public float curveAngleVariationForBranch;
    public float curveBackAngle;
    public float curveBackAngleVariation;
    public float branchBranchingFactor;
    public float branchingPointForBranch;
    public float branchSplitFactor;
    public float branchSplitAngle;
    public float branchSplitAngleVariation;
    public float branchTaper;
    public float branchWidthLengthRatio;

    public override void GenerateSpecifics()
    {
    }

    public override void GenerateSegments(int segNumber, Segment parent, float angle, float angleVariation, bool prevSplit)
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
        else
        {
            segment.lastSegment = true;
        }
    }

    public void SetSegmentProperties(int segNumber, Segment segment, Segment parent, float angle, float angleVariation, bool prevSplit)
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
            float rotateY = Random.Range(-15, 15);
            float rotateZ = angle + Random.Range(-angleVariation, angleVariation);
            segment.topRotation = segment.bottomRotation * Quaternion.Euler(0, rotateY, rotateZ);
            if (prevSplit)
            {
                segment.bottomRotation = segment.topRotation;
            }
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

    public void GenerateSplitting(Segment segment, int segNumber, float currentSplitProbability, float currentSplitAngle, float currentSplitAngleVariation)
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

    public override void GenerateBranches(Segment segment, float yRotationOffset)
    {
        int currentBranchNumber = Random.Range(1,branchNumber);
        float rotationY = 360.0f / currentBranchNumber;
        float rotationZ = branchingAngle + Random.Range(-1, 1) * branchingAngleVariation;

        for (int i = 0; i < currentBranchNumber; i++)
        {
            Branch branch = new Branch();
            branch.tree = tree;
            branch.segCount = segCount;
            branch.numberOfSectors = numberOfSectors;
            branch.widthLengthRatio = branchWidthLengthRatio;
            branch.baseRotation = segment.topRotation * Quaternion.Euler(0, yRotationOffset + i * rotationY, rotationZ);
            branch.basePoint = segment.bottom;
            branch.radius = segment.bottomRadius * tree.childParentRatio;
            branch.length = branch.radius * branch.widthLengthRatio;
            branch.taper = taper;
            branch.splitFactor = branchSplitFactor;
            branch.curveAngle = curveAngleForBranch;
            branch.curveAngleVariation = curveAngleVariationForBranch;
            branch.curveBackAngle = curveBackAngle;
            branch.curveBackAngleVariation = curveBackAngleVariation;
            branch.splitAngle = branchSplitAngle;
            branch.splitAngleVariation = branchSplitAngleVariation;
            branch.branchNumber = branchNumber;
            branch.branchingPoint = branchingPointForBranch;
            branch.branchingAngle = branchingAngle;
            branch.branchingAngleVariation = branchingAngleVariation;
            branch.branchingFactor = branchBranchingFactor;
            branch.branchingRotationY = branchingRotationY;

            branch.levelOfRecursion = 1;

            segment.childBranches.Add(branch);

            branch.GenerateStem();
        }
    }
}