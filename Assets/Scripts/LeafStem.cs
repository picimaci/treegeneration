using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class LeafStem : Stem
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

        segment.topRadius = segment.bottomRadius;
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

    public override void GenerateBranches(Segment segment, float yRotationOffset)
    {
    }

    public override void GenerateLeaves()
    {
        float rotationYOffset = 0;
        foreach(var segment in segments)
        {
            Vector3 step = (segment.top - segment.bottom) / tree.leafStemNodesPerSegment;
            for (int i = 0; i < tree.leafNodePerSegment; i++)
            {
                Vector3 leafBasePoint = segment.bottom + i * step;
                float rotationY = 360.0f / tree.leafStemsPerNode;
                for (int j = 0; j < tree.leavesPerNode; j++)
                {
                    Leaf leaf = new Leaf();
                    leaf.basePoint = leafBasePoint;
                    leaf.baseRotation = segment.topRotation * Quaternion.Euler(tree.leafRotationXAngle, rotationYOffset + j * rotationY, tree.leafAngle);
                    leaf.length = tree.leafLength;
                    leaf.width = tree.leafWidth;

                    segment.childLeaves.Add(leaf);
                    leaf.GenerateLeaf();
                }
            }
            rotationYOffset += tree.leafStemYRotation;
        }
    }
}
