using System;
using System.Collections.Generic;
using UnityEngine;

namespace RR
{
    /// <summary>
    /// SFX sintetizados em runtime (zero asset). Cada som e' a SOMA de parciais (seno com varredura de frequencia,
    /// envelope ataque curto + decaimento exponencial, ruido opcional): acordes e arpejos curtos dao o "magico"
    /// (GDD §12). Respeita Settings.Sound.
    /// </summary>
    public static class Sfx
    {
        const int Rate = 44100;
        static AudioSource _src;
        static readonly Dictionary<string, AudioClip> Clips = new Dictionary<string, AudioClip>();

        public static void Init(GameObject host)
        {
            _src = host.AddComponent<AudioSource>();
            _src.playOnAwake = false;
            float[] d;

            d = New(0.07f); Add(d, 1100, 1500, 0f, 0.06f, 0.45f); Add(d, 2200, 3000, 0f, 0.05f, 0.12f); Done("tap", d);
            d = New(0.12f); Add(d, 760, 560, 0f, 0.045f, 0.4f); Add(d, 980, 760, 0.06f, 0.05f, 0.4f); Done("flip", d);
            d = New(0.05f); Add(d, 900, 1100, 0f, 0.045f, 0.35f); Done("click", d);
            d = New(0.35f); Add(d, 200, 900, 0f, 0.3f, 0.3f); Add(d, 400, 1800, 0f, 0.3f, 0.12f); Add(d, 600, 1200, 0f, 0.26f, 0.35f, 0.75f); Done("release", d);
            d = New(0.18f); Add(d, 660, 990, 0f, 0.15f, 0.4f); Add(d, 1320, 1980, 0f, 0.12f, 0.2f); Done("absorb", d);
            // cristal completo: acorde maior em cascata (C5 E5 G5 + C6)
            d = New(0.5f); Add(d, 523, 523, 0f, 0.42f, 0.28f); Add(d, 659, 659, 0.04f, 0.4f, 0.26f); Add(d, 784, 784, 0.08f, 0.4f, 0.26f); Add(d, 1046, 1046, 0.12f, 0.35f, 0.12f); Done("full", d);
            // comporta destravando: clique + quinta subindo
            d = New(0.3f); Add(d, 500, 300, 0f, 0.03f, 0.45f, 0.6f); Add(d, 392, 587, 0.03f, 0.25f, 0.35f); Add(d, 784, 1175, 0.06f, 0.2f, 0.12f); Done("gate", d);
            d = New(0.5f); Add(d, 170, 70, 0f, 0.45f, 0.55f, 0.3f); Add(d, 85, 40, 0f, 0.45f, 0.4f); Add(d, 300, 120, 0f, 0.1f, 0.3f, 0.8f); Done("fail", d);
            d = New(0.5f); Add(d, 440, 440, 0f, 0.22f, 0.3f); Add(d, 330, 330, 0.2f, 0.3f, 0.3f); Done("lose", d);
            // vitoria: arpejo C5 E5 G5 C6 E6 com brilho uma oitava acima
            d = New(1.1f);
            float[] arp = { 523f, 659f, 784f, 1046f, 1318f };
            for (int i = 0; i < arp.Length; i++)
            {
                Add(d, arp[i], arp[i], i * 0.11f, 0.5f, 0.26f);
                Add(d, arp[i] * 2f, arp[i] * 2f, i * 0.11f, 0.3f, 0.07f);
            }
            Done("win", d);
            d = New(0.4f); Add(d, 1318, 1318, 0f, 0.35f, 0.3f); Add(d, 2636, 2636, 0f, 0.2f, 0.1f); Done("star", d);
        }

        public static void Play(string name, float pitch = 1f, float volume = 0.6f)
        {
            if (_src == null || !Settings.Sound || !Clips.TryGetValue(name, out AudioClip c)) return;
            _src.pitch = pitch;
            _src.PlayOneShot(c, volume);
        }

        static float[] New(float dur) => new float[Mathf.CeilToInt(dur * Rate)];

        /// <summary>Soma um parcial: seno varrendo f0..f1 a partir de t0 por dur segundos; `noise` mistura ruido branco.</summary>
        static void Add(float[] d, float f0, float f1, float t0, float dur, float amp, float noise = 0f)
        {
            int start = Mathf.FloorToInt(t0 * Rate);
            int n = Mathf.Min(d.Length - start, Mathf.CeilToInt(dur * Rate));
            var rng = new System.Random(start * 31 + (int)f0);
            double phase = 0;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)n;
                phase += 2 * Math.PI * Mathf.Lerp(f0, f1, t) / Rate;
                float env = Mathf.Min(1f, i / 150f) * Mathf.Exp(-4.5f * t);
                float v = (float)Math.Sin(phase) * (1f - noise) + ((float)rng.NextDouble() * 2f - 1f) * noise;
                d[start + i] += v * env * amp;
            }
        }

        static void Done(string name, float[] d)
        {
            for (int i = 0; i < d.Length; i++) d[i] = Mathf.Clamp(d[i], -1f, 1f);
            AudioClip clip = AudioClip.Create(name, d.Length, 1, Rate, false);
            clip.SetData(d, 0);
            Clips[name] = clip;
        }
    }
}
