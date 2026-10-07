# Rune Relay — protótipo v0.1 (greybox jogável)

Puzzle hybrid-casual em retrato. Você **planeja** (gira canais e escolhe a seta dos alternadores), toca em **LIBERAR** e **assiste** à cascata: orbes coloridos saem da fila das fontes e precisam encher cada cristal da sua cor. São 3 liberações por nível, e as estrelas vêm de quantas você gastou (3★ na 1ª).

- **GDD:** `docs/GDD.md` (o adendo de 2026-10-06 no topo vale sobre as §2, §6 e §18).
- **Por que este jogo:** `../00_PESQUISA/VEREDITO_VALIDACAO.md`.

## Estado (2026-10-06)

| Item | Estado |
|---|---|
| Núcleo C# puro (`RR.Core`: tabuleiro, simulação, solver, partida, progresso) | pronto, 46 testes (16 de regra + 30 do portão de conteúdo) |
| 30 níveis: L001–L010 à mão (curva da raia B: girar → cores → cruzamento → armadilha → comporta → alternador → chefe) e L011–L030 pelo **gerador por construção** (`client/tools/levelgen`) | prontos; todos com **1 solução**, nenhum começa resolvido, ≤10 peças tocáveis |
| View v2 (raia Arte/UX, 2026-10-06): direção "observatório arcano", peças tocáveis com aro dourado, cascata com brilho/rastro/faíscas, gema que enche, comporta deslizando, seta do alternador com halo | pronto, 7 arquivos em `Scripts/View/` (Art, Sfx, Fx, BoardView, Menu, Tutorial, Game), zero asset |
| Menu de níveis (grade com scroll, estrelas, cadeado), botão voltar (nível → menu → sair), painéis de vitória/falha com motivo, pílula "segure para acelerar", tutorial de anel nos níveis 1–2, configurações som/vibração (PlayerPrefs `rr.sound`/`rr.vibe`), haptic no Android | pronto |
| SFX sintetizados "mágicos" (absorb com pitch por orbe, acorde ao encher, arpejo na vitória) | pronto |
| Diário de playtest (`persistentDataPath/diario.csv`) | pronto |
| Build Windows (dev, janela retrato 540×960) | `client/Builds/win/RuneRelay.exe` |
| APK Android dev (IL2CPP ARM64, API 26+) | `client/Builds/android/RuneRelay-dev.apk` |
| Meta, moedas, booster, rewarded, IAP, interstitial | **fora da v0.1** (ver Próximos passos) |

## Como rodar

```bash
dotnet test client/tools/coretests
```
Núcleo + testes em ~1 s, sem abrir o Unity.

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.23f1/Editor/Unity.exe" -batchmode -nographics -projectPath "$(pwd -W)/client" -runTests -testPlatform EditMode -testResults "$(pwd -W)/client/Builds/editmode.xml" -logFile "$(pwd -W)/client/Builds/editmode.log"
```
Mesmos testes dentro do Unity. O veredito vem do XML, não do código de saída.

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.23f1/Editor/Unity.exe" -batchmode -nographics -quit -projectPath "$(pwd -W)/client" -executeMethod RR.EditorTools.Setup.BuildWindows -logFile "$(pwd -W)/client/Builds/build_win.log"
```
Build de Windows. Para Android, troque o método por `RR.EditorTools.Setup.BuildAndroidDev`.

```bash
client/Builds/win/RuneRelay.exe -batchmode -nographics -autoplay -logFile autoplay.log
```
Smoke no jogo compilado: resolve todos os níveis pelo solver e sai com código 0. O log termina em `AUTOPLAY OK 30/30`.

Flags de dev do executável:
- `-level N`: abre direto no nível N.
- `-shot foto.png`: tira uma foto e sai. Combina com `-solve` (aplica a solução), `-release` (libera) e `-shotdelay 2.5` (espera antes da foto).
- `-screen menu`: abre na seleção de níveis (para fotografar o menu).

Checklist visual do que conferir nas fotos: `client/docs_view_QA.md`.

Ao abrir no Editor, a cena `Assets/_RR/Scenes/Main.unity` é vazia de propósito: o `Game` nasce sozinho.

## Formato de nível (`client/Assets/_RR/Resources/Levels/L001.txt`…)

```
// comentário do designer
h Dica mostrada ao jogador.
q abab          ← fila da 1ª fonte (ordem de leitura); cores a..f = azul, vermelho, verde, amarelo, roxo, laranja
.   Ra2 .   Rb2 .
.   L3r Y01 L2r .
.   .   S0  .   .
```

| Token | Significado |
|---|---|
| `.` | vazio |
| `#` | bloqueio |
| `+` | cruzamento |
| `I0r` / `L2f` | canal reto/curva; giro 0–3 (0 = N, sentido horário); `r` = girável, `f` = fixo |
| `G10` | comporta: giro 0/1 + índice do cristal que a abre |
| `Y01` | alternador: giro 0–3 (rot 0 entra por S, sai a W/E) + seta inicial 0 = esquerda, 1 = direita |
| `S1` | fonte saindo pelo lado 1 |
| `Ra3` | cristal azul de capacidade 3 |

