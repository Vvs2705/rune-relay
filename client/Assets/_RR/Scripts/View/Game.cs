using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using RR.Core;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace RR
{
    /// <summary>
    /// Fluxo do jogo, o unico MonoBehaviour de fluxo: menu de niveis <-> nivel (planejar -> liberar -> resultado),
    /// toque, HUD por codigo, tutorial dos niveis 1-2 e diario de playtest (persistentDataPath/diario.csv). Nasce
    /// sozinho em qualquer cena. Voltar (Esc / back do Android): nivel -> menu -> sair.
    /// Flags de dev: -level N | -screen menu | -autoplay (resolve todos pelo Solver, sai 0/1; nao cria view) |
    /// -shot arquivo.png [-solve] [-release] [-shotdelay s] |
    /// -record pasta [-recordsec s] [-recordfps n] (PNG por quadro em 2x + audio float32 cru, depois sai) | -demo "roteiro" (ver Demo).
    /// </summary>
    public sealed class Game : MonoBehaviour
    {
        const float TickSeconds = 0.25f;   // raia B §6; segurar o dedo acelera 2x
        const string SaveKey = "rr.progress";

        string[] _levels;
        Progress _progress;
        int _level = -1, _absorbed;
        Session _s;
        FlowSim _sim;
        FlowSim.Orb[] _prev = new FlowSim.Orb[0];
        float _tickT, _levelStart;
        bool _busy, _firstMove, _tutorial;
        Vector2Int _screen;
        string _diary, _sid;

        Camera _cam;
        BoardView _view;
        Menu _menu;
        Tutorial _tut;
        RectTransform _safe, _hud, _dots, _panel, _failCard;   // _hud = tudo do nivel; o menu esconde o no' inteiro
        Text _title, _stars, _hint, _panelTitle, _panelBody, _failTitle, _failBody, _holdText;
        Image[] _panelStars;
        Image _failIcon, _hold, _holdIcon;
        Button _releaseBtn, _againBtn, _panelPrimary, _panelSecondary;
        Action _primaryAction, _secondaryAction;
        Coroutine _starsAnim;
        string _rec;                       // -record: pasta dos quadros
        int _recFrame, _recFrames, _recCh;
        BinaryWriter _audio;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (FindAnyObjectByType<Game>() == null) new GameObject("Game").AddComponent<Game>();
        }

        void Awake()
        {
            Application.targetFrameRate = 60;
            _levels = Resources.LoadAll<TextAsset>("Levels").OrderBy(t => t.name).Select(t => t.text).ToArray();
            _progress = Progress.Parse(PlayerPrefs.GetString(SaveKey, ""), _levels.Length);
            _sid = Guid.NewGuid().ToString("N").Substring(0, 8);
            _diary = Path.Combine(Application.persistentDataPath, "diario.csv");
            if (Arg("-autoplay") != null) return; // batchmode/nographics: nada de camera, HUD ou som

            Sfx.Init(gameObject);
            _cam = new GameObject("Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            _cam.orthographic = true;
            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.backgroundColor = Art.Bg;
            _cam.transform.position = new Vector3(0, 0, -10);
            Backdrop.Create(null);
            _view = new GameObject("Tabuleiro").AddComponent<BoardView>();
            Fx.Init(null);
            BuildHud();
            _menu = Menu.Create(_safe, _levels.Length);
            _menu.OnPlay = Load;
            _menu.OnSettingsChange = (k, v) => Log("settings_change", k, v ? "1" : "0");
            _tut = Tutorial.Create(null, _hud);

            _rec = Arg("-record");
            if (string.IsNullOrEmpty(_rec)) { _rec = null; return; }
            int fps = Mathf.Max(1, (int)ArgF("-recordfps", 30f));
            Time.captureFramerate = fps; // o tempo de jogo anda 1/fps por quadro, por mais lento que seja gravar
            _recFrames = Mathf.RoundToInt(ArgF("-recordsec", 30f) * fps); // teto; com -demo o fim do roteiro para antes
            _recCh = AudioSettings.speakerMode == AudioSpeakerMode.Mono ? 1 : 2; // ponytail: mono/estereo; 5.1 nao e' alvo
            Directory.CreateDirectory(_rec);
            _audio = new BinaryWriter(File.Create(Path.Combine(_rec, $"audio_{AudioSettings.outputSampleRate}_{_recCh}.f32")));
            AudioRenderer.Start();
        }

        /// <summary>-record: um PNG por quadro (2x a janela) e o audio do quadro em float32 cru; sai depois de -recordsec.</summary>
        void LateUpdate()
        {
            if (_rec == null) return;
            if (_recFrame < _recFrames)
            {
                ScreenCapture.CaptureScreenshot(Path.Combine(_rec, $"f{_recFrame:00000}.png"), 2);
                using (var buf = new NativeArray<float>(AudioRenderer.GetSampleCountForCaptureFrame() * _recCh, Allocator.Temp))
                {
                    AudioRenderer.Render(buf);
                    foreach (float f in buf) _audio.Write(f);
                }
            }
            else if (_recFrame == _recFrames + 2) // 2 quadros de folga para o ultimo PNG chegar ao disco
            {
                AudioRenderer.Stop();
                _audio.Dispose();
                Application.Quit(0);
            }
            _recFrame++;
        }

        void Start()
        {
            if (Arg("-autoplay") != null) { Autoplay(); return; }
            Log("session_start", Application.version, SystemInfo.deviceModel.Replace(",", " "));
            string lv = Arg("-level");
            if (lv != null && int.TryParse(lv, out int v)) Load(Mathf.Clamp(v - 1, 0, _levels.Length - 1));
            else if (Arg("-screen") == "menu") OpenMenu();
            else Load(_progress.Unlocked);
            string shot = Arg("-shot");
            if (!string.IsNullOrEmpty(shot)) StartCoroutine(Shot(shot));
            string demo = Arg("-demo");
            if (!string.IsNullOrEmpty(demo)) StartCoroutine(Demo(demo));
        }

        void Load(int i)
        {
            StopAllCoroutines(); // After() pendentes do nivel anterior nao podem abrir painel aqui
            _menu.Hide();
            _hud.gameObject.SetActive(true);
            _view.gameObject.SetActive(true);
            _level = i;
            _s = new Session(Board.Parse(_levels[i]));
            _sim = null;
            _busy = false;
            _absorbed = 0;
            _view.Build(_s.Board);
            _view.ShowPlanning(_s.Board);
            Fit();
            _hint.text = _s.Start.Hint;
            HideFailCard();
            ClosePanel();
            _levelStart = Time.time;
            _firstMove = false;
            _tutorial = i < 2;
            RefreshTutorial();
            RefreshHud();
            Log("level_start");
        }

        void OpenMenu()
        {
            StopAllCoroutines();
            _sim = null;
            _busy = false;
            _tut.Hide();
            HideFailCard();
            ClosePanel();
            _view.gameObject.SetActive(false);
            _hud.gameObject.SetActive(false); // HUD, dica, botoes, cartao de falha, painel e anel do tutorial: nada vaza atras do menu
            _menu.Show(_progress);
            Log("menu_open");
        }

        void Update()
        {
            if (_cam == null) return;
            if (_s != null && (_screen.x != Screen.width || _screen.y != Screen.height)) Fit();
            // Input.GetKeyDown: com o GameActivity o voltar do Android nao chega ao Input System, so ao Input Manager legado (Setup: Both)
            bool back = Input.GetKeyDown(KeyCode.Escape) || (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame);
            if (_menu.Visible)
            {
                if (back) { if (_menu.SettingsOpen) _menu.CloseSettings(); else Application.Quit(); }
                return;
            }
            if (_s == null) return;
            if (back) { OpenMenu(); return; }
            Pointer p = Pointer.current;
            if (_sim != null)
            {
                bool held = p != null && p.press.isPressed;
                SetHold(held);
                if (!_busy) Advance(held ? 2f : 1f);
                return;
            }
            if (_busy || p == null || !p.press.wasPressedThisFrame) return;
            Vector3 w = _cam.ScreenToWorldPoint(p.position.ReadValue());
            if (_view.CellAt(w, out int x, out int y)) TapCell(x, y);
        }

        /// <summary>Toque do jogador numa celula (dedo ou roteiro -demo). false = nao era tocavel.</summary>
        bool TapCell(int x, int y)
        {
            if (!_s.Tap(x, y)) return false;
            HideFailCard();
            _view.OnTap(x, y, _s.Board.At(x, y));
            _view.ShowPlanning(_s.Board);
            Sfx.Play("tap", 1f + 0.06f * (_s.Taps % 4));
            RefreshTutorial();
            if (_firstMove) return true;
            _firstMove = true;
            Log("first_move", ((Time.time - _levelStart) * 1000f).ToString("0", CultureInfo.InvariantCulture));
            return true;
        }

        void Release()
        {
            if (_sim != null || _busy || !_s.CanPlay) return;
            _sim = _s.Release();
            if (_sim == null) return;
            _prev = new FlowSim.Orb[0];
            _tickT = 1f; // o primeiro tick (fontes soltam o 1o orbe) ja no proximo quadro
            _absorbed = 0;
            _tut.Hide();
            HideFailCard();
            Sfx.Play("release");
            Log("release", _s.Used.ToString(), _s.Taps.ToString());
            RefreshHud();
        }

        void Advance(float speed)
        {
            _tickT += Time.deltaTime * speed / TickSeconds;
            while (_tickT >= 1f)
            {
                _tickT -= 1f;
                _prev = _sim.Orbs.ToArray();
                int[] before = (int[])_sim.Filled.Clone();
                bool running = _sim.Step();
                for (int c = 0; c < before.Length; c++)
                {
                    if (_sim.Filled[c] <= before[c]) continue;
                    _absorbed++;
                    if (_sim.Full(c)) Sfx.Play("full", 1f, 0.7f);
                    else Sfx.Play("absorb", 1f + 0.07f * _absorbed);
                }
                _view.OnTick(_sim, _prev);
                if (running) continue;
                _view.ShowSim(_sim, _prev, 1f);
                EndSim();
                return;
            }
            _view.ShowSim(_sim, _prev, _tickT);
        }

        void EndSim()
        {
            FlowSim sim = _sim;
            _s.End(sim);
            _busy = true;
            SetHold(false);
            if (_s.Won)
            {
                _sim = null;
                _progress.Record(_level, _s.Stars);
                PlayerPrefs.SetString(SaveKey, _progress.ToString());
                PlayerPrefs.Save();
                Log("level_complete", _s.Stars.ToString(), (Time.time - _levelStart).ToString("0.0", CultureInfo.InvariantCulture));
                Sfx.Play("win", 1f, 0.8f);
                _view.Celebrate();
                _hint.text = "Cascata completa!";
                StartCoroutine(After(1.1f, ShowWin));
            }
            else
            {
                Log("level_fail", sim.Outcome.ToString(), _s.Used.ToString());
                Sfx.Play("fail", 1f, 0.8f);
                _view.ShowFail(sim.FailX, sim.FailY);
                ShowFailCard(sim.Outcome, _s.Max - _s.Used);
                StartCoroutine(After(1.4f, () =>
                {
                    _sim = null;
                    _busy = false;
                    _view.ShowPlanning(_s.Board);
                    if (!_s.CanPlay) { Sfx.Play("lose"); ShowOut(); }
                    RefreshHud();
                }));
            }
            RefreshHud();
        }

        void Restart()
        {
            Log("restart");
            Load(_level);
        }

        // ---------- tutorial (niveis 1 e 2): anel na peca que falta, depois no LIBERAR ----------

        void RefreshTutorial()
        {
            bool hintOk = !_failCard.gameObject.activeSelf;
            _hint.gameObject.SetActive(hintOk);
            if (!_tutorial || _s == null || _sim != null) { _tut.Hide(); return; }
            Solver.Result r = Solver.Solve(_s.Board);
            if (r.Best == null) { _tut.Hide(); return; }
            for (int j = 0; j < r.Vars.Count; j++)
            {
                int cell = r.Vars[j];
                if (_s.Board.Cells[cell].State == r.Best[j]) continue;
                _tut.PointWorld(_view.CellWorld(cell % _s.Board.W, cell / _s.Board.W));
                return;
            }
            _hint.gameObject.SetActive(false); // a seta do tutorial fica na faixa da dica: anel + seta no LIBERAR ja dizem o que fazer
            _tut.PointUi((RectTransform)_releaseBtn.transform);
        }

        // ---------- camera e HUD ----------

        /// <summary>O tabuleiro ocupa a faixa entre a HUD de cima (termina em 86%) e a de baixo (22%).</summary>
        void Fit()
        {
            _screen = new Vector2Int(Screen.width, Screen.height);
            float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);
            Board b = _s.Board;
            float size = Mathf.Max((b.H + 0.7f) / (2f * 0.62f), (b.W + 0.7f) / (2f * aspect * 0.94f));
            _cam.orthographicSize = size;
            _cam.transform.position = new Vector3(0f, -0.08f * size, -10f);
            Rect r = Screen.safeArea;
            _safe.anchorMin = new Vector2(r.xMin / Screen.width, r.yMin / Screen.height);
            _safe.anchorMax = new Vector2(r.xMax / Screen.width, r.yMax / Screen.height);
        }

        void BuildHud()
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            var canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var sc = canvasGo.GetComponent<CanvasScaler>();
            sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            sc.referenceResolution = new Vector2(1080, 1920);
            sc.matchWidthOrHeight = 0.5f;
            _safe = Art.Node(canvasGo.transform, "AreaSegura", Vector2.zero, Vector2.one);
            _hud = Art.Node(_safe, "HudNivel", Vector2.zero, Vector2.one);

            // topo: voltar | Nivel N | total de estrelas
            Button backBtn = Art.NewButton(_hud, "", 1, Art.CellTap, new Vector2(0.03f, 0.905f), new Vector2(0.16f, 0.985f), OpenMenu, false);
            Art.Icon(backBtn.transform, "Chevron", Art.Chevron(), Art.Ink, new Vector2(0.28f, 0.25f), new Vector2(0.72f, 0.75f));
            _title = Art.NewText(_hud, "Titulo", 60, new Vector2(0.19f, 0.905f), new Vector2(0.6f, 0.985f), TextAnchor.MiddleLeft, true);
            _stars = Art.NewText(_hud, "Estrelas", 52, new Vector2(0.6f, 0.905f), new Vector2(0.86f, 0.985f), TextAnchor.MiddleRight);
            Art.Icon(_hud, "Estrela", Art.Star(), Art.Accent, new Vector2(0.87f, 0.92f), new Vector2(0.96f, 0.97f));
            Text lib = Art.NewText(_hud, "Liberacoes", 34, new Vector2(0.19f, 0.862f), new Vector2(0.42f, 0.9f), TextAnchor.MiddleLeft);
            lib.text = "Liberações";
            lib.color = Art.ComAlfa(Art.Ink, 0.9f);
            _dots = Art.Node(_hud, "Pontos", new Vector2(0.42f, 0.866f), new Vector2(0.7f, 0.896f));

            // base: dica / cartao de falha | RECOMECAR | LIBERAR / SEGURE
            _hint = Art.NewText(_hud, "Dica", 38, new Vector2(0.05f, 0.125f), new Vector2(0.95f, 0.215f));
            _failCard = Art.Panel(_hud, "Falha", Art.ComAlfa(Art.Bad, 0.22f), new Vector2(0.04f, 0.125f), new Vector2(0.96f, 0.215f), 30f).rectTransform;
            _failIcon = Art.Icon(_failCard, "Icone", Art.Cross(), Art.Bad, new Vector2(0.025f, 0.2f), new Vector2(0.15f, 0.8f));
            _failTitle = Art.NewText(_failCard, "Titulo", 40, new Vector2(0.18f, 0.5f), new Vector2(0.98f, 0.96f), TextAnchor.MiddleLeft, true);
            _failTitle.color = Art.Bad;
            _failBody = Art.NewText(_failCard, "Corpo", 34, new Vector2(0.18f, 0.04f), new Vector2(0.98f, 0.5f), TextAnchor.UpperLeft);
            _failCard.gameObject.SetActive(false);

            _againBtn = Art.NewButton(_hud, "RECOMEÇAR", 30, Art.CellTap, new Vector2(0.03f, 0.025f), new Vector2(0.3f, 0.115f), () =>
            {
                if (_sim != null || _busy) return;
                Restart();
            }, false);
            _releaseBtn = Art.NewButton(_hud, "LIBERAR", 64, Art.Accent, new Vector2(0.33f, 0.025f), new Vector2(0.97f, 0.115f), Release);
            _hold = Art.Panel(_hud, "Segure", Art.CellTap, new Vector2(0.33f, 0.025f), new Vector2(0.97f, 0.115f));
            _holdIcon = Art.Icon(_hold.transform, "Icone", Art.FastForward(), Art.Ink, new Vector2(0.05f, 0.28f), new Vector2(0.2f, 0.72f));
            _holdText = Art.NewText(_hold.transform, "Texto", 34, new Vector2(0.22f, 0f), new Vector2(0.97f, 1f), TextAnchor.MiddleCenter, true);
            _hold.gameObject.SetActive(false);

            // painel de resultado (vitoria / acabaram as liberacoes / fim)
            _panel = Art.Node(_hud, "Painel", new Vector2(-1f, -1f), new Vector2(2f, 2f));
            _panel.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
            Image box = Art.Panel(_panel, "Caixa", Art.Cell, new Vector2(0.36f, 0.4f), new Vector2(0.64f, 0.6f), 44f);
            _panelTitle = Art.NewText(box.transform, "Titulo", 56, new Vector2(0.05f, 0.78f), new Vector2(0.95f, 0.97f), TextAnchor.MiddleCenter, true);
            _panelStars = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                float x0 = 0.2f + i * 0.2f;
                _panelStars[i] = Art.Icon(box.transform, "Estrela" + i, Art.Star(), Art.Accent, new Vector2(x0, 0.5f), new Vector2(x0 + 0.2f, 0.76f));
            }
            _panelBody = Art.NewText(box.transform, "Corpo", 36, new Vector2(0.05f, 0.3f), new Vector2(0.95f, 0.5f));
            _panelSecondary = Art.NewButton(box.transform, "MENU", 34, Art.CellTap, new Vector2(0.05f, 0.06f), new Vector2(0.34f, 0.27f), () => _secondaryAction?.Invoke(), false);
            _panelPrimary = Art.NewButton(box.transform, "PRÓXIMO", 48, Art.Accent, new Vector2(0.37f, 0.06f), new Vector2(0.95f, 0.27f), () => _primaryAction?.Invoke());
            _panel.gameObject.SetActive(false);
        }

        void RefreshHud()
        {
            _title.text = $"Nível {_level + 1}";
            _stars.text = _progress.Total.ToString();
            foreach (Transform c in _dots) Destroy(c.gameObject);
            for (int i = 0; i < _s.Max; i++)
            {
                bool left = i < _s.Max - _s.Used;
                Art.Icon(_dots, "Ponto", left ? Art.Disc() : Art.Ring(0.7f), left ? Art.Accent : Art.PipeEdge,
                    new Vector2(i * 0.25f, 0f), new Vector2(i * 0.25f + 0.2f, 1f));
            }
            bool planning = _sim == null;
            _releaseBtn.gameObject.SetActive(planning);
            _hold.gameObject.SetActive(!planning);
            Art.SetEnabled(_releaseBtn, planning && !_busy && _s.CanPlay);
            Art.SetEnabled(_againBtn, planning && !_busy);
        }

        void SetHold(bool held)
        {
            _hold.color = held ? Art.Accent : Art.CellTap;
            _holdIcon.color = held ? Art.Rock : Art.Ink;
            _holdText.color = held ? Art.Rock : Art.Ink;
            _holdText.text = held ? "2× MAIS RÁPIDO" : "SEGURE PARA ACELERAR";
        }

        void ShowFailCard(Outcome o, int left)
        {
            string title, body;
            Sprite icon;
            float angle = 0f;
            switch (o)
            {
                case Outcome.WrongColor: title = "Cor errada!"; body = "O orbe entrou num cristal de outra cor e ele rachou."; icon = Art.Cross(); break;
                case Outcome.Spill: title = "Derramou!"; body = "O orbe saiu por um canal aberto, sem destino."; icon = Art.Arrow(); angle = -90f; break;
                case Outcome.Overflow: title = "Transbordou!"; body = "Aquele cristal já estava cheio."; icon = Art.Plus(); break;
                default: title = "Travou!"; body = "Os orbes pararam numa comporta que nunca abre."; icon = Art.Pause(); break;
            }
            if (left > 0) body += left > 1 ? $" Ajuste e libere de novo ({left} restantes)." : " Ajuste e libere de novo (1 restante).";
            _failIcon.sprite = icon;
            _failIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            _failTitle.text = title;
            _failBody.text = body;
            _hint.gameObject.SetActive(false);
            _failCard.gameObject.SetActive(true);
        }

        void HideFailCard()
        {
            _failCard.gameObject.SetActive(false);
            _hint.gameObject.SetActive(true);
        }

        void ShowPanel(string title, string body, int stars, string primary, Action onPrimary, string secondary, Action onSecondary)
        {
            _busy = true;
            _panel.gameObject.SetActive(true);
            _panelTitle.text = title;
            _panelBody.text = body;
            Art.SetLabel(_panelPrimary, primary);
            _primaryAction = onPrimary;
            _panelSecondary.gameObject.SetActive(secondary != null);
            if (secondary != null) Art.SetLabel(_panelSecondary, secondary);
            _secondaryAction = onSecondary;
            if (_starsAnim != null) StopCoroutine(_starsAnim);
            for (int i = 0; i < 3; i++) _panelStars[i].enabled = false;
            if (stars >= 0) _starsAnim = StartCoroutine(AnimateStars(stars));
        }

        IEnumerator AnimateStars(int stars)
        {
            for (int i = 0; i < 3; i++)
            {
                bool on = i < stars;
                Image s = _panelStars[i];
                s.enabled = true;
                s.color = on ? Art.Accent : Art.ComAlfa(Art.PipeEdge, 0.9f);
                if (on) Sfx.Play("star", 1f + 0.2f * i);
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime / 0.35f;
                    s.rectTransform.localScale = Vector3.one * (on ? Ease.OutBack(t) : 0.7f * Ease.OutCubic(t));
                    yield return null;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        void ClosePanel()
        {
            if (_starsAnim != null) { StopCoroutine(_starsAnim); _starsAnim = null; }
            _panel.gameObject.SetActive(false);
            _primaryAction = _secondaryAction = null;
        }

        void ShowWin()
        {
            if (_level >= _levels.Length - 1)
                ShowPanel("Fim do protótipo!", $"Você fez {_progress.Total} de {_levels.Length * 3} estrelas.\nObrigado por testar!", -1, "MENU", OpenMenu, null, null);
            else
                ShowPanel($"Nível {_level + 1} concluído!", _s.Used == 1 ? "Na primeira liberação!" : $"{_s.Used} liberações", _s.Stars,
                    "PRÓXIMO", () => Load(_level + 1), "MENU", OpenMenu);
        }

        void ShowOut() => ShowPanel("Acabaram as liberações", "Recomece o nível e tente outro plano.", -1, "RECOMEÇAR", Restart, "MENU", OpenMenu);

        // ---------- dev e diario ----------

        static string Arg(string name)
        {
            string[] a = Environment.GetCommandLineArgs();
            int i = Array.IndexOf(a, name);
            if (i < 0) return null;
            return i + 1 < a.Length && !a[i + 1].StartsWith("-") ? a[i + 1] : "";
        }

        static float ArgF(string name, float def) => Num(Arg(name), def);

        static float Num(string s, float def) =>
            !string.IsNullOrEmpty(s) && float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ? v : def;

        static IEnumerator After(float seconds, Action a)
        {
            yield return new WaitForSeconds(seconds);
            a();
        }

        /// <summary>
        /// Aplica a solucao de menos toques (dev: -solve; vira o booster "Revelar runa" depois). `step` 0 = de uma vez,
        /// sem som; &gt;0 = um toque a cada `step` s, com o anel do tutorial marcando a peca antes (parece um dedo).
        /// `leaveOne` deixa a peca de FailVar a 1 toque da solucao.
        /// </summary>
        IEnumerator Solve(float step, bool leaveOne)
        {
            Solver.Result r = Solver.Solve(_s.Board);
            if (r.Best == null) yield break;
            int keep = leaveOne ? FailVar(r) : -1;
            for (int j = 0; j < r.Vars.Count; j++)
            {
                int cell = r.Vars[j], x = cell % _s.Board.W, y = cell / _s.Board.W, n = _s.Board.Cells[cell].States;
                int target = j == keep ? (r.Best[j] + n - 1) % n : r.Best[j];
                while (_s.Board.Cells[cell].State != target)
                {
                    if (step <= 0f) { if (!_s.Tap(x, y)) break; _view.OnTap(x, y, _s.Board.At(x, y)); continue; }
                    _tut.PointWorld(_view.CellWorld(x, y));
                    yield return new WaitForSeconds(step);
                    if (!TapCell(x, y)) break;
                }
            }
            _view.ShowPlanning(_s.Board);
            RefreshTutorial();
        }

        /// <summary>A peca que, a 1 toque da solucao, falha do jeito mais legivel: cor errada antes de derrame, e o mais tarde possivel.</summary>
        int FailVar(Solver.Result r)
        {
            int best = -1, bestScore = -1;
            for (int j = 0; j < r.Vars.Count; j++)
            {
                Board b = _s.Board.Clone();
                for (int k = 0; k < r.Vars.Count; k++) b.Cells[r.Vars[k]].State = r.Best[k];
                ref Tile t = ref b.Cells[r.Vars[j]];
                t.State = (t.State + t.States - 1) % t.States;
                var sim = new FlowSim(b);
                sim.Run();
                int score = (sim.Outcome == Outcome.WrongColor ? 1000 : 0) + sim.Tick;
                if (score > bestScore) { bestScore = score; best = j; }
            }
            Debug.Log($"DEMO almost: peca ({r.Vars[best] % _s.Board.W},{r.Vars[best] / _s.Board.W}) fica errada");
            return best;
        }

        /// <summary>
        /// Dev: roteiro de criativo (-demo "almost wait:0.4 release solve:0.5 release"). Passos separados por espaco:
        /// wait:s | solve[:step] (padrao 0.15) | almost[:step] (padrao 0 = ja comeca assim) | tap:x,y | release
        /// (libera e espera a cascata acabar e o tabuleiro voltar ao planejamento). Com -record, o video acaba junto com o roteiro.
        /// </summary>
        IEnumerator Demo(string script)
        {
            yield return null;
            foreach (string step in script.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] kv = step.Split(':');
                string arg = kv.Length > 1 ? kv[1] : "";
                switch (kv[0])
                {
                    case "wait": yield return new WaitForSeconds(Num(arg, 1f)); break;
                    case "solve": yield return Solve(Num(arg, 0.15f), false); break;
                    case "almost": yield return Solve(Num(arg, 0f), true); break;
                    case "tap":
                        string[] xy = arg.Split(',');
                        int x = (int)Num(xy[0], -1f), y = (int)Num(xy.Length > 1 ? xy[1] : "", -1f);
                        _tut.PointWorld(_view.CellWorld(x, y));
                        yield return new WaitForSeconds(0.15f);
                        TapCell(x, y);
                        break;
                    case "release":
                        Release();
                        while (_sim != null) yield return null;
                        break;
                    default: Debug.LogWarning($"DEMO passo desconhecido: {step}"); break;
                }
            }
            if (_rec != null) _recFrames = Mathf.Min(_recFrames, _recFrame); // com -record, o fim do roteiro encerra o video
        }

        IEnumerator Shot(string path)
        {
            float delay = ArgF("-shotdelay", 1f);
            yield return null;
            if (_s != null && !_menu.Visible)
            {
                if (Arg("-solve") != null) yield return Solve(0f, false);
                if (Arg("-release") != null) Release();
            }
            yield return new WaitForSeconds(delay);
            ScreenCapture.CaptureScreenshot(path);
            yield return null;
            yield return null;
            Application.Quit(0);
        }

        void Autoplay()
        {
            int ok = 0;
            for (int i = 0; i < _levels.Length; i++)
            {
                var s = new Session(Board.Parse(_levels[i]));
                Solver.Result r = Solver.Solve(s.Board);
                if (r.Best != null)
                    for (int j = 0; j < r.Vars.Count; j++) s.Board.Cells[r.Vars[j]].State = r.Best[j];
                FlowSim sim = s.Release();
                sim.Run();
                s.End(sim);
                Debug.Log($"AUTOPLAY L{i + 1:000} {(s.Won ? "OK" : "FALHOU " + sim.Outcome)} estrelas={s.Stars} ticks={sim.Tick}");
                if (s.Won) ok++;
            }
            Debug.Log($"AUTOPLAY {(ok == _levels.Length ? "OK" : "FALHOU")} {ok}/{_levels.Length}");
            Application.Quit(ok == _levels.Length ? 0 : 1);
        }

        void Log(string evt, string a = "", string b = "")
        {
            try { File.AppendAllText(_diary, $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss},{_sid},{evt},{_level + 1},{a},{b}\n"); }
            catch (Exception) { } // ponytail: diario e' best-effort; disco cheio nunca derruba o jogo
        }
    }
}
