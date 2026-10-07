using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RR.Core;

namespace RR.Tests
{
    public class CoreTests
    {
        static FlowSim Run(string level)
        {
            var sim = new FlowSim(Board.Parse(level));
            sim.Run();
            return sim;
        }

        [Test]
        public void Parse_TokenInvalido_DizLinhaEColuna()
        {
            var e = Assert.Throws<FormatException>(() => Board.Parse("q a\nS1 X9 Ra1"));
            StringAssert.Contains("linha 2, coluna 2", e.Message);
        }

        [Test]
        public void Parse_OrbesDiferentesDaCapacidade_Rejeita()
        {
            Assert.Throws<FormatException>(() => Board.Parse("q aa\nS1 Ra1"));
        }

        [Test]
        public void CaminhoReto_Vence()
        {
            Assert.AreEqual(Outcome.Win, Run("q aa\nS1 I1f Ra2").Outcome);
        }

        [Test]
        public void CanalVirado_Derrama_NaCelulaDoCanal()
        {
            var sim = Run("q a\nS1 I0r Ra1");
            Assert.AreEqual(Outcome.Spill, sim.Outcome);
            Assert.AreEqual((1, 0), (sim.FailX, sim.FailY));
        }

        [Test]
        public void CorErrada_RachaOCristal()
        {
            Assert.AreEqual(Outcome.WrongColor, Run("q b\nq a\nS1 Ra1\nS1 Rb1").Outcome);
        }

        [Test]
        public void Alternador_SeparaAFila_EViraASeta()
        {
            const string sort = "q abab\n.   .   .\nRa2 Y0{0} Rb2\n.   S0  .";
            Assert.AreEqual(Outcome.Win, Run(string.Format(sort, 0)).Outcome);
            Assert.AreEqual(Outcome.WrongColor, Run(string.Format(sort, 1)).Outcome);
        }

        [Test]
        public void Cruzamento_PassaDuasCoresSemMisturar()
        {
            Assert.AreEqual(Outcome.Win, Run("q b\nq a\n.  S2  .\nS1 +   Ra1\n.  Rb1 .").Outcome);
        }

        [Test]
        public void Comporta_SeguraOrbeAteOCristalEncher()
        {
            var sim = new FlowSim(Board.Parse("q a\nq b\nS1 I1f Ra1\nS1 G10 Rb1"));
            sim.Step(); sim.Step();     // t0 solta os dois; t1 o vermelho bate na comporta fechada
            Assert.AreEqual((0, 1), (sim.Orbs[1].X, sim.Orbs[1].Y), "o vermelho espera antes da comporta");
            Assert.AreEqual(Outcome.Win, sim.Run());
        }

        [Test]
        public void Comporta_QueNuncaAbre_Trava()
        {
            Assert.AreEqual(Outcome.Stuck, Run("q a\nS1 G10 Ra1").Outcome);
        }

        [Test]
        public void Simulacao_EDeterministica()
        {
            string lvl = File.ReadAllText(Path.Combine(LevelsContentTests.Dir(), "L010.txt"));
            var a = Run(lvl); var b = Run(lvl);
            Assert.AreEqual((a.Outcome, a.Tick, a.FailX, a.FailY), (b.Outcome, b.Tick, b.FailX, b.FailY));
        }

        [Test]
        public void Previa_AcendeOCaminhoAteOCristal_SemVirarASeta()
        {
            var b = Board.Parse("q ab\n.   .   .\nRa1 Y00 Rb1\n.   S0  .");
            int[] lit = FlowSim.Lit(b);
            Assert.AreEqual(0, lit[1 * 3 + 0], "cristal azul aceso na cor do proximo orbe");
            Assert.AreEqual(-1, lit[1 * 3 + 2], "o ramo da direita fica apagado");
            Assert.AreEqual(0, b.At(1, 1).Arrow, "a previa nao muda o tabuleiro");
        }

        [Test]
        public void Partida_FalhaMantemGiros_EstrelasPorLiberacao()
        {
            var s = new Session(Board.Parse("q a\nS1 I0r Ra1"));
            var sim = s.Release(); sim.Run(); s.End(sim);
            Assert.IsFalse(s.Won);
            Assert.IsTrue(s.Tap(1, 0));
            sim = s.Release(); sim.Run(); s.End(sim);
            Assert.IsTrue(s.Won);
            Assert.AreEqual(2, s.Stars, "venceu na 2a liberacao");
            Assert.AreEqual(1, s.Board.At(1, 0).Rot, "o giro do jogador continua no tabuleiro");
        }

