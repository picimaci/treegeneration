using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Stem
{
    public int segCount;
    public int numberOfSectors;
    public Vector3 basePoint;
    public Quaternion baseRotation;
    public List<Segment> segments;
    public float radius;
    public float length;
    public float taper;
    public float split;

    public Vector3 splitAngle;
    public Vector3 splitAngleVariation;
    public Vector3 curveAngle;
    public Vector3 curveAngleVariation;

    public Vector3 branchingAngle;
    public Vector3 branchingAngleVariation;

    public float branchingPoint;
    public float branchingFactor;

    public Tree tree;

    public List<Vector3> vertices;
    public List<int> indices;
}
