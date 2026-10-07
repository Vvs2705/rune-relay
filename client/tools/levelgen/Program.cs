using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RR.Core;

namespace RR.LevelGen
{
    /// <summary>
    /// Gera niveis POR CONSTRUCAO (raia B §6): carva os caminhos ja resolvidos (fonte -> canais -> cristal), poe
    /// cruzamento onde dois caminhos se cruzam, alternador onde a fila deve se separar e comporta ligada a um cristal
    /// de outra fonte; calcula as filas simulando o destino de cada orbe; depois embaralha as pecas giraveis.
    /// So aceita nivel com SOLUCAO UNICA, conferida pelo mesmo Solver do jogo. Tudo e' sorteado a partir da semente,
    /// entao o mesmo arquivo sai sempre igual (reproduzivel). Nunca toca L001..L010 (feitos a mao).
    /// </summary>
    static class Program
    {
        sealed class Spec
        {
            public int N, W = 5, H = 8, Sources = 2, Colors = 2, MinQueue = 2, MaxQueue = 4, MinLen = 2, MaxLen = 6;
            public int Switches;          // fontes que recebem alternador
            public bool DoubleSwitch;     // uma delas recebe dois (3 cristais)
            public int Crosses, Gates, Rot, Decoys;
            public int MinTaps = 1, MaxTaps = 99;
            public string Hint = "", Note = "";
        }

