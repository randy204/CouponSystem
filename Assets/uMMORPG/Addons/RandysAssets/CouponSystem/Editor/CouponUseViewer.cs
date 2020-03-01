using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CouponUseViewer : EditorWindow {
    private enum State {
        BLANK,
        VIEW
    }

    private State state;
    private Vector2 _scrollPos;

    private List<CouponSystem_CouponUse> table = new List<CouponSystem_CouponUse>();

    public string code;
    public string account;
    public long dateUsedEpoch;
    public string dateUsed;

    [MenuItem("Randy's Assets/Coupon System/Coupon Use Viewer")]
    public static void Init() {
        CouponUseViewer window = GetWindow<CouponUseViewer>();
        window.minSize = new Vector2(800, 400);
        window.Show();
    }

    void OnEnable() {
        state = State.BLANK;

        RefreshDatabase();
    }

    private void RefreshDatabase() {
        if(Database.singleton != null)
            table = Database.singleton.CouponSystem_GetCouponUses();
    }

    void OnGUI() {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DisplayListArea();
        DisplayMainArea();
        EditorGUILayout.EndHorizontal();
    }

    void DisplayListArea() {
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        EditorGUILayout.Space();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, "box", GUILayout.ExpandHeight(true));

        foreach(CouponSystem_CouponUse row in table) {
            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("-", GUILayout.Width(25))) {
                Database.singleton.CouponSystem_RemoveCouponUse(row.code, row.account);
                RefreshDatabase();
                state = State.BLANK;
                return;
            }

            if(GUILayout.Button(row.code.ToString() + " : " + row.account, "box", GUILayout.ExpandWidth(true))) {
                code = row.code;
                account = row.account;
                dateUsedEpoch = row.dateused;
                dateUsed = UnixTimeStampToDateTime(Convert.ToDouble(dateUsedEpoch)).ToString();

                GUI.FocusControl("Wut");
                state = State.VIEW;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("CouponUses: " + table.Count, GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void DisplayMainArea() {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        switch(state) {
            case State.VIEW:
                DisplayViewMainArea();
                break;
            default:
                DisplayBlankMainArea();
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void DisplayBlankMainArea() {
        EditorGUILayout.LabelField(
            "Select a code:account from the left to view info.", GUILayout.ExpandHeight(true));
    }

    void DisplayViewMainArea() {
        EditorGUILayout.TextField(new GUIContent("Code: "), code);
        EditorGUILayout.TextField(new GUIContent("Account: "), account);
        EditorGUILayout.TextField(new GUIContent("DateUsedEpoch: "), dateUsedEpoch.ToString());
        EditorGUILayout.TextField(new GUIContent("DateUsed "), dateUsed);

        EditorGUILayout.Space();

        if(GUILayout.Button("Done", GUILayout.Width(100))) {
            code = "";
            account = "";
            dateUsedEpoch = 0;

            RefreshDatabase();
            GUI.FocusControl("Wut");

            state = State.BLANK;
        }
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
        // Unix timestamp is seconds past epoch
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}