using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

// Source: https://gist.github.com/Spaxe/57de76c993a4dc8bb37275acbe597ef2
public class TextureArray : MonoBehaviour
{
    private static string AbsolutePathToAssetPath(string absolutePath)
    {
        string assetPath = absolutePath.Replace(Application.dataPath, ""); // Hack to turn the absolute path into a relative path.
        if (assetPath[0] == '/' || assetPath[0] == '\\')
        {
            // Remove a leading slash if there is one.
            assetPath = assetPath.Substring(1);
        }

        // Make "Assets/" the first folder in the path.
        assetPath = "Assets/" + assetPath;
        return assetPath;
    }

    //[MenuItem("GameObject/Create 2D Texture Array from Images")]
    //static void Create2DFromImages()
    //{
    //    string firstImage = EditorUtility.OpenFilePanel("Select the first texture in the array", "Assets", "png");

    //    if (firstImage == null) // Operation cancelled
    //    {
    //        return;
    //    }

    //    firstImage = AbsolutePathToAssetPath(firstImage);

    //    int firstTextureIndex = 0;

    //    Regex regex = new Regex("[0-9]+");

    //    // Turn the file name into a formatting string.
    //    string filePattern = regex.Replace(firstImage, (Match m) =>
    //    {
    //        if (m.Success && !m.NextMatch().Success) // Final match
    //        {
    //            // Remember the starting index (probably either 0 or 1).
    //            firstTextureIndex = int.Parse(m.Value);

    //            // Replace number in the original file name with a format item.
    //            // We actually use string.Format to create the formatting string.
    //            return "{0," + m.Value.Length + ":D" + m.Value.Length + "}";
    //        }
    //        else // Not the final match
    //        {
    //            // No change
    //            return m.Value;
    //        }
    //    });

    //    int maxSlices = 32;

    //    // Load the first image
    //    Debug.Log("Loading " + firstImage);
    //    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(firstImage);

    //    if (tex != null)
    //    {
    //        // Create the texture array
    //        Texture2DArray textureArray = new Texture2DArray(tex.width, tex.height, maxSlices, TextureFormat.RGB24, false);
    //        textureArray.SetPixels(tex.GetPixels(0), 0, 0);

    //        // Load the other images
    //        for (int i = 1; i < maxSlices && tex != null; i++)
    //        {
    //            string filename = string.Format(filePattern, firstTextureIndex + i);
    //            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(filename);
    //            Debug.Log("Loading " + filename);
    //            if (tex != null)
    //            {
    //                textureArray.SetPixels(tex.GetPixels(0), i, 0);
    //            }
    //            else
    //            {
    //                Debug.Log("File not found; finishing.");
    //            }
    //        }
    //        textureArray.Apply();

    //        // Turn the file name for the first image into a file name for the texture array
    //        string assetPath = regex.Replace(firstImage, (Match m) =>
    //        {
    //            if (m.Success && !m.NextMatch().Success) // Final match
    //            {
    //                // Remember the starting index (probably either 0 or 1).
    //                firstTextureIndex = int.Parse(m.Value);

    //                // Replace number in the original file name with "arr"
    //                return "arr";
    //            }
    //            else // Not the final match
    //            {
    //                // No change
    //                return m.Value;
    //            }
    //        });

    //        // Change the file extension to .asset.
    //        assetPath = assetPath.Replace(".png", ".asset");

    //        // Save the texture array
    //        AssetDatabase.CreateAsset(textureArray, assetPath);
    //        Debug.Log("Saved asset to " + assetPath);
    //    }
    //    else
    //    {
    //        Debug.Log("File not found; finishing.");
    //    }
    //}

    private static void FillColorArray(string[] redSource, string[] greenSource, string[] blueSource, Color[] dest)
    {
        for (int i = 0; i < dest.Length; i++)
        {
            dest[i] = new Color(
                float.Parse(redSource[i + 1]),
                float.Parse(greenSource[i + 1]),
                float.Parse(blueSource[i + 1]));
        }
    }

    [MenuItem("GameObject/Create 1D Texture Array from CSV")]
    static void Create1DFromCSV()
    {
        string csvFile = EditorUtility.OpenFilePanel("Select the file containing texture array data", "Assets", "csv");

        if (csvFile == null) // Operation cancelled
        {
            return;
        }

        csvFile = AbsolutePathToAssetPath(csvFile);

        // Load the CSV file
        Debug.Log("Loading " + csvFile);
        TextAsset csvData = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFile);

        if (csvData != null)
        {
            string[] lines = csvData.text.Trim().Split('\n');
            int maxSlices = lines.Length / 3;

            string[] redSource = lines[0].Split(',');
            string[] greenSource = lines[1].Split(',');
            string[] blueSource = lines[2].Split(',');

            // Create the texture array
            Texture2DArray textureArray = new Texture2DArray(redSource.Length - 1, 1, maxSlices, TextureFormat.RGB9e5Float, false);

            // Fill first layer of the texture array.
            Color[] colors = new Color[redSource.Length - 1];
            FillColorArray(redSource, greenSource, blueSource, colors);
            textureArray.SetPixels(colors, 0, 0);

            // Load the other layers
            for (int i = 1; i < maxSlices; i++)
            {
                redSource = lines[i * 3].Split(',');
                greenSource = lines[i * 3 + 1].Split(',');
                blueSource = lines[i * 3 + 2].Split(',');

                // Fill subsequent layers of the texture array.
                FillColorArray(redSource, greenSource, blueSource, colors);
                textureArray.SetPixels(colors, i, 0);
            }
            textureArray.Apply();

            // Turn the file name for the CSV file into a file name for the texture array
            string assetPath = csvFile.Replace(".csv", ".asset");

            // Save the texture array
            AssetDatabase.CreateAsset(textureArray, assetPath);
            Debug.Log("Saved asset to " + assetPath);
        }
        else
        {
            Debug.Log("File not found; finishing.");
        }
    }
}
