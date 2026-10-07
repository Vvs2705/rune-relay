using System.Collections.Generic;
using RR.Core;
using UnityEngine;

namespace RR
{
    /// <summary>
    /// Desenha o Board com sprites gerados e toca o "juice" da cascata. O que gira (canal, alternador, fonte,
    /// comporta) e' montado na geometria do giro 0 dentro de `Spin`; cada giro vira -90 graus (tween). Nao guarda
    /// regra nenhuma: le o Board na fase de planejamento e o FlowSim durante a liberacao (OnTick a cada passo,
    /// ShowSim a cada quadro). Ordem de desenho: placa 0-2, bracos 3-7, pecas 8-12, orbes 18-20, ondas 30, falha 35.
    /// </summary>
    public sealed class BoardView : MonoBehaviour
    {
        sealed class Cell
        {
            public Tile Tile;
            public Transform Root, Spin;
            public SpriteRenderer Rim;
            public readonly List<SpriteRenderer> Cores = new List<SpriteRenderer>();
            public readonly List<SpriteRenderer> Glows = new List<SpriteRenderer>();
            public readonly List<int> CoreSide = new List<int>();    // lado (geometria do giro 0) de cada nucleo; -1 = centro
            public readonly List<SpriteRenderer> Pips = new List<SpriteRenderer>();
            public readonly List<GameObject> Queue = new List<GameObject>();
            public SpriteRenderer Arrow, ArrowHalo, GateL, GateR, Lock, Gem, Halo, Flash;
            public float Angle, Target, Punch, ArrowT, GateT, GemScale, GemTarget, FlashT, Phase;
            public int ArrowShown, Shown;                               // Shown: cristal = orbes mostrados; fonte = emitidos
            public bool GateOpenShown, FullShown;
        }

        sealed class OrbVis
        {
            public Transform Root;
            public SpriteRenderer Glow, Body, Glyph;
            public SpriteRenderer[] Trail;
            public int Color = -1;
        }

        sealed class Wave { public SpriteRenderer R; public float T = 2f, Size; }

        const float ArmRim = 0.30f, ArmGroove = 0.22f, ArmCore = 0.11f;
        const float GemMin = 0.30f, GemMax = 0.62f;

        Board _b;
        Cell[] _cells;
        readonly List<OrbVis> _orbs = new List<OrbVis>();
        readonly List<Wave> _ripples = new List<Wave>();
        SpriteRenderer _fail;
        float _failT, _shake, _bounce;
        bool _planning = true;

        public Vector3 CellPos(int x, int y) => new Vector3(x - (_b.W - 1) * 0.5f, (_b.H - 1) * 0.5f - y, 0f);
        public Vector3 CellWorld(int x, int y) => transform.TransformPoint(CellPos(x, y));

        public bool CellAt(Vector3 world, out int x, out int y)
        {
            Vector3 l = transform.InverseTransformPoint(world);
            x = Mathf.RoundToInt(l.x + (_b.W - 1) * 0.5f);
            y = Mathf.RoundToInt((_b.H - 1) * 0.5f - l.y);
            return _b != null && _b.In(x, y);
        }

        public void Build(Board b)
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
            _orbs.Clear();
            _ripples.Clear();
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            _shake = _bounce = 0f;
            _b = b;
            SpriteRenderer plate = Art.NewSprite(transform, "Placa", Art.Slab(), Art.ComAlfa(Art.CellEdge, 0.75f), -5, Vector2.zero, Vector2.one);
            plate.drawMode = SpriteDrawMode.Sliced;   // cantos no tamanho da celula, sem esticar
            plate.size = new Vector2(b.W + 0.5f, b.H + 0.5f);
            _cells = new Cell[b.Cells.Length];
            for (int y = 0; y < b.H; y++)
                for (int x = 0; x < b.W; x++)
                    _cells[y * b.W + x] = BuildCell(x, y);
            _fail = Art.NewSprite(transform, "Falha", Art.Cross(), Art.Bad, 35, Vector2.zero, Vector2.one * 0.9f);
            _fail.enabled = false;
        }

