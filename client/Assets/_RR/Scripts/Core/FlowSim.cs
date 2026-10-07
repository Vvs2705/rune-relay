using System.Collections.Generic;

namespace RR.Core
{
    public enum Outcome { Running, Win, Spill, WrongColor, Overflow, Stuck }

    /// <summary>
    /// Simulacao deterministica da liberacao (GDD de prototipo, raia B §6). Por tick: cada orbe vivo anda 1 celula
    /// (em ordem de criacao: alternador vira a seta depois de cada passagem); depois, em tick par, cada fonte solta o
    /// proximo orbe da fila. Comporta fechada SEGURA o orbe (decisao do coordenador em 2026-10-06; a raia B propos
    /// derramar): a cadeia vira dependencia legivel em vez de puzzle de tempo. Primeira falha encerra.
    /// </summary>
    public sealed class FlowSim
    {
        public struct Orb { public int X, Y, Dir, Color; public bool Alive; }

        public const int MaxTicks = 1000;   // caminho em ciclo nunca termina: vira Stuck
        public readonly Board B;            // copia: setas e comportas mudam aqui, o tabuleiro do jogador nao
        public readonly List<Orb> Orbs = new List<Orb>();
        public readonly int[] Filled;
        readonly int[] _next;
        public int Tick { get; private set; }
        public Outcome Outcome { get; private set; }
        public int FailX = -1, FailY = -1;

        public FlowSim(Board start)
        {
            B = start.Clone();
            Filled = new int[B.Crystals.Count];
            _next = new int[B.Sources.Count];
        }

        public bool Full(int crystal) => Filled[crystal] >= B.Cells[B.Crystals[crystal]].Cap;
        public int Emitted(int source) => _next[source];
        public bool GateOpen(in Tile gate) => Full(gate.Link);

        public Outcome Run()
        {
            while (Step()) { }
            return Outcome;
        }

        /// <summary>Um tick. Devolve false quando acabou (Outcome != Running).</summary>
        public bool Step()
        {
            if (Outcome != Outcome.Running) return false;
            bool moved = false;
            for (int i = 0; i < Orbs.Count && Outcome == Outcome.Running; i++)
            {
                Orb o = Orbs[i];
                if (!o.Alive) continue;
                int nx = o.X + Dir.DX[o.Dir], ny = o.Y + Dir.DY[o.Dir];
                if (!B.In(nx, ny)) { Fail(Outcome.Spill, o.X, o.Y); break; }
                ref Tile t = ref B.At(nx, ny);
                int entry = Dir.Opp(o.Dir);

                if (t.Kind == Kind.Crystal)
                {
                    if (t.Color != o.Color) { Fail(Outcome.WrongColor, nx, ny); break; }
                    if (Full(t.Index)) { Fail(Outcome.Overflow, nx, ny); break; }
                    Filled[t.Index]++;
                    o.Alive = false;
                }
                else
                {
                    if (t.Kind == Kind.Gate && !GateOpen(t) && (t.Sides & (1 << entry)) != 0) continue; // espera
                    if (!t.Route(entry, t.Arrow, out int exit)) { Fail(Outcome.Spill, nx, ny); break; }
                    if (t.Kind == Kind.Switch) t.Arrow ^= 1;
                    o.Dir = exit;
                }
                o.X = nx; o.Y = ny;
                Orbs[i] = o;
                moved = true;
            }

            bool pending = false;
            for (int s = 0; s < _next.Length; s++)
            {
                string q = B.Queues[s];
                if (_next[s] >= q.Length) continue;
                pending = true;
                if (Outcome != Outcome.Running || Tick % 2 != 0) continue;
                int cell = B.Sources[s];
                Orbs.Add(new Orb { X = cell % B.W, Y = cell / B.W, Dir = B.Cells[cell].Rot, Color = q[_next[s]] - 'a', Alive = true });
                _next[s]++;
                moved = true;
            }
            Tick++;
            if (Outcome != Outcome.Running) return false;

            bool alive = false;
            foreach (Orb o in Orbs) alive |= o.Alive;
            if (!alive && !pending)
            {
                bool all = true;
                for (int c = 0; c < Filled.Length; c++) all &= Full(c);
                Outcome = all ? Outcome.Win : Outcome.Stuck;
            }
            else if ((!moved && !pending) || Tick > MaxTicks) Outcome = Outcome.Stuck; // so' orbes parados em comporta que nunca abre
            return Outcome == Outcome.Running;
        }

        void Fail(Outcome why, int x, int y)
        {
            Outcome = why;
            FailX = x; FailY = y;
        }

        /// <summary>
        /// Previa da fase de planejamento: a cor (0..5) do proximo orbe de cada fonte em cada celula do caminho dele,
        /// com as setas atuais (sem virar) e comportas tratadas como abertas. -1 = apagada.
        /// </summary>
        public static int[] Lit(Board b)
        {
            var lit = new int[b.Cells.Length];
            for (int i = 0; i < lit.Length; i++) lit[i] = -1;
            for (int s = 0; s < b.Sources.Count; s++)
            {
                if (b.Queues[s].Length == 0) continue;
                int color = b.Queues[s][0] - 'a';
                int cell = b.Sources[s], x = cell % b.W, y = cell / b.W, d = b.Cells[cell].Rot;
                lit[cell] = color;
                for (int steps = 0; steps < b.Cells.Length * 4; steps++)
                {
                    int nx = x + Dir.DX[d], ny = y + Dir.DY[d];
                    if (!b.In(nx, ny)) break;
                    Tile t = b.At(nx, ny);
                    if (t.Kind == Kind.Crystal) { lit[ny * b.W + nx] = color; break; }
                    if (!t.Route(Dir.Opp(d), t.Arrow, out int e)) break;
                    lit[ny * b.W + nx] = color;
                    x = nx; y = ny; d = e;
                }
            }
            return lit;
        }
    }
}
