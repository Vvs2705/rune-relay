using System;
using System.Collections.Generic;

namespace RR.Core
{
    /// <summary>
    /// Forca bruta em todos os estados das pecas tocaveis (giro dos canais, seta dos alternadores).
    /// ponytail: 4^n x 2^a simulacoes; o portao de conteudo limita a 10 pecas tocaveis. Backtracking com poda por
    /// borda quando os niveis crescerem.
    /// </summary>
    public static class Solver
    {
        public sealed class Result
        {
            public readonly List<int> Vars = new List<int>();   // celulas tocaveis
            public int Solutions;
            public int MinTaps = -1;                             // -1 = sem solucao
            public int[] Best;                                   // estado de cada Var na solucao de menos toques
        }

        /// <summary>Numero de combinacoes de estados (custo do Solve). Unico lugar com essa conta.</summary>
        public static long Combos(Board b)
        {
            long combos = 1;
            foreach (Tile t in b.Cells) if (t.Tappable) combos *= t.States;
            return combos;
        }

        /// <summary>`stopAfter`: para ao achar essa quantidade de solucoes (2 = so' quer saber se e' unica).</summary>
        public static Result Solve(Board start, long maxCombos = 1 << 22, int stopAfter = int.MaxValue)
        {
            var r = new Result();
            long combos = Combos(start);
            for (int i = 0; i < start.Cells.Length; i++)
                if (start.Cells[i].Tappable) r.Vars.Add(i);
            if (combos > maxCombos) throw new InvalidOperationException($"{combos} combinacoes: nivel grande demais para o solver");

            Board b = start.Clone();
            var st = new int[r.Vars.Count];
            for (long n = 0; n < combos; n++)
            {
                long k = n;
                int taps = 0;
                for (int j = 0; j < st.Length; j++)
                {
                    int cell = r.Vars[j], states = b.Cells[cell].States;
                    st[j] = (int)(k % states);
                    k /= states;
                    b.Cells[cell].State = st[j];
                    taps += (st[j] - start.Cells[cell].State + states) % states;
                }
                if (new FlowSim(b).Run() != Outcome.Win) continue;
                r.Solutions++;
                if (r.MinTaps < 0 || taps < r.MinTaps) { r.MinTaps = taps; r.Best = (int[])st.Clone(); }
                if (r.Solutions >= stopAfter) break;
            }
            return r;
        }
    }
}
