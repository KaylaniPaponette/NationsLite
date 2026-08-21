using System.Collections.Generic;

[System.Serializable]
public struct AssetGroup
{
    public bool useNumberedKey;
    public string folderName;
    public List<AssetEntry> assets;
}