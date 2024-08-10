using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTag : MonoBehaviour
{
    [SerializeField] private int elevationLevel;

    public int ElevationLevel { get => elevationLevel; set => elevationLevel = value; }
}
