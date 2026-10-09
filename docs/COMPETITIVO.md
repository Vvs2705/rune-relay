# Rune Relay — Comparativo competitivo

**Data:** 2026-10-09 · **Status:** PROPOSTO (pesquisa de mesa; nenhum criativo, playtest ou coorte foi medido) · **GDD:** [`GDD.md`](GDD.md) (revisão 2026-10-09 no topo).

**Legenda:** FATO = número com fonte e data · INFERÊNCIA = deduzido de fatos · OPINIÃO = julgamento de design. O que não foi confirmado está escrito **não confirmado**. Downloads e notas da Google Play vêm do espelho androidrank.org (a página da Play bloqueou a leitura direta), consultado em 09/10/2026. As fontes estão numeradas no fim.

**Pergunta que este documento responde:** o sort é o subgênero mais quente do hybrid-casual e tem um líder financiado (Pixel Flow!). O que o Rune Relay pode ter que os líderes não têm, barato para o nosso time e difícil de copiar?

---

## 1. Os similares

| Jogo | Estúdio | Lançamento | Downloads (Play) | Nota (nº aval.) | Receita / escala | Mecânica | Monetização | Fontes |
|---|---|---|---|---|---|---|---|---|
| **Pixel Flow!** | Loom Games (Istambul); Scopely com a maioria desde fev/2026 | ago/2025 na loja; escala global ~out/2025 (data **não confirmada**) | 10M+ (34M somando Play e iOS, AppMagic) | 4,44 (228 mil); iOS 4,6 (65,9 mil) | US$198,4M de gasto bruto no 1º ano; pico de US$28,3M/mês [2]. US$105M de IAP até mai/2026 [3] | Atiradores coloridos com munição giram numa esteira em volta de uma arte pixel; 5 slots de espera | Ouro US$1,99–34,99; "Fail Offer" US$5,99; Golden Pass US$9,99; No Ads US$7,99; interstitial; vidas | [1][2][3][4][5] |
| **Magic Sort!** | Grand Games (Istambul) | 2024 | 10M+ | 4,52 (555 mil) | > US$40M no 1º ano (fonte única: DoF) [21] | Water sort com arte premium e cores ocultas | Quase só IAP; vidas; "+1 garrafa" com ouro na falha; LiveOps intenso | [20][21][22] |
| **Water Sort Puzzle** | IEC Global | ~2020 (**não confirmado**) | 100M+ | 4,56 (1,34M) | **não confirmado** | Despejar líquidos entre tubos até cada tubo ter uma cor | Anúncio + rewarded (desfazer, tubo extra) + remover anúncios | [11][13] |
| **Flow Free** | Big Duck Games | 2012 | 100M+ | 4,69 (1,55M) | **não confirmado** | Ligar pares da mesma cor preenchendo toda a grade | Banner + interstitial (~a cada 5 níveis); dicas por rewarded ou IAP; "desbloquear tudo + sem anúncios" US$4,99 | [14][15][16] |
| **Screwdom 3D** | iKame Games (Zego Studio) | jan/2025 | 10M+ | 4,57 (343 mil) | US$120M de IAP acumulado até mai/2026 [3] | Desparafusar parafusos coloridos para caixas da mesma cor, com buffer limitado | Anúncio, rewarded para ferramentas, No Ads, IAP | [3][17][18] |
| **Color Block Jam** | Rollic (Take-Two) | 2024 | 10M+ | 4,07 (338 mil) | US$148M de IAP acumulado até mai/2026 [3]; US$42M e 21,8M installs no 2º tri/2025 [26] | Deslizar blocos coloridos até a porta da mesma cor, com cronômetro | Boosters por IAP | [3][24][26] |
| **Hexa Sort** | Lion Studios Plus | jul/2023 (Android) | 10M+ | 4,48 (460 mil) | Pico de ~US$4M/mês de IAP; ~70% da receita em anúncios [31] | Empilhar hexágonos que se fundem sozinhos por cor | Pesado em anúncios | [30][31] |
| **Parking Jam 3D** | Popcore | até fev/2020 | 100M+ | 4,12 (1,77M) | **não confirmado** | Tirar os carros do estacionamento na ordem certa | Interstitial; retry só por rewarded; remover anúncios US$2,99 | [27][28] |
| **Trainyard / Trainyard Express** | Matt Rix (iOS 2010); Noodlecake (Android 2012) | 2010 | 500K+ (Express) | **não confirmado** | **não confirmado** | Trens coloridos em trilhos desenhados; **a chave muda de direção a cada trem que passa** (é o nosso alternador) | Premium | [39][40] |