        // Curva L011..L030: consolida (11-15), combina (16-20), tres fontes e cadeias (21-25), chefes (26-30).
        static readonly Spec[] Specs =
        {
            new Spec { N = 11, W = 5, H = 8, Sources = 2, Colors = 2, Crosses = 1, Rot = 4, MinTaps = 4, MaxTaps = 7, Note = "dois caminhos se cruzam", Hint = "Dois caminhos se cruzam. Resolva um de cada vez." },
            new Spec { N = 12, W = 5, H = 8, Sources = 2, Colors = 2, Switches = 1, MinQueue = 3, MaxQueue = 3, Rot = 3, MinTaps = 3, MaxTaps = 6, Note = "fila impar no alternador", Hint = "Fila ímpar: o primeiro lado recebe um orbe a mais." },
            new Spec { N = 13, W = 6, H = 8, Sources = 2, Colors = 2, Gates = 1, Rot = 4, MinTaps = 4, MaxTaps = 7, Note = "comporta", Hint = "Qual cristal abre a comporta? Veja a forma desenhada nela." },
            new Spec { N = 14, W = 6, H = 8, Sources = 2, Colors = 3, Switches = 1, Crosses = 1, Rot = 4, MinTaps = 4, MaxTaps = 8, Note = "alternador + cruzamento", Hint = "Três cores, duas fontes. Uma delas se divide." },
            new Spec { N = 15, W = 6, H = 8, Sources = 2, Colors = 3, Switches = 1, Gates = 1, Rot = 5, MinTaps = 5, MaxTaps = 9, Note = "alternador + comporta", Hint = "A comporta pode estar depois do alternador." },
            new Spec { N = 16, W = 6, H = 9, Sources = 3, Colors = 3, Crosses = 1, Rot = 5, MinTaps = 5, MaxTaps = 9, Note = "tres fontes", Hint = "Três fontes. Cada uma com seu caminho." },
            new Spec { N = 17, W = 6, H = 9, Sources = 2, Colors = 3, Switches = 2, MinQueue = 3, MaxQueue = 4, Rot = 4, MinTaps = 5, MaxTaps = 9, Note = "dois alternadores", Hint = "Dois alternadores. Cada seta decide um par de cores." },
            new Spec { N = 18, W = 6, H = 9, Sources = 3, Colors = 3, Gates = 1, Crosses = 1, Rot = 6, Decoys = 1, MinTaps = 6, MaxTaps = 10, Note = "primeira peca falsa", Hint = "Nem toda peça faz parte do caminho." },
            new Spec { N = 19, W = 6, H = 9, Sources = 2, Colors = 3, Switches = 1, DoubleSwitch = true, MinQueue = 4, MaxQueue = 6, Rot = 4, MinTaps = 4, MaxTaps = 9, Note = "alternador duplo", Hint = "Dois alternadores em sequência separam três cores." },
            new Spec { N = 20, W = 6, H = 9, Sources = 3, Colors = 3, Switches = 1, Gates = 1, Crosses = 1, Rot = 6, MinTaps = 6, MaxTaps = 11, Note = "marco 20: tudo junto", Hint = "Marco 20: tudo o que você aprendeu, de uma vez." },
            new Spec { N = 21, W = 6, H = 9, Sources = 3, Colors = 3, Gates = 2, Rot = 6, Decoys = 1, MinTaps = 6, MaxTaps = 11, Note = "comportas em cadeia", Hint = "Comportas em cadeia: uma abre a outra." },
            new Spec { N = 22, W = 6, H = 9, Sources = 3, Colors = 4, Switches = 1, Crosses = 2, Rot = 6, Decoys = 1, MinTaps = 6, MaxTaps = 11, Note = "quatro cores", Hint = "Quatro cores. Confira a forma, não só a cor." },
            new Spec { N = 23, W = 6, H = 10, Sources = 2, Colors = 4, Switches = 2, DoubleSwitch = true, MinQueue = 4, MaxQueue = 6, Rot = 5, MinTaps = 5, MaxTaps = 10, Note = "duplo + simples", Hint = "" },
            new Spec { N = 24, W = 6, H = 10, Sources = 3, Colors = 4, Switches = 1, Gates = 1, Crosses = 1, Rot = 7, Decoys = 2, MinTaps = 7, MaxTaps = 12, Note = "tres fontes, quatro cores", Hint = "" },
            new Spec { N = 25, W = 6, H = 10, Sources = 3, Colors = 4, Switches = 2, Gates = 1, MinQueue = 3, MaxQueue = 4, Rot = 6, Decoys = 2, MinTaps = 7, MaxTaps = 12, Note = "marco 25", Hint = "Marco 25. Planeje tudo antes de liberar." },
            new Spec { N = 26, W = 6, H = 10, Sources = 3, Colors = 4, Switches = 1, DoubleSwitch = true, Gates = 1, Crosses = 1, MinQueue = 4, MaxQueue = 6, Rot = 6, Decoys = 2, MinTaps = 6, MaxTaps = 12, Note = "alternador duplo + comporta", Hint = "" },
            new Spec { N = 27, W = 6, H = 10, Sources = 3, Colors = 4, Switches = 2, Gates = 2, MinQueue = 3, MaxQueue = 4, Rot = 7, Decoys = 2, MinTaps = 8, MaxTaps = 13, Note = "dois alternadores, duas comportas", Hint = "" },
            new Spec { N = 28, W = 6, H = 10, Sources = 3, Colors = 4, Switches = 1, Gates = 2, Crosses = 2, Rot = 8, Decoys = 3, MinTaps = 8, MaxTaps = 14, Note = "oito giraveis", Hint = "" },
            new Spec { N = 29, W = 6, H = 10, Sources = 3, Colors = 4, Switches = 2, DoubleSwitch = true, Gates = 1, Crosses = 1, MinQueue = 4, MaxQueue = 6, Rot = 7, Decoys = 3, MinTaps = 8, MaxTaps = 14, Note = "duplo + simples + comporta", Hint = "" },
            // 7 giraveis + 3 alternadores = 10 pecas tocaveis, o teto do solver (e do booster "Revelar runa")
            new Spec { N = 30, W = 6, H = 10, Sources = 3, Colors = 4, Switches = 2, DoubleSwitch = true, Gates = 2, Crosses = 1, MinQueue = 4, MaxQueue = 6, Rot = 7, Decoys = 3, MinTaps = 9, MaxTaps = 15, Note = "chefe 30", Hint = "O Observatório inteiro depende de você. Boa sorte." },
        };

        const long ComboBudget = 1L << 19;   // mesmo teto do portao de conteudo (LevelsContentTests)

        static int Main(string[] args)
        {
            string dir = FindLevelsDir();
            var only = new HashSet<int>(args.Where(a => int.TryParse(a, out _)).Select(int.Parse));
            int failed = 0;
            foreach (Spec s in Specs)
            {
                if (only.Count > 0 && !only.Contains(s.N)) continue;
                string text = null; int seed = 0, tries = 0;
                for (seed = s.N * 1000; seed < s.N * 1000 + 4000 && text == null; seed++) { tries++; text = TryBuild(s, seed); }
                if (text == null) { Console.WriteLine($"L{s.N:000}: FALHOU apos {tries} sementes"); failed++; continue; }
                File.WriteAllText(Path.Combine(dir, $"L{s.N:000}.txt"), text, new UTF8Encoding(false));
                Console.WriteLine($"L{s.N:000}: ok (semente {seed - 1}, {tries} tentativas)  {text.Split('\n')[0]}");
            }
            return failed == 0 ? 0 : 1;
        }

