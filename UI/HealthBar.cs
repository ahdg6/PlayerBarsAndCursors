using Microsoft.Xna.Framework;

namespace PlayerBarsAndCursors.UI;

/// <summary>
///     血条控件，继承自BaseBar，实现了血量百分比、颜色与位置的具体计算
/// </summary>
internal class HealthBar : BaseBar
{
    private BarSettings Cfg => BarsContainer.HealthBarConfig;
    
    /// <summary>
    ///     判断是否显示血条
    /// </summary>
    protected override bool ShouldDraw()
    {
        return Cfg.Show;
    }

    protected override BarSettings GetBarConfig()
    {
        return Cfg;
    }

    /// <summary>
    ///     根据玩家当前生命值计算填充百分比
    /// </summary>
    protected override float GetFillPercentage()
    {
        return (float)Player.statLife / Player.statLifeMax2;
    }

    /// <summary>
    ///     根据血量百分比动态设置血条颜色
    /// </summary>
    protected override Color GetBarColor()
    {
        var percentage = GetFillPercentage();
        // 根据血量百分比设置颜色，从满血的绿色调到低血时的红色调，透明度固定为200
        if (percentage > 0.9f) return new Color(10, 245, 0, 200);

        if (percentage > 0.5f) return new Color(200, 200, 0, 200);

        if (percentage > 0.15f) return new Color(200, 100, 0, 200);

        return new Color(245, 10, 0, 200);
    }

    /// <summary>
    ///     根据玩家重力方向计算血条的Y坐标
    ///     正常情况下血条位于玩家下方；倒转时位于玩家上方
    /// </summary>
    protected override float GetBarYPosition(float baseYPos, float zoom, int textureHeight)
    {
        // Cfg.Offset*zoom + textureHeight*Cfg.Scale 是两个条之间的总间距
        // gravDir =  1 → 正立，向下偏移
        // gravDir = -1 → 倒立，向上偏移
        return baseYPos + (Cfg.Offset * zoom + textureHeight * Cfg.Scale) * Player.gravDir;
    }
}