        [Test]
        public void Partida_TresFalhas_AcabamAsLiberacoes_ExtraDevolveUma()
        {
            var s = new Session(Board.Parse("q a\nS1 I0r Ra1"));
            for (int i = 0; i < Session.BaseReleases; i++) { var sim = s.Release(); sim.Run(); s.End(sim); }
            Assert.IsFalse(s.CanPlay);
            Assert.IsNull(s.Release());
            s.ExtraRelease();
            Assert.IsTrue(s.CanPlay);
            s.Restart();
            Assert.AreEqual((0, 0, 0), (s.Used, s.Board.At(1, 0).Rot, s.Taps));
        }

        [Test]
        public void Solver_AchaMenosToques_ENivelImpossivelDaMenosUm()
        {
            var r = Solver.Solve(Board.Parse("q a\nS1 L0r Ra1"));    // L precisa ligar W com E: impossivel
            Assert.AreEqual(-1, r.MinTaps);
            r = Solver.Solve(Board.Parse("q a\nS1 L3r .\n.  Ra1 ."));  // W->S = giro 2: 3 toques a partir do 3
            Assert.AreEqual(3, r.MinTaps);
            Assert.AreEqual(1, r.Solutions);
        }

        [Test]
        public void Progresso_IdaEVolta_ELixoViraZero()
        {
            var p = new Progress(4);
            p.Record(0, 3); p.Record(1, 1); p.Record(1, 0);
            Assert.AreEqual("3,1,0,0", p.ToString());
            Assert.AreEqual(2, Progress.Parse(p.ToString(), 4).Unlocked);
            Assert.AreEqual(2, Progress.Parse("x,9,-1,2,2,2", 4).Total, "lixo vira zero, sobra ignorada");
            Assert.AreEqual(0, Progress.Parse(null, 4).Total);
        }
    }

    /// <summary>Portao de conteudo: todo nivel em Resources/Levels precisa passar aqui antes de entrar no jogo.</summary>
    public class LevelsContentTests
    {
        public static string Dir()
        {
            foreach (string start in new[] { Directory.GetCurrentDirectory(), TestContext.CurrentContext.TestDirectory })
                for (var d = new DirectoryInfo(start); d != null; d = d.Parent)
                {
                    string p = Path.Combine(d.FullName, "Assets", "_RR", "Resources", "Levels");
                    if (Directory.Exists(p)) return p;
                }
            throw new DirectoryNotFoundException("Assets/_RR/Resources/Levels");
        }

        static string[] Files() => Directory.GetFiles(Dir(), "L*.txt").OrderBy(f => f).ToArray();

        [Test]
        public void Niveis_SaoSequenciais()
        {
            string[] names = Files().Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
            Assert.GreaterOrEqual(names.Length, 10);
            for (int i = 0; i < names.Length; i++) Assert.AreEqual($"L{i + 1:000}", names[i]);
        }

        [Test]
        public void TodoNivel_TemSolucao_ENaoComecaResolvido([ValueSource(nameof(Names))] string name)
        {
            Board b = Board.Parse(File.ReadAllText(Path.Combine(Dir(), name + ".txt")));
            Assert.AreNotEqual(Outcome.Win, new FlowSim(b).Run(), "comeca resolvido");
            Assert.LessOrEqual(Solver.Combos(b), 1L << 19, "combinacoes demais: o solver (e o booster) ficam lentos");
            var r = Solver.Solve(b, stopAfter: 2);
            Assert.LessOrEqual(r.Vars.Count, 10, "pecas tocaveis demais para o solver");
            Assert.GreaterOrEqual(r.MinTaps, 1, "sem solucao");
            Assert.AreEqual(1, r.Solutions, "solucao nao e' unica");
            TestContext.WriteLine($"{name}: {r.MinTaps} toques minimos, {r.Solutions} solucoes, {r.Vars.Count} pecas");
        }

        public static string[] Names() => Files().Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
    }
}