O arquivo descreve o **estado inicial** do nível. O parser recusa nível em que o total de orbes de uma cor ≠ capacidade daquela cor. O portão de conteúdo (`LevelsContentTests`) recusa nível sem solução, que começa resolvido ou com mais de 10 peças tocáveis.

## Playtest (Camada 0): coletar e ler o diário

Cada aparelho grava `diario.csv` em `persistentDataPath`. Para puxar de um Android com o build dev:
```bash
adb pull /sdcard/Android/data/br.com.vstack.runerelay/files/diario.csv playtest/testador01.csv
```
No Windows, o arquivo fica em `%USERPROFILE%\AppData\LocalLow\V-STACK\Rune Relay\diario.csv`.

```bash
python client/tools/diario_report.py playtest/
```
Imprime o funil por nível (inícios, conclusões, tempo mediano, 1º toque, falhas por motivo, quem não apertou LIBERAR) e já aplica o **portão da raia B**: passa com ≥50% terminando o nível 10 e ≥30% falhando ao menos uma vez nos níveis 9–10; refuta com >25% sem apertar LIBERAR no nível 2 ou mediana >3 min nos níveis 8–10. `--autoteste` prova o cálculo com dados sintéticos.

## Gerador de níveis (`client/tools/levelgen`)

```bash
dotnet run --project client/tools/levelgen
```
Regrava L011–L030 (nunca toca L001–L010, feitos à mão). Passe números para regerar só alguns: `dotnet run --project client/tools/levelgen 22 30`.

Como funciona: para cada nível há uma **especificação** em `Program.cs` (grade, fontes, cores, nº de alternadores, comportas, cruzamentos, giráveis, faixa de toques, peças falsas, dica). O gerador **carva os caminhos já resolvidos** (fonte → canais → cristal), põe cruzamento onde dois caminhos se cruzam, alternador onde a fila deve se separar (um alternador duplo separa 3 cores) e comporta ligada a um cristal de outra fonte (com 2+ comportas, em cadeia: links sempre descem de fonte, então nunca trava). As **filas** saem da simulação do destino de cada orbe. Depois ele embaralha as peças giráveis para fora da solução, recorta a grade ao conteúdo e **prova pelo mesmo `Solver` do jogo**: solução única, não começa resolvido, toques dentro da faixa, ≤10 peças tocáveis, ≤2¹⁹ combinações. Sementes partem de `N×1000`, então o mesmo arquivo sai sempre igual.

Para mudar um nível: edite a especificação e regere; para um nível à mão, escreva o `.txt` e deixe o portão de conteúdo (`LevelsContentTests`) reprovar o que estiver errado.

## Decisões (log)

- **2026-10-06 — core reespecificado** (raia B §6): fila de orbes + alternador (o "sort" de verdade) + comporta (cadeia); falha = cor errada, derrame, transbordo ou travamento.
- **2026-10-06 — comporta fechada SEGURA o orbe** (decisão do coordenador; a raia B propôs derramar). A cadeia vira dependência legível em vez de puzzle de tempo. Reverter se o playtest pedir.
- **2026-10-06 — Built-in Render Pipeline + Input System só** (sem URP): é 2D com sprites. Pacotes mínimos: inputsystem, ugui, test-framework e módulos.
- **2026-10-06 — níveis em texto puro**, não JSON: o `RR.Core` não depende do `JsonUtility`, e o mesmo arquivo roda no `dotnet test` e no Unity.
- **2026-10-06 — níveis 11–30 gerados por construção**, com solução única exigida pelo portão de conteúdo. Peças falsas são sempre fixas (uma girável fora do caminho criaria 2ª solução).

## Próximos passos (semana 2, plano da raia C)

1. **Playtest Camada 0** com 10–20 pessoas no APK (`client/Builds/android/RuneRelay-dev.apk`). Hipótese da raia B:
   - **Passa:** ≥50% terminam o nível 10 por vontade própria, e ≥30% falham pelo menos uma vez nos níveis 9–10.
   - **Refuta (ir para o plano B, Skyhold-lite):** mais de 25% não apertam LIBERAR sozinhos no nível 2, ou a mediana dos níveis 8–10 passa de 3 min.
   - Ler os diários com `python client/tools/diario_report.py playtest/` (ver seção Playtest).
2. **8–10 criativos de 9:16** (`adb shell screenrecord`) mostrando orbes fluindo e se ordenando. O gate de marketability vem antes de qualquer meta.
3. ~~Gerador de níveis~~ feito (30 níveis). Próximo: afinar a curva com o `diario.csv` do playtest e gerar L031+ se o D7 pedir.
4. **v0.2, só depois do gate M:** moedas, booster "Revelar runa" (o `-solve` já é o motor), rewarded "+1 liberação" (`Session.ExtraRelease` já existe), tela de restauração, AAB release e teste interno/fechado no Play.


## Repositório e licença
- Mudanças: `CHANGELOG.md` e Pull Requests; versões como tags `vX.Y.Z` com APK em Releases.
- Regras: `main` sempre verde; trabalho em `feat/…`/`fix/…` via PR; commits em português com escopo (`feat(core): …`).
- © V-STACK / Vinicius Souza. **Todos os direitos reservados.** Código visível para acompanhamento, sem licença de uso, cópia ou redistribuição.
