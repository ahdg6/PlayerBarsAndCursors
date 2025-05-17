using Microsoft.Xna.Framework;

namespace PlayerBarsAndCursors.UI;

/// <summary>
///     蓝条控件，继承自BaseBar，实现了法力百分比、固定颜色以及相对于血条的偏移计算
/// </summary>
internal class ManaBar : BaseBar
{
    private BarSettings Cfg => BarsContainer.ManaBarConfig;

    /// <summary>
    ///     判断是否显示蓝条
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
    ///     根据玩家当前法力值计算填充百分比
    /// </summary>
    protected override float GetFillPercentage()
    {
        return (float)Player.statMana / Player.statManaMax2;
    }

    /// <summary>
    ///     蓝条使用固定的颜色
    /// </summary>
    protected override Color GetBarColor()
    {
        return new Color(100, 130, 210, 200);
    }

    /// <summary>
    ///     根据玩家重力方向计算蓝条的Y坐标
    ///     蓝条相对于血条会有额外的偏移（上下各偏移1个像素以及血条高度）
    /// </summary>
    protected override float GetBarYPosition(float baseYPos, float zoom, int textureHeight)
    {
        // Cfg.Offset*zoom + textureHeight*Cfg.Scale 是两个条之间的总间距
        // gravDir =  1 → 正立，向下偏移
        // gravDir = -1 → 倒立，向上偏移
        return baseYPos + (Cfg.Offset * zoom + textureHeight * Cfg.Scale) * Player.gravDir;
    }
}