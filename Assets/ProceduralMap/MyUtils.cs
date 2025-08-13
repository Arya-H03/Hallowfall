using System.Collections.Generic;
using UnityEngine;

public enum DirectionEnum
{
    None,
    Right,
    Left,
    Up,
    Down,
    UpRight,
    UpLeft,
    DownRight,
    DownLeft
}

public static class MyUtils
{
    // -------------------------
    //  Direction Utilities
    // -------------------------

    public static Vector2Int[] GetCardinalDirectionsVectorArray() => new Vector2Int[]
    {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down,
    };

    public static Vector2Int[] GetAllDirectionsVectorArray() => new Vector2Int[]
    {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down,
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
    };

    public static List<Vector2Int> GetCardinalDirectionsVectorList() => new List<Vector2Int>
    {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };


    public static List<Vector2Int> GetAllDirectionsVectorList() => new List<Vector2Int>
    {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down,
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
    };

    public static List<DirectionEnum> GetCardinalDirectionEnumList() => new List<DirectionEnum>
    {
        DirectionEnum.Right,
        DirectionEnum.Left,
        DirectionEnum.Up,
        DirectionEnum.Down
    };

    public static List<DirectionEnum> GetAllDirectionEnumList() => new List<DirectionEnum>
    {
        DirectionEnum.Right,
        DirectionEnum.Left,
        DirectionEnum.Up,
        DirectionEnum.Down,
        DirectionEnum.UpRight,
        DirectionEnum.UpLeft,
        DirectionEnum.DownRight,
        DirectionEnum.DownLeft,
    };

    public static List<DirectionEnum> GetHorizontalDirectionEnumList() => new List<DirectionEnum>
    {
        DirectionEnum.Right,
        DirectionEnum.Left
    };

    public static List<DirectionEnum> GetVerticalDirectionEnumList() => new List<DirectionEnum>
    {
        DirectionEnum.Up,
        DirectionEnum.Down
    };

    public static Dictionary<DirectionEnum, Vector2Int> GetDirectionDicWithEnumKey()
    {
        return new Dictionary<DirectionEnum, Vector2Int>
    {
        { DirectionEnum.Right,        Vector2Int.right },
        { DirectionEnum.Left,         Vector2Int.left},
        { DirectionEnum.Up,          Vector2Int.up },
        { DirectionEnum.Down,       Vector2Int.down },
        { DirectionEnum.UpRight,     new Vector2Int(1, 1)  },
        { DirectionEnum.UpLeft,      new Vector2Int(-1, 1) },
        { DirectionEnum.DownRight,  new Vector2Int(1, -1) },
        { DirectionEnum.DownLeft,   new Vector2Int(-1, -1) }
    };
    }

    public static Vector2Int GetVectorFromDir(DirectionEnum dirEnum)
    {
        switch (dirEnum)
        {
            case DirectionEnum.Right:
                return Vector2Int.right;
            case DirectionEnum.Left:
                return Vector2Int.left;
            case DirectionEnum.Up:
                return Vector2Int.up;
            case DirectionEnum.Down:
                return Vector2Int.down;
            case DirectionEnum.UpRight:
                return new Vector2Int(1, 1);
            case DirectionEnum.UpLeft:
                return new Vector2Int(-1, 1);
            case DirectionEnum.DownRight:
                return new Vector2Int(1, -1);
            case DirectionEnum.DownLeft:
                return new Vector2Int(-1, -1);

            default: return Vector2Int.zero;
        }
    }

    public static DirectionEnum GetDirFromVector(Vector2Int vector)
    {
        if (vector == Vector2Int.right) return DirectionEnum.Right;
        if (vector == Vector2Int.left) return DirectionEnum.Left;
        if (vector == Vector2Int.up) return DirectionEnum.Up;
        if (vector == Vector2Int.down) return DirectionEnum.Down;
        if (vector == new Vector2Int(1, 1)) return DirectionEnum.UpRight;
        if (vector == new Vector2Int(-1, 1)) return DirectionEnum.UpLeft;
        if (vector == new Vector2Int(1, -1)) return DirectionEnum.DownRight;
        if (vector == new Vector2Int(-1, -1)) return DirectionEnum.DownLeft;

        return DirectionEnum.None;
    }