        Cell BuildCell(int x, int y)
        {
            Tile t = _b.At(x, y);
            var c = new Cell { Tile = t, Root = new GameObject($"{x},{y} {t.Kind}").transform, Phase = (x * 7 + y * 3) * 0.6f };
            c.Root.SetParent(transform, false);
            c.Root.localPosition = CellPos(x, y);
            if (t.Kind == Kind.Empty)
            {
                Art.NewSprite(c.Root, "Marca", Art.Disc(), Art.ComAlfa(Art.Ink, 0.08f), 0, Vector2.zero, Vector2.one * 0.06f);
                return c;
            }
            Color face = t.Kind == Kind.Block ? Art.Rock : t.Tappable ? Art.CellTap : Art.Cell;
            Art.NewSprite(c.Root, "Borda", Art.Slab(), Art.CellEdge, 0, new Vector2(0f, -0.03f), Vector2.one * 0.96f);
            Art.NewSprite(c.Root, "Face", Art.Slab(), face, 1, Vector2.zero, Vector2.one * 0.94f);
            if (t.Kind == Kind.Block)
            {
                Art.NewSprite(c.Root, "Xis", Art.Cross(), Art.ComAlfa(Art.PipeEdge, 0.8f), 2, Vector2.zero, Vector2.one * 0.34f);
                return c;
            }
            if (t.Tappable) c.Rim = Art.NewSprite(c.Root, "Aro", Art.SlabFrame(), Art.ComAlfa(Art.Accent, 0.6f), 2, Vector2.zero, Vector2.one * 0.94f);

            c.Spin = new GameObject("Spin").transform;
            c.Spin.SetParent(c.Root, false);
            c.Angle = c.Target = -90f * t.Rot;
            c.Spin.localRotation = Quaternion.Euler(0, 0, c.Angle);
            switch (t.Kind)
            {
                case Kind.Straight: Arms(c, 0, 2); break;
                case Kind.Curve: Arms(c, 0, 1); break;
                case Kind.Cross:
                    Arms(c, 0, 1, 2, 3);
                    Art.NewSprite(c.Spin, "Ponte", Art.Ring(0.72f), Art.ComAlfa(Art.Ink, 0.55f), 8, Vector2.zero, Vector2.one * 0.34f);
                    break;
                case Kind.Gate:
                    {
                        Arms(c, 0, 2);
                        Color linked = Art.Palette[_b.Cells[_b.Crystals[t.Link]].Color];
                        c.GateL = Art.NewSprite(c.Spin, "BarraE", Art.Square(), linked, 9, new Vector2(-0.19f, 0f), new Vector2(0.36f, 0.13f));
                        c.GateR = Art.NewSprite(c.Spin, "BarraD", Art.Square(), linked, 9, new Vector2(0.19f, 0f), new Vector2(0.36f, 0.13f));
                        c.Lock = Art.NewSprite(c.Spin, "Cadeado", Art.Lock(), linked, 10, Vector2.zero, Vector2.one * 0.34f);
                        Art.NewSprite(c.Lock.transform, "Forma", Art.Shape(_b.Cells[_b.Crystals[t.Link]].Color), Art.Rock, 11, new Vector2(0f, -0.4f), Vector2.one * 0.4f);
                        break;
                    }
                case Kind.Switch:
                    Arms(c, 2, 3, 1);
                    c.ArrowHalo = Art.NewSprite(c.Spin, "Brilho", Art.Glow(), Art.ComAlfa(Art.Accent, 0.55f), 9, Vector2.zero, Vector2.one * 0.5f);
                    c.Arrow = Art.NewSprite(c.Spin, "Seta", Art.Arrow(), Art.Ink, 10, Vector2.zero, Vector2.one * 0.3f);
                    c.ArrowT = c.ArrowShown = t.Arrow;
                    PlaceArrow(c);
                    break;
                case Kind.Source:
                    {
                        Arms(c, 0);
                        Art.NewSprite(c.Root, "Poco", Art.Disc(), Art.Rock, 8, Vector2.zero, Vector2.one * 0.74f);
                        Art.NewSprite(c.Root, "Aro", Art.Ring(0.84f), Art.ComAlfa(Art.Ink, 0.9f), 9, Vector2.zero, Vector2.one * 0.78f);
                        string q = _b.Queues[t.Index];
                        for (int i = 0; i < q.Length; i++)
                        {
                            int col = q[i] - 'a', row = i / 3, column = i % 3, perRow = Mathf.Min(3, q.Length - row * 3);
                            var go = new GameObject("Fila" + i);
                            go.transform.SetParent(c.Root, false);
                            go.transform.localPosition = new Vector2((column - (perRow - 1) * 0.5f) * 0.2f, (q.Length > 3 ? 0.1f : 0f) - row * 0.21f);
                            Art.NewSprite(go.transform, "Brilho", Art.Glow(), Art.ComAlfa(Art.Palette[col], 0.5f), 9, Vector2.zero, Vector2.one * 0.3f);
                            Art.NewSprite(go.transform, "Orbe", Art.Shape(col), Art.Palette[col], 10, Vector2.zero, Vector2.one * 0.16f);
                            c.Queue.Add(go);
                        }
                        break;
                    }
                case Kind.Crystal:
                    {
                        Color cc = Art.Palette[t.Color];
                        c.Halo = Art.NewSprite(c.Root, "Halo", Art.Halo(), cc, 6, Vector2.zero, Vector2.one * 1.4f);
                        c.Halo.enabled = false;
                        Art.NewSprite(c.Root, "Interior", Art.Diamond(), Art.Darken(cc, 0.78f), 7, Vector2.zero, Vector2.one * 0.82f);
                        Art.NewSprite(c.Root, "Moldura", Art.DiamondFrame(), cc, 8, Vector2.zero, Vector2.one * 0.9f);
                        c.GemScale = c.GemTarget = GemMin;
                        c.Gem = Art.NewSprite(c.Root, "Gema", Art.Gem(), cc, 9, new Vector2(0f, 0.02f), Vector2.one * GemMin);
                        Art.NewSprite(c.Gem.transform, "Forma", Art.Shape(t.Color), Art.ComAlfa(Color.white, 0.85f), 10, new Vector2(0f, 0.05f), Vector2.one * 0.3f);
                        c.Flash = Art.NewSprite(c.Root, "Flash", Art.Diamond(), Color.white, 12, Vector2.zero, Vector2.one * 0.85f);
                        c.Flash.enabled = false;
                        float step = Mathf.Min(0.15f, 0.72f / t.Cap);
                        for (int i = 0; i < t.Cap; i++)
                            c.Pips.Add(Art.NewSprite(c.Root, "Pip", Art.Disc(), Art.PipeEdge, 9, new Vector2((i - (t.Cap - 1) * 0.5f) * step, -0.36f), Vector2.one * 0.09f));
                        break;
                    }
            }
            return c;
        }

