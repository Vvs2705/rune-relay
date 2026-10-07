"""Relatorio do playtest (Camada 0) a partir de um ou mais diario.csv do Rune Relay.

Uso:
  python client/tools/diario_report.py                       # le o diario do build Windows deste PC
  python client/tools/diario_report.py pasta_ou_csv [...]    # varios arquivos (um por aparelho/testador)
  python client/tools/diario_report.py --autoteste           # prova o calculo com dados sinteticos

Android (build dev): adb pull /sdcard/Android/data/br.com.vstack.runerelay/files/diario.csv testador01.csv

Colunas do CSV (Game.Log): utc, sessao, evento, nivel, a, b
  level_start | first_move a=ms | release a=liberacao b=toques | level_fail a=motivo b=liberacao
  level_complete a=estrelas b=segundos | restart | session_start | menu_open | settings_change
"""
import csv
import glob
import os
import statistics
import sys
from collections import defaultdict

PADRAO = os.path.join(os.environ.get("USERPROFILE", ""), r"AppData\LocalLow\V-STACK\Rune Relay\diario.csv")


def ler(caminhos):
    linhas = []
    for c in caminhos:
        arquivos = glob.glob(os.path.join(c, "*.csv")) if os.path.isdir(c) else [c]
        for a in arquivos:
            with open(a, encoding="utf-8", newline="") as f:
                for r in csv.reader(f):
                    if len(r) >= 6:
                        # a sessao vira "arquivo:sid" para dois aparelhos nunca se misturarem
                        linhas.append((r[0], os.path.basename(a) + ":" + r[1], r[2], int(r[3] or 0), r[4], r[5]))
    return linhas


def analisar(linhas):
    por_nivel = defaultdict(lambda: {"inicios": set(), "completos": set(), "falhas": defaultdict(int), "tempos": [],
                                     "first_move": [], "liberacoes": [], "sem_release": set(), "falhou_alguem": set()})
    sessoes = set()
    for _, sid, evt, nivel, a, b in linhas:
        sessoes.add(sid)
        n = por_nivel[nivel]
        if evt == "level_start":
            n["inicios"].add(sid)
            n["sem_release"].add(sid)
        elif evt == "release":
            n["sem_release"].discard(sid)
        elif evt == "first_move" and a.isdigit():
            n["first_move"].append(int(a) / 1000)
        elif evt == "level_fail":
            n["falhas"][a] += 1
            n["falhou_alguem"].add(sid)
        elif evt == "level_complete":
            n["completos"].add(sid)
            try:
                n["tempos"].append(float(b))
                n["liberacoes"].append(1)
            except ValueError:
                pass
    return sessoes, por_nivel


def mediana(v):
    return statistics.median(v) if v else 0.0


