// Generated from TagsGenerator.cs

public static class Tags
{
    public const string Untagged = nameof(Untagged);
    public const string Respawn = nameof(Respawn);
    public const string Finish = nameof(Finish);
    public const string EditorOnly = nameof(EditorOnly);
    public const string MainCamera = nameof(MainCamera);
    public const string Player = nameof(Player);
    public const string GameController = nameof(GameController);
}

public static class Layers
{
    public const int Default = 0;
    public const int TransparentFX = 1;
    public const int IgnoreRaycast = 2;
    public const int Water = 4;
    public const int UI = 5;
    public const int Attraction = 6;
}

public static class LayerMasks
{
    public const int Default = 1 << Layers.Default;
    public const int TransparentFX = 1 << Layers.TransparentFX;
    public const int IgnoreRaycast = 1 << Layers.IgnoreRaycast;
    public const int Water = 1 << Layers.Water;
    public const int UI = 1 << Layers.UI;
    public const int Attraction = 1 << Layers.Attraction;
}