        static string FindLevelsDir()
        {
            for (var d = new DirectoryInfo(Directory.GetCurrentDirectory()); d != null; d = d.Parent)
            {
                string p = Path.Combine(d.FullName, "Assets", "_RR", "Resources", "Levels");
                if (Directory.Exists(p)) return p;
                p = Path.Combine(d.FullName, "client", "Assets", "_RR", "Resources", "Levels");
                if (Directory.Exists(p)) return p;
            }
            throw new DirectoryNotFoundException("rode dentro de 01_RUNE_RELAY");
        }

        // ------------------------------------------------------------------ construcao

        sealed class Draft
        {
            public readonly int W, H;
            public readonly Tile[] Cells;
            public readonly int[] Owner;                 // fonte dona da celula (-1 livre)
            public readonly int[] Parent;                // cristal -> celula do alternador pai (-1 raiz), para cores irmas
            public int Crosses;
            public Draft(int w, int h)
            {
                W = w; H = h; Cells = new Tile[w * h]; Owner = new int[w * h]; Parent = new int[w * h];
                for (int i = 0; i < Owner.Length; i++) { Owner[i] = -1; Parent[i] = -1; }
            }
            public bool In(int x, int y) => x >= 0 && y >= 0 && x < W && y < H;
            public int Idx(int x, int y) => y * W + x;
            public bool Free(int x, int y) => In(x, y) && Cells[Idx(x, y)].Kind == Kind.Empty;
        }

        sealed class Ctx
        {
            public Draft G; public Random R; public Spec S; public int Src; public bool AllowCross;
            public List<int> Leaves = new List<int>();
        }

