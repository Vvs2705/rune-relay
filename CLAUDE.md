# Instruções para o Claude — Rune Relay

Este repositório é **só do Rune Relay** (GitHub `Vvs2705/rune-relay`). Cada chat aberto aqui cuida **apenas deste jogo**.

Regras comuns dos jogos do estúdio (escopo, git, guardrails, processo, arte, testes), editadas num lugar só:
@~/.claude/estudio/REGRAS_JOGOS_ESTUDIO.md

Se ele não carregar (outro PC, sessão na nuvem), as regras do estúdio versionadas ficam no repo `Vvs2705/time-jogos` (`CLAUDE-TIME.md` e `docs/PRATICAS_APROVADAS.md`; cópia local em `C:\Users\VINICIUS\Videos\MEUS PROJETOS\TIME DE JOGOS\time-jogos`). Se nenhum dos dois carregar, o mínimo é: só este jogo; mudança entra por PR na `main`; nada de item aleatório pago; anúncio sem `AD_ID`; um processo pesado por vez.

## Deste jogo
- Código do jogo em `client/Assets/_RR/` (núcleo em C# puro em `Scripts/Core`, testável fora do Unity).

## Situação
- Estado: v0.1 jogável (30 níveis com solução única, APK dev). Conselho de 2026-11-27: **SEGUIR COM AJUSTES**; o próximo passo é a Camada 0 por build WebGL, na ordem do `docs/GDD.md` §29.2.
- §6.1 decidido pelo conselho: **opção A** (contador de liberações persiste até a vitória); o Vinicius pode trocar por B ou C.
- Comece por: `docs/GDD.md` §29, `docs/conselho/2026-11-27-revisao-gdd.md`, `README.md`, `CHANGELOG.md`.
