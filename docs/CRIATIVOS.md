# Criativos de UA 9:16 — Rune Relay

Primeira leva para o gate de marketabilidade (GDD §17; README, "Próximos passos" item 2). Tudo gravado do jogo de verdade (build Windows), sem IA paga, sem upload, nada publicado. 1080×1920, 30 fps, H.264 + AAC (SFX do próprio jogo), cartão final "RUNE RELAY", sem selo de loja.

| # GDD | Arquivo (`client/Builds/creatives/`) | Gancho (0–2 s) | Duração | Níveis | Hipótese que testa |
|---|---|---|---|---|---|
| 5 "Satisfying chain" | `rr_05_cascata.mp4` | sem texto: as peças encaixam e a cascata começa no 1º segundo | 17,4 s | L029 → L026 → L025 | O fluxo de orbes enchendo cristais prende sozinho, sem regra explicada (thumb-stop / CTR de "ASMR visual"). |
| 1 "Só gire uma peça" | `rr_01_gire_uma_peca.mp4` | **SÓ GIRE UMA PEÇA** | 11,5 s | L030 | A regra se entende em menos de 2 s, e o "quase deu → um toque resolve" (orbe vermelho no cristal verde, depois a cascata) dá vontade de jogar. |
| 7 "Fake easy → real hard" | `rr_07_facil_dificil.mp4` | **NÍVEL 1 / FÁCIL** | 17,8 s | L001 → L010 → L030 | A promessa de profundidade (tabuleiro crescendo de verdade) atrai quem gosta de puzzle de lógica, e não só o casual. |
| 10 "Você faria melhor?" | `rr_10_voce_faria_melhor.mp4` | **ACHA FÁCIL?** → CTA **VOCÊ FARIA MELHOR?** | 10,9 s | L028 | O fail-ad (desafio ao ego) dá CTR mais barato que o payoff. Risco: installs de pior qualidade, então comparar o D1 com o #1. |

Ler na Fase A (§16): CTR e CPI por criativo; secundárias: views de 3 s (thumb-stop) e retenção até o fim. Cada vídeo tem uma folha de contato ao lado (`rr_0X_contato.png`, 1 quadro a cada 2 s).

## Como reproduzir

```bash
# 1. build (o mesmo de sempre)
"/c/Program Files/Unity/Hub/Editor/6000.3.23f1/Editor/Unity.exe" -batchmode -nographics -quit -projectPath "$(pwd -W)/client" -executeMethod RR.EditorTools.Setup.BuildWindows -logFile "$(pwd -W)/client/Builds/build_win.log"
# 2. grava os 8 clipes e monta os 4 vídeos (~7 min). A janela do jogo PRECISA aparecer: não minimize.
python client/Builds/creatives/montar.py gravar
# só remontar textos/cortes a partir dos quadros já gravados:
python client/Builds/creatives/montar.py rr_01 rr_10
```

Os quadros ficam em `%TEMP%/rr_rec` (ou na pasta de `RR_REC`), fora do projeto. Cada clipe é uma chamada do executável:

```bash
client/Builds/win/RuneRelay.exe -screen-fullscreen 0 -screen-width 540 -screen-height 960 -level 29 \
  -demo "solve:0 wait:0.5 release wait:1.0" -record "$TEMP/rr_rec/c5a" -logFile "$TEMP/rr_rec/c5a.log"
```

| Clipe | Nível | `-demo` | Vai para |
|---|---|---|---|
| c5a / c5b / c5c | 29 / 26 / 25 | `solve:0 wait:0.5 release wait:1.0` | #5 |
| c1 | 30 | `almost wait:0.6 release solve:0.6 wait:0.5 release wait:1.0` | #1 |
| c7a / c7b / c7c | 1 / 10 / 30 | `solve:0.25` / `solve:0.18` / `solve:0.15`, depois `wait:0.3 release wait:0.8` (c7c: `wait:1.0`) | #7 |
| c10 | 28 | `almost:0.2 wait:0.4 release wait:2.5` | #10 |

### Flags de dev (em `Game.cs`)

- `-record pasta [-recordsec s] [-recordfps n]`: fixa `Time.captureFramerate` (padrão 30) e grava 1 PNG por quadro em 2× a janela (540×960 → 1080×1920), mais o áudio do jogo em float32 cru (`audio_48000_2.f32`, via `AudioRenderer`). Depois sai. `-recordsec` é só um teto (padrão 30); com `-demo`, o vídeo acaba quando o roteiro acaba.
- `-demo "passos"`: roteiro separado por espaço. `wait:s` | `solve[:passo]` (gira até a solução, um toque a cada `passo` s, padrão 0.15, com o anel do tutorial marcando a peça como se fosse um dedo; `0` = tudo de uma vez) | `almost[:passo]` (igual, mas deixa UMA peça a 1 toque da solução, escolhida para falhar por **cor errada** o mais tarde possível; o log diz qual) | `tap:x,y` | `release` (libera e espera a cascata acabar).
- Os 2 primeiros quadros são de antes do roteiro agir, por isso o `montar.py` começa no quadro 2.

### Notas

- Cortar 1,0 s depois da vitória é de propósito: o painel aparece em 1,1 s, e no L030 ele diz "Fim do protótipo!".
- O contador de estrelas no topo vem do `PlayerPrefs` da máquina que gravou, então muda de clipe para clipe.
- A marca "Development Build" não sai na captura em 2× (conferido no canto inferior direito).
- `client/Builds/` está no `client/.gitignore`. Para versionar o `montar.py` (e, se quiser, os MP4, ~8 MB), é preciso liberar `!/Builds/creatives/`. Sem isso, a montagem só existe nesta máquina.
