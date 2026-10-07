using UnityEngine;
using UnityEngine.UI;

namespace RR
{
    /// <summary>
    /// "Mao fantasma" dos niveis 1 e 2: anel pulsante sobre a peca que falta girar (mundo) e depois anel + seta
    /// sobre o botao LIBERAR (canvas). O Game decide o alvo (pelo Solver) e esconde apos o toque.
    /// </summary>
    public sealed class Tutorial : MonoBehaviour
    {
        Transform _world;
        RectTransform _ui, _arrow;
        float _t;

        public static Tutorial Create(Transform worldParent, RectTransform uiParent)
        {
            var go = new GameObject("Tutorial");
            go.transform.SetParent(worldParent, false);
            var t = go.AddComponent<Tutorial>();
            t._world = new GameObject("AnelMundo").transform;
            t._world.SetParent(go.transform, false);
            Art.NewSprite(t._world, "Halo", Art.Halo(), Art.ComAlfa(Art.Accent, 0.6f), 32, Vector2.zero, Vector2.one * 1.6f);
            Art.NewSprite(t._world, "Anel", Art.Ring(0.86f), Art.Accent, 33, Vector2.zero, Vector2.one * 1.02f);
            t._world.gameObject.SetActive(false);

            t._ui = Art.Node(uiParent, "AnelUI", Vector2.zero, Vector2.one);
            Image ring = Art.Icon(t._ui, "Anel", Art.SlabFrame(), Art.Accent, new Vector2(-0.04f, -0.12f), new Vector2(1.04f, 1.12f));
            ring.preserveAspect = false;
            t._arrow = Art.Icon(t._ui, "Seta", Art.Arrow(), Art.Accent, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f)).rectTransform;
            t._arrow.sizeDelta = new Vector2(90f, 90f);
            t._arrow.localRotation = Quaternion.Euler(0f, 0f, -90f);
            t._ui.gameObject.SetActive(false);
            return t;
        }

        public void PointWorld(Vector3 pos)
        {
            _world.position = pos;
            _world.gameObject.SetActive(true);
            _ui.gameObject.SetActive(false);
        }

        public void PointUi(RectTransform target)
        {
            _ui.SetParent(target, false);
            _ui.anchorMin = Vector2.zero; _ui.anchorMax = Vector2.one;
            _ui.offsetMin = _ui.offsetMax = Vector2.zero;
            _ui.gameObject.SetActive(true);
            _world.gameObject.SetActive(false);
        }

        public void Hide()
        {
            _world.gameObject.SetActive(false);
            _ui.gameObject.SetActive(false);
        }

        void Update()
        {
            _t += Time.deltaTime;
            float pulse = 0.5f + 0.5f * Mathf.Sin(_t * 4.5f);
            _world.localScale = Vector3.one * (0.95f + 0.12f * pulse);
            _ui.localScale = Vector3.one * (1f + 0.04f * pulse);
            _arrow.anchoredPosition = new Vector2(0f, 95f + 30f * pulse);
        }
    }
}
