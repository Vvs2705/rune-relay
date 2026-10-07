using UnityEngine;

namespace RR
{
    /// <summary>Configuracoes minimas, persistidas no PlayerPrefs (fora do save de progresso).</summary>
    public static class Settings
    {
        const string SoundKey = "rr.sound", VibeKey = "rr.vibe";
        static bool _loaded, _sound, _vibe;

        public static bool Sound { get { Load(); return _sound; } set { Load(); _sound = value; PlayerPrefs.SetInt(SoundKey, value ? 1 : 0); PlayerPrefs.Save(); } }
        public static bool Vibe { get { Load(); return _vibe; } set { Load(); _vibe = value; PlayerPrefs.SetInt(VibeKey, value ? 1 : 0); PlayerPrefs.Save(); } }

        static void Load()
        {
            if (_loaded) return;
            _loaded = true;
            _sound = PlayerPrefs.GetInt(SoundKey, 1) == 1;
            _vibe = PlayerPrefs.GetInt(VibeKey, 1) == 1;
        }
    }

    /// <summary>Curvas de tween (por codigo, sem DOTween).</summary>
    public static class Ease
    {
        public static float OutCubic(float t) { t = Mathf.Clamp01(t); return 1f - Mathf.Pow(1f - t, 3f); }
        public static float OutBack(float t) { t = Mathf.Clamp01(t); const float c = 1.70158f; float u = t - 1f; return 1f + u * u * ((c + 1f) * u + c); }
        /// <summary>Sobe ate' 1 e volta a 0 (punch).</summary>
        public static float Punch(float t) { t = Mathf.Clamp01(t); return Mathf.Sin(t * Mathf.PI) * (1f - t * 0.5f); }
    }

    /// <summary>
    /// O UNICO ParticleSystem do jogo: material criado em runtime (Sprites/Default, sempre incluido no build) com a
    /// textura de brilho do Art. Nunca emite sozinho: Burst() emite N particulas por chamada. Tambem o haptic.
    /// </summary>
    public sealed class Fx : MonoBehaviour
    {
        static Fx _i;
        ParticleSystem _ps;

        public static void Init(Transform parent)
        {
            if (_i != null) return;
            var go = new GameObject("Fx");
            go.transform.SetParent(parent, false);
            _i = go.AddComponent<Fx>();
            _i._ps = go.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = _i._ps.main;
            main.loop = true;            // fica "tocando" para sempre sem emitir nada: Emit() sempre funciona
            main.playOnAwake = false;
            main.startLifetime = 0.7f;
            main.startSpeed = 0f;
            main.startSize = 0.2f;
            main.gravityModifier = 0.25f;
            main.maxParticles = 1024;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            ParticleSystem.EmissionModule em = _i._ps.emission;
            em.enabled = false;
            ParticleSystem.ShapeModule sh = _i._ps.shape;
            sh.enabled = false;
            var g = new Gradient();
            g.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.9f, 0.5f), new GradientAlphaKey(0f, 1f) });
            ParticleSystem.ColorOverLifetimeModule col = _i._ps.colorOverLifetime;
            col.enabled = true;
            col.color = new ParticleSystem.MinMaxGradient(g);
            ParticleSystem.SizeOverLifetimeModule sz = _i._ps.sizeOverLifetime;
            sz.enabled = true;
            sz.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0.1f));
            var r = go.GetComponent<ParticleSystemRenderer>();
            r.renderMode = ParticleSystemRenderMode.Billboard;
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("UI/Default"); // ponytail: ambos sao always-included; o segundo e' so' rede de seguranca
            r.material = new Material(shader) { mainTexture = Art.Glow().texture };
            r.sortingOrder = 40;
            _i._ps.Play();
        }

        /// <summary>Explosao radial de `n` faiscas na cor `c` (tamanho e velocidade em unidades de mundo).</summary>
        public static void Burst(Vector3 pos, Color c, int n, float speed = 2.2f, float size = 0.22f)
        {
            if (_i == null) return;
            var ep = new ParticleSystem.EmitParams();
            for (int k = 0; k < n; k++)
            {
                float a = Random.value * Mathf.PI * 2f, s = speed * Random.Range(0.35f, 1f);
                ep.position = pos;
                ep.velocity = new Vector3(Mathf.Cos(a) * s, Mathf.Sin(a) * s, 0f);
                ep.startColor = Color.Lerp(c, Color.white, Random.Range(0f, 0.45f));
                ep.startSize = size * Random.Range(0.5f, 1.3f);
                ep.startLifetime = Random.Range(0.4f, 0.9f);
                _i._ps.Emit(ep, 1);
            }
        }

        /// <summary>Vibracao curta (falha e vitoria). So' no aparelho e so' com Settings.Vibe.</summary>
        public static void Haptic()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Settings.Vibe) Handheld.Vibrate();
#endif
        }
    }

    /// <summary>Fundo arcano: brilho indigo atras do tabuleiro + motas de luz que derivam devagar (atmosfera barata).</summary>
    public sealed class Backdrop : MonoBehaviour
    {
        SpriteRenderer[] _motes;
        Vector3[] _home;
        float[] _phase;

        public static Backdrop Create(Transform parent)
        {
            var go = new GameObject("Fundo");
            go.transform.SetParent(parent, false);
            var b = go.AddComponent<Backdrop>();
            Art.NewSprite(go.transform, "Brilho", Art.Glow(), Art.ComAlfa(Art.BgGlow, 0.9f), -20, new Vector2(0f, 0.4f), Vector2.one * 9f);
            Art.NewSprite(go.transform, "Brilho2", Art.Glow(), Art.ComAlfa(Art.Palette[4], 0.12f), -19, new Vector2(-1.5f, -2.5f), Vector2.one * 5f);
            const int n = 16;
            b._motes = new SpriteRenderer[n];
            b._home = new Vector3[n];
            b._phase = new float[n];
            var rng = new System.Random(7);
            for (int i = 0; i < n; i++)
            {
                b._home[i] = new Vector3((float)rng.NextDouble() * 7f - 3.5f, (float)rng.NextDouble() * 11f - 5.5f, 0f);
                b._phase[i] = (float)rng.NextDouble() * 6.28f;
                float s = 0.08f + (float)rng.NextDouble() * 0.16f;
                b._motes[i] = Art.NewSprite(go.transform, "Mota", Art.Glow(), Art.ComAlfa(Art.Ink, 0.2f), -18, b._home[i], Vector2.one * s);
            }
            return b;
        }

        void Update()
        {
            float t = Time.time;
            for (int i = 0; i < _motes.Length; i++)
            {
                float p = _phase[i];
                _motes[i].transform.localPosition = _home[i] + new Vector3(Mathf.Sin(t * 0.23f + p) * 0.3f, Mathf.Sin(t * 0.17f + p * 1.7f) * 0.4f + Mathf.Repeat(t * 0.05f + p, 1f) * 0.5f, 0f);
                _motes[i].color = Art.ComAlfa(Art.Ink, 0.08f + 0.14f * (0.5f + 0.5f * Mathf.Sin(t * 0.9f + p)));
            }
        }
    }
}
