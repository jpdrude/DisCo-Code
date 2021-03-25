using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeometryConstraint : MonoBehaviour
{
    private ConstraintType type;
    public ConstraintType Type { get { return type; } set { type = value; } }

    public GeometryConstraint()
    {
        type = ConstraintType.None;
    }

    public enum ConstraintType
    {
        None,
        Point,
        Curve,
        Box
    }
}

