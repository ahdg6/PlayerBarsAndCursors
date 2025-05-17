using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

public class CursorSettings
{
    [DefaultValue(true)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.ShowBossPointer")]
    public bool ShowBossPointer { get; set; } = true;

    [DefaultValue(true)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.ShowMousePointer")]
    public bool ShowMousePointer { get; set; } = true;

    [DefaultValue(2)]
    [Range(0, 3)]
    [Increment(1)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.AutoHideOnMapStyle")]
    public int AutoHideOnMapStyle { get; set; } = 2;

    [DefaultValue(true)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.OnlyShowInBossFight")]
    public bool OnlyShowInBossFight { get; set; } = true;


    [DefaultValue(true)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.ShowBossIcons")]
    public bool ShowBossIcons { get; set; } = true;

    /// <summary>
    ///     最多同时指向多少个 Boss，0 或负数 表示不限制
    /// </summary>
    [DefaultValue(3)]
    [Range(0, 10)]
    [Increment(1)]
    [Slider]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.MaxBossPointers")]
    public int MaxBossPointers { get; set; } = 3;

    /// <summary>整体缩放（0.5–3）</summary>
    [DefaultValue(1f)]
    [Range(0.5f, 3f)]
    [Increment(0.1f)]
    [Slider]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.Scale")]
    public float Scale { get; set; } = 1f;

    /// <summary>指针距离玩家中心的半径（0–300）</summary>
    [DefaultValue(64f)]
    [Range(0f, 300f)]
    [Increment(1f)]
    [Slider]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.PointerRadius")]
    public float PointerRadius { get; set; } = 64f;

    /// <summary>鼠标与任一 Boss 靠近时才触发辉光的阈值（0–100）</summary>
    [DefaultValue(12f)]
    [Range(0f, 100f)]
    [Increment(1f)]
    [Slider]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.CloseDistanceThreshold")]
    public float CloseDistanceThreshold { get; set; } = 12f;

    /// <summary>辉光相对于指针的额外缩放（1–2）</summary>
    [DefaultValue(1.2f)]
    [Range(1f, 2f)]
    [Increment(0.01f)]
    [Slider]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.GlowScaleMultiplier")]
    public float GlowScaleMultiplier { get; set; } = 1.2f;

    [DefaultValue(typeof(Color), "255,120,120,120")]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.BossPointerColor")]
    public Color BossPointerColor { get; set; } = new(255, 120, 120, 120);

    [DefaultValue(typeof(Color), "255,120,120,40")]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.BossGlowColor")]
    public Color BossGlowColor { get; set; } = new(255, 120, 120, 40);

    [DefaultValue(typeof(Color), "120,255,120,120")]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.MousePointerColor")]
    public Color MousePointerColor { get; set; } = new(120, 255, 120, 120);

    [DefaultValue(typeof(Color), "120,255,120,40")]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.CursorSettings.MouseGlowColor")]
    public Color MouseGlowColor { get; set; } = new(120, 255, 120, 40);
}