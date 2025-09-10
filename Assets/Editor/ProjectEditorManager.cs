using MVC.Global;
using System;
using UnityEditor;
using UnityEditor.Build;
using Utilities;

public class ProjectEditorManager
{
    /// <summary>
    /// Get project info to update editor values
    /// </summary>
    [MenuItem("Project/Update Properties", false, 1)]
    protected static void UpdateProperties()
    {
        try
        {
            SerializedObject playerSettingsManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(Configuration.Properties.ProjectSettings)[0]);
            PlayerSettings.companyName = ProjectInfo.CompanyName;
            PlayerSettings.productName = ProjectInfo.ProductName;
            PlayerSettings.bundleVersion = ProjectInfo.BundleVersion;
            PlayerSettings.macOS.buildNumber = ProjectInfo.BuildNumber;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, ProjectInfo.Identifier);
            playerSettingsManager.ApplyModifiedProperties();
            playerSettingsManager.Update();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
