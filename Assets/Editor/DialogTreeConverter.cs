using UnityEngine;
using UnityEditor;

public class DialogTreeConverter : EditorWindow
{
    DialogSetUp sceneDialogSetup;
    DialogTree dialogTreeAsset;

    [MenuItem("Tools/Dialog/Copy Scene Dialog To Tree")]
    static void Init()
    {
        GetWindow<DialogTreeConverter>("Dialog Copier");
    }

    void OnGUI()
    {
        GUILayout.Label("Copy Scene Dialog to DialogTree", EditorStyles.boldLabel);

        sceneDialogSetup = (DialogSetUp)EditorGUILayout.ObjectField("Scene Dialog Setup", sceneDialogSetup, typeof(DialogSetUp), true);
        dialogTreeAsset = (DialogTree)EditorGUILayout.ObjectField("DialogTree Asset", dialogTreeAsset, typeof(DialogTree), false);

        if (GUILayout.Button("Copy Dialog"))
        {
            if (sceneDialogSetup == null || dialogTreeAsset == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign both Scene Dialog and DialogTree Asset.", "OK");
                return;
            }

            Undo.RecordObject(dialogTreeAsset, "Copy Dialog Sections");
            dialogTreeAsset.sections = sceneDialogSetup.sceneDialog;
            EditorUtility.SetDirty(dialogTreeAsset);

            EditorUtility.DisplayDialog("Success", "Dialog copied to DialogTree!", "OK");
        }
    }
}
