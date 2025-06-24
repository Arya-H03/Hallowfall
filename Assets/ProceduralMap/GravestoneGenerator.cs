using UnityEngine;
using UnityEditor;
public class GravestoneGenerator
{
    [MenuItem("Tools/Generate Random Gravestone")]

    public static void GenerateGraveStone()
    {
        GraveYardLayoutProfile graveYardLayoutProfile = AssetDatabase.LoadAssetAtPath<GraveYardLayoutProfile>("Assets/ProceduralMap/GraveYardLayoutProfile.asset");
        if(graveYardLayoutProfile == null )
        {
            Debug.LogError("GraveYardLayoutProfile not found!");
            return;
        }

        GameObject newGravestone = graveYardLayoutProfile.GenerateRandomGraveStone();

        string fileName = "Gravestone_" + System.Guid.NewGuid().ToString().Substring(0, 6) + ".prefab";
        string assetPath = "Assets/ProceduralMap/GraveyardProps/" + fileName;

        PrefabUtility.SaveAsPrefabAsset(newGravestone, assetPath);
        GameObject.DestroyImmediate(newGravestone);
        AssetDatabase.Refresh();

        Debug.Log("Gravestone prefab created at: " + assetPath);
    }

    
}
