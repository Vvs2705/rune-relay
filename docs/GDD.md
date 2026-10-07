> **ADENDO 2026-10-06 — core reespecificado (vale sobre §2, §6 e §18).** A validação multiagente (`../../00_PESQUISA/VEREDITO_VALIDACAO.md`, raia B §6) achou que o core abaixo é um puzzle de girar canos sem falha e sem *sort*. Regra em vigor no protótipo: cada fonte tem uma **fila visível de orbes**; o jogador planeja (girar canal é grátis) e toca **LIBERAR** (3 por nível; estrelas = 3/2/1 pela liberação que venceu); o orbe anda 1 célula por tick; **alternador** envia cada orbe para um lado e vira a seta (o *sort* real); **comporta** abre quando o cristal ligado enche, e até lá **segura** os orbes (decisão do coordenador; a raia B propunha derramar). Falha = cor errada, derrame, transbordo ou travamento; os giros do jogador continuam no tabuleiro. Undo, preview pago, sobrecarga, temporizador, mistura proibida, receptor com ordem e portais saem do MVP. Implementação: `../client` (ver `../README.md`).

# GDD 01 — RUNE RELAY
## Hybrid-casual Puzzle / Portrait / Android + iOS

> **Hipótese comercial:** um puzzle visual de fluxo/sort com compreensão em <2 segundos pode adquirir barato; uma meta leve e monetização híbrida podem elevar LTV sem destruir a simplicidade.

# 1. VISÃO DO PRODUTO

**Nome provisório:** Rune Relay  
**Gênero:** Hybrid-casual puzzle / logic-flow / sort  
**Plataforma:** Android primeiro; iOS após validação.  
**Orientação:** Portrait.  
**Público:** 16–55+, casual puzzle, sort/block/logic, jogadores de sessões curtas.  
**Proposta de valor:** conectar fluxos de mana colorida por uma máquina arcana compacta, resolver congestionamentos e assistir a uma cascata satisfatória de energia.  
**Diferencial:** o “sort” acontece por fluxo contínuo e roteamento, não por mover pilhas estáticas.  
**Fantasy:** restaurar um observatório arcano quebrado, uma máquina por vez.  
**Referências funcionais, não copiáveis:** Color Block Jam, Screwdom, All in Hole, jogos de flow/sort e lógica espacial.

# 2. GAMEPLAY

## Core mechanic
O tabuleiro contém:
- fontes de mana por cor;
- runas-canal;
- divisores;
- comportas;
- receptores;
- bloqueios.

O jogador toca/arrasta peças permitidas ou rotaciona runas para criar um caminho. A mana começa a fluir. Se destinos corretos forem alimentados, o tabuleiro limpa em cascata.

## Core loop
`observar → redirecionar → liberar fluxo → cascata → recompensa → meta → próximo nível`

## Controles
- tap: rotacionar;
- drag curto: mover peça em slots permitidos;
- hold: preview do fluxo;
- botão undo;
- booster opcional.

## Vitória
Todos os receptores necessários preenchidos sem overflow final.

## Derrota
- medidor de sobrecarga enche;
- movimentos acabam em níveis específicos;
- tempo apenas em eventos, não na campanha inicial.

## Onboarding
Tutorial contextual; primeira ação em <10 s; nenhum texto longo.

## Sessão típica
3–8 min, 3–10 níveis.

## Dificuldade
Novas regras entram uma por vez:
1. cor;
2. cruzamento;
3. comporta;
4. mistura proibida;
5. temporizador local;
6. receptor com ordem;
7. portais.

# 3. PRIMEIROS 10 MINUTOS

**0:00–0:30** — animação curtíssima de observatório apagado. Um cristal pulsa. O jogador toca uma runa; fluxo azul acende a máquina. Primeira vitória em ~20 s.

**0:30–1:30** — níveis 2–3 ensinam rotação e receptor de duas cores. Feedback háptico e visual forte. Sem loja.