def relatorio(linhas):
    sessoes, por_nivel = analisar(linhas)
    total = len(sessoes)
    saida = [f"Sessoes (aparelho:sid): {total}", "",
             f"{'Niv':>3} {'inic':>5} {'compl':>5} {'%':>5} {'t med s':>8} {'1o toque s':>10} {'falhas (motivo)':<40} {'sem Liberar'}"]
    for nivel in sorted(por_nivel):
        if nivel == 0:
            continue  # eventos de menu
        n = por_nivel[nivel]
        inic, compl = len(n["inicios"]), len(n["completos"])
        falhas = ", ".join(f"{k}:{v}" for k, v in sorted(n["falhas"].items())) or "-"
        saida.append(f"{nivel:>3} {inic:>5} {compl:>5} {100 * compl // max(1, inic):>4}% {mediana(n['tempos']):>8.0f} "
                     f"{mediana(n['first_move']):>10.1f} {falhas:<40} {len(n['sem_release'])}")

    # Portao da raia B (GDD de prototipo §6): passa / refuta
    saida += ["", "PORTAO (raia B, GDD de prototipo sec. 6):"]
    inic1 = len(por_nivel[1]["inicios"]) if 1 in por_nivel else 0
    compl10 = len(por_nivel[10]["completos"]) if 10 in por_nivel else 0
    falhou_9_10 = len(por_nivel[9]["falhou_alguem"] | por_nivel[10]["falhou_alguem"]) if 9 in por_nivel or 10 in por_nivel else 0
    sem_rel2 = len(por_nivel[2]["sem_release"]) if 2 in por_nivel else 0
    inic2 = len(por_nivel[2]["inicios"]) if 2 in por_nivel else 0
    med_8_10 = mediana([t for lv in (8, 9, 10) if lv in por_nivel for t in por_nivel[lv]["tempos"]])
    pct10 = 100 * compl10 / max(1, inic1)
    pct_falha = 100 * falhou_9_10 / max(1, inic1)
    pct_sem_rel2 = 100 * sem_rel2 / max(1, inic2)
    saida.append(f"  terminam o nivel 10: {pct10:.0f}% dos {inic1} que comecaram  (passa >= 50%)  -> {'PASSA' if pct10 >= 50 else 'nao'}")
    saida.append(f"  falham ao menos 1x nos niveis 9-10: {pct_falha:.0f}%  (passa >= 30%: a pressao existe)  -> {'PASSA' if pct_falha >= 30 else 'nao'}")
    saida.append(f"  nao apertam LIBERAR sozinhos no nivel 2: {pct_sem_rel2:.0f}%  (refuta > 25%)  -> {'REFUTA' if pct_sem_rel2 > 25 else 'ok'}")
    saida.append(f"  mediana de tempo nos niveis 8-10: {med_8_10:.0f} s  (refuta > 180 s)  -> {'REFUTA' if med_8_10 > 180 else 'ok'}")
    return "\n".join(saida)


def autoteste():
    # 4 testadores: 2 chegam ao fim do 10, 1 trava no 5, 1 nunca aperta Liberar no 2
    linhas = []

    def ev(sid, evt, nivel, a="", b=""):
        linhas.append(("2026-10-06T00:00:00", sid, evt, nivel, a, b))

    for sid in ("x.csv:A", "x.csv:B"):
        for lv in range(1, 11):
            ev(sid, "level_start", lv)
            ev(sid, "first_move", lv, "2500")
            if lv >= 9:
                ev(sid, "release", lv, "1", "4")
                ev(sid, "level_fail", lv, "WrongColor", "1")
            ev(sid, "release", lv, "2", "5")
            ev(sid, "level_complete", lv, "2", "60" if lv < 8 else "150")
    for lv in range(1, 6):
        ev("y.csv:C", "level_start", lv)
        ev("y.csv:C", "release", lv, "1", "3")
        if lv < 5:
            ev("y.csv:C", "level_complete", lv, "3", "40")
        else:
            ev("y.csv:C", "level_fail", lv, "Spill", "1")
    ev("y.csv:D", "level_start", 1); ev("y.csv:D", "release", 1, "1", "1"); ev("y.csv:D", "level_complete", 1, "3", "20")
    ev("y.csv:D", "level_start", 2)  # desiste sem liberar

    sessoes, por_nivel = analisar(linhas)
    assert len(sessoes) == 4
    assert len(por_nivel[10]["completos"]) == 2
    assert por_nivel[5]["falhas"]["Spill"] == 1
    assert por_nivel[2]["sem_release"] == {"y.csv:D"}
    assert mediana(por_nivel[1]["first_move"]) == 2.5
    texto = relatorio(linhas)
    assert "terminam o nivel 10: 50%" in texto and "PASSA" in texto
    assert "nao apertam LIBERAR sozinhos no nivel 2: 25%" in texto and "(refuta > 25%)  -> ok" in texto
    so_a_d = relatorio([l for l in linhas if l[1] in ("x.csv:A", "y.csv:D")])  # 1 de 2 sem liberar = 50%
    assert "nivel 2: 50%" in so_a_d and "REFUTA" in so_a_d
    print(texto)
    print("\nautoteste OK")


if __name__ == "__main__":
    args = sys.argv[1:]
    if args == ["--autoteste"]:
        autoteste()
        sys.exit(0)
    caminhos = args or [PADRAO]
    faltando = [c for c in caminhos if not os.path.exists(c)]
    if faltando:
        print("nao encontrei:", ", ".join(faltando))
        sys.exit(2)
    print(relatorio(ler(caminhos)))
