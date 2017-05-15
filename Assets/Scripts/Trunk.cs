using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trunk : Stem
{
    public float baseSplit;
    public float baseSplitPoint;
    public Vector3 baseSplitAngle;
    public Vector3 baseSplitAngleVariation;
    public float flare;

    public Vector3 curveBackAngleForBranch;
    public Vector3 curveBackAngleVariationForBranch;
    public float splitForBranch;

    public void GenerateForTesting()
    {
        segCount = 7;
        basePoint = new Vector3(0,0,0);
        baseRotation = Quaternion.Euler(0,0,0);
        taper = 0.7f;
        length = 8;
        baseSplitPoint = 0.5f;
        baseSplit = 0f;
        split = 0.5f;
        radius = 0.5f;
        numberOfSectors = 4;
        curveAngle = new Vector3(5,0,5);
        curveAngleVariation = new Vector3(-10,15,-10);
        splitAngle = new Vector3(0,45,30);
        splitAngleVariation = new Vector3(0,-90,-30);
        baseSplitAngle = new Vector3(0,0,15);
        baseSplitAngleVariation = new Vector3(0,0,-30);
        flare = 2.0f;

        branchingPoint = 0.8f;
        branchingFactor = 0.5f;
        branchingAngle = new Vector3(0,180,45);
        branchingAngleVariation = new Vector3(0,-360,5);

        GenerateTrunk();
    }

    public void GenerateTrunk()
    {
        segments = new List<Segment>();
        vertices = new List<Vector3>();
        indices = new List<int>();

        GenerateSegments(1, null, curveAngle, curveAngleVariation, false);
        GenerateTrunkMesh();
    }

    public void GenerateSegments(int segNumber, Segment parent, Vector3 angle, Vector3 angleVariation, bool prevSplit)
    {
        float radiusReduceStep = radius * (1-taper) / segCount;
        float splitThisShit = split;
        float baseSplitThisShit = baseSplit;
        Vector3 curveVariation = curveAngleVariation;

        Segment segment = new Segment();
        segment.numberOfSectors = numberOfSectors;
        segment.length = length / segCount;

        if (segNumber == segCount || segNumber == segCount - 1)
        {
            segment.length *= 1.5f;
        }

        if (segNumber == 1)
        {
            segment.bottomRadius = radius * tree.childParentRatio * flare;
            segment.bottom = basePoint;
            segment.bottomRotation = baseRotation;
            segment.topRotation = baseRotation;
            segment.topRadius = radius * tree.childParentRatio;
        }
        else
        {
            segment.bottomRadius = parent.topRadius;
            segment.bottom = parent.top;
            segment.bottomRotation = parent.topRotation;
            segment.parent = parent;
            segment.topRotation = segment.bottomRotation * Quaternion.Euler(angle + (float)tree.random.NextDouble() * angleVariation);
            segment.topRadius = segment.bottomRadius - radiusReduceStep;
        }

        segment.GenerateSegment();

        segments.Add(segment);

        if (prevSplit)
        {
            splitThisShit *= 0.5f;
            baseSplitThisShit *= 0.7f;
        }
        float nextRand = (float) tree.random.NextDouble();

        if (segNumber > segCount * branchingPoint && nextRand < branchingFactor)
        {
            GenerateBranches(segment);
        }

        if (segNumber < segCount)
        {
            if (segNumber < segCount * baseSplitPoint)
            {

                //Debug.Log(nextRand);
                if (nextRand < baseSplitThisShit)
                {
                    GenerateSegments(segNumber + 1, segment, -baseSplitAngle - curveAngle,
                        baseSplitAngleVariation + curveVariation, true);
                    GenerateSegments(segNumber + 1, segment, baseSplitAngle + curveAngle,
                        baseSplitAngleVariation + curveVariation, true);
                }
                else
                {
                    GenerateSegments(segNumber + 1, segment, curveAngle, curveVariation, false);
                }
            }
            else
            {
                if (nextRand < splitThisShit)
                {
                    GenerateSegments(segNumber + 1, segment, -splitAngle - curveAngle,
                        splitAngleVariation + curveVariation, true);
                    GenerateSegments(segNumber + 1, segment, splitAngle + curveAngle,
                        splitAngleVariation + curveVariation, true);
                }
                else
                {
                    GenerateSegments(segNumber + 1, segment, curveAngle, curveVariation, false);
                }
            }
        }
    }

    public void GenerateTrunkMesh()
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
        branch.baseRotation = segment.bottomRotation;
        branch.basePoint = segment.bottom;
        branch.radius = segment.bottomRadius * tree.childParentRatio;
        branch.length = length * tree.childParentRatio;
        branch.taper = taper;
        branch.split = splitForBranch;
        branch.curveAngle = curveAngle;
        branch.curveAngleVariation = curveAngleVariation;
        branch.curveBackAngle = curveBackAngleForBranch;
        branch.curveBackAngleVariation = curveBackAngleVariationForBranch;
        branch.splitAngle = splitAngle;
        branch.splitAngleVariation = splitAngleVariation;
        branch.branchingPoint = 0;
        branch.branchingAngle = branchingAngle;
        branch.branchingAngleVariation = branchingAngleVariation;
        branch.branchingFactor = 0.5f;

        branch.levelOfRecursion = 1;

        segment.branchList.Add(branch);

        branch.GenerateBranch();
    }
}