**1:30–2:30** — primeira falha controlada: fluxo entra no receptor errado. Undo gratuito ensina correção.

**2:30–3:30** — nível 5 introduz cadeia: energizar um receptor abre um segundo caminho. Primeiro “momento wow”.

**3:30–4:30** — jogador recebe estrelas e restaura a primeira bancada do observatório. Meta apresentada em 20–30 s.

**4:30–5:30** — nível 6 traz nova cor; tutorial por gesto.

**5:30–6:30** — nível 7 usa duas fontes simultâneas; primeira decisão real de ordem.

**6:30–7:30** — nível 8: dificuldade sobe; se falhar, surge primeira oferta de rewarded: “desfazer a última sobrecarga”. Pode recusar sem punição.

**7:30–8:30** — jogador retorna à meta, abre pequeno artefato de coleção e vê o próximo cômodo.

**8:30–10:00** — níveis 9–11; ao terminar, recebe a primeira missão diária simples. Loja/IAP continuam discretos; starter pack só após o jogador provar intenção com ~10 níveis.

# 4. PRIMEIRO DIA

Conteúdo D0/D1:
- 35–50 níveis;
- 3 famílias mecânicas;
- 2 salas do observatório;
- 1 mini-coleção;
- 5 missões;
- 1 evento curto opcional.

Primeira meta: restaurar a Sala de Condução.

Primeiro rewarded: revive/undo após falha, nunca antes de o jogador entender o jogo.

Primeira compra potencial: remove ads + pequeno pacote de moedas após nível 12–15.

Razões para voltar:
- daily reward;
- missão de 24h;
- próxima sala;
- coleção quase completa;
- evento de 3 dias.

# 5. PROGRESSÃO

**Curta:** concluir nível, ganhar estrelas/moedas, restaurar objeto.  
**Média:** completar sala/capítulo e desbloquear novas runas.  
**Longa:** restaurar observatório, coleções, temporadas leves.

- **D1:** 30–50 níveis; 2 salas.
- **D3:** 80–120 níveis; comportas/portais.
- **D7:** 160–220 níveis; evento semanal e coleção.
- **D14:** 300+ níveis se métricas justificarem.
- **D30:** capítulos, desafios e race assíncrona opcional.
- **D60:** novos tiles + evento temático.
- **D90:** segunda ala do observatório e LiveOps reutilizável.

# 6. ECONOMIA

## Moedas
**Coins:** soft currency.
Fontes:
- vitória;
- daily;
- missão;
- rewarded.

Sinks:
- undo extra;
- boosters;
- cosmetic restoration choices leves.

**Crystals:** hard currency opcional após 0.2.
Fontes limitadas por gameplay + IAP.
Sinks: bundles/boosters/evento.

## Boosters
- Preview Flow;
- Freeze Overload;
- Swap Rune;
- Undo+.

Evitar economia complexa no MVP.

# 7. MONETIZAÇÃO

## Rewarded ads
- continuar após falha;
- dobrar coin reward 1–2x/dia;
- booster pré-nível opcional;
- baú diário extra.

## Interstitial
Somente após:
- nível concluído;
- no máximo após 2–4 níveis;
- nunca nos primeiros 5 minutos;
- removido para pagantes/no-ads.

## IAP
- Remove Ads;
- Starter Pack;
- coins/crystals;
- booster bundles;
- event pack quando existir.

Preço relativo:
- starter baixo;
- remove ads baixo/médio;
- bundles 3 faixas.

Sem subscription no lançamento.

# 8. RETENÇÃO

**D1:** primeira sala incompleta + recompensa de retorno.  
**D3:** coleção e primeira mecânica nova.  
**D7:** mini-evento competitivo assíncrono.  
**D14:** capítulo com visual novo.  
**D30:** season board leve somente se D7 saudável.

Daily rewards: 7 dias, sem punição severa por quebra de streak.

