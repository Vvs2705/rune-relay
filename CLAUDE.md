# Instruções para o Claude — Rune Relay

Este repositório é **só do Rune Relay** (GitHub `Vvs2705/rune-relay`). Cada chat aberto aqui cuida **apenas deste jogo**.

## Escopo
- Leia e altere apenas arquivos deste repositório. Não mexa nos outros jogos do estúdio, na pasta raiz `JOGOS NOVOS` nem em outros repositórios, a não ser que o Vinicius peça explicitamente neste chat.
- Se o pedido for sobre outro jogo, avise e sugira abrir o chat no grupo daquele jogo no menu lateral.
- Os documentos deste repositório descrevem só o Rune Relay (nada de portfólio com os outros jogos).

## Como trabalhar
- Português brasileiro. Código do jogo em `client/Assets/_RR/` (núcleo em C# puro em `Scripts/Core`, testável fora do Unity).
- Git: trabalho em branch (`feat/`, `fix/`, `docs/`, `art/`), commits em português com escopo (ver `CONTRIBUTING.md`), entra por Pull Request na `main`. Merge, push na `main`, tags e Releases só com aval do Vinicius neste chat.
- PC com 7,7 GB de RAM: um processo pesado por vez (Unity **ou** emulador **ou** Blender).
- Guardrails do estúdio: 1 moeda no MVP; offline com teto de 2 h; nada de item aleatório pago (ECA Digital, Lei 15.211/2025); interstitial desligado na v0.1; criativos gravados do jogo real; créditos de IA (Tripo etc.) só com aval explícito.
- Memória do estúdio (leitura; atualize só a parte deste jogo): `C:\Users\VINICIUS\Videos\MEUS PROJETOS\Agentes\_memoria\` — `STATUS_jogos-novos-validacao.md` e, se existir, o arquivo deste projeto.

## Situação
- Estado: v0.1 jogável (30 níveis gerados com prova de solução única, APK de desenvolvimento). O portfólio decidiu terminar o Forge Street antes; só avance aqui quando o Vinicius pedir.
- Decisão pendente do Vinicius: o "Recomeçar" devolve as 3 liberações (3★ grátis, rewarded esvaziado) — opções A/B/C no `docs/GDD.md` §6.1.
- Comece por: `README.md`, `docs/GDD.md` (adendo e revisão de 2026-10-09 no topo), `docs/COMPETITIVO.md`, `CHANGELOG.md`.
