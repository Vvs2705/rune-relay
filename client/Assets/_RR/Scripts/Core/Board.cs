using System;
using System.Collections.Generic;

namespace RR.Core
{
    /// <summary>Lados/direcoes: 0=N, 1=E, 2=S, 3=W. Linha 0 e' a de cima (N = y-1).</summary>
    public static class Dir
    {
        public static readonly int[] DX = { 0, 1, 0, -1 };
        public static readonly int[] DY = { -1, 0, 1, 0 };
        public static int Opp(int d) => (d + 2) & 3;
    }

    public enum Kind { Empty, Block, Straight, Curve, Cross, Gate, Switch, Source, Crystal }

    /// <summary>
    /// Uma celula. Rot: Straight/Gate 0=N-S 1=E-W; Curve 0=N+E 1=E+S 2=S+W 3=W+N; Source = lado de saida;
    /// Switch = orbe entra pelo lado Opp(Rot) (rot 0: entra por S, viajando para N) e sai a esquerda (Arrow 0) ou direita (1).
    /// </summary>
    public struct Tile
    {
        public Kind Kind;
        public int Rot;
        public bool Rotatable;  // Straight/Curve; Switch sempre aceita toque (troca a seta), nunca gira
        public int Arrow;       // Switch: seta inicial
        public int Color;       // Crystal: 0..5
        public int Cap;         // Crystal: capacidade
        public int Link;        // Gate: indice do cristal que a abre
        public int Index;       // Source/Crystal: ordem de leitura

        public int States => Kind == Kind.Straight ? 2 : Kind == Kind.Curve ? 4 : Kind == Kind.Switch ? 2 : 1;
        public bool Tappable => Kind == Kind.Switch || (Rotatable && States > 1);

        /// <summary>Estado de toque (0..States-1): o giro, ou a seta do alternador.</summary>
        public int State
        {
            get => Kind == Kind.Switch ? Arrow : Rot;
            set { if (Kind == Kind.Switch) Arrow = value; else Rot = value; }
        }

        public int SwitchIn => Dir.Opp(Rot);
        public int SwitchOut(int arrow) => arrow == 0 ? (Rot + 3) & 3 : (Rot + 1) & 3;

        /// <summary>Lados abertos (bitmask N=1 E=2 S=4 W=8) para desenhar.</summary>
        public int Sides
        {
            get
            {
                switch (Kind)
                {
                    case Kind.Straight: case Kind.Gate: return Rot == 0 ? 0b0101 : 0b1010;
                    case Kind.Curve: return (1 << Rot) | (1 << ((Rot + 1) & 3));
                    case Kind.Cross: return 0b1111;
                    case Kind.Switch: return (1 << SwitchIn) | (1 << SwitchOut(0)) | (1 << SwitchOut(1));
                    case Kind.Source: return 1 << Rot;
                    default: return 0;
                }
            }
        }

        /// <summary>Para onde vai um orbe que entra pelo lado `entry`. false = nao passa (derrama).
        /// Comporta: so' a geometria; quem confere se esta aberta e' o FlowSim.</summary>
        public bool Route(int entry, int arrow, out int exit)
        {
            exit = -1;
            switch (Kind)
            {
                case Kind.Straight:
                case Kind.Gate:
                    if ((Sides & (1 << entry)) == 0) return false;
                    exit = Dir.Opp(entry); return true;
                case Kind.Curve:
                    if (entry == Rot) { exit = (Rot + 1) & 3; return true; }
                    if (entry == ((Rot + 1) & 3)) { exit = Rot; return true; }
                    return false;
                case Kind.Cross:
                    exit = Dir.Opp(entry); return true;
                case Kind.Switch:
                    if (entry != SwitchIn) return false;
                    exit = SwitchOut(arrow); return true;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Tabuleiro + filas das fontes. Formato de nivel (texto, um arquivo por nivel):
    ///   // comentario   h dica ao jogador   q abab (fila da proxima fonte, em ordem de leitura; cores a..f)
    ///   linhas da grade de cima para baixo, tokens separados por espaco:
    ///   .  vazio   #  bloqueio   +  cruzamento
    ///   I0r / L2f  canal reto/curva, giro 0-3, r=giravel f=fixo
    ///   G10        comporta: giro 0/1, cristal (indice) que a abre
    ///   Y01        alternador: giro 0-3, seta inicial 0=esquerda 1=direita
    ///   S1         fonte saindo pelo lado 0-3
    ///   Ra3        cristal: cor a-f, capacidade 1-9
    /// </summary>
    public sealed class Board
    {
        public readonly int W, H;
        public readonly Tile[] Cells;
        public readonly List<string> Queues;
        public readonly List<int> Sources = new List<int>();   // indices de celula
        public readonly List<int> Crystals = new List<int>();
        public string Hint = "";                                // linha "h ...": dica ao jogador

        public Board(int w, int h, Tile[] cells, List<string> queues)
        {
            W = w; H = h; Cells = cells; Queues = queues;
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].Kind == Kind.Source) { cells[i].Index = Sources.Count; Sources.Add(i); }
                else if (cells[i].Kind == Kind.Crystal) { cells[i].Index = Crystals.Count; Crystals.Add(i); }
            }
        }