# 9. LIVE OPS

Eventos reutilizáveis:
- Relay Rush: completar X níveis em 30 min;
- Crystal Hunt: coletáveis adicionados a níveis normais;
- Perfect Flow: sequência sem falha;
- Teamless Race: leaderboard assíncrono por grupo.

Produção barata: eventos alteram regras/recompensas em conteúdo existente, não exigem mapa novo.

# 10. CONTEÚDO DE LANÇAMENTO

Se passar soft launch:
- 400–600 níveis;
- 8–10 famílias de tiles;
- 6 salas de meta;
- 50–80 artefatos/objetos;
- 15–20 boosters/ofertas combinadas;
- 4 templates de evento;
- ~30 SFX;
- 4–6 músicas curtas/loops.

MVP: apenas 30–40 níveis.

# 11. ARTE

Estilo: stylized clean, arcano, alto contraste.  
Paleta: fundos escuros suaves + mana muito saturada; cor nunca é único sinal — usar símbolos.  
Câmera: 2D/2.5D fixa portrait.  
Assets: peças modulares simples; VFX são mais importantes que personagens.

# 12. ÁUDIO

Essenciais:
- ligar runa;
- fluxo;
- receptor;
- combo;
- sobrecarga;
- vitória;
- meta restoration.

Música discreta, sem competir com feedback.

Haptic:
- encaixe;
- receptor concluído;
- cadeia;
- falha.

# 13. TECNOLOGIA

Unity/C#.

Arquitetura:
- `GameBootstrap`
- `LevelDefinition`
- `BoardRuntime`
- `FlowSolver`
- `LevelManager`
- `SaveManager`
- `EconomyManager`
- `AnalyticsManager`
- `AdsManager`
- `IAPManager`
- `RemoteConfig`
- `LiveOpsEventManager`
- `AudioManager`
- `UIManager`

Levels data-driven em ScriptableObject/JSON validado.

O solver deve permitir simulação determinística para testes.

# 14. ANALYTICS

Eventos:
- session_start/end
- tutorial_step
- tutorial_complete
- level_start
- first_move_time
- level_complete
- level_fail
- fail_reason
- undo_use
- booster_use
- rewarded_offer/show/complete
- interstitial_show
- restoration_step
- currency_source/sink
- iap_view/purchase
- event_enter/complete

Métricas:
- FTUE completion;
- time-to-first-action;
- fail curve;
- levels/session;
- D1/D3/D7/D30;
- rewarded opt-in;
- ads/DAU;
- payer conversion;
- ARPDAU;
- CPI/LTV.

# 15. KPIs — TARGETS INTERNOS

Contexto: alvos de decisão, não benchmarks universais.

**Continue forte:**
- tutorial ≥ 88%;
- D1 ≥ 30%;
- D3 ≥ 16%;
- D7 ≥ 8%;
- D30 ≥ 2.5%;
- ≥4 levels/session no D1;
- rewarded opt-in ≥ 35%;
- payer conversion soft launch ≥ 1.5%;
- crash-free >99.5%.

**UA de protótipo, Android low-cost geo:** CPI inicial ≤ ~US$0,60 é promissor; US$0,60–1,00 exige leitura de LTV; >US$1,00 sem retenção excepcional é ruim. Calibrar por rede/país.

# 16. TESTE DE MERCADO

Fase A — creative-only:
- 6–10 vídeos;
- Brasil/Filipinas/Indonésia;
- 3–5 mil cliques combinados se orçamento permitir.

Fase B — 1.000–2.000 installs:
- Android;
- retention + ad behavior.

Fase C — 3.000–5.000 installs:
- adicionar Canadá/Austrália em amostra menor;
- medir IAP e ARPDAU.

# 17. 10 CRIATIVOS DE UA