**Também observados:** Ball Sort Puzzle (HM Games, 100M+, 4,55 de 1,07M [12]); Yarn Loop (Combo Games/Spyke, clone do Pixel Flow! com tema de lã, US$4,4M/mês em abr/2026 [10]); Marble Sort! (Voodoo, bolinhas numa esteira caem na caixa da cor, 10M+, 4,69 [42]); Bus Jam (Rollic, 10M+ [29]); Rail Maze (Spooky House, 5M+, 4,21 [41]); e jogos novos de trilho com chave, todos minúsculos: Railsort (set/2026, 29 downloads [43]), Color Cube: Rail Sort (Amobear, ~7 mil installs estimados [45]), Train Sorting Puzzle (Bugbomb, 1 avaliação [44]). O "Screw Jam" citado na lista original não foi confirmado (o app com esse nome tem só 1M+).

### Mercado (FATO, com fonte)
- Puzzle fez US$7,6B de gasto bruto no H1/2026, +20% sobre o ano anterior, 2º gênero do mundo; o Royal Match sozinho fez US$883,7M (AppMagic via PocketGamer.biz, 16/07/2026 [32]). *Outro recorte da AppMagic, só casual, dá US$4,9B; não comparar os dois.*
- O sort passou o Blast no início de 2026 e virou o 3º subgênero de puzzle em IAP, puxado por muitas variações e não por um hit só (Mobidictum, 21/07/2026 [33]).
- Só três hybrid-casual de puzzle passaram de US$100M de IAP: Color Block Jam (US$148M), Screwdom (US$120M) e Pixel Flow! (US$105M) (MobileGamer.biz, 17/06/2026 [3]).
- O Pixel Flow! teve 23% da receita do top-10 hybridcasual puzzle no H1/2026 e 30% do sort (Gamigion, 06/08/2026 [47]).
- Antes de 2023, o sort inteiro faturava menos de US$2M por ano. Tendências para 2026 segundo a DoF: **sistemas que se resolvem sozinhos**, esteiras e fases baseadas em arte (06/02/2026 [21]).
- Retenção, GameAnalytics 2026 (16.262 jogos, todos os gêneros): D1 do top-25% ~30% e do top-10% ~40%; D7 mediano abaixo de 4%, top-25% 6–7%, top-10% 11–12% (04/06/2026 [34]). D1/D7 específico de puzzle em 2026: **não confirmado**.

### 1.1 Pixel Flow! em detalhe (ameaça nº 1)
- **O toque:** o jogador toca num "porco" atirador da fila da frente. Cada um tem cor e munição. Ele entra numa esteira circular em volta de uma arte pixel e atira só nos cubos da sua cor [4][5].
- **Falha:** quem acaba a munição sai; quem ainda tem vai para um de 5 slots de espera e pode ser relançado. Quando slots e esteira lotam, perde. As fases são determinísticas; a falha vem de ordem ou timing [4][5]. A regra oficial de derrota e o sistema de vidas: **não confirmados** por fonte oficial.
- **Monetização:** compra de espaço extra na bandeja para continuar [9]; "Fail Offer" de US$5,99 [5]; interstitial na falha e na vitória, agressivo depois do ~nível 20; rewarded só para vidas no menu; não dá para fugir dos anúncios em modo avião [4].
- **Escala em dez/2025:** ~US$550 mil/dia somando IAP e anúncios, ~1M de jogadores ativos/dia, ~200 mil downloads/dia; anúncios eram 30–40% da receita [4].
- **Scopely:** comprou a maioria em fev/2026 com avaliação acima de US$1B; a Loom tinha ~20 pessoas [6][7]. **A Loom processou um clonador nos EUA e os clones saíram das lojas** [10].
- **Reviews:** anúncio depois de cada nível após ~10 níveis; "recicla puzzles, só ~100"; booster de ~1.900 moedas; não roda offline [8].
- **Riscos apontados pela DoF:** retenção inicial modesta para a categoria; exige muita atenção, o que estreita o público; clones devem encarecer o CPI [4].

