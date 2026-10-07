using System;

namespace RR.Core
{
    /// <summary>
    /// Regras de uma partida (raia B §6): girar e' gratis; cada Liberar gasta 1 de 3; falha mantem os giros
    /// (a simulacao roda numa copia); estrelas = 3 na 1a liberacao, 2 na 2a, 1 na 3a (ou extra).
    /// </summary>
    public sealed class Session
    {
        public const int BaseReleases = 3;
        public readonly Board Start;
        public Board Board { get; private set; }
        public int Used { get; private set; }
        public int Max { get; private set; } = BaseReleases;
        public int Taps { get; private set; }
        public bool Won { get; private set; }
        public int Stars { get; private set; }

        public Session(Board start)
        {
            Start = start;
            Board = start.Clone();
        }

        public bool CanPlay => !Won && Used < Max;

        public bool Tap(int x, int y)
        {
            if (!CanPlay || !Board.Tap(x, y)) return false;
            Taps++;
            return true;
        }

        /// <summary>A view toca a simulacao passo a passo e chama End no fim. null = sem liberacao.</summary>
        public FlowSim Release()
        {
            if (!CanPlay) return null;
            Used++;
            return new FlowSim(Board);
        }

        public void End(FlowSim sim)
        {
            if (sim.Outcome != Outcome.Win) return;
            Won = true;
            Stars = Math.Max(1, 4 - Used);
        }

        /// <summary>+1 liberacao (o rewarded do GDD; no MVP sem anuncio).</summary>
        public void ExtraRelease() => Max++;

        public void Restart()
        {
            Board = Start.Clone();
            Used = 0; Max = BaseReleases; Taps = 0; Won = false; Stars = 0;
        }
    }

    /// <summary>Estrelas por nivel. Texto "3,2,0" para o PlayerPrefs; lixo vira zero, nunca lanca.</summary>
    public sealed class Progress
    {
        public readonly int[] Stars;

        public Progress(int levels) { Stars = new int[levels]; }

        /// <summary>Indice do ultimo nivel jogavel: o primeiro ainda nao vencido.</summary>
        public int Unlocked
        {
            get
            {
                int i = 0;
                while (i < Stars.Length - 1 && Stars[i] > 0) i++;
                return i;
            }
        }

        public int Total
        {
            get { int t = 0; foreach (int s in Stars) t += s; return t; }
        }

        public void Record(int level, int stars)
        {
            if (stars > Stars[level]) Stars[level] = stars;
        }

        public override string ToString() => string.Join(",", Stars);

        public static Progress Parse(string s, int levels)
        {
            var p = new Progress(levels);
            if (string.IsNullOrEmpty(s)) return p;
            string[] parts = s.Split(',');
            for (int i = 0; i < Math.Min(parts.Length, levels); i++)
                if (int.TryParse(parts[i], out int v) && v >= 0 && v <= 3) p.Stars[i] = v;
            return p;
        }
    }
}
