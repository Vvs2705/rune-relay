> **Revisão 2026-10-09 (comparativo de mercado):** o Rune Relay foi comparado com 9 similares reais de 2025–2026 (tabela, matriz de recursos, reclamações e fontes em [`COMPETITIVO.md`](COMPETITIVO.md)). O que mudou neste GDD:
> 1. **Posicionamento (§1):** "fluxo" sozinho deixou de ser diferencial. O Pixel Flow! (Loom/Scopely) ocupa o "flow sort" e lidera o hybrid-casual de puzzle no H1/2026. O que é nosso: **planejar a rota e assistir à separação**, com o alternador dividindo uma fila misturada, sem sorte e com solução provada pelo solver. O alternador não é inédito (Trainyard, premium de 2010), mas nenhum hit hybrid-casual de 2025–2026 o usa.
> 2. **Contradições com o adendo de 2026-10-06 resolvidas no próprio texto (§2, §3, §6, §7, §9):** undo, preview e medidor de sobrecarga riscados; 1 moeda no MVP; interstitial desligado na v0.1; boosters fora de qualquer corrida/ranking; baú só com conteúdo fixo.
> 3. **Exploit achado no protótipo (§6.1):** "Recomeçar" devolve as 3 liberações, então 3★ (e as moedas por estrela) saem de graça e o rewarded "+1 liberação" quase não vale nada. Proposta: o contador de liberações do nível persiste até a vitória, e uma sequência "Fluxo Perfeito" dá valor ao rewarded sem criar vidas.
> 4. **KPIs e kill criteria recalibrados (§15.1, §26)** pela régua do veredito: D1 ≥ 26%, D7 ≥ 5%, coorte de 1,5–2 mil installs, cancelar só após 2 iterações; CPI-alvo US$0,60 (cancelar acima de US$1,20 em 8–10 criativos).
> 5. **Novas seções no fim:** §27 Diferenciais competitivos (7 propostas, 3 recomendadas) e §28 Plano de validação. Nenhuma seção foi renumerada ou apagada; texto superado ficou ~~riscado~~.
> 6. A hard currency "Crystals" colide com o receptor "cristal" do core: renomear antes de existir (v0.2+).

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
**Diferencial (revisão 2026-10-09):** fluxo contínuo já existe no líder (Pixel Flow!). O nosso é **programar e assistir**: o jogador monta a rota antes, toca LIBERAR e vê a fila misturada se separar no alternador. Tudo é visível e determinístico (zero sorte), e cada nível tem solução única provada pelo solver. Ver §27.  
**Fantasy:** restaurar um observatório arcano quebrado, uma máquina por vez.  
**Referências funcionais, não copiáveis:** Color Block Jam, Screwdom, All in Hole, jogos de flow/sort e lógica espacial.  
**Concorrentes diretos (revisão 2026-10-09):** Pixel Flow! (ameaça nº 1), Magic Sort, Water/Ball Sort, Flow Free, Screwdom, Color Block Jam, Hexa Sort, Parking Jam e Trainyard (o único com chave que vira a cada passagem, como o nosso alternador). Comparativo em `COMPETITIVO.md`.

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
- ~~drag curto: mover peça em slots permitidos;~~
- ~~hold: preview do fluxo;~~
- ~~botão undo;~~
- booster opcional.

**Regra vigente (adendo 2026-10-06):** tap gira canal ou troca a seta inicial do alternador; botão **LIBERAR** (3 por nível); segurar acelera a cascata. Sem drag, sem preview e sem undo: o planejamento é grátis e a falha devolve o tabuleiro.

## Vitória
Todos os receptores necessários preenchidos sem overflow final.

## Derrota
- ~~medidor de sobrecarga enche;~~
- ~~movimentos acabam em níveis específicos;~~
- tempo apenas em eventos, não na campanha inicial.

**Regra vigente:** uma liberação falha por cor errada, derrame, transbordo ou travamento; o tabuleiro volta (giros mantidos) e gasta 1 de 3 liberações. Sem liberações, o nível acaba (ver §6.1 para o que acontece depois).

## Onboarding
Tutorial contextual; primeira ação em <10 s; nenhum texto longo.

## Sessão típica
3–8 min, 3–10 níveis.