INFERÊNCIA: o Pixel Flow! é o parente mais próximo do Rune Relay (fila visível + capacidade + cor + tudo determinístico), mas **não tem rota nem chave**. O jogador dele escolhe a ordem; o nosso escolhe o caminho.

---

## 2. Matriz de recursos

`sim` / `não` = confirmado na pesquisa ou na descrição da loja · `?` = não verificado nesta rodada. A última coluna separa o que já existe no protótipo v0.1 (**v0.1**) do que é proposta deste documento (**prop.**).

| Recurso | Pixel Flow! | Magic Sort | Water Sort | Flow Free | Screwdom | Color Block Jam | Trainyard | **Rune Relay** |
|---|---|---|---|---|---|---|---|---|
| Informação toda visível | sim | **não** (cores ocultas) | sim | sim | ? | sim | sim | **sim (v0.1)** |
| Sorte decide o resultado | não | **sim** (reclamado) | não | não | ? | não | não | **não (v0.1)** |
| O jogador desenha/escolhe a rota | não | não | não | sim | não | não | sim | **sim (v0.1)** |
| Chave que vira a cada passagem | não | não | não | não | não | não | **sim** | **sim (v0.1)** |
| Planejar e depois assistir | parcial (atira sozinho, mas o toque é durante) | não | não | não | não | não | sim | **sim (v0.1)** |
| Falha por capacidade/erro | sim (5 slots) | ? | ? | não | sim (buffer) | timer | sim (colisão) | **sim: cor, derrame, transbordo (v0.1)** |
| Timer | ? | ? | não | só no modo contra o tempo | **não** | **sim** (reclamado) | não | **não** |
| Vidas | sim | sim (reclamado) | ? | ? | ? | ? | n/a (premium) | **não (prop.: D4)** |
| Oferta paga no momento da falha | sim (US$5,99, espaço extra) | sim (+1 garrafa) | rewarded | não | rewarded | ? | não | **rewarded +1 liberação (v0.2)** |
| Anúncio depois de todo nível | sim (reclamado) | não | sim (reclamado) | não (~a cada 5) | sim (reclamado) | ? | não | **não (interstitial off na v0.1)** |
| Joga offline | **não** (reclamado) | ? | ? | ? | ? | ? | sim | **sim (sem backend)** |
| Puzzle diário | ? | ? | ? | **sim** | ? | ? | ? | **prop.: D1** |
| Resultado compartilhável | não achado | não achado | não achado | não achado | não achado | não achado | ? | **prop.: D1** |
| Editor de níveis | não achado | não achado | não achado | não achado | não achado | não achado | ? | **prop.: D6** |
| Todo nível provado solúvel | ? | ? | **não** (nível com cor faltando) | ? | ? | **não** (nível bugado) | ? | **sim: solver + CI (v0.1)** |
| Símbolo além da cor (daltônico) | ? | ? | ? | **sim** (rótulos) | ? | ? | ? | **sim: 6 formas (v0.1)** |
| Pagar para tirar anúncios | sim (US$7,99) | n/a | sim | sim (US$4,99) | sim, mas tira as ferramentas grátis (reclamado) | ? | n/a | **v0.2** |

---

## 3. O que os jogadores reclamam nos líderes (oportunidades)

