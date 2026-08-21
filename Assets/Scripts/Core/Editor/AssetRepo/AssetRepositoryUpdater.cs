using UnityEngine;
using UnityEditor;

public class AssetRepositoryUpdater : AssetPostprocessor
{
    const string kAssetRepositoryPath = "Assets/Resources/AssetRepo.asset"; 

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        var repo = AssetDatabase.LoadAssetAtPath<AssetRepository>(kAssetRepositoryPath);
        if (repo == null)
        {
            Debug.LogError("Could not find Asset Repository");
            return;
        }

        bool shouldUpdate = false;
        foreach (var importedAsset in importedAssets)
        {
            foreach (var assetGroup in repo.assetGroups)
            {
                if (importedAsset.StartsWith(assetGroup.folderName))
                    shouldUpdate = true;
            }

            if (shouldUpdate)
                break;
        }

        if (shouldUpdate)
            repo.PopulateFromFolders();
    }
}