## Dificuldade
Novas regras entram uma por vez:
1. cor;
2. cruzamento;
3. comporta;
4. ~~mistura proibida;~~ (fora do MVP)
5. ~~temporizador local;~~ (fora do MVP; conflita com "tempo só em eventos")
6. ~~receptor com ordem;~~ (fora do MVP)
7. ~~portais.~~ (fora do MVP)

**Ordem vigente (L001–L030):** girar → cores → cruzamento → armadilha de cor → comporta (cadeia) → alternador (sort) → chefes. Próxima regra candidata, só se o D7 pedir: alternador de 3 saídas ou fonte com fila oculta parcial (testar uma por vez).

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

> **Revisão 2026-10-09:** a curva implementada é a da validação de design de 2026-10-06 (raia B): níveis `L001`–`L010` em `client/Assets/_RR/Resources/Levels/`, com tempo-alvo e falha-alvo por nível; o README resume a ordem. Onde este roteiro fala em undo, sobrecarga e "desfazer a última sobrecarga", leia: a falha devolve o tabuleiro e gasta 1 liberação; a 1ª oferta de rewarded é **+1 liberação** no nível 9. O **Desafio do Dia** (§27, D1) aparece depois do nível 10, no lugar da "missão diária simples".

# 4. PRIMEIRO DIA

Conteúdo D0/D1:
- 35–50 níveis;
- 3 famílias mecânicas;
- 2 salas do observatório;
- 1 mini-coleção;
- 5 missões;
- 1 evento curto opcional;
- **Desafio do Dia** com resultado compartilhável (§27, D1; revisão 2026-10-09).

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

**Crystals:** hard currency opcional após 0.2. *(Revisão 2026-10-09: o nome colide com o receptor "cristal"; renomear. Só existe depois de haver ralo no mapa de fontes e ralos. Guardrail do estúdio: 1 moeda no MVP.)*
Fontes limitadas por gameplay + IAP.
Sinks: bundles/boosters/evento.

## Boosters
- Preview Flow;
- Freeze Overload;
- Swap Rune;
- Undo+.

Evitar economia complexa no MVP.

## 6.1 Revisão 2026-10-09 — economia do MVP e o exploit do "Recomeçar"

**No MVP (v0.2):** 1 moeda (Coins); 1 booster, **Revelar runa** (o solver gira 1 peça para a orientação da solução e a trava), 100 moedas, 3 grátis; os outros 4 boosters acima ficam para depois. Moeda só na 1ª vitória ou quando a estrela melhora (10 + 5×★), com id de transação `nivel:estrela` para não duplicar com reload.

**Exploit (achado lendo `Session.Restart` do protótipo):** recomeçar zera o contador e devolve 3 liberações. Quem falha recomeça e tenta de novo valendo 3★. Efeitos: estrela não mede nada, a moeda por estrela vira farm, e o rewarded "+1 liberação" só poupa refazer os giros (opt-in vai sair baixo).

Opções (a decisão é do Vinicius; custo e teste de cada uma):

| Opção | Como funciona | Custo | Risco | Teste que decide |
|---|---|---|---|---|
| **A (recomendada)** | O contador de liberações do nível persiste até a vitória (inclusive ao sair e voltar). Recomeçar continua grátis, mas a estrela máxima cai a cada liberação gasta. | P (≤ 1 dia no `Session`/`Progress` + 1 teste negativo) | Jogador sente que "perdeu" a 3★ | Playtest Camada 0: taxa de recomeço × abandono nos níveis 8–10 |
| **B** | A + **Fluxo Perfeito**: sequência de níveis vencidos na 1ª liberação multiplica as moedas (×1,5 / ×2 / ×3). O rewarded "+1 liberação" **protege a sequência**. Sem vidas. | P (≤ 3 dias) | Pressão de sequência irrita casual | A/B na v0.2: opt-in do rewarded e D7 com e sem sequência |
| **C** | Vidas (5, recarga por tempo), padrão do gênero | M | Vidas são reclamação recorrente (Magic Sort: 30 s de anúncio por 1 vida; ver `COMPETITIVO.md`); lê relógio do aparelho | Só se B não monetizar: A/B por RemoteConfig com 2 coortes de 2 mil |

