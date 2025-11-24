using System;
using UnityEngine;

[Serializable]
public class AgentData
{
    public string id;
    public string type;
    public Vector3Serializable position;
    public string state;
}

[Serializable]
public struct Vector3Serializable
{
    public float x;
    public float y;
    public float z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
    public static Vector3Serializable FromVector3(Vector3 v) => new Vector3Serializable(v.x, v.y, v.z);
}

