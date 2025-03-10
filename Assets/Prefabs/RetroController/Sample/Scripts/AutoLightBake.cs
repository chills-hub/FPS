﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class AutoLightBake : MonoBehaviour
{
    void Start()
    {
        if (Lightmapping.lightingDataAsset == null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            string message = "To display the " + sceneName + " scene correctly, you need to " +
                "bake the lightmap. Do you wanna do this automatically?";
            if (EditorUtility.DisplayDialog("Lightmap Bake", message, "Yes", "No"))
            {
#if UNITY_2020_1_OR_NEWER
                Lightmapping.lightingSettings.lightmapMaxSize = 128;
                Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
                Lightmapping.lightingSettings.prioritizeView = false;
                Lightmapping.lightingSettings.compressLightmaps = true;
                Lightmapping.bakeCompleted += () =>
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                };
#elif UNITY_2019_2_OR_NEWER
                LightmapEditorSettings.textureCompression = false;
                LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.ProgressiveCPU;
                Lightmapping.bakeCompleted += () =>
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                };

#else
                LightmapEditorSettings.textureCompression = false;
                Lightmapping.completed = () =>
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                };

#endif
                Lightmapping.BakeAsync();
            }
        }
    }

    bool Is2019_2Plus(string version)
    {
        return version.StartsWith("2019.2") || version.StartsWith("2019.3");
    }

}
#endif