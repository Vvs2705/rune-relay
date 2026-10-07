using System;
using RR.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RR
{
    /// <summary>
    /// Selecao de niveis (grade de 4 colunas com scroll; cresce com Resources/Levels) + painel de configuracoes
    /// (som e vibracao, PlayerPrefs). uGUI por codigo dentro da area segura. Estado nunca so' por cor: bloqueado =
    /// cadeado, atual = moldura dourada, estrelas = 3 icones cheios/vazios; toggle = posicao do botao + texto.
    /// </summary>
    public sealed class Menu : MonoBehaviour
    {
        public Action<int> OnPlay;
        public Action<string, bool> OnSettingsChange;

        sealed class Toggle { public Button Btn; public Image Pill, Knob; public Text State; public Func<bool> Get; }

        RectTransform _root, _grid, _settings;
        Text _total;
        Toggle _sound, _vibe;
        int _levels;

        public bool Visible => _root.gameObject.activeSelf;
        public bool SettingsOpen => _settings.gameObject.activeSelf;

        public static Menu Create(RectTransform safe, int levels)
        {
            RectTransform root = Art.Node(safe, "Menu", Vector2.zero, Vector2.one);
            var m = root.gameObject.AddComponent<Menu>();
            m._root = root;
            m._levels = levels;
            m.Build();
            root.gameObject.SetActive(false);
            return m;
        }

        void Build()
        {
            RectTransform bg = Art.Node(_root, "Fundo", new Vector2(-1f, -1f), new Vector2(2f, 2f)); // cobre alem da area segura
            bg.gameObject.AddComponent<Image>().color = Art.ComAlfa(Art.Bg, 0.9f);

            Text title = Art.NewText(_root, "Titulo", 66, new Vector2(0.05f, 0.9f), new Vector2(0.55f, 0.985f), TextAnchor.MiddleLeft, true);
            title.text = "RUNE RELAY";
            title.color = Art.Accent;
            _total = Art.NewText(_root, "Total", 52, new Vector2(0.52f, 0.9f), new Vector2(0.74f, 0.985f), TextAnchor.MiddleRight);
            Art.Icon(_root, "Estrela", Art.Star(), Art.Accent, new Vector2(0.75f, 0.918f), new Vector2(0.82f, 0.967f));
            Button gear = Art.NewButton(_root, "", 1, Art.CellTap, new Vector2(0.84f, 0.9f), new Vector2(0.97f, 0.985f), OpenSettings, false);
            Art.Icon(gear.transform, "Engrenagem", Art.Gear(), Art.Ink, new Vector2(0.22f, 0.22f), new Vector2(0.78f, 0.78f));
            Text sub = Art.NewText(_root, "Sub", 36, new Vector2(0.05f, 0.855f), new Vector2(0.95f, 0.9f), TextAnchor.MiddleLeft);
            sub.text = "Escolha um nível";
            sub.color = Art.ComAlfa(Art.Ink, 0.85f);

            // grade com scroll
            RectTransform viewport = Art.Node(_root, "Viewport", new Vector2(0.02f, 0.02f), new Vector2(0.98f, 0.85f));
            viewport.gameObject.AddComponent<Image>().color = Color.clear; // alvo de raycast: arrastar no vazio tambem rola
            viewport.gameObject.AddComponent<RectMask2D>();
            _grid = Art.Node(viewport, "Grade", new Vector2(0f, 1f), new Vector2(1f, 1f));
            _grid.pivot = new Vector2(0.5f, 1f);
            var layout = _grid.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(200f, 220f);      // 4 x 200 + folgas = 908 < 939 (canvas de 19,5:9)
            layout.spacing = new Vector2(20f, 24f);
            layout.padding = new RectOffset(24, 24, 20, 80);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 4;
            layout.childAlignment = TextAnchor.UpperCenter;
            var fit = _grid.gameObject.AddComponent<ContentSizeFitter>();
            fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = _grid;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 40f;

            // configuracoes
            _settings = Art.Node(_root, "Config", new Vector2(-1f, -1f), new Vector2(2f, 2f));
            _settings.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
            Image box = Art.Panel(_settings, "Caixa", Art.Cell, new Vector2(0.37f, 0.42f), new Vector2(0.63f, 0.58f), 40f);
            Text ct = Art.NewText(box.transform, "Titulo", 56, new Vector2(0.05f, 0.76f), new Vector2(0.95f, 0.97f), TextAnchor.MiddleCenter, true);
            ct.text = "Configurações";
            _sound = MakeToggle(box.transform, "Som", 0.52f, 0.74f, () => Settings.Sound, v => { Settings.Sound = v; OnSettingsChange?.Invoke("sound", v); if (v) Sfx.Play("click"); });
            _vibe = MakeToggle(box.transform, "Vibração", 0.28f, 0.5f, () => Settings.Vibe, v => { Settings.Vibe = v; OnSettingsChange?.Invoke("vibe", v); if (v) Fx.Haptic(); });
            Art.NewButton(box.transform, "FECHAR", 44, Art.Accent, new Vector2(0.25f, 0.04f), new Vector2(0.75f, 0.24f), CloseSettings);
            _settings.gameObject.SetActive(false);
        }

        Toggle MakeToggle(Transform parent, string label, float y0, float y1, Func<bool> get, Action<bool> set)
        {
            Text l = Art.NewText(parent, label, 44, new Vector2(0.06f, y0), new Vector2(0.5f, y1), TextAnchor.MiddleLeft);
            l.text = label;
            var t = new Toggle { Get = get };
            t.Pill = Art.Panel(parent, "Toggle " + label, Art.PipeEdge, new Vector2(0.52f, y0 + 0.02f), new Vector2(0.94f, y1 - 0.02f), 50f);
            t.Btn = t.Pill.gameObject.AddComponent<Button>();
            t.Btn.targetGraphic = t.Pill;
            t.Knob = Art.Icon(t.Pill.transform, "Botao", Art.Orb(), Art.Ink, new Vector2(0.03f, 0.1f), new Vector2(0.33f, 0.9f));
            t.State = Art.NewText(t.Pill.transform, "Estado", 32, new Vector2(0.34f, 0f), new Vector2(0.97f, 1f), TextAnchor.MiddleCenter, true);
            t.Btn.onClick.AddListener(() => { set(!get()); Refresh(t); });
            Refresh(t);
            return t;
        }

        static void Refresh(Toggle t)
        {
            bool on = t.Get();
            t.Pill.color = on ? Art.Good : Art.PipeEdge;
            t.Knob.rectTransform.anchorMin = new Vector2(on ? 0.67f : 0.03f, 0.1f);
            t.Knob.rectTransform.anchorMax = new Vector2(on ? 0.97f : 0.33f, 0.9f);
            t.State.rectTransform.anchorMin = new Vector2(on ? 0.03f : 0.34f, 0f);
            t.State.rectTransform.anchorMax = new Vector2(on ? 0.66f : 0.97f, 1f);
            t.State.text = on ? "LIGADO" : "DESLIGADO";
            t.State.color = on ? Art.Rock : Art.Ink;
        }

        /// <summary>Mostra a grade com o progresso atual (reconstroi os cartoes: N e' pequeno).</summary>
        public void Show(Progress p)
        {
            _root.gameObject.SetActive(true);
            _total.text = p.Total.ToString();
            foreach (Transform c in _grid) Destroy(c.gameObject);
            int current = p.Unlocked;
            for (int i = 0; i < _levels; i++)
            {
                bool open = i <= current || p.Stars[i] > 0; // vencido fora de ordem (ex.: -level) tambem fica jogavel
                int level = i;
                Image card = Art.Panel(_grid, "Nivel " + (i + 1), open ? Art.CellTap : Art.Cell, Vector2.zero, Vector2.one, 30f);
                var b = card.gameObject.AddComponent<Button>();
                b.targetGraphic = card;
                ColorBlock cb = b.colors;
                cb.pressedColor = new Color(0.72f, 0.72f, 0.72f, 1f);
                cb.disabledColor = new Color(0.7f, 0.7f, 0.75f, 1f);
                cb.fadeDuration = 0.06f;
                b.colors = cb;
                b.interactable = open;
                if (open) b.onClick.AddListener(() => { Sfx.Play("click"); OnPlay?.Invoke(level); });
                if (i == current) Art.Icon(card.transform, "Atual", Art.SlabFrame(), Art.Accent, new Vector2(-0.03f, -0.03f), new Vector2(1.03f, 1.03f)).preserveAspect = false;
                Text n = Art.NewText(card.transform, "Numero", open ? 64 : 34, new Vector2(0.05f, open ? 0.4f : 0.68f), new Vector2(0.95f, open ? 0.95f : 0.98f), TextAnchor.MiddleCenter, true);
                n.text = (i + 1).ToString();
                n.color = open ? Art.Ink : Art.InkDim;
                if (!open)
                {
                    Art.Icon(card.transform, "Cadeado", Art.Lock(), Art.InkDim, new Vector2(0.3f, 0.18f), new Vector2(0.7f, 0.62f));
                    continue;
                }
                for (int s = 0; s < 3; s++)
                    Art.Icon(card.transform, "Estrela" + s, Art.Star(), s < p.Stars[i] ? Art.Accent : Art.ComAlfa(Art.PipeEdge, 0.9f),
                        new Vector2(0.14f + s * 0.26f, 0.08f), new Vector2(0.34f + s * 0.26f, 0.36f));
            }
            _grid.anchoredPosition = Vector2.zero;
        }

        public void Hide() => _root.gameObject.SetActive(false);

        public void OpenSettings()
        {
            Sfx.Play("click");
            Refresh(_sound);
            Refresh(_vibe);
            _settings.gameObject.SetActive(true);
        }

        public void CloseSettings() => _settings.gameObject.SetActive(false);
    }
}