    public static Dictionary<Vector2Int, DirectionEnum> GetDirectionDicWithVectorKey()
    {
        return new Dictionary<Vector2Int, DirectionEnum>
    {
        { Vector2Int.right,        DirectionEnum.Right },
        { Vector2Int.left,         DirectionEnum.Left },
        { Vector2Int.up,           DirectionEnum.Up },
        { Vector2Int.down,         DirectionEnum.Down },
        { new Vector2Int(1, 1),    DirectionEnum.UpRight },
        { new Vector2Int(-1, 1),   DirectionEnum.UpLeft },
        { new Vector2Int(1, -1),   DirectionEnum.DownRight },
        { new Vector2Int(-1, -1),  DirectionEnum.DownLeft }
    };
    }

   
    public static DirectionEnum FindDirectionEnumBetweenTwoPoints(Vector2Int from,Vector2Int to) =>  GetDirFromVector(GetSign(to - from)); 

    public static DirectionEnum GetRandomVerticalDirectionEnum() =>
        GetVerticalDirectionEnumList()[Random.Range(0, 2)];

    public static DirectionEnum GetRandomHorizontalDirectionEnum() =>
        GetHorizontalDirectionEnumList()[Random.Range(0, 2)];

    public static DirectionEnum GetRandomDirectionEnum(List<DirectionEnum> directions) =>
        directions[Random.Range(0, directions.Count)];

    // -------------------------
    //  Field Validation
    // -------------------------

    public static void ValidateFields<T>(Object owner, T field, string fieldName) where T : Object
    {
        if (field == null)
        {
            Debug.LogWarning($"Required reference '{fieldName}' is not set on '{owner.name}' ({owner.GetType().Name})", owner);
        }
    }

    public static void ValidateFields<T>(Object owner, T[] field, string fieldName) where T : Object
    {
        if (field == null || field.Length == 0)
        {
            Debug.LogWarning($"Required reference '{fieldName}' is not set on '{owner.name}' ({owner.GetType().Name})", owner);
        }
    }

    // -------------------------
    //  Random Utilities
    // -------------------------

    // For reference types (GameObject, Sprite, etc.)
    public static T GetRandomRef<T>(T[] input) where T : class
    {
        if (input == null || input.Length == 0)
        {
            Debug.LogError("Input array is null or empty");
            return null;
        }

        return input[Random.Range(0, input.Length)];
    }

    public static T GetRandomRef<T>(List<T> input) where T : class
    {
        if (input == null || input.Count == 0)
        {
            Debug.LogError("Input List is null or empty");
            return null;
        }

        return input[Random.Range(0, input.Count)];
    }

    public static T GetRandomRef<T>(T[] input, float chanceOfReturningNothing) where T : class
    {
        if (input == null || input.Length == 0)
        {
            Debug.LogError("Input array is null or empty");
            return null;
        }

        return Random.value < 1 - chanceOfReturningNothing
            ? input[Random.Range(0, input.Length)]
            : null;
    }

    // For value types (Vector3, int, etc.)
    public static T? GetRandomValue<T>(T[] input) where T : struct
    {
        if (input == null || input.Length == 0)
        {
            Debug.LogError("Input array is null or empty");
            return null;
        }

        return input[Random.Range(0, input.Length)];
    }

    public static T? GetRandomValue<T>(T[] input, float chanceOfReturningNothing) where T : struct
    {
        if (input == null || input.Length == 0)
        {
            Debug.LogError("Input array is null or empty");
            return null;
        }

        return Random.value < 1 - chanceOfReturningNothing
            ? input[Random.Range(0, input.Length)]
            : (T?)null;
    }

    // -------------------------
    //  Bounds Utilities
    // -------------------------

    public static Bounds GetCombinedBounds(GameObject root)
    {
        SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
            return new Bounds(root.transform.position, Vector3.zero);

        Bounds combinedBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds;
    }

    // -------------------------
    //  Binary Space Partitioning
    // -------------------------