| # | Reclamação recorrente | Onde | Fonte | Oportunidade para nós |
|---|---|---|---|---|
| 1 | Anúncio depois de cada nível e no retry | Pixel Flow!, Screwdom, Water Sort, Parking Jam | [8][18][13][28] | Interstitial desligado na v0.1; na v0.2, só depois do nível 12–15 e com intervalo ≥ 90 s (GDD §7) |
| 2 | "No Ads" que não funciona ou que tira as ajudas grátis | Screwdom, Parking Jam | [18][28] | Remove Ads tira só o interstitial; rewarded e dicas continuam (escrever isso na loja) |
| 3 | Muro de dificuldade que só passa com booster ou anúncio; economia de moedas desbalanceada | Pixel Flow!, Screwdom, Magic Sort, Color Block Jam | [8][18][22][23][46] | **D2:** todo nível provado solúvel sem booster; a dica do solver é honesta |
| 4 | Vidas: 30 s de anúncio por 1 vida que se perde em ~10 s | Magic Sort | [23] | **D4:** sem vidas; o rewarded protege a sequência |
| 5 | Repetição e níveis reciclados | Pixel Flow!, Magic Sort | [8][23] | Gerador com prova de solução única produz nível novo sem custo de autoria; **D6** (editor) a longo prazo |
| 6 | Não joga offline | Pixel Flow! | [4][8] | **D2:** offline por padrão (não temos backend) |
| 7 | Sorte e informação oculta, "derrota forçada" | Magic Sort | [23] | **D2:** fila visível e simulação determinística |
| 8 | Nível bugado ou sem solução | Color Block Jam, Water Sort | [46][13] | **D2:** o CI barra nível sem solução (`LevelsContentTests`) |
| 9 | Falta de modo daltônico em sort com cores parecidas | jogos de sort em geral | [38] | **D7:** forma por cor, já implementado |
| 10 | Anúncio enganoso (não mostra o jogo real) | gênero puzzle | [36][37] | **D2:** todos os nossos criativos são gravados do build real (`docs/CRIATIVOS.md`) |
| 11 | Progresso não passa de um aparelho para outro | Parking Jam | [28] | Fora do MVP (sem backend); anotar para a v1.0 |

INFERÊNCIA: as reclamações 1, 3, 4 e 6 são a máquina de receita desses jogos, não descuido. Não repeti-las é barato para nós e caro para eles. É o tipo de diferencial que o líder não copia.

---

## 4. Diferenciais propostos

Régua de custo para o nosso time (1 dev + Claude Code, Unity 6, sem backend): **P** ≤ 1 semana · **M** 2–4 semanas · **G** ≥ 1 mês. Status de todos: **PROPOSTO** (nada medido).

### D1 — Desafio do Dia compartilhável (sem backend)
- **Por que importa:** entre os líderes pesquisados, só o Flow Free tem puzzle diário [15]; não achamos resultado compartilhável, editor nem replay em nenhum deles. Um nível igual para todos no mesmo dia cria conversa ("fiz em 1 liberação, e você?") e um motivo diário de voltar que não depende de LiveOps.
- **Como funciona:** o gerador (`client/tools/levelgen`) produz offline 365 níveis com prova de solução única; o build embarca a lista e escolhe pela data. Ao vencer, o botão **Compartilhar** monta um texto curto para WhatsApp/Telegram:
  ```
  Rune Relay · Desafio 14/10
  ★★★ na 1ª liberação · 7 giros (mínimo 6)
  ●■▲ sequência: 5 dias
  ```
  Sem moeda nem prêmio por desafio (só a sequência visual), então mexer no relógio do aparelho não rende nada.
- **Custo:** **P** (gerar e embarcar a lista, 1 entrada no menu, 1 tela de resultado, intent de compartilhar texto do Android).
- **Risco:** pouca gente compartilha; desafio difícil demais afasta o casual (usar a faixa de dificuldade dos níveis 11–20).
- **Validar barato:** criativo #12 "Desafio de hoje" no Gate M; na v0.2, desafio ligado para metade da coorte por RemoteConfig (`daily_start`, `share_tap`).

