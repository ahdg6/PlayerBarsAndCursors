using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace PlayerBarsAndCursors.UI
{
    /// <summary>
    /// 绘制多个指向标：优先使用 BarsContainer.ActiveBoss
    /// 仅当鼠标与任一 Boss 指针靠近时，才对那对指针绘制辉光
    /// 其余优化同前：预分配列表、静态纹理缓存、手写距离检测
    /// </summary>
    internal class EnhancedTargetPointersUI : UIElement
    {
        //—— 静态纹理 & 缓存 ——//
        private static readonly Texture2D PointerTexture;
        private static readonly Texture2D GlowTexture;

        // 指针数据，增加 IsMouse 区分
        private struct PointerInfo
        {
            public Vector2 Position;
            public float   Rotation;
            public Color   PointerColor;
            public Color   GlowColor;
            public bool    IsMouse;
        }

        // 重用列表，避免频繁分配
        private readonly List<PointerInfo> _pointers = new(8);

        // 配置快捷访问
        private CursorSettings Cfg => BarsContainer.CursorConfig;

        // 静态构造：生成一次性纹理
        static EnhancedTargetPointersUI() {
            const int W = 10, H = 15;
            var gfx = Main.instance.GraphicsDevice;
            PointerTexture = new Texture2D(gfx, W, H);
            GlowTexture = new Texture2D(gfx, W, H);
            CreatePointerAndGlowTextures(W, H);
        }

        private static void CreatePointerAndGlowTextures(int W, int H)
        {
            // 1. 指针三角形
            var ptr = new Color[W*H];
            for (int y = 0; y < H; y++)
            {
                float t = y / (float)(H - 1);
                int L = (int)MathHelper.Lerp(W/2f, 0, t), R = (int)MathHelper.Lerp(W/2f, W-1, t);
                for (int x = L; x <= R; x++) ptr[y*W + x] = Color.White;
            }
            PointerTexture.SetData(ptr);

            // 2. 3×3 盒状模糊辉光
            var glow = new Color[W*H];
            for (int i = 0; i < glow.Length; i++)
            {
                int x = i % W, y = i / W;
                int r=0, g=0, b=0, a=0, cnt=0;
                for (int dy=-1; dy<=1; dy++)
                for (int dx=-1; dx<=1; dx++)
                {
                    int xx = x+dx, yy = y+dy;
                    if (xx<0||yy<0||xx>=W||yy>=H) continue;
                    var c = ptr[yy*W + xx];
                    r+=c.R; g+=c.G; b+=c.B; a+=c.A; cnt++;
                }
                glow[i] = new Color((byte)(r/cnt), (byte)(g/cnt), (byte)(b/cnt), (byte)((a/cnt)*0.6f));
            }
            GlowTexture.SetData(glow);
        }

        public override void Draw(SpriteBatch sb)
        {
            var cfg = Cfg;
            //—— 开关 & 状态 校验 ——//
            if (!cfg.ShowBossPointer && !cfg.ShowMousePointer) return;
            if (Main.mapStyle == cfg.AutoHideOnMapStyle)      return;
            if (Main.LocalPlayer.statLife <= 0)                return;

            _pointers.Clear();

            //—— 1. Boss 指针 列表 ——//
            NPC primary = BarsContainer.ActiveBoss;
            if (cfg.ShowBossPointer && primary is { active: true, life: > 0 })
            {
                AddBoss(primary, cfg);
                int max = cfg.MaxBossPointers > 0 ? cfg.MaxBossPointers : int.MaxValue;

                // 仅在需要更多指针且尚未达到上限时，遍历其他 NPC
                if (_pointers.Count < max)
                {
                    var plr = Main.LocalPlayer;
                    for (int i = 0; i < Main.maxNPCs && _pointers.Count < max; i++)
                    {
                        var npc = Main.npc[i];
                        if (npc == primary || !npc.active || npc.life <= 0) continue;
                        if (npc.GetBossHeadTextureIndex() == -1) continue;
                        AddBoss(npc, cfg);
                    }
                }
            }

            //—— 2. 鼠标指针 ——//
            if (cfg.ShowMousePointer && (!cfg.OnlyShowInBossFight || primary != null))
                AddMouse(cfg);

            if (_pointers.Count == 0) return;

            //—— 3. 只检测 鼠标 与 Boss 的距离 ——//
            int mouseIdx = -1;
            for (int i = 0; i < _pointers.Count; i++)
                if (_pointers[i].IsMouse) { mouseIdx = i; break; }

            var glowSet = new HashSet<int>();
            if (mouseIdx >= 0)
            {
                float thr2 = cfg.CloseDistanceThreshold * cfg.CloseDistanceThreshold;
                var mPos = _pointers[mouseIdx].Position;
                for (int i = 0; i < _pointers.Count; i++)
                {
                    if (_pointers[i].IsMouse) continue;
                    // boss pointer
                    if (Vector2.DistanceSquared(mPos, _pointers[i].Position) < thr2)
                    {
                        glowSet.Add(mouseIdx);
                        glowSet.Add(i);
                        // 如果只想第一个匹配就 break，可以在此 break
                    }
                }
            }

            //—— 4. 绘制辉光（仅靠近的那对） ——//
            if (glowSet.Count > 0)
            {
                var orgG = new Vector2(GlowTexture.Width/2f, 0f);
                float sG = cfg.Scale * cfg.GlowScaleMultiplier;
                foreach (int idx in glowSet)
                {
                    var p = _pointers[idx];
                    sb.Draw(GlowTexture, p.Position, null, p.GlowColor,
                            p.Rotation, orgG, sG, SpriteEffects.None, 0f);
                }
            }

            //—— 5. 绘制主指针 ——//
            var orgP = new Vector2(PointerTexture.Width/2f, 0f);
            float sP   = cfg.Scale;
            for (int i = 0; i < _pointers.Count; i++)
            {
                var p = _pointers[i];
                sb.Draw(PointerTexture, p.Position, null, p.PointerColor,
                        p.Rotation, orgP, sP, SpriteEffects.None, 0f);
            }
        }

        // 将 Boss 指针加入列表
        private void AddBoss(NPC boss, CursorSettings cfg)
        {
            var dir = boss.Center - Main.LocalPlayer.Center;
            dir.Normalize();
            _pointers.Add(new PointerInfo {
                Position     = BarsContainer.PlayerScreenPos + dir * cfg.PointerRadius,
                Rotation     = dir.ToRotation() + MathHelper.PiOver2,
                PointerColor = cfg.BossPointerColor,
                GlowColor    = cfg.BossGlowColor,
                IsMouse      = false
            });
        }

        // 将鼠标指针加入列表
        private void AddMouse(CursorSettings cfg)
        {
            var dir = Main.MouseScreen - BarsContainer.PlayerScreenPos;
            dir.Normalize();
            _pointers.Add(new PointerInfo {
                Position     = BarsContainer.PlayerScreenPos + dir * cfg.PointerRadius,
                Rotation     = dir.ToRotation() + MathHelper.PiOver2,
                PointerColor = cfg.MousePointerColor,
                GlowColor    = cfg.MouseGlowColor,
                IsMouse      = true
            });
        }
    }
}
