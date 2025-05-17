using System.ComponentModel;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;

public class BarSettings {
    [DefaultValue(true)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.BarSettings.Show")]         // 自定义本地化键
    public bool Show { get; set; }

    [DefaultValue(false)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.BarSettings.AutoHideWhenFull")]
    public bool AutoHideWhenFull { get; set; }
    
    [DefaultValue(false)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.BarSettings.OnlyShowInBossFight")]
    public bool OnlyShowInBossFight { get; set; }

    [Increment(0.1f)]
    [Range(0.5f, 3f)]
    [Slider]
    [DefaultValue(1.2f)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.BarSettings.Scale")]
    public float Scale { get; set; }

    [Increment(1)]
    [Range(-150, 150)]
    [Slider]
    [DefaultValue(28)]
    [LabelKey("$Mods.PlayerBarsAndCursors.Config.BarSettings.Offset")]
    public int Offset { get; set; }

    // 如果有计算属性或用 [JsonIgnore] 的字段，也可用：
    [JsonIgnore]
    [ShowDespiteJsonIgnore]  // 强制在 UI 中显示
    public string DebugInfo => $"Scale={Scale},Offset={Offset}";
}