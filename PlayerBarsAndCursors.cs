using ReLogic.Content.Sources;
using Terraria.ModLoader;

namespace PlayerBarsAndCursors;

public class PlayerBarsAndCursors : Mod
{
    public PlayerBarsAndCursors()
    {
        Instance = this;
    }

    public static PlayerBarsAndCursors Instance { get; set; }

    public override IContentSource CreateDefaultContentSource()
    {
        return base.CreateDefaultContentSource();
    }
}