        static string TryBuild(Spec s, int seed)
        {
            var rng = new Random(seed);
            var g = new Draft(s.W, s.H);
            int switchesLeft = s.Switches;
            var sourceCells = new List<int>();

            for (int src = 0; src < s.Sources; src++)
            {
                int wantSwitches = 0;
                if (switchesLeft > 0) { wantSwitches = s.DoubleSwitch && switchesLeft == s.Switches ? 2 : 1; switchesLeft--; }
                var ctx = new Ctx { G = g, R = rng, S = s, Src = src, AllowCross = s.Crosses > 0 && src > 0 };
                bool ok = false;
                for (int attempt = 0; attempt < 30 && !ok; attempt++)
                {
                    var snapshot = Snapshot(g);
                    int x = rng.Next(s.W), y = rng.Next(s.H);
                    if (!g.Free(x, y)) continue;
                    int d = rng.Next(4);
                    if (!g.Free(x + Dir.DX[d], y + Dir.DY[d])) continue;
                    int cell = g.Idx(x, y);
                    g.Cells[cell] = new Tile { Kind = Kind.Source, Rot = d };
                    g.Owner[cell] = src;
                    ctx.Leaves.Clear();
                    ok = Walk(ctx, cell, -1, d, rng.Next(s.MinLen, s.MaxLen + 1), wantSwitches);
                    if (ok) sourceCells.Add(cell); else Restore(g, snapshot);
                }
                if (!ok) return null;
            }
            if (g.Crosses < s.Crosses) return null;
            if (g.Cells.Count(t => t.Kind == Kind.Switch) != s.Switches + (s.DoubleSwitch ? 1 : 0)) return null; // a dica promete N alternadores

            // cores: irmaos (mesmo alternador pai) sempre diferentes; espalha pela paleta
            var crystals = Enumerable.Range(0, g.Cells.Length).Where(i => g.Cells[i].Kind == Kind.Crystal).ToList();
            int next = 0;
            foreach (int c in crystals.OrderBy(i => g.Parent[i]).ThenBy(i => i))
            {
                var taken = new HashSet<int>();
                if (g.Parent[c] >= 0)
                    foreach (int o in crystals) if (o != c && g.Parent[o] == g.Parent[c]) taken.Add(g.Cells[o].Color);
                int color = next % s.Colors;
                for (int k = 0; k < s.Colors && taken.Contains(color); k++) color = (color + 1) % s.Colors;
                g.Cells[c].Color = color;
                next++;
            }

            // filas: o destino de cada orbe e' simulado (alternadores viram a seta a cada passagem)
            var queues = new Dictionary<int, string>();
            foreach (int sc in sourceCells.OrderBy(i => i))
            {
                string q = null;
                for (int n = rng.Next(s.MinQueue, s.MaxQueue + 1); n <= s.MaxQueue + 3 && q == null; n++) q = Queue(g, sc, n);
                if (q == null) return null;
                queues[sc] = q;
            }

            // comportas: numa reta de uma fonte i, ligada a um cristal de fonte j < i (grafo aciclico, nunca trava)
            var crystalIndex = crystals.OrderBy(i => i).ToList();
            var gateOwners = new HashSet<int>();
            for (int k = 0; k < s.Gates; k++)
            {
                var hosts = Enumerable.Range(0, g.Cells.Length)
                    .Where(i => g.Cells[i].Kind == Kind.Straight && g.Owner[i] > 0 && !gateOwners.Contains(g.Owner[i])).ToList();
                if (hosts.Count == 0) return null;
                // cadeia (2+ comportas): a 1a fica na fonte mais baixa e abre com a fonte 0; cada seguinte fica numa fonte
                // acima e abre com um cristal da fonte da comporta anterior. Links sempre descem: nunca trava.
                if (s.Gates >= 2)
                {
                    int want = k == 0 ? hosts.Min(i => g.Owner[i]) : hosts.Where(i => g.Owner[i] > gateOwners.Max()).Select(i => g.Owner[i]).DefaultIfEmpty(-1).Min();
                    hosts = hosts.Where(i => g.Owner[i] == want).ToList();
                    if (hosts.Count == 0) return null;
                }
                int host = hosts[rng.Next(hosts.Count)];
                var targets = crystalIndex.Where(c => g.Owner[c] < g.Owner[host]).ToList();
                if (s.Gates >= 2 && k > 0) targets = targets.Where(c => g.Owner[c] == gateOwners.Max()).ToList();
                if (targets.Count == 0) return null;
                int target = targets[rng.Next(targets.Count)];
                g.Cells[host].Kind = Kind.Gate;
                g.Cells[host].Link = crystalIndex.IndexOf(target);
                gateOwners.Add(g.Owner[host]);
            }

            // giraveis: espalha pelas fontes, embaralha para fora da solucao
            var channels = Enumerable.Range(0, g.Cells.Length)
                .Where(i => g.Cells[i].Kind == Kind.Straight || g.Cells[i].Kind == Kind.Curve).ToList();
            if (channels.Count < s.Rot) return null;
            var chosen = new List<int>();
            foreach (var group in channels.GroupBy(i => g.Owner[i]).OrderBy(gr => rng.Next()))
                if (chosen.Count < s.Rot) chosen.Add(group.OrderBy(_ => rng.Next()).First());
            foreach (int i in channels.OrderBy(_ => rng.Next()))
                if (chosen.Count < s.Rot && !chosen.Contains(i)) chosen.Add(i);
            foreach (int i in chosen)
            {
                ref Tile t = ref g.Cells[i];
                t.Rotatable = true;
                t.Rot = t.Kind == Kind.Straight ? 1 - t.Rot : (t.Rot + rng.Next(1, 4)) & 3;
            }
            for (int i = 0; i < g.Cells.Length; i++)
                if (g.Cells[i].Kind == Kind.Switch) g.Cells[i].Arrow = rng.Next(2);

            // pecas falsas: fixas, encostadas num caminho, parecendo continuar
            for (int k = 0; k < s.Decoys; k++)
            {
                var spots = Enumerable.Range(0, g.Cells.Length).Where(i =>
                {
                    int x = i % g.W, y = i / g.W;
                    if (g.Cells[i].Kind != Kind.Empty) return false;
                    for (int d = 0; d < 4; d++)
                    {
                        int nx = x + Dir.DX[d], ny = y + Dir.DY[d];
                        if (g.In(nx, ny) && g.Owner[g.Idx(nx, ny)] >= 0 && g.Cells[g.Idx(nx, ny)].Kind != Kind.Crystal) return true;
                    }
                    return false;
                }).ToList();
                if (spots.Count == 0) break;
                int spot = spots[rng.Next(spots.Count)];
                g.Cells[spot] = rng.Next(2) == 0
                    ? new Tile { Kind = Kind.Straight, Rot = rng.Next(2) }
                    : new Tile { Kind = Kind.Curve, Rot = rng.Next(4) };
            }

            string text = Emit(s, seed, g, queues);

            // prova pelo mesmo codigo do jogo
            Board b;
            try { b = Board.Parse(text); } catch (FormatException) { return null; }
            if (Solver.Combos(b) > ComboBudget) return null;
            if (new FlowSim(b).Run() == Outcome.Win) return null;
            Solver.Result r = Solver.Solve(b, stopAfter: 2);
            if (r.Solutions != 1 || r.MinTaps < s.MinTaps || r.MinTaps > s.MaxTaps || r.Vars.Count > 10) return null;
            return text.Replace("{STATS}", $"giraveis={s.Rot} toques={r.MinTaps} combinacoes={Solver.Combos(b)}");
        }