**Exploit catalog mínimo (teste negativo para cada um):** recomeçar e reganhar 3★; sair do nível e voltar; reload logo depois da concessão de moedas; rewarded concedido duas vezes (id de transação); Desafio do Dia rejogado mudando a data do aparelho (o desafio não dá moeda, só sequência visual; ver §27).

# 7. MONETIZAÇÃO

## Rewarded ads
- continuar após falha;
- dobrar coin reward 1–2x/dia;
- booster pré-nível opcional;
- baú diário extra *(revisão 2026-10-09: conteúdo fixo e visível antes de assistir; nada aleatório — ECA Digital, Lei 15.211/2025)*;
- *(revisão)* proteger a sequência "Fluxo Perfeito" (§6.1, opção B). Teto: 5 rewarded de moeda por dia.

## Interstitial
**Revisão 2026-10-09: desligado na v0.1** (para medir a retenção limpa). Na v0.2, entra via RemoteConfig depois do nível 12–15, com intervalo ≥ 90 s, e só se o D1 da coorte sem interstitial já estiver medido.

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
- Teamless Race: leaderboard assíncrono por grupo. *(Revisão 2026-10-09: boosters pagos desligados em qualquer corrida ou ranking; sem isso é pay-to-win.)*
- *(Revisão)* **Desafio do Dia**: 1 nível por data, igual para todos, gerado offline e embarcado no build (365 níveis/ano pelo gerador), resultado compartilhável em texto. Detalhe em §27, D1.

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

> **Revisão 2026-10-09 (estado real, v0.1):** o núcleo implementado é menor que a lista acima: `Board`, `FlowSim`, `Solver`, `Session`/`Progress` em C# puro (46 testes, `dotnet test` sem abrir o Unity), níveis em **texto** (não JSON/ScriptableObject) e gerador por construção em `client/tools/levelgen` com prova de solução única. Os managers de Ads/IAP/RemoteConfig/LiveOps só nascem na v0.2. Os diferenciais da §27 foram escolhidos para reusar esse núcleo: o solver já sabe a solução (dica honesta, selo de mestre) e o gerador já produz nível com prova (Desafio do Dia, editor).

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
- *(revisão 2026-10-09)* daily_start / daily_complete{stars,taps} / share_tap{tipo}
- *(revisão)* streak_start / streak_break / streak_protect_rewarded
- *(revisão)* master_seal{level} (resolveu com o mínimo de giros)

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

## 15.1 Revisão 2026-10-09 — gates oficiais de decisão

Os números acima viram meta "forte" (estão acima do top quartil: GameAnalytics 2026 dá D1 mediano ~20% e D7 do top-25% em 6–7%; fonte no veredito). A decisão usa esta régua:

| Métrica | Continuar | Iterar | Cancelar (só após 2 iterações) |
|---|---|---|---|
| CPI no Gate M (BR/PH/ID, 8–10 criativos) | ≤ US$0,60 | US$0,60–1,20 | > US$1,20 (2× o alvo) em todos |
| Tutorial (chegar ao nível 3) | ≥ 88% | 75–88% | < 75% após 1 correção |
| D1 | ≥ 26% | 20–26% | < 20% |
| D7 | ≥ 5% | 3,5–5% | < 3,5% |
| Rewarded opt-in (v0.2) | ≥ 35% | 20–35% | não cancela sozinho |

- **Coorte:** 1,5–2 mil installs pagos por leitura (com 1 mil, a margem do D7 é ±1,7 pp). Comparar pago com pago.
- **Payer conversion** só na fase C, com ≥ 5 mil installs; na fase B o gate de dinheiro é IAA (opt-in, impressões/DAU, ARPDAU).
- **Camada 0 (playtest, antes de qualquer UA):** passa com ≥ 50% terminando o nível 10 por vontade própria e ≥ 30% falhando ao menos uma vez nos níveis 9–10; refuta com > 25% sem apertar LIBERAR no nível 2 ou mediana > 3 min nos níveis 8–10 (`client/tools/diario_report.py` já aplica).
- **KPIs dos diferenciais** (HIPÓTESE, medir na v0.2): ver §28.

# 16. TESTE DE MERCADO

