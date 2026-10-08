# Changelog — Rune Relay

Formato [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/), versões [SemVer](https://semver.org/lang/pt-BR/). Histórico anterior ao repositório reconstruído dos documentos.

## [Unreleased]
### Adicionado
- Flags de dev `-record` e `-demo` para gravar gameplay real em 9:16 (quadros 1080×1920 + áudio, tempo fixo); 4 criativos de UA (#1, #5, #7 e #10 do GDD §17) em `client/Builds/creatives/`, receita em `docs/CRIATIVOS.md`.

## [0.1.0] — 2026-10-06
### Adicionado
- Núcleo C# puro (Board, FlowSim, Solver, Session/Progress) com 46 testes.
- Gerador de níveis por construção com prova de solução única pelo Solver; 30 níveis (`client/Assets/_RR/Resources/Levels/`).
- View v2: menu, tutorial, efeitos e SFX procedurais; builds Windows e Android (APK dev).