        static (Tile[], int[], int[], int) Snapshot(Draft g) => ((Tile[])g.Cells.Clone(), (int[])g.Owner.Clone(), (int[])g.Parent.Clone(), g.Crosses);
        static void Restore(Draft g, (Tile[] c, int[] o, int[] p, int x) s)
        {
            Array.Copy(s.c, g.Cells, s.c.Length); Array.Copy(s.o, g.Owner, s.o.Length); Array.Copy(s.p, g.Parent, s.p.Length); g.Crosses = s.x;
        }

        /// <summary>
        /// Carva a partir de `cur` (ja' definida: fonte ou alternador, saida `forced`) ou de um canal ainda vazio
        /// (entrada `entry`). Termina pondo um cristal. Pode virar a celula atual em alternador e abrir dois ramos.
        /// </summary>
        static bool Walk(Ctx c, int cur, int entry, int forced, int len, int switches)
        {
            Draft g = c.G;
            int step = 0;
            while (true)
            {
                int x = cur % g.W, y = cur / g.W;
                bool preset = g.Cells[cur].Kind != Kind.Empty;

                if (!preset && switches > 0 && step >= 1 && c.R.Next(3) > 0)
                {
                    int rot = Dir.Opp(entry), left = (rot + 3) & 3, right = (rot + 1) & 3;
                    if (g.Free(x + Dir.DX[left], y + Dir.DY[left]) && g.Free(x + Dir.DX[right], y + Dir.DY[right]))
                    {
                        g.Cells[cur] = new Tile { Kind = Kind.Switch, Rot = rot };
                        g.Owner[cur] = c.Src;
                        int give = switches - 1, toLeft = c.R.Next(2) == 0 ? give : 0;
                        int before = c.Leaves.Count;
                        if (!Walk(c, cur, -1, left, c.R.Next(1, 4), toLeft)) return false;
                        if (!Walk(c, cur, -1, right, c.R.Next(1, 4), give - toLeft)) return false;
                        for (int i = before; i < c.Leaves.Count; i++) if (g.Parent[c.Leaves[i]] < 0) g.Parent[c.Leaves[i]] = cur;
                        return true;
                    }
                }

                bool ending = step >= len;
                int exit = -1;
                if (preset) { exit = forced; if (!Avail(c, x, y, exit, ending)) return false; }
                else
                {
                    var cands = new List<int>();
                    for (int d = 0; d < 4; d++) if (d != entry && Avail(c, x, y, d, ending)) { cands.Add(d); if (d == Dir.Opp(entry)) cands.Add(d); } // reta pesa 2x
                    if (cands.Count == 0)
                    {
                        if (ending) return false;
                        ending = true;   // sem saida: tenta terminar aqui
                        for (int d = 0; d < 4; d++) if (d != entry && Avail(c, x, y, d, true)) cands.Add(d);
                        if (cands.Count == 0) return false;
                    }
                    exit = cands[c.R.Next(cands.Count)];
                    g.Cells[cur] = Channel(entry, exit);
                    g.Owner[cur] = c.Src;
                }

                int nx = x + Dir.DX[exit], ny = y + Dir.DY[exit], n = g.Idx(nx, ny);
                if (ending)
                {
                    g.Cells[n] = new Tile { Kind = Kind.Crystal, Cap = 1 };
                    g.Owner[n] = c.Src;
                    c.Leaves.Add(n);
                    return true;
                }
                if (g.Cells[n].Kind == Kind.Straight)   // cruzamento: atravessa a reta do outro caminho
                {
                    g.Cells[n].Kind = Kind.Cross; g.Cells[n].Rot = 0; g.Crosses++;
                    n = g.Idx(nx + Dir.DX[exit], ny + Dir.DY[exit]);
                }
                cur = n; entry = Dir.Opp(exit); step++;
            }
        }