Fase A — creative-only:
- 6–10 vídeos;
- Brasil/Filipinas/Indonésia;
- 3–5 mil cliques combinados se orçamento permitir.

Fase B — ~~1.000~~ 1.500–2.000 installs *(revisão 2026-10-09: margem do D7)*:
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

> **Revisão 2026-10-09:** #1, #5, #7 e #10 já foram gravados do build real (`docs/CRIATIVOS.md`). Contra o Pixel Flow!, os próximos têm de abrir com o que ele não tem: **fila misturada ●○●○ → alternador → duas cores separadas** nos 2 primeiros segundos. Novos ganchos: **#11 "Sem sorte"** (mesmo nível, mesma solução, sempre funciona) e **#12 "Desafio de hoje"** (resultado compartilhável na tela). Testar também orbes "físicos" (bolinhas de gude) contra mana abstrata, como pediu a raia A.

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

**(Revisão 2026-10-09) O líder do subgênero define a régua.**  
O Pixel Flow! (Loom/Scopely, US$198,4M de gasto bruto no 1º ano) e o Magic Sort (> US$40M no 1º ano) fixam o polish e o volume de níveis esperados (fontes em `COMPETITIVO.md`). Mitigar com o hook que eles não têm (alternador/sort por rota), Desafio do Dia barato de manter e gerador com prova, que produz nível sem custo de autoria. Não competir em volume de LiveOps.

**(Revisão) Parecer clone do Pixel Flow!**  
A Loom processou um estúdio que clonou o Pixel Flow! e os clones saíram das lojas. Não usar esteira em volta de arte pixel, "atirador" com munição nem slots de espera com a mesma leitura; o nosso visual é rota + cristal + alternador.

**(Revisão) "Planejar e assistir" pode ler como lento no vídeo.**  
O gancho de 3 s precisa mostrar o orbe já correndo. Os criativos começam no LIBERAR, nunca no planejamento.

**(Revisão) Monetização sem atrito.**  
Sem vidas e com recomeço grátis, o rewarded perde valor (§6.1). Corrigir o exploit antes de ligar anúncios.

---

# 26. KILL CRITERIA

> **Revisão 2026-10-09:** números recalibrados pela régua do veredito (§15.1). O valor antigo ficou riscado.

## CONTINUAR
- CPI dentro da faixa-alvo do geo (≤ US$0,60);
- tutorial ≥88%;
- D1 ~~≥30%~~ ≥ 26%;
- D7 ~~≥8% com 1k+ coorte~~ ≥ 5% com coorte de 1,5–2 mil;
- rewarded opt-in ≥35%;
- criativos conseguem gerar múltiplos hooks vencedores.

## ITERAR
- D1 ~~24–30%~~ 20–26%;
- D7 ~~5–8%~~ 3,5–5%;
- CPI 20–50% acima do target;
- boa retenção, mas baixa monetização;
- core bom, mas fail curve ruim.

Limite: **duas grandes iterações** antes de nova decisão.

## CANCELAR
- tutorial <75% após uma correção;
- D1 ~~<24%~~ < 20% em coorte confiável, após duas iterações;
- D7 ~~<5%~~ < 3,5% após duas iterações;
- CPI >2× target em 8–10 criativos distintos;
- jogadores não entendem o hook em vídeo;
- produção de nível não consegue ser reduzida a pipeline modular.


---

# 27. DIFERENCIAIS COMPETITIVOS (revisão 2026-10-09)

Resumo. O porquê, o custo detalhado, os riscos e a validação de cada um estão em [`COMPETITIVO.md`](COMPETITIVO.md) §4. Status de todos: **PROPOSTO** (nada foi medido).

