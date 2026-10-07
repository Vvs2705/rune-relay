using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RR
{
    /// <summary>
    /// Direcao de arte em codigo (zero asset): paleta, sprites procedurais (duros com 4 amostras de antialias e
    /// SUAVES com alfa continuo, no padrao do Formas.cs do ARKANA) e a fabrica de uGUI. Cada cor de mana tem
    /// tambem uma FORMA (GDD §11: cor nunca e' o unico sinal): azul disco, vermelho quadrado, verde triangulo,
    /// amarelo losango, roxo anel, laranja cruz.
    /// </summary>
    public static class Art
    {
        // ---------- paleta ----------
        /// <summary>Mana a..f: azul, vermelho, verde, amarelo, roxo, laranja. Saturadas para o fundo escuro.</summary>
        public static readonly Color[] Palette =
            { Hex(0x3EC1FF), Hex(0xFF4F6A), Hex(0x46E58A), Hex(0xFFD54A), Hex(0xB47CFF), Hex(0xFF8C3A) };
        public static readonly Color
            Bg = Hex(0x0E1120), BgGlow = Hex(0x2B2F5C),
            Cell = Hex(0x1C2238), CellEdge = Hex(0x11152A), CellTap = Hex(0x2A3356),
            Pipe = Hex(0x4C5A80), PipeDark = Hex(0x0B0E1A), PipeEdge = Hex(0x2E3756),
            Rock = Hex(0x080A12), Accent = Hex(0xFFD66B), AccentDark = Hex(0xB98A2B),
            Ink = Hex(0xF1F3FB), InkDim = Hex(0x9AA3BF), Bad = Hex(0xFF3B55), Good = Hex(0x4BE38A);

        public static Color ComAlfa(Color c, float a) => new Color(c.r, c.g, c.b, a);
        public static Color Darken(Color c, float k) => new Color(c.r * (1f - k), c.g * (1f - k), c.b * (1f - k), c.a);
        public static Color Lighten(Color c, float k) => Color.Lerp(c, Color.white, k);
        static Color Hex(int rgb) => new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f);

        // ---------- sprites duros (1 unidade de mundo = sprite inteiro) ----------
        const int Side = 64;
        static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static Sprite Square() => Get("sq", (x, y) => true);
        public static Sprite Rounded() => Get("rounded", (x, y) => RoundRect(x, y, 0.28f) <= 0f);
        public static Sprite Disc() => Get("disc", (x, y) => x * x + y * y <= 0.96f);
        public static Sprite Ring() => Ring(0.67f);
        /// <summary>Anel com furo de raio `inner` (0..0.98).</summary>
        public static Sprite Ring(float inner) => Get("ring" + Mathf.RoundToInt(inner * 100f), (x, y) =>
        {
            float d = x * x + y * y; return d <= 0.96f && d >= inner * inner;
        }, 128);
        public static Sprite Triangle() => Get("tri", (x, y) => y > -0.8f && Mathf.Abs(x) < (0.9f - y) * 0.58f);
        public static Sprite Diamond() => Get("dia", (x, y) => Mathf.Abs(x) + Mathf.Abs(y) < 0.98f);
        public static Sprite DiamondFrame() => Get("diaf", (x, y) => { float d = Mathf.Abs(x) + Mathf.Abs(y); return d < 0.98f && d > 0.80f; }, 128);
        public static Sprite Plus() => Get("plus", (x, y) => (Mathf.Abs(x) < 0.3f || Mathf.Abs(y) < 0.3f) && Mathf.Abs(x) < 0.9f && Mathf.Abs(y) < 0.9f);
        public static Sprite Cross() => Get("x", (x, y) => Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(y)) < 0.22f && Mathf.Abs(x) < 0.8f);
        /// <summary>Estrela de 5 pontas (a fonte embutida nao tem o glifo).</summary>
        public static Sprite Star() => Get("star", (x, y) =>
        {
            const float seg = Mathf.PI * 2f / 5f;
            float f = Mathf.Abs(Mathf.Repeat(Mathf.Atan2(x, y), seg) / seg * 2f - 1f); // 1 na ponta, 0 no vale
            return x * x + y * y <= Mathf.Pow(Mathf.Lerp(0.45f, 0.98f, f * f), 2f);
        }, 128);
        /// <summary>Seta cheia apontando para +x.</summary>
        public static Sprite Arrow() => Get("arrow", (x, y) => x > -0.7f && Mathf.Abs(y) < (0.8f - x) * 0.6f);
        /// <summary>Chevron "menor que" (botao voltar).</summary>
        public static Sprite Chevron() => Get("chev", (x, y) => Mathf.Abs(y) < 0.8f && Mathf.Abs(x - (-0.4f + Mathf.Abs(y) * 1.05f)) < 0.2f, 128);
        /// <summary>Dois triangulos para +x (segure para acelerar).</summary>
        public static Sprite FastForward() => Get("ff", (x, y) => TriRight(x, y, -0.95f, -0.05f) || TriRight(x, y, 0.05f, 0.95f));
        /// <summary>Duas barras verticais (travou).</summary>
        public static Sprite Pause() => Get("pause", (x, y) => Mathf.Abs(Mathf.Abs(x) - 0.38f) < 0.18f && Mathf.Abs(y) < 0.75f);
        /// <summary>Engrenagem de 8 dentes (configuracoes).</summary>
        public static Sprite Gear() => Get("gear", (x, y) =>
        {
            float r = Mathf.Sqrt(x * x + y * y);
            if (r < 0.28f) return false;
            if (r <= 0.6f) return true;
            return r <= 0.92f && Mathf.Cos(8f * Mathf.Atan2(y, x)) > 0.25f;
        }, 128);
        /// <summary>Cadeado: corpo embaixo, alca em cima (comporta fechada, nivel bloqueado).</summary>
        public static Sprite Lock() => Get("lock", (x, y) =>
        {
            if (y <= 0.05f && y >= -0.85f && Mathf.Abs(x) < 0.6f) return true;
            float r = Mathf.Sqrt(x * x + y * y);
            return y > 0f && r >= 0.3f && r <= 0.46f;
        }, 128);

        static bool TriRight(float x, float y, float x0, float x1) => x >= x0 && x <= x1 && Mathf.Abs(y) < (x1 - x) / (x1 - x0) * 0.75f;

        /// <summary>Distancia (negativa dentro) ate' um retangulo arredondado de meia-largura 1 e raio r.</summary>
        static float RoundRect(float x, float y, float r)
        {
            float qx = Mathf.Max(Mathf.Abs(x) - (1f - r), 0f), qy = Mathf.Max(Mathf.Abs(y) - (1f - r), 0f);
            float inside = Mathf.Min(Mathf.Max(Mathf.Abs(x) - (1f - r), Mathf.Abs(y) - (1f - r)), 0f);
            return Mathf.Sqrt(qx * qx + qy * qy) + inside - r;
        }

        /// <summary>Forma da cor c: azul disco, vermelho quadrado, verde triangulo, amarelo losango, roxo anel, laranja cruz.</summary>
        public static Sprite Shape(int c)
        {
            switch (c)
            {
                case 0: return Disc();
                case 1: return Get("sqs", (x, y) => Mathf.Abs(x) < 0.78f && Mathf.Abs(y) < 0.78f);
                case 2: return Triangle();
                case 3: return Diamond();
                case 4: return Ring(0.5f);
                default: return Plus();
            }
        }

        static Sprite Get(string key, Func<float, float, bool> inside, int side = Side)
        {
            if (Cache.TryGetValue(key, out Sprite s) && s != null) return s;
            var tex = new Texture2D(side, side, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var px = new Color32[side * side];
            for (int y = 0; y < side; y++)
                for (int x = 0; x < side; x++)
                {
                    int n = 0;
                    for (int sy = 0; sy < 2; sy++)
                        for (int sx = 0; sx < 2; sx++)
                            if (inside((x + 0.25f + sx * 0.5f) / side * 2f - 1f, (y + 0.25f + sy * 0.5f) / side * 2f - 1f)) n++;
                    px[y * side + x] = new Color32(255, 255, 255, (byte)(n * 63));
                }
            tex.SetPixels32(px);
            tex.Apply();
            s = Sprite.Create(tex, new Rect(0, 0, side, side), new Vector2(0.5f, 0.5f), side);
            Cache[key] = s;
            return s;
        }

        // ---------- sprites SUAVES (cor + alfa continuos por pixel): o que da' cara de jogo e nao de placeholder ----------
        static float Smooth(float a, float b, float v) { float t = Mathf.Clamp01((v - a) / (b - a)); return t * t * (3f - 2f * t); }

        /// <summary>Brilho radial (orbe, halo de luz, particula).</summary>
        public static Sprite Glow() => Soft("glow", 64, (x, y) =>
            new Color(1f, 1f, 1f, Mathf.Pow(Mathf.Clamp01(1f - Mathf.Sqrt(x * x + y * y)), 2.2f)));
        /// <summary>Halo em anel: brilho no aro (r=0.62), cai rapido para dentro e esvaece para fora.</summary>
        public static Sprite Halo() => Soft("halo", 64, (x, y) =>
        {
            float r = Mathf.Sqrt(x * x + y * y), d = r - 0.62f;
            float a = Mathf.Exp(-d * d / (d < 0f ? 0.008f : 0.035f));
            return new Color(1f, 1f, 1f, a * (1f - Smooth(0.85f, 1f, r)));
        });
        /// <summary>Disco com volume (claro em cima): corpo do orbe e botoes redondos.</summary>
        public static Sprite Orb() => Soft("orb", 64, (x, y) =>
        {
            float r = Mathf.Sqrt(x * x + y * y);
            float v = Mathf.Lerp(0.55f, 1f, (y + 1f) * 0.5f) + 0.25f * Mathf.Exp(-((x + 0.3f) * (x + 0.3f) + (y - 0.35f) * (y - 0.35f)) / 0.08f);
            return new Color(v, v, v, Mathf.Clamp01((0.96f - r) * 24f));
        });
        /// <summary>Gema facetada (losango): coroa clara, pavilhao escuro, mesa central e cintura brilhante. A cor vem do tint.</summary>
        public static Sprite Gem() => Soft("gem", 64, (x, y) =>
        {
            float d = Mathf.Abs(x) + Mathf.Abs(y);
            float v = 0.55f + (y > 0.12f ? 0.25f : 0f) + (x < 0f ? 0.08f : 0f) - (y < -0.5f ? 0.15f : 0f);
            if (y > 0.12f && y < 0.62f && Mathf.Abs(x) < 0.3f) v = 1f;          // mesa
            if (Mathf.Abs(y - 0.12f) < 0.035f) v = 1f;                            // cintura
            return new Color(v, v, v, Mathf.Clamp01((0.98f - d) * 24f));
        });

        const int SlabSide = 48; const float SlabBorder = 16f;
        /// <summary>Placa de cantos redondos com degrade vertical (clara em cima). 9-slice para uGUI; escalada para celulas.</summary>
        public static Sprite Slab() => Soft("slab", SlabSide, (x, y) =>
        {
            float v = Mathf.Lerp(0.78f, 1f, (y + 1f) * 0.5f);
            return new Color(v, v, v, Mathf.Clamp01(-RoundRect(x, y, SlabBorder / (SlabSide * 0.5f)) * SlabSide * 0.5f + 0.5f));
        }, SlabBorder);
        /// <summary>Moldura (so' a borda) da placa: aro da peca tocavel e foco de botao.</summary>
        public static Sprite SlabFrame() => Soft("slabframe", 96, (x, y) =>
        {
            float d = RoundRect(x, y, 0.3f);
            float a = Mathf.Clamp01(-d * 48f + 0.5f) - Mathf.Clamp01(-(d + 0.1f) * 48f + 0.5f);
            return new Color(1f, 1f, 1f, a);
        });

        static Sprite Soft(string key, int side, Func<float, float, Color> f, float border = 0f)
        {
            if (Cache.TryGetValue(key, out Sprite s) && s != null) return s;
            var tex = new Texture2D(side, side, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var px = new Color32[side * side];
            for (int y = 0; y < side; y++)
                for (int x = 0; x < side; x++)
                    px[y * side + x] = f((x + 0.5f) / side * 2f - 1f, (y + 0.5f) / side * 2f - 1f);
            tex.SetPixels32(px);
            tex.Apply();
            var r = new Rect(0, 0, side, side);
            s = border > 0f
                ? Sprite.Create(tex, r, new Vector2(0.5f, 0.5f), side, 0, SpriteMeshType.FullRect, new Vector4(border, border, border, border))
                : Sprite.Create(tex, r, new Vector2(0.5f, 0.5f), side);
            Cache[key] = s;
            return s;
        }

        public static SpriteRenderer NewSprite(Transform parent, string name, Sprite sprite, Color color, int order,
            Vector2 pos, Vector2 scale, float angle = 0f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            go.transform.localRotation = Quaternion.Euler(0, 0, angle);
            var r = go.AddComponent<SpriteRenderer>();
            r.sprite = sprite;
            r.color = color;
            r.sortingOrder = order;
            return r;
        }

        // ---------- uGUI por codigo ----------
        static Font _font;
        public static Font Font() => _font != null ? _font : (_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        public static RectTransform Node(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            var rt = (RectTransform)new GameObject(name, typeof(RectTransform)).transform;
            rt.SetParent(parent, false);
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static Text NewText(Transform parent, string name, int size, Vector2 anchorMin, Vector2 anchorMax,
            TextAnchor align = TextAnchor.MiddleCenter, bool bold = false)
        {
            var t = Node(parent, name, anchorMin, anchorMax).gameObject.AddComponent<Text>();
            t.font = Font();
            t.fontSize = size;
            t.color = Ink;
            t.alignment = align;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            var sh = t.gameObject.AddComponent<Shadow>();
            sh.effectColor = new Color(0f, 0f, 0f, 0.7f);
            sh.effectDistance = new Vector2(0f, -3f);
            return t;
        }

        /// <summary>Icone uGUI (sem raycast, aspecto preservado).</summary>
        public static Image Icon(Transform parent, string name, Sprite sprite, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var img = Node(parent, name, anchorMin, anchorMax).gameObject.AddComponent<Image>();
            img.sprite = sprite;
            img.color = color;
            img.preserveAspect = true;
            img.raycastTarget = false;
            return img;
        }

        /// <summary>Placa 9-slice de cantos redondos com `radiusPx` de raio no canvas (1080x1920 de referencia).</summary>
        public static Image Panel(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax, float radiusPx = 36f)
        {
            var img = Node(parent, name, anchorMin, anchorMax).gameObject.AddComponent<Image>();
            Sprite s = Slab();
            img.sprite = s;
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = SlabBorder * 100f / (s.pixelsPerUnit * Mathf.Max(radiusPx, 1f));
            img.color = color;
            return img;
        }

        /// <summary>Botao com estados (normal / pressionado / desabilitado por tint E alfa do rotulo). `primary` = rotulo escuro sobre dourado.</summary>
        public static Button NewButton(Transform parent, string label, int size, Color color, Vector2 anchorMin, Vector2 anchorMax,
            Action onClick, bool primary = true)
        {
            Image img = Panel(parent, "Btn " + label, color, anchorMin, anchorMax);
            Image shade = Panel(img.transform, "Sombra", new Color(0f, 0f, 0f, 0.35f), Vector2.zero, Vector2.one);
            shade.rectTransform.offsetMin = new Vector2(0f, -8f);
            shade.rectTransform.offsetMax = new Vector2(0f, -8f);
            shade.transform.SetAsFirstSibling();
            shade.raycastTarget = false;
            var b = img.gameObject.AddComponent<Button>();
            b.targetGraphic = img;
            ColorBlock cb = b.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
            cb.pressedColor = new Color(0.72f, 0.72f, 0.72f, 1f);
            cb.selectedColor = Color.white;
            cb.disabledColor = new Color(0.45f, 0.45f, 0.5f, 0.6f);
            cb.fadeDuration = 0.06f;
            b.colors = cb;
            b.onClick.AddListener(() => onClick());
            Text t = NewText(img.transform, "Label", size, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter, true);
            t.text = label;
            t.color = primary ? Rock : Ink;
            return b;
        }

        /// <summary>Habilita/desabilita com cara de desabilitado: tint do fundo (ColorBlock) + rotulo esmaecido.</summary>
        public static void SetEnabled(Button b, bool on)
        {
            b.interactable = on;
            Text t = b.GetComponentInChildren<Text>();
            if (t != null) t.color = ComAlfa(t.color, on ? 1f : 0.35f);
        }

        public static string Label(Button b) => b.GetComponentInChildren<Text>().text;
        public static void SetLabel(Button b, string s) => b.GetComponentInChildren<Text>().text = s;
    }
}
