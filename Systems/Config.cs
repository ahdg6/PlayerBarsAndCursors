using Microsoft.Xna.Framework;
using PlayerBarsAndCursors.UI;
using Terraria.ModLoader.Config;

namespace PlayerBarsAndCursors.Systems;

internal class PlayerBarsAndCursors : ModConfig
{
    [Header("$Mods.PlayerBarsAndCursors.Config.Headers.BarHeader")]
    public BarSettings HealthBar = new()
    {
        Show = true,
        AutoHideWhenFull = false,
        OnlyShowInBossFight = false,
        Scale = 1.2f,
        Offset = 12
    };

    public BarSettings ManaBar = new()
    {
        Show = true,
        AutoHideWhenFull = false,
        OnlyShowInBossFight = false,
        Scale = 1.2f,
        Offset = 20
    };

    public BarSettings WingTimeBar = new()
    {
        Show = true,
        AutoHideWhenFull = true,
        OnlyShowInBossFight = false,
        Scale = 1.0f,
        Offset = -48
    };
    
    [Header("$Mods.PlayerBarsAndCursors.Config.Headers.CursorHeader")]
    public CursorSettings CursorSettings = new()
    {
        ShowBossPointer = true,
        ShowMousePointer = true,
        AutoHideOnMapStyle = 2,
        OnlyShowInBossFight = true,
        ShowBossIcons = true,
        MaxBossPointers = 3,
        Scale = 0.8f,
        PointerRadius = 64f,
        CloseDistanceThreshold = 12f,
        GlowScaleMultiplier = 1.05f,
        BossPointerColor = new Color(255, 120, 120, 120),
        BossGlowColor = new Color(255, 120, 120, 40),
        MousePointerColor = new Color(120, 255, 120, 120),
        MouseGlowColor = new Color(120, 255, 120, 40)
    };

    public override ConfigScope Mode => ConfigScope.ClientSide;

    public override void OnChanged()
    {
        BarsContainer.HealthBarConfig = HealthBar;
        BarsContainer.ManaBarConfig = ManaBar;
        BarsContainer.WingTimeBarConfig = WingTimeBar;
        BarsContainer.CursorConfig = CursorSettings;
    }
}