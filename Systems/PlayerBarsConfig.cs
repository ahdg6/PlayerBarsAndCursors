using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace PlayerBarsAndCursors.Systems {
    internal class PlayerBarsConfig : ModConfig {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        
        [Header("$Mods.PlayerBarsAndCursors.Config.Headers.BarHeader")]
        public BarSettings HealthBar = new BarSettings {
            Show              = true,
            AutoHideWhenFull  = false,
            OnlyShowInBossFight = false,
            Scale             = 1.2f,
            Offset            = 12
        };
        public BarSettings ManaBar = new BarSettings {
            Show              = true,
            AutoHideWhenFull  = false,
            OnlyShowInBossFight = false,
            Scale             = 1.2f,
            Offset            = 20
        };
        public BarSettings WingTimeBar = new BarSettings {
            Show              = true,
            AutoHideWhenFull  = true,
            OnlyShowInBossFight = false,
            Scale             = 1.0f,
            Offset            = -48
        };

        [Header("$Mods.PlayerBarsAndCursors.Config.Headers.CursorHeader")]
        public CursorSettings CursorSettings = new CursorSettings {
            ShowBossPointer       = true,
            ShowMousePointer      = true,
            AutoHideOnMapStyle    = 2,
            OnlyShowInBossFight   = true,
            Scale                 = 0.8f,
            PointerRadius         = 64f,
            CloseDistanceThreshold= 12f,
            GlowScaleMultiplier   = 1.05f,
            BossPointerColor      = new Color(255,120,120,120),
            BossGlowColor         = new Color(255,120,120,40),
            MousePointerColor     = new Color(120,255,120,120),
            MouseGlowColor        = new Color(120,255,120,40),
        };

        public override void OnChanged() {
            // 配置变动时推给 UI 容器
            UI.BarsContainer.HealthBarConfig   = HealthBar;
            UI.BarsContainer.ManaBarConfig     = ManaBar;
            UI.BarsContainer.WingTimeBarConfig = WingTimeBar;
            UI.BarsContainer.CursorConfig      = CursorSettings;
        }
    }
}