    public static List<BoundsInt> PerformeBinarySpacePartitioning(List<BoundsInt> listOfZonesToPartition, int minWidth, int minHeight)
    {
        List<BoundsInt> newSpaces = new List<BoundsInt>();

        foreach (BoundsInt zoneBound in listOfZonesToPartition)
        {
            Queue<BoundsInt> zoneQueue = new Queue<BoundsInt>();
            zoneQueue.Enqueue(zoneBound);

            while (zoneQueue.Count > 0)
            {
                BoundsInt zoneToSplit = zoneQueue.Dequeue();

                if (Random.Range(1, 3) == 1)
                {
                    if (zoneToSplit.size.y >= minHeight * 2)
                        SpiltZoneHorizontally(zoneQueue, zoneToSplit);
                    else if (zoneToSplit.size.x >= minWidth * 2)
                        SplitZoneVertically(zoneQueue, zoneToSplit);
                    else
                        newSpaces.Add(zoneToSplit);
                }
                else
                {
                    if (zoneToSplit.size.x >= minWidth * 2)
                        SplitZoneVertically(zoneQueue, zoneToSplit);
                    else if (zoneToSplit.size.y >= minHeight * 2)
                        SpiltZoneHorizontally(zoneQueue, zoneToSplit);
                    else
                        newSpaces.Add(zoneToSplit);
                }
            }
        }

        return newSpaces;
    }

    private static void SpiltZoneHorizontally(Queue<BoundsInt> zoneQueue, BoundsInt zoneToSplit)
    {
        List<int> validSplits = new List<int>();
        for (int i = 1; i < zoneToSplit.size.y; i++)
            if (i % 2 == 0) validSplits.Add(i);

        if (validSplits.Count == 0) return;

        int ySplit = validSplits[Random.Range(0, validSplits.Count)];

        BoundsInt zone1 = new BoundsInt(
            zoneToSplit.min,
            new Vector3Int(zoneToSplit.size.x, ySplit, zoneToSplit.size.z)
        );

        BoundsInt zone2 = new BoundsInt(
            new Vector3Int(zoneToSplit.min.x, zoneToSplit.min.y + ySplit, zoneToSplit.min.z),
            new Vector3Int(zoneToSplit.size.x, zoneToSplit.size.y - ySplit, zoneToSplit.size.z)
        );

        zoneQueue.Enqueue(zone1);
        zoneQueue.Enqueue(zone2);
    }

    private static void SplitZoneVertically(Queue<BoundsInt> zoneQueue, BoundsInt zoneToSplit)
    {
        List<int> validSplits = new List<int>();
        for (int i = 1; i < zoneToSplit.size.x; i++)
            if (i % 2 == 0) validSplits.Add(i);

        if (validSplits.Count == 0) return;

        int xSplit = validSplits[Random.Range(0, validSplits.Count)];

        BoundsInt zone1 = new BoundsInt(
            zoneToSplit.min,
            new Vector3Int(xSplit, zoneToSplit.size.y, zoneToSplit.size.z)
        );

        BoundsInt zone2 = new BoundsInt(
            new Vector3Int(zoneToSplit.min.x + xSplit, zoneToSplit.min.y, zoneToSplit.min.z),
            new Vector3Int(zoneToSplit.size.x - xSplit, zoneToSplit.size.y, zoneToSplit.size.z)
        );

        zoneQueue.Enqueue(zone1);
        zoneQueue.Enqueue(zone2);
    }

    // -------------------------
    //  Array Bound Checks
    // -------------------------

    public static bool IsWithinArrayBounds<T>(T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    public static bool IsWithinArrayBounds(int arrayLength, int index)
    {
        return index >= 0 && index < arrayLength;
    }

    public static bool IsWithinArrayBounds<T>(T[,] array, Vector2Int indexCoord)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);

        return indexCoord.x >= 0 && indexCoord.x < width &&
               indexCoord.y >= 0 && indexCoord.y < height;
    }

    public static bool IsWithinArrayBounds(int arrayWidth, int arrayHeight, Vector2Int indexCoord)
    {

        return indexCoord.x >= 0 && indexCoord.x < arrayWidth &&
               indexCoord.y >= 0 && indexCoord.y < arrayHeight;
    }


    public static Vector2Int GetSign(Vector2Int v)
    {
        Vector2Int result = new();

        if(v.x == 0) result.x = 0;
        else result.x = v.x / Mathf.Abs(v.x);
      
        if (v.y == 0) result.y = 0;
        else result.y = v.y / Mathf.Abs(v.y);
       
        return result;


    }

    public static Vector3 GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }

 
}
