using Microsoft.Xna.Framework;

namespace PlayerBarsAndCursors.UI;

/// <summary>
///     蓝条控件，继承自BaseBar，实现了法力百分比、固定颜色以及相对于血条的偏移计算
/// </summary>
internal class WingTimeBar : BaseBar
{
    private BarSettings Cfg => BarsContainer.WingTimeBarConfig;

    /// <summary>
    ///     判断是否显示飞行时间条
    /// </summary>
    protected override bool ShouldDraw()
    {
        if (Cfg.Show)
            // 飞行时间是重置式的，只有飞行时才应该显示
            return Player.controlJump && Player.wingTime > 0 && !Player.mount.Active;

        return false;
    }

    protected override BarSettings GetBarConfig()
    {
        return Cfg;
    }

    /// <summary>
    ///     根据玩家当前法力值计算填充百分比
    /// </summary>
    protected override float GetFillPercentage()
    {
        return Player.wingTime / Player.wingTimeMax;
    }

    /// <summary>
    ///     飞行时间条使用灰白色
    /// </summary>
    protected override Color GetBarColor()
    {
        return new Color(200, 200, 200, 220);
    }

    /// <summary>
    ///     根据玩家重力方向计算飞行时间条的Y坐标
    ///     飞行时间条与血条异侧
    /// </summary>
    protected override float GetBarYPosition(float baseYPos, float zoom, int textureHeight)
    {
        // Cfg.Offset*zoom + textureHeight*Cfg.Scale 是两个条之间的总间距
        // gravDir =  1 → 正立，向下偏移
        // gravDir = -1 → 倒立，向上偏移
        return baseYPos + (Cfg.Offset * zoom + textureHeight * Cfg.Scale) * Player.gravDir;
    }
}