using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SAS.SceneManagement
{
    public enum FadeType
    {
        Alpha,
        Shutter,
        RadialWipe,
        GradientTexture
    }

    [Serializable]
    public class SpriteArrayWrapper
    {
        [ConditionalField("FadeType", FadeType.GradientTexture)] [SerializeField]
        private Sprite[] m_Sprites;

        public int Length => m_Sprites.Length;

        public Texture GetRandomTexture()
        {
            return m_Sprites[UnityEngine.Random.Range(0, m_Sprites.Length)].texture;
        }
    }

    public class SceneFader : MonoBehaviour, ILoadingScreen
    {
        [SerializeField] private FadeType m_FadeType;

        [ConditionalField(nameof(m_FadeType), FadeType.GradientTexture)] [SerializeField]
        private SpriteArrayWrapper m_FadePattern;

        [SerializeField] private Image m_Image;
        [SerializeField] private float m_FadeInDuration = 1f;
        [SerializeField] private float m_FadeOutDuration = 1f;
        private int _fadeAmount = Shader.PropertyToID("_Amount");
        private int _useShutters = Shader.PropertyToID("_UseShutters");
        private int _useRadialWipe = Shader.PropertyToID("_UseRadialWipe");
        private int _useGradientTexture = Shader.PropertyToID("_UseGradientTexture");
        private int _useAlpha = Shader.PropertyToID("_UseAlpha");
        private int _mainTex = Shader.PropertyToID("_MainTex");

        private int? _lastEffect;
        private Material _material;
        private TaskQueue _fadeQueue = new();

        protected virtual void Awake()
        {
            var material = m_Image.material;
            m_Image.material = new Material(material);
            _material = m_Image.material;
        }

        public virtual void SetActive(bool active)
        {
            _fadeQueue.Enqueue(done => { StartFade(active, done); });
        }

        private void StartFade(bool active, Action done)
        {
            if (active)
                gameObject.SetActive(true);

            FaderSetup(m_FadeType);
            var duration = active ? m_FadeInDuration : m_FadeOutDuration;
            StartCoroutine(Fade(active, duration, done));
        }

        private IEnumerator Fade(bool active, float duration, Action done)
        {
            var from = active ? 0f : 1f;
            var to = active ? 1f : 0f;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                SetFadeAmount(Mathf.Lerp(from, to, elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetFadeAmount(to);

            if (active)
                OnFadeInComplete?.Invoke();
            else
            {
                OnFadeOutComplete?.Invoke();
                gameObject.SetActive(false);
            }

            done?.Invoke();
        }

        private void FaderSetup(FadeType fadeType)
        {
            ResetAllFadeType();

            switch (fadeType)
            {
                case FadeType.Shutter:
                    SwitchEffect(_useShutters);
                    break;
                case FadeType.Alpha:
                    SwitchEffect(_useAlpha);
                    break;
                case FadeType.RadialWipe:
                    SwitchEffect(_useRadialWipe);
                    break;
                case FadeType.GradientTexture:
                    _material.SetTexture(_mainTex, m_FadePattern.GetRandomTexture());
                    SwitchEffect(_useGradientTexture);
                    break;
            }
        }

        private void SwitchEffect(int effectToTurnOn)
        {
            _material.SetFloat(effectToTurnOn, 1);
            _lastEffect = effectToTurnOn;
        }

        private void ResetAllFadeType()
        {
            _material.SetFloat(_useAlpha, 0);
            _material.SetFloat(_useShutters, 0);
            _material.SetFloat(_useRadialWipe, 0);
            _material.SetFloat(_useGradientTexture, 0);
        }

        public Action OnFadeInComplete { get; set; }
        public Action OnFadeOutComplete { get; set; }

        private void SetFadeAmount(float value)
        {
            _material.SetFloat(_fadeAmount, value);
        }
    }
}