1. **“Só gire uma peça”** — 2 s: rota errada explodindo; gameplay: giro correto; payoff: cascata inteira.
2. **“Escolha esquerda ou direita”** — split decision; uma escolha falha.
3. **“Não misture vermelho e azul”** — jogador aparentemente erra; explosão; solução visual.
4. **“99% erram esta ordem”** — sequência de comportas.
5. **“Satisfying chain”** — sem texto; 8 receptores acendendo.
6. **“Salve a máquina em 10 s”** — medidor quase cheio.
7. **“Fake easy → real hard”** — três níveis crescentes reais.
8. **“Um movimento perfeito”** — solução em um gesto.
9. **“Restaurando o observatório”** — antes/depois da meta.
10. **“Você faria melhor?”** — falha proposital seguida de CTA.

# 18. MVP

Necessário:
- 30–40 níveis;
- 4 tiles;
- 1 booster;
- fail/win;
- analytics;
- rewarded test;
- 6 criativos;
- 1 tela de meta fake/leve.

Não necessário:
- 500 níveis;
- battle pass;
- social;
- backend próprio;
- subscription.

# 19. VERSÃO 0.1

- board;
- solver;
- 30 níveis;
- VFX;
- FTUE;
- analytics;
- Android build.

# 20. VERSÃO 0.2

Somente se CPI/FTUE forem aceitáveis:
- 100 níveis;
- meta real;
- rewarded;
- interstitial com cap;
- remove ads;
- RemoteConfig;
- 2 eventos.

# 21. VERSÃO 1.0

Somente se D7/LTV justificarem:
- 400+ níveis;
- 6 salas;
- 4 eventos;
- IAP completo;
- iOS;
- ASO/localization.

# 22. ESTIMATIVA DE PRODUÇÃO

| Área | Peso |
|---|---|
| Programação | Baixo–Médio |
| Game Design | Médio |
| Arte | Baixo |
| UI | Médio |
| Áudio | Baixo |
| Backend | Baixo |
| Analytics | Médio |
| Conteúdo | Médio |

Complexidade relativa: **4/10**.

# 23. REUTILIZAÇÃO

De COE/ARKANA:
- Unity bootstrap/build;
- save;
- analytics wrappers futuros;
- audio;
- Android device lab;
- CI;
- UI safe area;
- pooling/VFX.

Criar framework compartilhado:
`MobileCore` = Save + Analytics + Ads + IAP + RemoteConfig + Audio + Haptics + Consent + BuildConfig.

# 24. IA

Usar para:
- ideação de níveis;
- geração/variação de ícones temporários;
- QA de combinações;
- programação assistida;
- localization;
- variantes de copy;
- storyboard de anúncios;
- análise de reviews.

Não usar IA em runtime.

# 25. RISCOS E MITIGAÇÃO

**Puzzle parece derivativo.**  
Mitigar com fluxo contínuo e linguagem arcana própria.

**Níveis caros de produzir.**  
Criar editor/validator e geração assistida, sempre revisão humana.

**Interstitial destrói retenção.**  
RemoteConfig + cap + primeiro ad tardio.

**Meta fraca.**  
Só aprofundar se core já retiver.

---

# 26. KILL CRITERIA

## CONTINUAR
- CPI dentro da faixa-alvo do geo;
- tutorial ≥88%;
- D1 ≥30%;
- D7 ≥8% com 1k+ coorte;
- rewarded opt-in ≥35%;
- criativos conseguem gerar múltiplos hooks vencedores.

## ITERAR
- D1 24–30%;
- D7 5–8%;
- CPI 20–50% acima do target;
- boa retenção, mas baixa monetização;
- core bom, mas fail curve ruim.

Limite: **duas grandes iterações** antes de nova decisão.

## CANCELAR
- tutorial <75% após uma correção;
- D1 <24% em coorte confiável;
- D7 <5% após duas iterações;
- CPI >2× target em 8–10 criativos distintos;
- jogadores não entendem o hook em vídeo;
- produção de nível não consegue ser reduzida a pipeline modular.
