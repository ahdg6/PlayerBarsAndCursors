using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace PlayerBarsAndCursors.UI;

/// <summary>
///     UI容器
/// </summary>
internal class BarsContainer : UIState {
    public HealthBar HealthBar;
    public ManaBar ManaBar;
    public WingTimeBar WingTimeBar;
    public EnhancedTargetPointersUI TargetPointer;
    

    // 由于有很多子 UI 元素需要这些，在这里提供
    public static NPC ActiveBoss { get; private set; }
    public static Vector2 PlayerScreenPos { get; private set; }
    
    // 接收配置
    public static BarSettings HealthBarConfig    { get; set; }
    public static BarSettings ManaBarConfig      { get; set; }
    public static BarSettings WingTimeBarConfig  { get; set; }
    public static CursorSettings CursorConfig { get; set; }

    public override void OnInitialize() {
        HealthBar   = new HealthBar();
        ManaBar     = new ManaBar();
        WingTimeBar = new WingTimeBar();
        TargetPointer = new EnhancedTargetPointersUI();

        Append(HealthBar);
        Append(ManaBar);
        Append(WingTimeBar);
        Append(TargetPointer);
    }

    public override void Update(GameTime gameTime)
    {
        PlayerScreenPos = Main.LocalPlayer.Center.ToScreenPosition();
        ActiveBoss = null;
        foreach (NPC npc in Main.npc)
        {
            if (npc.active && npc.boss)
            {
                ActiveBoss = npc;
                break;
            }
        }
    }
}