        public Board Clone() => new Board(W, H, (Tile[])Cells.Clone(), new List<string>(Queues));

        public bool In(int x, int y) => x >= 0 && y >= 0 && x < W && y < H;
        public ref Tile At(int x, int y) => ref Cells[y * W + x];

        /// <summary>Toque do jogador: gira o canal ou troca a seta do alternador.</summary>
        public bool Tap(int x, int y)
        {
            if (!In(x, y)) return false;
            ref Tile t = ref At(x, y);
            if (!t.Tappable) return false;
            t.State = (t.State + 1) % t.States;
            return true;
        }

        public static Board Parse(string text)
        {
            var rows = new List<string[]>();
            var queues = new List<string>();
            var lineNo = new List<int>();
            string hint = "";
            string[] lines = text.Replace("\r", "").Split('\n');
            for (int n = 0; n < lines.Length; n++)
            {
                string l = lines[n].Trim();
                if (l.Length == 0 || l.StartsWith("//")) continue;
                if (l.StartsWith("q ")) { queues.Add(l.Substring(2).Replace(" ", "")); continue; }
                if (l.StartsWith("h ")) { hint = l.Substring(2).Trim(); continue; }
                rows.Add(l.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
                lineNo.Add(n + 1);
            }
            if (rows.Count == 0) throw new FormatException("nivel sem grade");
            int w = rows[0].Length, h = rows.Count;
            var cells = new Tile[w * h];
            for (int y = 0; y < h; y++)
            {
                if (rows[y].Length != w) throw new FormatException($"linha {lineNo[y]}: {rows[y].Length} tokens, esperado {w}");
                for (int x = 0; x < w; x++)
                    cells[y * w + x] = ParseToken(rows[y][x], lineNo[y], x + 1);
            }
            var b = new Board(w, h, cells, queues) { Hint = hint };
            b.Validate();
            return b;
        }

        static Tile ParseToken(string s, int line, int col)
        {
            var t = new Tile();
            bool ok = true;
            switch (s[0])
            {
                case '.': t.Kind = Kind.Empty; ok = s.Length == 1; break;
                case '#': t.Kind = Kind.Block; ok = s.Length == 1; break;
                case '+': t.Kind = Kind.Cross; ok = s.Length == 1; break;
                case 'I':
                case 'L':
                    t.Kind = s[0] == 'I' ? Kind.Straight : Kind.Curve;
                    ok = s.Length == 3 && Digit(s[1], 0, t.Kind == Kind.Straight ? 1 : 3, out t.Rot) && (s[2] == 'r' || s[2] == 'f');
                    t.Rotatable = ok && s[2] == 'r';
                    break;
                case 'G':
                    t.Kind = Kind.Gate;
                    ok = s.Length == 3 && Digit(s[1], 0, 1, out t.Rot) && Digit(s[2], 0, 9, out t.Link);
                    break;
                case 'Y':
                    t.Kind = Kind.Switch;
                    ok = s.Length == 3 && Digit(s[1], 0, 3, out t.Rot) && Digit(s[2], 0, 1, out t.Arrow);
                    break;
                case 'S':
                    t.Kind = Kind.Source;
                    ok = s.Length == 2 && Digit(s[1], 0, 3, out t.Rot);
                    break;
                case 'R':
                    t.Kind = Kind.Crystal;
                    ok = s.Length == 3 && s[1] >= 'a' && s[1] <= 'f' && Digit(s[2], 1, 9, out t.Cap);
                    t.Color = ok ? s[1] - 'a' : 0;
                    break;
                default: ok = false; break;
            }
            if (!ok) throw new FormatException($"linha {line}, coluna {col}: token '{s}' invalido");
            return t;
        }

        static bool Digit(char c, int min, int max, out int v)
        {
            v = c - '0';
            return v >= min && v <= max;
        }

        void Validate()
        {
            if (Queues.Count != Sources.Count)
                throw new FormatException($"{Sources.Count} fontes e {Queues.Count} filas (q)");
            var orbs = new int[6];
            var caps = new int[6];
            foreach (string q in Queues)
                foreach (char c in q)
                {
                    if (c < 'a' || c > 'f') throw new FormatException($"fila '{q}': cor '{c}' invalida");
                    orbs[c - 'a']++;
                }
            foreach (int i in Crystals) caps[Cells[i].Color] += Cells[i].Cap;
            for (int c = 0; c < 6; c++)
                if (orbs[c] != caps[c])
                    throw new FormatException($"cor {(char)('a' + c)}: {orbs[c]} orbes para capacidade {caps[c]}");
            foreach (Tile t in Cells)
                if (t.Kind == Kind.Gate && t.Link >= Crystals.Count)
                    throw new FormatException($"comporta ligada ao cristal {t.Link}, mas ha {Crystals.Count}");
        }
    }
}
