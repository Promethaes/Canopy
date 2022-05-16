using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using Canopy;
using System;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView btView;
    InspectorView inspectorView;
    Button saveButton;

    [MenuItem("Tools/Canopy/Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (!(Selection.activeObject is BehaviourTree))
            return false;
        OpenWindow();
        return true;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        //GUID from .meta file
        var tree = LoadAssetByGUID.Load<VisualTreeAsset>("1530149fafe11784f987aeba157f711f");
        tree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        
        var styleSheet = LoadAssetByGUID.Load<StyleSheet>("806b0aa08637abe4b93e46dc71208a9f");
        root.styleSheets.Add(styleSheet);

        btView = root.Query<BehaviourTreeView>();
        btView.OnNodeSelected = OnNodeSelectionChanged;
        inspectorView = root.Query<InspectorView>();
        saveButton = root.Query<Button>();
        //more with this later
        saveButton.clickable.clicked += () => AssetDatabase.SaveAssets();

        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnSelectionChange()
    {
        var tree = Selection.activeObject as BehaviourTree;

        //test to see if component exists on game object
        if (!tree)
            tree = Selection.activeGameObject?.GetComponent<BehaviourTreeController>()?.behaviourTree;

        if (tree && (AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()) || Application.isPlaying))
            btView?.PopulateView(tree);
    }

    void OnNodeSelectionChanged(NodeView nodeView)
    {
        inspectorView.UpdateSelection(nodeView);

    }

    private void OnInspectorUpdate()
    {
        btView?.UpdateNodeStates();
    }
}