### D2 — Jogo honesto: zero sorte, zero paywall, offline, anúncio real
- **Por que importa:** quatro das reclamações mais repetidas nos líderes são o oposto disto: muro de dificuldade que só passa com booster (Pixel Flow! [8], Screwdom [18], Magic Sort [22][23], Color Block Jam [46]); cores ocultas que viram sorte (Magic Sort [23]); não jogar offline (Pixel Flow! [4][8]); nível bugado ou sem solução (Color Block Jam [46], Water Sort [13]). E anúncio que não mostra o jogo real já foi proibido pela ASA no Reino Unido [37] e estudado como padrão de engano [36].
- **Como funciona:** o núcleo já garante quase tudo: fila visível, simulação determinística, portão de conteúdo (`LevelsContentTests`) que recusa nível sem solução, jogo sem backend. Vira promessa pública: "todo nível tem solução sem booster, e a gente prova; joga sem internet; o anúncio é o jogo". A dica "Revelar runa" sai da solução do solver (gira a peça certa, não uma qualquer). Os criativos continuam gravados do build real (`-record`/`-demo`).
- **Custo:** **P** (texto e screenshots da loja, selo na tela de nível, criativo #11 "Sem sorte"). O motor já existe.
- **Risco:** quebrar a promessa num nível futuro; mitigado pelo CI, que barra nível sem prova.
- **Validar barato:** teste de listing no Play Console (com e sem a promessa); criativo #11 no Gate M.

### D3 — O alternador como assinatura do sort
- **Por que importa:** o Pixel Flow! tem fila, capacidade e cor, mas não tem rota nem chave [4][5]. A chave que vira a cada passagem existe desde o Trainyard (2010, premium; o Express tem 500K+ [39][40]), e os jogos novos de trilho com chave são minúsculos (Railsort: 29 downloads [43]; Color Cube Rail Sort: ~7 mil [45]). Não achamos nenhum hit hybrid-casual de 2025–2026 com alternador. E a DoF aponta "sistemas que se resolvem sozinhos" como tendência do sort em 2026 [21]: é exatamente o LIBERAR → cascata.
- **Como funciona:** a peça que separa ●○●○ em dois cristais vira o centro da marca: ícone do app, primeiro segundo de todo criativo e nome curto da regra no tutorial ("a seta vira a cada orbe"). Nenhuma regra nova: só foco.
- **Custo:** **P** (2–3 criativos novos com o `-demo` atual; ícone).
- **Risco:** a regra da seta que vira pode não se ler em 2 s; a mecânica não é inédita, então a defesa é a execução (gerador + solver + velocidade), não a ideia. Se o CPI não melhorar, o vídeo volta para "cascata".
- **Validar barato:** Gate M com 2 criativos D3 contra o #5 atual, mesmo orçamento e geo.

### D4 — Fluxo Perfeito, sem vidas
- **Por que importa:** a monetização do gênero acontece na falha. O Pixel Flow! vende "Fail Offer" de US$5,99 e espaço extra [5][9]; o Magic Sort vende "+1 garrafa" e cobra 30 s de anúncio por 1 vida, que se perde em ~10 s, e isso é reclamado [22][23]. No protótipo, recomeçar devolve as 3 liberações, então o nosso rewarded "+1 liberação" quase não tem valor (GDD §6.1).
- **Como funciona:** o contador de liberações persiste até a vitória; vencer na 1ª liberação aumenta a sequência "Fluxo Perfeito", que multiplica as moedas (×1,5 / ×2 / ×3). Ao falhar, o rewarded protege a sequência. Nada de vidas nem de espera.
- **Custo:** **P** (ajuste em `Session`/`Progress`, 1 contador na HUD, testes negativos do exploit).
- **Risco:** pressão de sequência irrita o casual; pode monetizar menos que vidas.
- **Validar barato:** A/B na v0.2 (opções A × B da §6.1 do GDD); se nenhuma monetizar, testar vidas (opção C).

### D5 — Selo de Mestre (mínimo de giros)
- **Por que importa:** o público de lógica (Flow Free, 100M+ e 1,5M de avaliações [14]) joga por maestria. Não achamos camada de eficiência nos sort hybrid-casual pesquisados (não verificado em todos).
- **Como funciona:** o solver já calcula `MinTaps`. Resolver com esse número de giros na 1ª liberação dá um selo no menu de níveis. Opcional; não mexe na estrela.
- **Custo:** **P**.
- **Risco:** contar giros faz o jogador hesitar em testar; mostrar o selo só depois da vitória, nunca um contador durante o planejamento.
- **Validar barato:** telemetria `master_seal` no playtest Camada 1.

### D6 — Editor de níveis com código de texto
- **Por que importa:** o Pixel Flow! é criticado por reciclar níveis ("só ~100") [8], e não achamos editor em nenhum líder. Para quem não tem solver, UGC vira nível impossível; para nós, o solver é a barreira de qualidade.
- **Como funciona:** editor simples na grade, com as mesmas peças do jogo; o solver roda no aparelho e só libera o compartilhamento com **solução única**; o nível vira um código de texto (o formato `.txt` do jogo já é texto) que outro jogador cola para jogar. Sem servidor.
- **Custo:** **M** (a UI de edição no toque é o grosso; parser e solver já existem; a força bruta segue limitada a 10 peças tocáveis).
- **Risco:** pouca gente cria; código copiado tem atrito sem deep link.
- **Validar barato:** só depois do D7 ≥ 5%; protótipo com 10 testers.

### D7 — Acessível por padrão
- **Por que importa:** modo daltônico é pedido recorrente em jogos de sort com tubos de cores parecidas [38]; entre os líderes pesquisados, só o Flow Free tem rótulos para daltônicos [15].
- **Como funciona:** já implementado: cada cor tem forma (disco, quadrado, triângulo, losango, anel, cruz), não há timer, tudo se joga com um polegar. Vira argumento de loja e de review.
- **Custo:** **P** (só comunicação; conferir as fotos de QA com filtro de deuteranopia).
- **Risco:** baixo; o ganho de instalação é pequeno, a aposta é na nota da loja.
- **Validar barato:** filtro de daltonismo nas fotos de QA; pergunta no playtest.

---

## 5. Os 3 que eu faria primeiro

1. **D3 — Alternador como assinatura.** O primeiro gate que pode matar o jogo é o CPI no Gate M, e o veredito pede um vídeo de 3 s com orbes se ordenando sozinhos. D3 é esse vídeo, ocupa o espaço vazio entre o Trainyard (premium, de nicho) e o Pixel Flow! (sem rota) e custa só criativo.
2. **D2 — Jogo honesto.** Transforma em promessa pública o que o núcleo já garante e responde de uma vez a quatro reclamações recorrentes dos líderes. Eles não copiam sem perder receita, porque o muro de dificuldade e o anúncio por nível são o modelo de negócio deles.
3. **D1 — Desafio do Dia compartilhável.** É o único que traz instalação orgânica e motivo de voltar todo dia sem backend, e reusa o gerador. Custa P.

D4 não é opcional: é a correção da monetização (GDD §6.1) e entra junto com o rewarded na v0.2. D5 e D7 são quase grátis; D6 só depois de o D7 passar.

**Não competir em:** volume de LiveOps, vidas logo de cara, booster pago em corrida, item aleatório pago, hard currency no MVP, anúncio por nível.

---

## 6. Riscos de mercado e lacunas

- **O líder define a régua de polish e volume.** Pixel Flow! (US$198,4M no 1º ano) e Magic Sort (> US$40M) fixam o padrão. Clones encarecem o CPI do subgênero inteiro [4]. O Rune Relay pode passar no Gate M e cair no D7 por falta de volume; o gerador com prova é a resposta, não LiveOps.
- **"Planejar e assistir" estreita o público.** A DoF diz que o Pixel Flow! exige muita atenção e isso reduz o alcance [4]; o nosso planejamento exige ainda mais. Os criativos começam no LIBERAR, nunca no planejamento.
- **Distância visual do Pixel Flow!** A Loom já processou um clonador e tirou os clones das lojas [10]. Nada de esteira em volta de arte pixel, atirador com munição ou slots de espera com a mesma leitura.
- **Mecânica não patenteável.** O alternador existe desde 2010; se funcionar, será copiado. A defesa é chegar antes com volume de níveis provados e a promessa D2 cumprida.
- **Lacunas (não confirmado):** contadores lidos direto da Play (usamos o espelho androidrank); receita de Water Sort, Ball Sort, Flow Free, Parking Jam, Bus Jam e Hexa Sort em 2025–26; data de lançamento de Water Sort e Ball Sort; regra oficial de derrota e vidas do Pixel Flow!, participação dos EUA no gasto dele (66% segundo a AppMagic, ~9% segundo DoF/AppSamurai) e data de lançamento (ago × out/2025); receita do 1º ano do Magic Sort (fonte única); retenção de puzzle em 2026; tamanho do sort em US$ no H1/2026 (US$477,6M só num post de LinkedIn); threads do Reddit não acessadas; existência de puzzle diário, editor ou compartilhamento nos líderes além do Flow Free.

---

## Fontes (consultadas em 09/10/2026)

- [1] https://www.androidrank.org/application/pixel_flow/com.loomgames.pixelflow
- [2] https://www.pocketgamer.biz/loom-games-pixel-flow-makes-almost-200m-in-player-spending-during-first-year/ (24/08/2026)
- [3] https://mobilegamer.biz/data-digest-pixel-flow-hits-100m-mays-top-games-neverness-to-everness-pokemon-go-more/ (17/06/2026)
- [4] https://www.deconstructoroffun.com/blog/2026/2/13/pixel-flow-the-publishers-dream (13/02/2026)
- [5] https://www.apppricinglab.com/app/apple/6751056652 (dados de 21/03/2026)
- [6] https://www.pocketgamer.biz/why-scopely-acquired-the-pixel-flow-team/ (19/02/2026)
- [7] https://mobilegamer.biz/scopely-acquires-pixel-flow-maker-loom-games/ (fev/2026)
- [8] https://worldsapps.com/reviews-pixel-flow (agregador de reviews: fonte fraca, usar como sinal)
- [9] https://www.deconstructoroffun.com/blog/pixel-flow-and-the-rise-of-sort-puzzles (10/03/2026)
- [10] https://news.17173.com/content/04152026/153310034.shtml (15/04/2026)
- [11] https://www.androidrank.org/application/water_sort_puzzle/com.gma.water.sort.puzzle
- [12] https://www.androidrank.org/application/ball_sort_puzzle/com.GMA.Ball.Sort.Puzzle
- [13] https://splitmetrics.com/apps/water-sort-puzzle/id1514542157 · https://justuseapp.com/en/app/1514542157/water-sort-puzzle/reviews
- [14] https://www.androidrank.org/application/flow_free/com.bigduckgames.flow
- [15] https://en.wikipedia.org/wiki/Flow_Free
- [16] https://apps.apple.com/us/app/flow-free-shapes/id6642642713
- [17] https://www.androidrank.org/application/screwdom_3d/com.ig.screwdom
- [18] https://grand-screen.com/games/screwdom-3d/reviews/ · https://games.appmatch.jp/6740043080-2/
- [20] https://www.androidrank.org/application/magic_sort/com.grandgames.magicsort
- [21] https://www.deconstructoroffun.com/blog/2026/2/6/sort-puzzles-how-a-new-subgenre-is-born (06/02/2026)
- [22] https://felixbraberg.substack.com/p/the-monetization-engine-behind-magic
- [23] https://grand-screen.com/games/magic-sort/reviews/
- [24] https://www.androidrank.org/application/color_block_jam/com.GybeGames.ColorBlockJam
- [26] https://gamedevreports.substack.com/p/appmagic-top-10-hybrid-casual-games
- [27] https://www.androidrank.org/application/parking_jam_3d/com.lszenlamzr.parkingjam
- [28] https://justuseapp.com/en/app/1498229533/parking-jam-3d/reviews
- [29] https://www.androidrank.org/application/bus_jam/com.nt.games.busjam
- [30] https://www.androidrank.org/application/hexa_sort/com.gamebrain.hexasort
- [31] https://maf.ad/en/blog/hexa-sort-hybrid-casual/
- [32] https://www.pocketgamer.biz/h1-2026-genre-analysis-strategy-stumbles-rpgs-fall-and-puzzle-revenue-ramps-up/ (16/07/2026)
- [33] https://mobidictum.com/sort-block-screw-mechanics-puzzle-games-market/ (21/07/2026)
- [34] https://gamedevreports.substack.com/p/gameanalytics-mobile-and-pc-game (04/06/2026)
- [36] https://psu.edu/news/research/story/five-ways-fake-mobile-games-fail-meet-advertised-expectations (13/01/2025)
- [37] https://asa.org.uk/news/don-t-hate-the-player-hate-the-game-play-misleading-gameplay-in-videogame-ads.html
- [38] https://marlvel.ai/apps/nut-sort-3d-color-match/reviews
- [39] https://ar5iv.arxiv.org/html/1603.00928 · https://geardiary.com/2010/10/20/blue-plate-special-trainyard-for-iphonetouch/
- [40] https://www.androidrank.org/search?q=trainyard
- [41] https://www.androidrank.org/application/rail_maze/com.spookyhousestudios.railmaze
- [42] https://www.androidrank.org/application/marble_sort/com.microdose.balljam
- [43] https://www.appbrain.com/app/railsort-train-brain-puzzle/com.cyduval.railshift
- [44] https://apps.apple.com/us/app/-/id6746746682
- [45] https://appgoblin.info/apps/6771673362
- [46] https://worldsapps.com/reviews-color-block-jam (agregador de reviews: fonte fraca)
- [47] https://www.gamigion.com/?p=34952 (06/08/2026; citada no veredito de validação de 2026-10-06)