        /// <summary>Bracos do centro ate' os lados (geometria do giro 0; N = +y): brilho, aro, calha e nucleo; depois o no' central.</summary>
        static void Arms(Cell c, params int[] sides)
        {
            foreach (int s in sides)
            {
                var dir = new Vector2(Dir.DX[s], -Dir.DY[s]);
                bool vertical = s % 2 == 0;
                c.Glows.Add(Art.NewSprite(c.Spin, "Brilho", Art.Glow(), Art.ComAlfa(Art.Pipe, 0f), 3, dir * 0.25f, vertical ? new Vector2(0.55f, 0.8f) : new Vector2(0.8f, 0.55f)));
                Art.NewSprite(c.Spin, "AroBraco", Art.Square(), Art.PipeEdge, 4, dir * 0.25f, vertical ? new Vector2(ArmRim, 0.5f) : new Vector2(0.5f, ArmRim));
                Art.NewSprite(c.Spin, "Calha", Art.Square(), Art.PipeDark, 5, dir * 0.25f, vertical ? new Vector2(ArmGroove, 0.5f) : new Vector2(0.5f, ArmGroove));
                c.Cores.Add(Art.NewSprite(c.Spin, "Nucleo", Art.Square(), Art.Pipe, 6, dir * 0.25f, vertical ? new Vector2(ArmCore, 0.5f) : new Vector2(0.5f, ArmCore)));
                c.CoreSide.Add(s);
            }
            c.Glows.Add(Art.NewSprite(c.Spin, "BrilhoNo", Art.Glow(), Art.ComAlfa(Art.Pipe, 0f), 3, Vector2.zero, Vector2.one * 0.6f));
            Art.NewSprite(c.Spin, "AroNo", Art.Disc(), Art.PipeEdge, 4, Vector2.zero, Vector2.one * ArmRim);
            Art.NewSprite(c.Spin, "CalhaNo", Art.Disc(), Art.PipeDark, 5, Vector2.zero, Vector2.one * ArmGroove);
            c.Cores.Add(Art.NewSprite(c.Spin, "NucleoNo", Art.Disc(), Art.Pipe, 7, Vector2.zero, Vector2.one * ArmCore));
            c.CoreSide.Add(-1);
        }

