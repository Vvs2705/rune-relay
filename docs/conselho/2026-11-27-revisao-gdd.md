# Ata do Conselho — Rune Relay: revisão do GDD (2026-11-27)

> Aberta por: producer · Estado: DECIDIDA · Rodadas simuladas inline (um executor seguindo os papéis do `Vvs2705/time-jogos`), não são opiniões independentes.

## 0. Entrada
- **Pedido:** "trabalhe em cima dos GDDs, analise se o jogo faz sentido, as stacks, mecânicas, veja o que funcionou nos outros jogos e pode ser aplicado. Pode melhorar, criar, adicionar, mexer no que for necessário."
- **Jogo:** Rune Relay (`Vvs2705/rune-relay`) · branch `docs/conselho-2026-11-27`

## 1. Triagem (7 eixos)
| Eixo | Leitura | Evidência |
|---|---|---|
| Utilidade | Alta: único puzzle do portfólio, 1º do ranking unificado (8,11) | `00_PESQUISA/VEREDITO_VALIDACAO.md` |
| Funcionalidade | Core provado em código: 30 níveis com solução única, autoplay 30/30 | `README.md` |
| Viabilidade | Custo zero de arte (procedural); WebGL viável no mesmo projeto | `Scripts/View/` sem asset |
| Aplicação | Falta só gente jogando | nenhum `diario.csv` |
| Necessidade | É agora: o resto do portfólio espera a Play; este pode testar por link | GO_NOGO do Little Terminal, STATUS do DC |
| Ajustes | Opção A do §6.1; tour do DC; `-testsession`; vSync 0 | GDD §6.1, seção de lições |
| Melhorias | Camada 0 por WebGL fura o bloqueio da Play Console | verificação de desenvolvedor no Brasil desde 30/09/2026 (veredito) |

**Veredito: SEGUIR COM AJUSTES.**

## 2. Rodada 1
- **creative-director:** o core "planejar e assistir" é a identidade; o risco é o jogador não apertar LIBERAR. A opção A preserva o valor da estrela sem vidas. Veto: qualquer timer ou vida no MVP.
- **technical-director:** WebGL de um jogo 2D procedural é o build mais barato que temos; medir carga e FPS no aparelho de entrada antes de chamar testador. Veto: misturar a troca para o 27f1 com o build do playtest.
- **producer:** 6 PRs pequenos, na ordem do §29.2; a Camada 0 é o critério de pronto desta fase. Veto: abrir a v0.2 antes da Camada 0.

## 3. Rodada 2
Sem divergência. O TD aceita o tour antes do playtest (o CD pediu); o CD aceita adiar o 27f1 (o TD pediu).

## 4. Decisão
Consenso. Plano no GDD §29. ADR não necessário (nenhuma mudança de arquitetura; o build WebGL é um método novo no `Setup.cs`).

## 5. Despacho
| # | Raia | Entrega | Depende de |
|---|---|---|---|
| 1 | lead-programmer | PR #3 fechado | — |
| 2 | gameplay-programmer | opção A + teste vermelho | 1 |
| 3 | build-ci | `Setup.BuildWebGL` + Cloudflare Pages + "Copiar diário" | 1 |
| 4 | ux/gameplay | tour do DC | 2 |
| 5 | qa-lead | Camada 0 com 15–20 pessoas | 3, 4 |

Nenhuma raia foi iniciada nesta sessão (pedido era de GDD). Fica pronta para o próximo "pode seguir".
