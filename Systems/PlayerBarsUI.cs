using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PlayerBarsAndCursors.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace PlayerBarsAndCursors.Systems {
    [Autoload(Side = ModSide.Client)]
    internal class PlayerBarsUI : ModSystem {
        internal BarsContainer BarsContainer;
        private UserInterface _barsUI;
        private ModKeybind _toggleKey;

        public override void Load() {
            // 注册一个开关按键，比如 P
            _toggleKey = KeybindLoader.RegisterKeybind(Mod, "Toggle Player Bars", "P");
            
            BarsContainer = new BarsContainer();
            BarsContainer.Activate();

            _barsUI = new UserInterface();
            _barsUI.SetState(BarsContainer);
        }

        public override void UpdateUI(GameTime gameTime) {
            // 按键按下时切换
            if (_toggleKey.JustPressed) {
                if (_barsUI.CurrentState == null)
                    _barsUI.SetState(BarsContainer);
                else
                    _barsUI.SetState(null);
            }

            // 再更新一次 UI（如果 CurrentState 是 null，什么都不做）
            _barsUI?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int idx = layers.FindIndex(l => l.Name == "Vanilla: Interface Logic 2");
            layers.Insert(idx, new LegacyGameInterfaceLayer(
                "Player Bars: Bars Container",
                () => {
                    // Draw 也只在 CurrentState != null 时绘制
                    if (_barsUI.CurrentState != null)
                        _barsUI.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI
            ));
        }
    }
}