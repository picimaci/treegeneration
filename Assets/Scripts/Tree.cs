using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts;

public class Tree
{
    public Shape shape;
    public Trunk trunk;
    public float splitAngle;
    public int recursionDepth;
    public float widthLengthRatio;
    public float childParentRatio;

    public System.Random random;

    public List<Vector3> vertices;
    public List<int> indices;

    public void GenerateForTesting()
    {
        random = new System.Random();
        childParentRatio = 0.8f;
        GenerateTree();
    }

    public void GenerateTree()
    {
        vertices = new List<Vector3>();
        indices = new List<int>();
        trunk = new Trunk();
        trunk.tree = this;
        trunk.GenerateForTesting();
        GenerateTreeMesh();
    }

    public void GenerateTreeMesh()
    {
        GenerateMeshRecursively(trunk);
    }

    public void GenerateMeshRecursively(Stem stem)
    {
        int offset = vertices.Count;
        foreach (var vertice in stem.vertices)
        {
            vertices.Add(vertice);
        }
        foreach (var indice in stem.indices)
        {
            indices.Add(offset + indice);
        }
        foreach (var segment in stem.segments)
        {
            foreach (var branch in segment.branchList)
            {
                GenerateMeshRecursively(branch);
            }
        }
    }

    public class TrunkProperties
    {

    }
}