        /// <summary>Cor do nucleo e do brilho de cada braco: a cor do fluxo (ou apagado). No alternador, a saida inativa fica esmaecida.</summary>
        static void Colorize(Cell c, int lit, Tile tile)
        {
            Color core = lit >= 0 ? Art.Palette[lit] : Art.Pipe;
            for (int i = 0; i < c.Cores.Count; i++)
            {
                bool active = true;
                if (tile.Kind == Kind.Switch && c.CoreSide[i] >= 0)
                    active = ((c.CoreSide[i] + tile.Rot) & 3) != tile.SwitchOut(1 - tile.Arrow);
                c.Cores[i].color = active ? core : Art.ComAlfa(Art.Pipe, 0.35f);
                c.Glows[i].color = Art.ComAlfa(core, lit >= 0 && active ? 0.32f : 0f);
            }
        }

        static void PlaceArrow(Cell c)
        {
            float x = Mathf.Lerp(-0.27f, 0.27f, c.ArrowT);
            c.Arrow.transform.localPosition = new Vector2(x, 0f);
            c.Arrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(180f, 0f, c.ArrowT));
            c.ArrowHalo.transform.localPosition = new Vector2(x, 0f);
        }

        void SetFill(Cell c, int filled, bool snap)
        {
            c.GemTarget = Mathf.Lerp(GemMin, GemMax, c.Pips.Count == 0 ? 1f : filled / (float)c.Pips.Count);
            if (snap) { c.GemScale = c.GemTarget; c.Gem.transform.localScale = Vector3.one * c.GemScale; }
            for (int k = 0; k < c.Pips.Count; k++)
            {
                bool on = k < filled;
                c.Pips[k].color = on ? Art.Palette[c.Tile.Color] : Art.PipeEdge;
                c.Pips[k].transform.localScale = Vector3.one * (on ? 0.12f : 0.09f);
            }
        }

        /// <summary>Feedback do toque (o Board ja mudou): canal gira 90 graus no sentido horario, alternador troca a seta.</summary>
        public void OnTap(int x, int y, Tile now)
        {
            Cell c = _cells[y * _b.W + x];
            c.Tile = now;
            if (now.Kind == Kind.Switch) { c.ArrowShown = now.Arrow; Sfx.Play("flip"); }
            else c.Target -= 90f;
            c.Punch = 1f;
            Ripple(CellPos(x, y), Art.Accent, 1.1f);
        }

        public void ShowPlanning(Board b)
        {
            _planning = true;
            int[] lit = FlowSim.Lit(b);
            for (int i = 0; i < _cells.Length; i++)
            {
                Cell c = _cells[i];
                c.Tile = b.Cells[i];
                if (c.Spin == null) continue;
                Colorize(c, lit[i], c.Tile);
                if (c.Arrow != null) { c.ArrowT = c.ArrowShown = c.Tile.Arrow; PlaceArrow(c); }
                if (c.Lock != null) { c.GateT = 0f; c.GateOpenShown = false; PlaceGate(c); }
                foreach (GameObject q in c.Queue) q.SetActive(true);
                if (c.Gem != null)
                {
                    SetFill(c, 0, true);
                    c.Halo.enabled = false;
                    c.Flash.enabled = false;
                    c.FullShown = false;
                }
                c.Shown = 0;
            }
            foreach (OrbVis o in _orbs) o.Root.gameObject.SetActive(false);
            _fail.enabled = false;
        }

        /// <summary>Um passo da simulacao acabou de rodar: dispara o que muda de estado (seta, comporta, cristal, fonte).</summary>
        public void OnTick(FlowSim sim, FlowSim.Orb[] prev)
        {
            _planning = false;
            for (int i = 0; i < _cells.Length; i++)
            {
                Cell c = _cells[i];
                Tile tile = sim.B.Cells[i];
                switch (tile.Kind)
                {
                    case Kind.Switch:
                        if (tile.Arrow != c.ArrowShown) { c.ArrowShown = tile.Arrow; c.Punch = 0.6f; Sfx.Play("flip", 1f, 0.35f); }
                        break;
                    case Kind.Gate:
                        {
                            bool open = sim.GateOpen(tile);
                            if (open && !c.GateOpenShown)
                            {
                                c.GateOpenShown = true;
                                c.Punch = 1f;
                                Color linked = Art.Palette[sim.B.Cells[sim.B.Crystals[tile.Link]].Color];
                                Fx.Burst(CellWorld(i % _b.W, i / _b.W), linked, 16, 2f, 0.18f);
                                Ripple(CellPos(i % _b.W, i / _b.W), linked, 1.2f);
                                Sfx.Play("gate");
                            }
                            break;
                        }
                    case Kind.Crystal:
                        {
                            int filled = sim.Filled[tile.Index];
                            if (filled > c.Shown)
                            {
                                c.Shown = filled;
                                SetFill(c, filled, false);
                                c.Punch = 1f;
                                Color cc = Art.Palette[tile.Color];
                                Ripple(CellPos(i % _b.W, i / _b.W), cc, 0.9f);
                                Fx.Burst(CellWorld(i % _b.W, i / _b.W), cc, 8, 1.6f, 0.16f);
                                if (sim.Full(tile.Index) && !c.FullShown)
                                {
                                    c.FullShown = true;
                                    c.FlashT = 1f;
                                    c.Flash.enabled = true;
                                    c.Halo.enabled = true;
                                    c.Punch = 1.5f;
                                    Fx.Burst(CellWorld(i % _b.W, i / _b.W), cc, 26, 2.6f, 0.22f);
                                }
                            }
                            break;
                        }
                    case Kind.Source:
                        {
                            int em = sim.Emitted(tile.Index);
                            for (int k = 0; k < c.Queue.Count; k++) c.Queue[k].SetActive(k >= em);
                            if (em > c.Shown)
                            {
                                c.Shown = em;
                                c.Punch = 0.5f;
                                Ripple(CellPos(i % _b.W, i / _b.W), Art.Palette[sim.B.Queues[tile.Index][em - 1] - 'a'], 0.8f);
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>Quadro da liberacao: `t` (0..1) interpola os orbes entre o tick anterior (`prev`) e o atual.</summary>
        public void ShowSim(FlowSim sim, FlowSim.Orb[] prev, float t)
        {
            while (_orbs.Count < sim.Orbs.Count) _orbs.Add(NewOrb());
            for (int i = 0; i < _orbs.Count; i++)
            {
                OrbVis v = _orbs[i];
                if (i >= sim.Orbs.Count) { v.Root.gameObject.SetActive(false); continue; }
                FlowSim.Orb o = sim.Orbs[i];
                bool had = i < prev.Length;
                bool show = o.Alive || (had && prev[i].Alive);
                v.Root.gameObject.SetActive(show);
                if (!show) continue;
                if (v.Color != o.Color)
                {
                    v.Color = o.Color;
                    Color c = Art.Palette[o.Color];
                    v.Glow.color = Art.ComAlfa(c, 0.6f);
                    v.Body.color = Art.Lighten(c, 0.12f);
                    v.Glyph.color = Art.ComAlfa(Color.white, 0.9f);
                    v.Glyph.sprite = Art.Shape(o.Color);
                    for (int k = 0; k < v.Trail.Length; k++) v.Trail[k].color = Art.ComAlfa(c, 0.32f - 0.09f * k);
                }
                Vector3 from = had ? CellPos(prev[i].X, prev[i].Y) : CellPos(o.X, o.Y), to = CellPos(o.X, o.Y);
                v.Root.localPosition = Vector3.Lerp(from, to, t);
                float s = !had ? Ease.OutBack(t) : o.Alive ? 1f : 1f - Ease.OutCubic(t);
                v.Root.localScale = Vector3.one * s;
                v.Glow.transform.localScale = Vector3.one * (0.62f + 0.06f * Mathf.Sin(Time.time * 9f + i));
                for (int k = 0; k < v.Trail.Length; k++)
                {
                    float lag = t - 0.17f * (k + 1);
                    v.Trail[k].transform.localPosition = Vector3.Lerp(from, to, Mathf.Max(0f, lag)) - v.Root.localPosition;
                    v.Trail[k].enabled = had && o.Alive && from != to;
                }
            }
        }

        OrbVis NewOrb()
        {
            var v = new OrbVis { Root = new GameObject("Orbe").transform, Trail = new SpriteRenderer[3] };
            v.Root.SetParent(transform, false);
            for (int k = 0; k < 3; k++)
                v.Trail[k] = Art.NewSprite(v.Root, "Rastro", Art.Glow(), Color.white, 18, Vector2.zero, Vector2.one * (0.42f - 0.1f * k));
            v.Glow = Art.NewSprite(v.Root, "Brilho", Art.Glow(), Color.white, 19, Vector2.zero, Vector2.one * 0.62f);
            v.Body = Art.NewSprite(v.Root, "Corpo", Art.Orb(), Color.white, 20, Vector2.zero, Vector2.one * 0.3f);
            v.Glyph = Art.NewSprite(v.Root, "Forma", Art.Disc(), Color.white, 21, Vector2.zero, Vector2.one * 0.13f);
            v.Root.gameObject.SetActive(false);
            return v;
        }

        void PlaceGate(Cell c)
        {
            float t = Ease.OutCubic(c.GateT);
            c.GateL.transform.localPosition = new Vector2(-0.19f - 0.3f * t, 0f);
            c.GateR.transform.localPosition = new Vector2(0.19f + 0.3f * t, 0f);
            Color col = Art.ComAlfa(c.GateL.color, 1f - t);
            c.GateL.color = c.GateR.color = col;
            c.Lock.color = col;
            c.Lock.transform.localScale = Vector3.one * (0.34f * (1f + 0.7f * t));
            bool visible = c.GateT < 1f;
            c.GateL.enabled = c.GateR.enabled = visible;
            c.Lock.gameObject.SetActive(visible);
        }

        public void Ripple(Vector3 pos, Color color, float size)
        {
            Wave r = null;
            foreach (Wave k in _ripples) if (k.T >= 1f) { r = k; break; }
            if (r == null)
            {
                r = new Wave { R = Art.NewSprite(transform, "Onda", Art.Ring(0.82f), color, 30, Vector2.zero, Vector2.one) };
                _ripples.Add(r);
            }
            r.T = 0f;
            r.Size = size;
            r.R.color = color;
            r.R.transform.localPosition = pos;
            r.R.enabled = true;
        }

        public void ShowFail(int x, int y)
        {
            _shake = 1f;
            Fx.Haptic();
            if (!_b.In(x, y)) return;
            _fail.transform.localPosition = CellPos(x, y);
            _fail.color = Art.Bad;
            _fail.enabled = true;
            _failT = 1f;
            _cells[y * _b.W + x].Punch = 1.5f;
            Ripple(CellPos(x, y), Art.Bad, 1.3f);
            Fx.Burst(CellWorld(x, y), Art.Bad, 18, 2.2f, 0.2f);
        }

        public void Celebrate()
        {
            _bounce = 1f;
            Fx.Haptic();
            for (int i = 0; i < _cells.Length; i++)
            {
                Cell c = _cells[i];
                if (c.Gem == null) continue;
                c.Punch = 1.5f;
                Fx.Burst(CellWorld(i % _b.W, i / _b.W), Art.Palette[c.Tile.Color], 30, 3.2f, 0.24f);
            }
        }

        void Update()
        {
            if (_cells == null) return;
            float dt = Time.deltaTime, now = Time.time;
            foreach (Cell c in _cells)
            {
                if (c.Spin == null) continue;
                if (c.Angle != c.Target)
                {
                    c.Angle = Mathf.MoveTowards(c.Angle, c.Target, 900f * dt);
                    c.Spin.localRotation = Quaternion.Euler(0, 0, c.Angle);
                }
                if (c.Punch > 0f)
                {
                    c.Punch = Mathf.Max(0f, c.Punch - dt * 5f);
                    c.Root.localScale = Vector3.one * (1f + 0.12f * c.Punch);
                }
                if (c.Rim != null)
                    c.Rim.color = Art.ComAlfa(Art.Accent, _planning ? 0.35f + 0.4f * (0.5f + 0.5f * Mathf.Sin(now * 2.6f + c.Phase)) : 0.12f);
                if (c.Arrow != null && c.ArrowT != c.ArrowShown)
                {
                    c.ArrowT = Mathf.MoveTowards(c.ArrowT, c.ArrowShown, dt * 5f);
                    PlaceArrow(c);
                }
                if (c.Lock != null && c.GateOpenShown && c.GateT < 1f)
                {
                    c.GateT = Mathf.Min(1f, c.GateT + dt * 2.5f);
                    PlaceGate(c);
                }
                if (c.Gem != null)
                {
                    if (c.GemScale != c.GemTarget)
                    {
                        c.GemScale = Mathf.MoveTowards(c.GemScale, c.GemTarget, dt * 1.2f);
                        c.Gem.transform.localScale = Vector3.one * c.GemScale;
                    }
                    if (c.FlashT > 0f)
                    {
                        c.FlashT = Mathf.Max(0f, c.FlashT - dt * 3f);
                        c.Flash.color = Art.ComAlfa(Color.white, c.FlashT * 0.9f);
                        c.Flash.enabled = c.FlashT > 0f;
                    }
                    if (c.Halo.enabled) c.Halo.color = Art.ComAlfa(Art.Palette[c.Tile.Color], 0.6f + 0.3f * Mathf.Sin(now * 4f));
                }
                for (int k = 0; k < c.Queue.Count; k++)
                {
                    if (!c.Queue[k].activeSelf) continue;
                    c.Queue[k].transform.localScale = Vector3.one * (1f + 0.14f * (0.5f + 0.5f * Mathf.Sin(now * 5f)));
                    for (int j = k + 1; j < c.Queue.Count; j++) c.Queue[j].transform.localScale = Vector3.one;
                    break;
                }
            }
            foreach (Wave r in _ripples)
            {
                if (r.T >= 1f) { if (r.R.enabled) r.R.enabled = false; continue; }
                r.T = Mathf.Min(1f, r.T + dt / 0.45f);
                r.R.transform.localScale = Vector3.one * (r.Size * Mathf.Lerp(0.25f, 1.15f, Ease.OutCubic(r.T)));
                r.R.color = Art.ComAlfa(r.R.color, (1f - r.T) * 0.9f);
            }
            if (_fail.enabled)
            {
                _failT -= dt * 0.8f;
                _fail.color = Art.ComAlfa(Art.Bad, Mathf.Clamp01(_failT + 0.3f));
                _fail.transform.localScale = Vector3.one * (0.9f + 0.15f * Mathf.Sin(now * 12f));
            }
            if (_shake > 0f)
            {
                _shake = Mathf.Max(0f, _shake - dt / 0.45f);
                Vector2 off = Random.insideUnitCircle * 0.09f * _shake;
                transform.localPosition = new Vector3(off.x, off.y, 0f);
            }
            if (_bounce > 0f)
            {
                _bounce = Mathf.Max(0f, _bounce - dt / 0.6f);
                transform.localScale = Vector3.one * (1f + 0.04f * Ease.Punch(1f - _bounce));
            }
        }
    }
}
