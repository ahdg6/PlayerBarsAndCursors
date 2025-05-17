using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace PlayerBarsAndCursors.UI
{
    /// <summary>
    /// 绘制两个指向标：一个指向场上 boss（可独立开/关），一个指向玩家鼠标位置（可独立开/关）。
    /// 在指针纹理预生成的基础上，额外生成了经过轻微模糊的辉光纹理，
    /// 以便在指针靠近时绘制出柔和且颜色区分明显的发光效果，而不会出现全白现象。
    /// 支持可选 Boss 头像，头像存在时替换指针但保留辉光。
    /// </summary>
    internal class EnhancedTargetPointersUI : UIElement
    {
        // 静态指针纹理与辉光纹理（均只生成一次保证性能）
        private static Texture2D PointerTexture;
        private static Texture2D GlowTexture;

        // 指针信息：位置、旋转、颜色，以及是否为鼠标指针、对应的 Boss 实例
        private struct PointerInfo
        {
            public Vector2 Position;
            public float   Rotation;
            public Color   PointerColor;
            public Color   GlowColor;
            public bool    IsMouse;
            public NPC     BossNpc;   // 若 IsMouse==false，则此字段有效
        }

        // 重用列表，避免每帧分配
        private readonly List<PointerInfo> _pointers = new(8);

        // 从 BarsContainer 里拿到用户配置
        private CursorSettings Cfg => BarsContainer.CursorConfig;

        // 静态构造：第一次访问时生成纹理
        static EnhancedTargetPointersUI() => CreatePointerAndGlowTextures();

        /// <summary>
        /// 生成指针纹理与预模糊的辉光纹理  
        /// 指针纹理设计为一个小三角形：尖端在纹理顶部（只有1个像素），底部逐渐扩展；  
        /// 然后对指针纹理进行简单盒状模糊，生成柔和的 GlowTexture，用于绘制辉光效果。
        /// </summary>
        private static void CreatePointerAndGlowTextures()
        {
            const int W = 10, H = 15;
            var gfx = Main.instance.GraphicsDevice;

            // 原始指针三角形
            PointerTexture = new Texture2D(gfx, W, H);
            var ptrData = new Color[W * H];
            for (int y = 0; y < H; y++)
            {
                float t = y / (float)(H - 1);
                int L = (int)MathHelper.Lerp(W / 2f, 0, t), R = (int)MathHelper.Lerp(W / 2f, W - 1, t);
                for (int x = L; x <= R; x++)
                    ptrData[y * W + x] = Color.White;
            }
            PointerTexture.SetData(ptrData);

            // 3×3 盒状模糊生成辉光
            GlowTexture = new Texture2D(gfx, W, H);
            var glowData = new Color[W * H];
            for (int i = 0; i < glowData.Length; i++)
            {
                int x = i % W, y = i / W;
                int r = 0, g = 0, b = 0, a = 0, cnt = 0;
                for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    int xx = x + dx, yy = y + dy;
                    if (xx < 0 || yy < 0 || xx >= W || yy >= H) continue;
                    var c = ptrData[yy * W + xx];
                    r += c.R; g += c.G; b += c.B; a += c.A; cnt++;
                }
                glowData[i] = new Color(
                    (byte)(r / cnt),
                    (byte)(g / cnt),
                    (byte)(b / cnt),
                    (byte)((a / cnt) * 0.6f)
                );
            }
            GlowTexture.SetData(glowData);
        }

        public override void Draw(SpriteBatch sb)
        {
            var cfg = Cfg;

            // 全局开关
            if (!cfg.ShowBossPointer && !cfg.ShowMousePointer)
                return;

            // 打开地图时自动隐藏
            if (Main.mapStyle == cfg.AutoHideOnMapStyle)
                return;

            // 玩家存活检查
            if (Main.LocalPlayer.statLife <= 0)
                return;

            _pointers.Clear();

            // 场上主 Boss
            NPC primary = BarsContainer.ActiveBoss;

            // Boss 指针：优先加入主 Boss，再根据 MaxBossPointers 添加额外 Boss
            if (cfg.ShowBossPointer && primary is { active: true, life: > 0 })
            {
                AddPointer(primary, cfg, isMouse: false);
                int max = cfg.MaxBossPointers > 0 ? cfg.MaxBossPointers : int.MaxValue;
                if (_pointers.Count < max)
                {
                    for (int i = 0; i < Main.maxNPCs && _pointers.Count < max; i++)
                    {
                        var npc = Main.npc[i];
                        if (npc == primary || !npc.active || npc.life <= 0) continue;
                        if (npc.GetBossHeadTextureIndex() == -1) continue;
                        AddPointer(npc, cfg, isMouse: false);
                    }
                }
            }

            // 鼠标指针（仅当必要时）
            if (cfg.ShowMousePointer && (!cfg.OnlyShowInBossFight || primary != null))
                AddPointer(null, cfg, isMouse: true);

            if (_pointers.Count == 0)
                return;

            float scale = cfg.Scale;

            // 只检测“鼠标 ⇆ Boss”之间的距离，记录需发光的索引
            int mouseIdx = _pointers.FindIndex(p => p.IsMouse);
            var glowSet = new HashSet<int>();
            if (mouseIdx >= 0)
            {
                float thr2 = cfg.CloseDistanceThreshold * cfg.CloseDistanceThreshold;
                var mPos = _pointers[mouseIdx].Position;
                for (int i = 0; i < _pointers.Count; i++)
                {
                    if (i == mouseIdx) continue;
                    if (Vector2.DistanceSquared(mPos, _pointers[i].Position) < thr2)
                    {
                        glowSet.Add(mouseIdx);
                        glowSet.Add(i);
                        // 如果只需第一对，可在此处 break
                    }
                }
            }

            // 先绘制辉光层（包括有头像和普通指针的 Boss 以及鼠标）
            if (glowSet.Count > 0)
            {
                var originG = new Vector2(GlowTexture.Width / 2f, 0f);
                float scaleG = scale * cfg.GlowScaleMultiplier;
                foreach (var idx in glowSet)
                {
                    var p = _pointers[idx];
                    sb.Draw(
                        GlowTexture,
                        p.Position,
                        null,
                        p.GlowColor,
                        p.Rotation,
                        originG,
                        scaleG,
                        SpriteEffects.None,
                        0f
                    );
                }
            }

            // 再绘制指针本体 & 可选 Boss 头像（互斥）
            var originP = new Vector2(PointerTexture.Width / 2f, 0f);
            float scaleP = scale;
            for (int i = 0; i < _pointers.Count; i++)
            {
                var p = _pointers[i];

                if (p.IsMouse)
                {
                    // 鼠标始终绘制指针
                    sb.Draw(
                        PointerTexture,
                        p.Position,
                        null,
                        p.PointerColor,
                        p.Rotation,
                        originP,
                        scaleP,
                        SpriteEffects.None,
                        0f
                    );
                }
                else
                {
                    // Boss：有头像时绘制头像，无头像或关闭时绘制指针
                    int headIdx = p.BossNpc.GetBossHeadTextureIndex();
                    bool useIcon = cfg.ShowBossIcons && headIdx >= 0;
                    if (useIcon)
                    {
                        DrawBossIcon(sb, p, cfg);
                    }
                    else
                    {
                        sb.Draw(
                            PointerTexture,
                            p.Position,
                            null,
                            p.PointerColor,
                            p.Rotation,
                            originP,
                            scaleP,
                            SpriteEffects.None,
                            0f
                        );
                    }
                }
            }
        }

        // 添加一个指针（Boss 或鼠标）
        private void AddPointer(NPC boss, CursorSettings cfg, bool isMouse)
        {
            Vector2 dir = isMouse
                ? Main.MouseScreen - BarsContainer.PlayerScreenPos
                : boss.Center       - Main.LocalPlayer.Center;
            dir.Normalize();

            _pointers.Add(new PointerInfo
            {
                Position     = BarsContainer.PlayerScreenPos + dir * cfg.PointerRadius,
                Rotation     = dir.ToRotation() + MathHelper.PiOver2,
                PointerColor = isMouse ? cfg.MousePointerColor : cfg.BossPointerColor,
                GlowColor    = isMouse ? cfg.MouseGlowColor   : cfg.BossGlowColor,
                IsMouse      = isMouse,
                BossNpc      = boss
            });
        }

        // 绘制 Boss 头像
        private void DrawBossIcon(SpriteBatch sb, PointerInfo p, CursorSettings cfg)
        {
            var boss = p.BossNpc;
            int idx = boss.GetBossHeadTextureIndex();
            Texture2D headTex = (idx >= 0 && idx < TextureAssets.NpcHeadBoss.Length)
                ? TextureAssets.NpcHeadBoss[idx].Value
                : TextureAssets.Projectile[Terraria.ID.ProjectileID.ShadowBeamFriendly].Value;

            // 头像稍微往内偏移一点，避免与箭头重叠
            var dir = (boss.Center - Main.LocalPlayer.Center);
            dir.Normalize();
            var headPos = BarsContainer.PlayerScreenPos + dir * (cfg.PointerRadius - 12f * cfg.Scale);

            float iconScale = cfg.Scale * 0.8f;
            sb.Draw(
                headTex,
                headPos,
                null,
                Color.White,
                0f,
                headTex.Size() * 0.5f,
                iconScale,
                boss.GetBossHeadSpriteEffects(),
                0f
            );
        }
    }
}