| # | Diferencial | O que o líder faz | Custo | Recomendado |
|---|---|---|---|---|
| D1 | **Desafio do Dia compartilhável**: 1 nível por data, igual para todos, embarcado no build (sem backend); o resultado vira texto de 3 linhas (estrelas, giros, sequência) para WhatsApp | Só o Flow Free tem puzzle diário; não achamos resultado compartilhável em nenhum líder | P | **Sim (3º)** |
| D2 | **Jogo honesto**: zero sorte (fila visível, simulação determinística), zero paywall (todo nível provado solúvel sem booster), joga offline, e todo anúncio é gameplay real gravado do build | Muro de dificuldade que só passa com booster (Pixel Flow!, Screwdom, Magic Sort); Pixel Flow! não joga offline; Magic Sort esconde cores; anúncio enganoso é alvo de reguladores | P | **Sim (2º)** |
| D3 | **Alternador como assinatura do sort**: a fila misturada se separa sozinha pela seta; vira ícone, gancho de 2 s e nome da regra | Pixel Flow! ordena por fila + capacidade, sem rota; a chave que vira a cada passagem só existe no Trainyard (premium, 2010) | P | **Sim (1º)** |
| D4 | **Fluxo Perfeito sem vidas**: sequência de vitórias na 1ª liberação multiplica moedas; o rewarded protege a sequência (§6.1, opção B) | Vidas e oferta na falha (Magic Sort: 30 s de anúncio por 1 vida; Pixel Flow!: "Fail Offer" de US$5,99) | P | Na v0.2 |
| D5 | **Selo de Mestre**: resolver com o mínimo de giros (o solver já calcula `MinTaps`), camada de maestria para o público de lógica | Não achamos nos líderes pesquisados | P | Na v0.2 |
| D6 | **Editor + código de nível**: o jogador monta um nível, o solver exige solução única e o nível vira um código de texto compartilhável (sem servidor) | Nenhum líder tem editor; Pixel Flow! é criticado por reciclar níveis | M | Depois do D7 ≥ 5% |
| D7 | **Acessível por padrão**: cada cor tem forma própria (já implementado), sem timer, jogável com uma mão | Modo daltônico é pedido recorrente em sort; só o Flow Free tem rótulos | P | Já pronto; vira argumento de loja |

**Por que estes três primeiro:** D3 ataca o primeiro gate que pode matar o jogo (CPI no Gate M), ocupa o espaço vazio entre o Trainyard e o Pixel Flow! e custa só criativo. D2 transforma em promessa pública o que o núcleo já garante e responde de uma vez a quatro reclamações recorrentes dos líderes; eles não podem copiar sem perder receita. D1 é o único que traz instalação orgânica e motivo de voltar todo dia sem backend, e reusa o gerador.

**O que NÃO fazer para "competir":** volume de LiveOps, vidas logo de cara, booster pago em corrida, item aleatório pago, hard currency no MVP.

# 28. PLANO DE VALIDAÇÃO DOS DIFERENCIAIS (revisão 2026-10-09)

| # | Hipótese (refutável) | Como validar barato | Passa se | Refuta se |
|---|---|---|---|---|
| D3 | O gancho "fila misturada → alternador separa" tem CPI menor que o "cascata genérica" | Gate M: 2 criativos D3 contra o #5 atual, mesmo orçamento e geo | CPI do D3 ≤ 0,85× o do #5 | CPI do D3 ≥ o do #5 nos dois geos |
| D1 | Desafio do Dia traz retorno e compartilhamento | v0.2: entra para metade da coorte (RemoteConfig) | ≥ 25% dos DAU jogam o desafio e D7 da metade com desafio ≥ +1 pp | < 10% jogam ou D7 igual |
| D2 | A promessa "jogo honesto" (sem sorte, sem paywall, offline) melhora a conversão da loja | Teste de listing (screenshot/descrição) no Play Console | Conversão da loja +10% relativo | Sem diferença em 2 semanas |
| D4 | Sequência dá valor ao rewarded sem derrubar retenção | A/B v0.2 (§6.1 opção A × B) | Opt-in ≥ 35% e D7 não cai | D7 cai > 0,5 pp |
| D5 | Selo de Mestre aumenta níveis por sessão do público de lógica | Playtest Camada 1 + telemetria `master_seal` | ≥ 15% dos jogadores ativos buscam selo em ≥ 5 níveis | < 5% |
| D6 | Jogadores criam e trocam níveis | Só depois do D7 ≥ 5%: protótipo de editor com 10 testers | ≥ 3 de 10 criam um nível válido sem ajuda | Nenhum cria |

Instrumentação já prevista na §14 (revisão). Nada aqui roda antes do playtest Camada 0 e do Gate M.
