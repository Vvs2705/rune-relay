# View v0.1 — QA visual (raia Arte + UX/UI, 2026-10-06)

## Direção de arte (5 linhas)
1. Observatório arcano à noite: fundo índigo-preto (#0E1120) com brilho índigo difuso e motas de luz derivando; o tabuleiro é uma placa escura e TUDO que brilha é mana.
2. Seis manas saturadas, cada uma com FORMA (azul disco, vermelho quadrado, verde triângulo, amarelo losango, roxo anel, laranja cruz): a forma aparece no orbe, na fila da fonte, na gema do cristal e no cadeado da comporta.
3. Canal = runa: aro claro + calha escura + núcleo que acende na cor do fluxo com brilho suave embaixo; apagado é aço azulado, nunca cinza chapado.
4. Hierarquia: tabuleiro > LIBERAR (dourado, com sombra) > resto da HUD (texto marfim/azul-claro). Estado nunca só por cor: tocável = aro dourado que respira; fechado = cadeado; usado = anel vazio.
5. A cascata é a estrela: orbes com brilho + rastro de 3 fantasmas, gema que cresce a cada orbe, flash + halo + faíscas no cristal cheio, comporta que desliza e estoura o cadeado, seta do alternador que gira com halo.

## O que mudou
- `Art.cs`: paleta final, sprites duros novos (chevron, engrenagem, cadeado, avanço rápido, pausa) e SUAVES (Glow, Halo, Orb, Gem, Slab 9-slice, SlabFrame); uGUI com sombra no texto, botão com estados (pressionado/desabilitado + rótulo esmaecido).
- `Sfx.cs` (novo): mini-sintetizador aditivo: tap, flip, click, release, absorb (pitch sobe por orbe), full (acorde C-E-G), gate, fail, lose, win (arpejo), star.
- `Fx.cs` (novo): `Settings` (rr.sound/rr.vibe no PlayerPrefs), `Ease`, `Fx` = o único ParticleSystem (material runtime Sprites/Default, `Burst`), `Haptic` (Android, fora do editor), `Backdrop`.
- `BoardView.cs`: peças redesenhadas, `OnTick` (seta, comporta, cristal, fonte), ondas de toque/absorção, shake na falha, bounce + faíscas na vitória.
- `Menu.cs` (novo): grade 4 colunas com scroll, estrelas, bloqueado/atual, painel de configurações com toggles.
- `Tutorial.cs` (novo): anel pulsante na peça que falta (Solver decide) e anel + seta no LIBERAR; some ao liberar.
- `Game.cs`: menu <-> nível, voltar (Esc/back: nível -> menu -> sair), cartão de falha com ícone por motivo, indicador SEGURE PARA ACELERAR (acende em 2×), painel com estrelas animadas e PRÓXIMO/MENU, `-screen menu`, eventos `menu_open` e `settings_change`.

## Checklist para o coordenador (fotos)
- [ ] `-level 1 -shot`: aro dourado respirando SÓ na peça L1r (2,2); anel pulsante do tutorial sobre ela; fonte = poço escuro com aro claro e 3 orbes azuis; cristal = losango com interior escuro, gema pequena e 3 pips vazios; LIBERAR dourado com sombra, RECOMEÇAR azul-escuro; chevron "<" no topo esquerdo; 3 pontos dourados em "Liberações".
- [ ] `-level 1 -solve -shot`: canal aceso em azul do poço ao cristal (núcleo azul + brilho); anel do tutorial saiu da peça e envolve o botão LIBERAR com seta dourada pulsando em cima; a dica some enquanto a seta aponta para o botão (volta ao liberar/tocar).
- [ ] `-level 10 -shot`: dois alternadores com seta marfim + halo dourado apontando para a direita (Y11), saída inativa esmaecida; comporta G10 com duas barras AZUIS + cadeado azul com disco escuro (cristal 0 = Ra2); cruzamento com anel "ponte"; bloco "#" não existe aqui (ok).
- [ ] `-level 10 -solve -release -shotdelay 2 -shot`: orbes brilhando com rastro atrás no sentido do movimento; LIBERAR substituído pela pílula "SEGURE PARA ACELERAR" com ">>"; pips acendendo e gema crescendo nos cristais atingidos; seta de algum alternador já virada para a esquerda.
- [ ] `-level 6 -solve -release -shotdelay 3 -shot`: cristal azul cheio com halo pulsando e gema grande; comporta aberta = barras e cadeado sumiram, canal vermelho aceso com orbes vermelhos passando.
- [ ] `-screen menu -shot`: título RUNE RELAY dourado, engrenagem à direita, grade 4 colunas; nível 1 com moldura dourada (atual), demais com cadeado e número apagado; estrelas sob os níveis abertos; níveis vencidos fora de ordem (ex.: L6 via `-level`) abertos com suas estrelas; NENHUM elemento da HUD do nível visível ("<", "Liberações", RECOMEÇAR, LIBERAR, dica).
- [ ] Falha (manual ou nível 5 sem -solve + -release): X vermelho pulsando na célula, onda vermelha, tabuleiro treme, cartão vermelho embaixo com ícone + título "Cor errada!" e texto com liberações restantes.
- [ ] Vitória manual: faíscas nas cores dos cristais, painel com 3 estrelas entrando uma a uma (som de sino subindo), botões MENU e PRÓXIMO.
- [ ] Log: `-batchmode -nographics -autoplay` continua terminando em `AUTOPLAY OK 10/10` (o modo autoplay não cria câmera, HUD nem som).
