using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class RCS_EditionSelector : EditorWindow {
    private int selectedOption = 0;
    private readonly List<string> options = new List<string>() { "uMMORPG 2D and 3D", "uMMORPG CE", "Not Selected" };
    private readonly List<string> defines = new List<string>() { "RCS2D3D", "RCSCE" };
    private static RCS_EditionSelector window;

    static RCS_EditionSelector() {
        EditorApplication.update += Startup;
    }

    private static void Startup() {
        EditorApplication.update -= Startup;
        
        if(!PlayerPrefs.HasKey("RCS_EditionSet")) {
            Debug.LogError("Randy's Coupon System - uMMORPG Edition is not set, please setup. On the menu bar click on \"Randy's Assets\\Coupon System\\Edition Selector\" and select correct edition.");
        }
    }

    void OnEnable() {
        selectedOption = GetCurrentDefineIndex(GetDefinesAsList());
    }
    
    [MenuItem("Randy's Assets/Coupon System/Edition Selector")]
    public static void Init() {
        ShowWindow();
    }

    private static void ShowWindow() {
        if(window ==null) window = GetWindow<RCS_EditionSelector>();
        window.minSize = new Vector2(300, 50);
        window.maxSize = new Vector2(300, 50);
        window.Show();
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        EditorGUILayout.Space();
        selectedOption = EditorGUILayout.Popup("uMMORPG Edition:", selectedOption, options.ToArray());

        if(GUILayout.Button("Save")) {
            SaveDefines();
        }

        GUILayout.EndVertical();
    }

    private string GetDefinesAsString() {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
    }

    private List<string> GetDefinesAsList() {
        return GetDefinesAsString().Split(';').ToList();
    }

    private bool ListContains(List<string> defines, string define) {
        foreach(string str in defines) {
            if(str == define)
                return true;
        }

        return false;
    }

    private int GetCurrentDefineIndex(List<string> defines) {
        for(int cnt = 0; cnt < this.defines.Count; cnt++) {
            for(int cnt2 = 0; cnt2 < defines.Count; cnt2++) {
                if(defines[cnt2] == this.defines[cnt]) {
                    return cnt;
                }
            }
        }

        return this.defines.Count;
    }
    
    private string CleanAddonDefines(List<string> defines) {
        foreach(string define in this.defines) {
            defines.Remove(define);
        }

        return string.Join(";", defines);
    }

    private void SaveDefines() {
        List<string> defines = GetDefinesAsList();

        if(selectedOption < (options.Count - 1)) {
            if(ListContains(defines, this.defines[selectedOption]))
                return;
        }
        
        string cleanedDefines = CleanAddonDefines(defines);

        if(selectedOption == (options.Count - 1)) {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, cleanedDefines);
            PlayerPrefs.DeleteKey("RCS_EditionSet");
        } else {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, (cleanedDefines + ";" + this.defines[selectedOption]));
            PlayerPrefs.SetInt("RCS_EditionSet", 1);
        }
    }
}