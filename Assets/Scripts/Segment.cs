using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Segment
{
    public Vector3 bottom;
    public Vector3 top;
    public float bottomRadius;
    public float topRadius;
    public float length;
    public Quaternion bottomRotation;
    public Quaternion topRotation;
    public List<Branch> childBranches;
    public List<LeafStem> childLeafStems;
    public List<Leaf> childLeaves;
    public int numberOfSectors;
    public int bottomOffset;
    public int topOffset;
    public bool lastSegment;

    public List<int> indices;
    public List<Vector3> bottomVertices;
    public List<Vector3> topVertices;
    public List<Vector2> bottomUvs;
    public List<Vector2> topUvs;

    public Segment parent;

    public void GenerateSegment()
    {
        bottomVertices = new List<Vector3>();
        topVertices = new List<Vector3>();
        indices = new List<int>();
        bottomUvs = new List<Vector2>();
        topUvs = new List<Vector2>();
        childBranches = new List<Branch>();
        childLeafStems = new List<LeafStem>();
        childLeaves = new List<Leaf>();
        top = bottom + topRotation * new Vector3(0, length, 0);
        lastSegment = false;
    }

    public void GenerateSegmentMeshWithSplits()
    {
        for (int i = 0; i <= numberOfSectors; i++)
        {
            Vector3 bottomVerticeBase = new Vector3((float) (bottomRadius * Math.Sin(i * 2 * Math.PI / numberOfSectors)), 0, (float) (bottomRadius * Math.Cos(i * 2 * Math.PI / numberOfSectors)));
            Vector3 bottomVertice = bottom + bottomRotation * bottomVerticeBase;

            bottomVertices.Add(bottomVertice);
            bottomUvs.Add(new Vector2((float)i / numberOfSectors,0));
        }
        for (int i = 0; i <= numberOfSectors; i++)
        {
            Vector3 topVerticeBase = new Vector3((float) (topRadius * Math.Sin(i * 2 * Math.PI / numberOfSectors)), length, (float) (topRadius * Math.Cos(i * 2 * Math.PI / numberOfSectors)));
            Vector3 topVertice = bottom + topRotation * topVerticeBase;

            topVertices.Add(topVertice);
            topUvs.Add(new Vector2((float)i / numberOfSectors, 1));
        }

        for (int i = 0; i < numberOfSectors; i++)
        {
            indices.Add(i + bottomOffset);
            indices.Add(numberOfSectors + i + 2 + topOffset);
            indices.Add(numberOfSectors + i + 1 + topOffset);
            indices.Add(i + bottomOffset);
            indices.Add(i + 1 + bottomOffset);
            indices.Add(numberOfSectors + i + 2 + topOffset);
        }

        if (lastSegment)
        {
            topVertices.Add(top);
            topUvs.Add(new Vector2(0,0));
            for (int i = 0; i < numberOfSectors; i++)
            {
                indices.Add(2 * numberOfSectors + 2 + topOffset);
                indices.Add(numberOfSectors + i + 1 + topOffset);
                indices.Add(numberOfSectors + i + 2 + topOffset);
            }
        }
    }
}
