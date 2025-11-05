using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ColorCombinationGenerator
{
    [MenuItem("Tools/Generate Color Combinations")]
    public static void GenerateCombinations()
    {
        List<(string name, Color color)> baseColors = new List<(string, Color)>
        {
            ("Red", Color.red),
            ("Blue", Color.blue),
            ("Green", Color.green)
        };

        int maxCount = 5;
        string folderPath = "Assets/GeneratedColorCombinations";

        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets", "GeneratedColorCombinations");
        var allCombos = GetAllCombinations(baseColors.Select(c => c.name).ToList(), maxCount);
        var uniqueCombos = allCombos
            .Select(c => c.OrderBy(x => x).ToList())
            .Distinct(new ListComparer<string>())
            .ToList();

        foreach (var combo in uniqueCombos)
        {
            Color resultColor = AverageColor(combo, baseColors);
            string resultName = GetGroupedName(combo);

            ColorCombination asset = ScriptableObject.CreateInstance<ColorCombination>();
            asset.inputColors = combo;
            asset.resultColor = new NamedColor
            {
                name = resultName,
                color = resultColor
            };

            AssetDatabase.CreateAsset(asset, $"{folderPath}/{resultName}.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generation ended. Generated {uniqueCombos.Count} unique combinations.");
    }

    static List<List<string>> GetAllCombinations(List<string> colors, int maxCount)
    {
        var result = new List<List<string>>();
        void Recurse(List<string> current, int depth)
        {
            if (depth > maxCount) return;
            if (current.Count > 0)
                result.Add(new List<string>(current));

            foreach (var color in colors)
            {
                var newCombo = new List<string>(current) { color };
                Recurse(newCombo, depth + 1);
            }
        }
        Recurse(new List<string>(), 0);
        return result;
    }

    static Color AverageColor(List<string> combo, List<(string name, Color color)> baseColors)
    {
        var cols = combo.Select(name => baseColors.First(c => c.name == name).color).ToList();
        float r = cols.Average(c => c.r);
        float g = cols.Average(c => c.g);
        float b = cols.Average(c => c.b);
        return new Color(r, g, b);
    }

    static string GetGroupedName(List<string> combo)
    {
        var grouped = combo.GroupBy(c => c)
            .Select(g => $"{g.Count()}x{g.Key}")
            .ToList();
        return string.Join(" + ", grouped);
    }

    class ListComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T> x, List<T> y)
        {
            if (x == null || y == null || x.Count != y.Count)
                return false;
            for (int i = 0; i < x.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(x[i], y[i]))
                    return false;
            }
            return true;
        }

        public int GetHashCode(List<T> obj)
        {
            unchecked
            {
                int hash = 19;
                foreach (var item in obj)
                    hash = hash * 31 + (item == null ? 0 : item.GetHashCode());
                return hash;
            }
        }
    }
}