        /// <summary>A proxima celula na direcao d serve? Livre, ou (se nao for o fim) uma reta alheia perpendicular com a celula seguinte livre.</summary>
        static bool Avail(Ctx c, int x, int y, int d, bool ending)
        {
            Draft g = c.G;
            int nx = x + Dir.DX[d], ny = y + Dir.DY[d];
            if (g.Free(nx, ny)) return true;
            if (ending || !c.AllowCross || !g.In(nx, ny)) return false;
            Tile t = g.Cells[g.Idx(nx, ny)];
            bool perpendicular = t.Rot == (d % 2 == 0 ? 1 : 0);   // reta E-W (rot 1) para quem anda N-S
            return t.Kind == Kind.Straight && g.Owner[g.Idx(nx, ny)] != c.Src && perpendicular && g.Free(nx + Dir.DX[d], ny + Dir.DY[d]);
        }

        static Tile Channel(int entry, int exit)
        {
            if (exit == Dir.Opp(entry)) return new Tile { Kind = Kind.Straight, Rot = entry % 2 == 0 ? 0 : 1 };
            return new Tile { Kind = Kind.Curve, Rot = exit == ((entry + 1) & 3) ? entry : exit };
        }

        /// <summary>Fila de n orbes: segue cada um ate o cristal (virando setas), devolve as cores; null se algum cristal ficar sem orbe.</summary>
        static string Queue(Draft g, int source, int n)
        {
            var arrows = new Dictionary<int, int>();
            var count = new Dictionary<int, int>();
            var sb = new StringBuilder();
            for (int k = 0; k < n; k++)
            {
                int cur = source, d = g.Cells[source].Rot;
                for (int guard = 0; guard < 400; guard++)
                {
                    int nx = cur % g.W + Dir.DX[d], ny = cur / g.W + Dir.DY[d];
                    if (!g.In(nx, ny)) return null;
                    int idx = g.Idx(nx, ny);
                    Tile t = g.Cells[idx];
                    if (t.Kind == Kind.Crystal) { sb.Append((char)('a' + t.Color)); count[idx] = count.TryGetValue(idx, out int v) ? v + 1 : 1; break; }
                    int arrow = arrows.TryGetValue(idx, out int a) ? a : t.Arrow;
                    if (!t.Route(Dir.Opp(d), arrow, out int exit)) return null;
                    if (t.Kind == Kind.Switch) arrows[idx] = arrow ^ 1;
                    cur = idx; d = exit;
                }
            }
            // todo cristal desta fonte precisa receber >= 1 orbe; a capacidade e' o que chegou
            for (int i = 0; i < g.Cells.Length; i++)
            {
                if (g.Cells[i].Kind != Kind.Crystal || g.Owner[i] != g.Owner[source]) continue;
                if (!count.TryGetValue(i, out int v) || v > 9) return null;
                g.Cells[i].Cap = v;
            }
            return sb.ToString();
        }

        static string Emit(Spec s, int seed, Draft g, Dictionary<int, string> queues)
        {
            var sb = new StringBuilder();
            sb.Append($"// L{s.N:000}: {s.Note} | gerado (semente {seed}) {{STATS}}\n");
            if (s.Hint.Length > 0) sb.Append("h ").Append(s.Hint).Append('\n');
            foreach (var kv in queues.OrderBy(k => k.Key)) sb.Append("q ").Append(kv.Value).Append('\n');
            // recorta a grade ao conteudo: celulas maiores na tela, e a ordem de leitura (indices de fonte/cristal) se mantem
            int x0 = g.W, x1 = -1, y0 = g.H, y1 = -1;
            for (int i = 0; i < g.Cells.Length; i++)
                if (g.Cells[i].Kind != Kind.Empty) { x0 = Math.Min(x0, i % g.W); x1 = Math.Max(x1, i % g.W); y0 = Math.Min(y0, i / g.W); y1 = Math.Max(y1, i / g.W); }
            for (int y = y0; y <= y1; y++)
                sb.Append(string.Join(" ", Enumerable.Range(x0, x1 - x0 + 1).Select(x => Token(g.Cells[g.Idx(x, y)]).PadRight(3))).TrimEnd()).Append('\n');
            return sb.ToString();
        }

        static string Token(Tile t)
        {
            switch (t.Kind)
            {
                case Kind.Block: return "#";
                case Kind.Cross: return "+";
                case Kind.Straight: return "I" + t.Rot + (t.Rotatable ? "r" : "f");
                case Kind.Curve: return "L" + t.Rot + (t.Rotatable ? "r" : "f");
                case Kind.Gate: return "G" + t.Rot + t.Link;
                case Kind.Switch: return "Y" + t.Rot + t.Arrow;
                case Kind.Source: return "S" + t.Rot;
                case Kind.Crystal: return "R" + (char)('a' + t.Color) + t.Cap;
                default: return ".";
            }
        }
    }
}
