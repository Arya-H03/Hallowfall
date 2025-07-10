using System.Collections.Generic;
using UnityEngine;

public enum DirectionEnum
{
    None,
    Right,
    Left,
    Up,
    Down
}

public static class MyUtils
{
    public static Vector2Int[] GetCardinalDirections()
    {
        return new Vector2Int[]
        {   Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down,
        };
    }

    public static Vector2Int[] GetAllDirections()
    {
        return new Vector2Int[]
        {   Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down,
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        };
    }

    public static List<DirectionEnum> GetAllDirectionList()
    {
        return new List<DirectionEnum> { DirectionEnum.Right, DirectionEnum.Left, DirectionEnum.Up, DirectionEnum.Down };
    }

    public static List<DirectionEnum> GetHorizontalDirectionList()
    {
        return new List<DirectionEnum> { DirectionEnum.Right, DirectionEnum.Left };
    }

    public static List<DirectionEnum> GetVerticalDirectionList()
    {
        return new List<DirectionEnum> { DirectionEnum.Up, DirectionEnum.Down };
    }

    public static DirectionEnum GetRandomVerticalDirectionEnum()
    {
        return GetVerticalDirectionList()[Random.Range(0, 2)];
    }

    public static DirectionEnum GetRandomHorizontalDirectionEnum()
    {
        return GetHorizontalDirectionList()[Random.Range(0, 2)];
    }

    public static DirectionEnum GetRandomDirectionEnum(List<DirectionEnum> directions)
    {
        return directions[Random.Range(0, directions.Count)];
    }

    public static Bounds GetCombinedBounds(GameObject root)
    {
        SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
            return new Bounds(root.transform.position, Vector3.zero);

        Bounds combinedBounds = root.GetComponent<SpriteRenderer>().bounds;

        for (int i = 0; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds;
    }

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
                //Slice horizontally first then if not slice vertically 
                if (Random.Range(1, 3) == 1)
                {
                    if (zoneToSplit.size.y >= minHeight * 2)
                    {
                        SpiltZoneHorizontally(zoneQueue, zoneToSplit);
                    }
                    else if (zoneToSplit.size.x >= minWidth * 2)
                    {
                        SplitZoneVertically(zoneQueue, zoneToSplit);
                    }
                    else
                    {
                        newSpaces.Add(zoneToSplit);
                    }
                }
                //Slice vertically first then if not slice horizontally 
                else
                {
                    if (zoneToSplit.size.x >= minWidth * 2)
                    {
                        SplitZoneVertically(zoneQueue, zoneToSplit);
                    }
                    else if (zoneToSplit.size.y >= minHeight * 2)
                    {
                        SpiltZoneHorizontally(zoneQueue, zoneToSplit);
                    }
                    else
                    {
                        newSpaces.Add(zoneToSplit);
                    }
                }
            }

        }
        return newSpaces;
    }

    private static void SpiltZoneHorizontally(Queue<BoundsInt> zoneQueue, BoundsInt zoneToSplit)
    {
        int ySplit = Random.Range(1, zoneToSplit.size.y);
        BoundsInt zone1 = new BoundsInt(zoneToSplit.min, new Vector3Int(zoneToSplit.size.x, ySplit, zoneToSplit.size.z));
        BoundsInt zone2 = new BoundsInt(new Vector3Int(zoneToSplit.min.x, zoneToSplit.min.y + ySplit, zoneToSplit.min.z), new Vector3Int(zoneToSplit.size.x, zoneToSplit.size.y - ySplit, zoneToSplit.size.z));

        zoneQueue.Enqueue(zone1);
        zoneQueue.Enqueue(zone2);
    }

    private static void SplitZoneVertically(Queue<BoundsInt> zoneQueue, BoundsInt zoneToSplit)
    {
        int xSplit = Random.Range(1, zoneToSplit.size.x);
        BoundsInt zone1 = new BoundsInt(zoneToSplit.min, new Vector3Int(xSplit, zoneToSplit.size.y, zoneToSplit.size.z));
        BoundsInt zone2 = new BoundsInt(new Vector3Int(zoneToSplit.min.x + xSplit, zoneToSplit.min.y, zoneToSplit.min.z), new Vector3Int(zoneToSplit.size.x - xSplit, zoneToSplit.size.y, zoneToSplit.size.z));

        zoneQueue.Enqueue(zone1);
        zoneQueue.Enqueue(zone2);
    }

    public static void ValidateFields<T>(Object owner, T field, string fieldName) where T : Object
    {
        if (field == null)
        {
            Debug.LogWarning($"Required reference '{fieldName}' is not set on '{owner.name}' ({owner.GetType().Name})", owner);
        }
    }

    public static void ValidateFields<T>(Object owner, T [] field, string fieldName) where T : Object
    {
        if (field == null || field.Length ==0) 
        {
            Debug.LogWarning($"Required reference '{fieldName}' is not set on '{owner.name}' ({owner.GetType().Name})", owner);
        }
    }

    // For reference types (e.g., GameObject, Sprite, etc.)
    public static T GetRandomRef<T>(T[] input) where T : class
    {
        if (input == null || input.Length == 0)
        {
            Debug.LogError("Input array is null or empty");
            return null;
        }
        return input[Random.Range(0, input.Length)];
    }

    public static T GetRandomRef<T>(T[] input, float chanceOfReturningNothing) where T : class
    {
        if (input == null || input.Length == 0)
        {
            Debug.LogError("Input array is null or empty");
            return null;
        }

        if (Random.value < 1 - chanceOfReturningNothing)
        {
            return input[Random.Range(0, input.Length)];
        }

        return null;
    }

    // For value types (e.g., Vector3, int, etc.)

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

        if (Random.value < 1 - chanceOfReturningNothing)
        {
            return input[Random.Range(0, input.Length)];
        }

        return null;
    }

   
}
