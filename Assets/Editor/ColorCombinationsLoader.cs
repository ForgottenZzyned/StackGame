using UnityEngine;
using UnityEditor;
using System.Linq;

public class ColorCombinationsLoader
{
    [MenuItem("Tools/Load All ColorCombinations")]
    public static void LoadAllCombinations()
    {
        string folderPath = "Assets/GeneratedColorCombinations";
        string[] guids = AssetDatabase.FindAssets("t:ColorCombination", new[] { folderPath });

        ColorCombination[] combinations = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ColorCombination>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        Debug.Log($"Загружено {combinations.Length} комбинаций");
        foreach (var combo in combinations)
        {
            Debug.Log($"Комбинация: {combo.resultColor.name}");
        }
    }
    [MenuItem("Tools/Update Combination Manager")]
    public static void UpdateManager()
    {
        string folderPath = "Assets/GeneratedColorCombinations";
        string managerPath = "Assets/ColorCombinationManager.asset";

        string[] guids = AssetDatabase.FindAssets("t:ColorCombination", new[] { folderPath });
        var combinations = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ColorCombination>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(a => a != null)
            .ToList();
        var manager = AssetDatabase.LoadAssetAtPath<ColorCombinationManager>(managerPath);
        if (manager == null)
        {
            manager = ScriptableObject.CreateInstance<ColorCombinationManager>();
            AssetDatabase.CreateAsset(manager, managerPath);
        }

        manager.allCombinations = combinations;

        EditorUtility.SetDirty(manager);
        AssetDatabase.SaveAssets();
        Debug.Log($"Менеджер обновлён. Загружено {combinations.Count} комбинаций.");
    }
}
