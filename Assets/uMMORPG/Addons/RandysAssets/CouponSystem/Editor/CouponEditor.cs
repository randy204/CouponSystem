using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CouponEditor : EditorWindow {
    private enum State {
        BLANK,
        EDIT,
        ADD
    }

    private State state;
    private Vector2 _scrollPos;

    private List<CouponSystem_Coupon> table = new List<CouponSystem_Coupon>();

    private string oldCode = "";
    private string newCode = "";
    private int couponType_SelectedIndex;
    private string[] couponType_Options =  Enum.GetNames(typeof(CouponSystem_CouponType));
    private string itemName = "";
    private int amount = 0;
    private int singleUse = 0;
    private long expiration = 0;

    private string month = "";
    private string day = "";
    private string year = "";
    private string hour = "";
    private string minute = "";
    private string second = "";
    private int amPM_SelectedIndex = 0;
    private string[] amPM_Options = new string[2] { "AM", "PM" };

    [MenuItem("Randy's Assets/Coupon System/Coupon Editor")]
    public static void Init() {
        CouponEditor window = GetWindow<CouponEditor>();
        window.minSize = new Vector2(800, 400);
        window.Show();
    }

    void OnEnable() {
        state = State.BLANK;

        RefreshDatabase();
    }

    private void RefreshDatabase() {
        //if(Database.singleton == null) Database.singleton = new Database();
        
        if(Database.singleton != null)
            table = Database.singleton.CouponSystem_GetCoupons();
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

        foreach(CouponSystem_Coupon row in table) {
            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("-", GUILayout.Width(25))) {
                Database.singleton.CouponSystem_RemoveCoupon(row.code);
                RefreshDatabase();
                state = State.BLANK;
                return;
            }

            if(GUILayout.Button(row.code, "box", GUILayout.ExpandWidth(true))) {
                ResetVariables();

                oldCode = row.code;
                newCode = oldCode;
                couponType_SelectedIndex = Convert.ToInt32(row.couponType);
                itemName = row.itemName;
                amount = row.amount;
                singleUse = row.singleUse;
                expiration = row.expiration;

                GUI.FocusControl("Wut");
                state = State.EDIT;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Coupons: " + table.Count, GUILayout.Width(100));

        if(GUILayout.Button("New Coupon")) {
            ResetVariables();
            state = State.ADD;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void DisplayMainArea() {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        switch(state) {
            case State.ADD:
                DisplayAddMainArea();
                break;
            case State.EDIT:
                DisplayEditMainArea();
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
            "Please select a coupon to edit or click new coupon.", GUILayout.ExpandHeight(true));
    }

    private void SharedArea() {
        newCode = EditorGUILayout.TextField(new GUIContent("Code: "), newCode);
        couponType_SelectedIndex = EditorGUILayout.Popup("CouponType: ", couponType_SelectedIndex, couponType_Options);
        itemName = EditorGUILayout.TextField(new GUIContent("ItemName: "), itemName);
        amount = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Amount: "), amount.ToString()));
        singleUse = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("SingleUse: "), singleUse.ToString()));
        expiration = Convert.ToInt64(EditorGUILayout.TextField(new GUIContent("Expiration: "), expiration.ToString()));

        EditorGUILayout.Space();

        PopulateEpochVariables();
        UnixEpochTimeGenerator();
    }

    void DisplayEditMainArea() {
        SharedArea();

        EditorGUILayout.Space();

        if(GUILayout.Button("Done", GUILayout.Width(100))) {
            if(newCode != oldCode) {
                foreach(CouponSystem_Coupon row in table) {
                    if((row.code).Equals(newCode)) {
                        return;
                    }
                }
            }

            Database.singleton.CouponSystem_EditCoupon(oldCode, newCode, (CouponSystem_CouponType)couponType_SelectedIndex, itemName, amount, singleUse, expiration);

            ResetVariables();

            RefreshDatabase();
            GUI.FocusControl("Wut");

            state = State.BLANK;
        }

        if(GUILayout.Button("Cancel", GUILayout.Width(100))) {
            ResetVariables();

            GUI.FocusControl("Wut");

            state = State.BLANK;
        }
    }

    private void ResetVariables() {
        oldCode = "";
        newCode = "";
        couponType_SelectedIndex = 0;
        itemName = "";
        amount = 0;
        singleUse = 0;
        expiration = 0;

        month = "";
        day = "";
        year = "";
        hour = "";
        minute = "";
        second = "";
        amPM_SelectedIndex = 0;
}

    private void PopulateEpochVariables() {
        if(!month.Equals("")) {
            return;
        }

        DateTime dt = UnixTimeStampToDateTime(Convert.ToDouble(expiration));
        month = dt.Month.ToString();
        day = dt.Day.ToString();
        year = dt.Year.ToString();
        hour = dt.Hour.ToString();
        minute = dt.Minute.ToString();
        second = dt.Second.ToString();
        
        if(dt.ToString("tt").Equals("AM")) {
            amPM_SelectedIndex = 0;
        } else {
            amPM_SelectedIndex = 1;
        }
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
        // Unix timestamp is seconds past epoch
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

    private void UnixEpochTimeGenerator() {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Expiration Time Generator");
        month = EditorGUILayout.TextField(new GUIContent("Month: "), month);
        day = EditorGUILayout.TextField(new GUIContent("Day: "), day);
        year = EditorGUILayout.TextField(new GUIContent("Year: "), year);
        hour = EditorGUILayout.TextField(new GUIContent("Hour: "), hour);
        minute = EditorGUILayout.TextField(new GUIContent("Minute: "), minute);
        second = EditorGUILayout.TextField(new GUIContent("Second: "), second);
        amPM_SelectedIndex = EditorGUILayout.Popup("AM PM: ", amPM_SelectedIndex, amPM_Options);
        if(GUILayout.Button("Convert", GUILayout.Width(100))) {
            try {
                DateTime dt = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5} {6}", month, day, year, hour, minute, second, amPM_Options[amPM_SelectedIndex])).ToUniversalTime();
                expiration = (long)dt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            } catch { }
        }
        EditorGUILayout.EndVertical();
    }

    void DisplayAddMainArea() {
        SharedArea();

        EditorGUILayout.Space();

        if(GUILayout.Button("Done", GUILayout.Width(100))) {
            foreach(CouponSystem_Coupon row in table) {
                if(row.code.Equals(newCode)) {
                    return;
                }
            }

            Database.singleton.CouponSystem_AddCoupon(newCode, (CouponSystem_CouponType)couponType_SelectedIndex, itemName, amount, singleUse, expiration);

            ResetVariables();

            RefreshDatabase();
            GUI.FocusControl("Wut");

            state = State.BLANK;
        }

        if(GUILayout.Button("Cancel", GUILayout.Width(100))) {
            ResetVariables();

            GUI.FocusControl("Wut");

            state = State.BLANK;
        }
    }
}