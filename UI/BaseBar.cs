using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace PlayerBarsAndCursors.UI;

/// <summary>
///     基础的条形UI元素，封装了绘制流程（包括背景和前景），由子类实现具体数值计算、位置与颜色设置
/// </summary>
internal abstract class BaseBar : UIElement
{
    // 公共纹理：背景条（不随数值变化）和前景条（根据数值缩放）
    protected Texture2D BarTextureBg = TextureAssets.Hb2.Value;
    protected Texture2D BarTextureFg = TextureAssets.Hb1.Value;

    // 便于子类获取当前玩家对象
    protected Player Player => Main.LocalPlayer;

    /// <summary>
    ///     获取当前条形需要填充的比例（0 ~ 1）
    /// </summary>
    protected abstract float GetFillPercentage();

    /// <summary>
    ///     获取条形颜色，默认为白色（子类可重写实现自定义颜色）
    /// </summary>
    protected virtual Color GetBarColor()
    {
        return Color.White;
    }

    /// <summary>
    ///     根据玩家状态计算条形的Y轴绘制位置（子类必须实现）
    /// </summary>
    /// <param name="baseYPos">玩家屏幕中Y轴的中心位置</param>
    /// <param name="zoom">当前游戏缩放倍率</param>
    /// <param name="textureHeight">条形纹理高度</param>
    /// <returns>返回条形的Y坐标</returns>
    protected abstract float GetBarYPosition(float baseYPos, float zoom, int textureHeight);

    /// <summary>
    ///     判断当前条形是否需要绘制（子类可根据显示设置实现）
    /// </summary>
    protected abstract bool ShouldDraw();

    /// <summary>
    ///     判断当前条形是否需要绘制（子类可根据显示设置实现）
    /// </summary>
    protected abstract BarSettings GetBarConfig();

    /// <summary>
    ///     根据公共计算公式得到X轴绘制位置（保证条形居中）
    /// </summary>
    protected float GetBarXPosition(int textureWidth, float scale, Vector2 screenPos)
    {
        return screenPos.X - textureWidth / 2f * scale;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // 根据子类判断是否需要绘制该条
        if (!ShouldDraw())
            return;

        // 如果玩家死亡，则不显示
        if (Player.statLife <= 0)
            return;

        // 计算当前的填充百分比（并保证在0～1范围内）
        var fillPercentage = Utils.Clamp(GetFillPercentage(), 0f, 1f);
        var _cfg = GetBarConfig();

        // 当启用满值自动隐藏功能时，如果填充满就直接返回
        if (_cfg.AutoHideWhenFull && fillPercentage >= 1f)
            return;

        if (_cfg.OnlyShowInBossFight)
            if (BarsContainer.ActiveBoss == null)
                return;

        var zoom = Main.GameZoomTarget;
        // 从父 UIState(UIElement) 获取玩家屏幕位置，由于他先于我渲染，所以是没问题的。
        var playerScreenPos = BarsContainer.PlayerScreenPos;

        // X轴始终居中显示，使用前景纹理宽度进行计算
        var xPos = GetBarXPosition(BarTextureFg.Width, _cfg.Scale, playerScreenPos);
        // Y轴位置由子类根据具体逻辑计算（例如血条与蓝条之间的相对位置）
        var yPos = GetBarYPosition(playerScreenPos.Y, zoom, BarTextureFg.Height);

        // 获取当前条形颜色
        var barColor = GetBarColor();

        // 绘制背景条（固定全宽的部分）
        spriteBatch.Draw(
            BarTextureBg,
            new Vector2(xPos, yPos),
            new Rectangle(0, 0, BarTextureBg.Width, BarTextureBg.Height),
            barColor,
            0f,
            Vector2.Zero,
            _cfg.Scale,
            SpriteEffects.None,
            0f);

        // 绘制前景条（根据当前数值缩放宽度）
        spriteBatch.Draw(
            BarTextureFg,
            new Vector2(xPos, yPos),
            new Rectangle(0, 0, (int)(BarTextureFg.Width * fillPercentage), BarTextureFg.Height),
            barColor,
            0f,
            Vector2.Zero,
            _cfg.Scale,
            SpriteEffects.None,
            0f);
    }
}