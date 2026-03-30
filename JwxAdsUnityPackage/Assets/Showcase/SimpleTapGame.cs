using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using UnityEngine.UI;

namespace JwxAdsSDK
{
    public class SimpleTapGame : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private int startScore = 0;
        [SerializeField] private float moveInterval = 1.2f;
        [SerializeField] private Vector2 targetSize = new Vector2(100, 100);
        [SerializeField] private float targetScale = 0.01f;
        [SerializeField] private string backgroundSpriteName = "bg";
        [SerializeField] private string targetSpriteName = "JWX-Main-Logo-1-2";
        [SerializeField] private Color32 tapParticleColor = new Color32(0xEB, 0x00, 0x43, 0xFF);
        [SerializeField] private int tapParticleCount = 20;
        [SerializeField] private float tapParticleSize = 18f;
        [SerializeField] private float tapParticleLifetime = 0.35f;
        [SerializeField] private float tapParticleSpeed = 240f;
        [Header("Ads")]
        [SerializeField] private int rewardedScoreThreshold = 10;

        private Text scoreText;
        private RectTransform canvasRect;
        private RectTransform targetRect;
        private int score;
        private Sprite tapParticleSprite;
        private bool rewardedShown;

        private void Start()
        {
            score = startScore;
            EnsureEventSystem();
            BuildUi();
            UpdateScore();
            StartCoroutine(MoveLoop());

            JwxAdsManager.InitializeAds();
            JwxAdsManager.LoadRewardedAd();
        }

        private static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            eventSystem.AddComponent<InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif
            DontDestroyOnLoad(eventSystem);
        }

        private void BuildUi()
        {
            var canvasObject = new GameObject("SimpleTapCanvas");
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();

            canvasRect = canvasObject.GetComponent<RectTransform>();

            CreateBackground(canvasObject.transform);

            scoreText = CreateText(canvasObject.transform, "ScoreText", "Score: 0", 36, TextAnchor.UpperLeft, FontStyle.Bold);
            scoreText.color = Color.black;
            var scoreRect = scoreText.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0f, 1f);
            scoreRect.anchorMax = new Vector2(0f, 1f);
            scoreRect.pivot = new Vector2(0f, 1f);
            scoreRect.anchoredPosition = new Vector2(20, -20);
            scoreRect.sizeDelta = new Vector2(260, 40);

            var instructions = CreateText(canvasObject.transform, "Instructions", "Tap the Logo", 36, TextAnchor.UpperCenter, FontStyle.Bold);
            instructions.color = Color.black;
            var instructionsRect = instructions.GetComponent<RectTransform>();
            instructionsRect.anchorMin = new Vector2(0.5f, 1f);
            instructionsRect.anchorMax = new Vector2(0.5f, 1f);
            instructionsRect.pivot = new Vector2(0.5f, 1f);
            instructionsRect.anchoredPosition = new Vector2(0, -90);
            instructionsRect.sizeDelta = new Vector2(600, 40);

            var targetObject = new GameObject("Target");
            targetObject.transform.SetParent(canvasObject.transform, false);

            targetRect = targetObject.AddComponent<RectTransform>();

            var image = targetObject.AddComponent<Image>();
            var targetSprite = LoadSprite(targetSpriteName);
            if (targetSprite != null)
            {
                image.sprite = targetSprite;
                image.preserveAspect = true;
                image.color = Color.white;
                image.SetNativeSize();
                targetRect.sizeDelta = targetRect.sizeDelta * targetScale;
            }
            else
            {
                image.color = new Color(0.2f, 0.65f, 1f, 1f);
                targetRect.sizeDelta = targetSize;
            }

            var button = targetObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(OnTargetClicked);

            MoveTarget();
        }

        private void CreateBackground(Transform parent)
        {
            var backgroundObject = new GameObject("Background");
            backgroundObject.transform.SetParent(parent, false);
            backgroundObject.transform.SetAsFirstSibling();

            var rect = backgroundObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = backgroundObject.AddComponent<Image>();
            var backgroundSprite = LoadSprite(backgroundSpriteName);
            if (backgroundSprite != null)
            {
                image.sprite = backgroundSprite;
                image.preserveAspect = false;
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            }
        }

        private static Text CreateText(Transform parent, string name, string text, int fontSize, TextAnchor alignment, FontStyle fontStyle = FontStyle.Normal)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            var rect = textObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 40);

            var label = textObject.AddComponent<Text>();
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = alignment;
            label.color = Color.white;
            label.fontStyle = fontStyle;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return label;
        }

        private IEnumerator MoveLoop()
        {
            var wait = new WaitForSeconds(moveInterval);
            while (true)
            {
                yield return wait;
                MoveTarget();
            }
        }

        private void OnTargetClicked()
        {
            score++;
            UpdateScore();
            TryShowRewardedAtScore();
            MoveTarget();
            SpawnTapEffect();
        }

        private void TryShowRewardedAtScore()
        {
            if (rewardedShown)
            {
                return;
            }

            if (score < rewardedScoreThreshold)
            {
                return;
            }

            rewardedShown = true;
            JwxAdsManager.ShowRewardedAd();
        }

        private void UpdateScore()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        private void MoveTarget()
        {
            if (canvasRect == null || targetRect == null)
            {
                return;
            }

            var size = targetRect.sizeDelta;
            var padding = new Vector2(40, 120);
            var min = canvasRect.rect.min + padding + (size * 0.5f);
            var max = canvasRect.rect.max - padding - (size * 0.5f);

            var x = Random.Range(min.x, max.x);
            var y = Random.Range(min.y, max.y);
            targetRect.anchoredPosition = new Vector2(x, y);
        }

        private void SpawnTapEffect()
        {
            if (targetRect == null)
            {
                return;
            }

            EnsureTapSprite();
            var parent = targetRect.parent;

            for (int i = 0; i < tapParticleCount; i++)
            {
                var particleObject = new GameObject("TapParticle");
                particleObject.transform.SetParent(parent, false);

                var rect = particleObject.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(tapParticleSize, tapParticleSize);
                rect.position = targetRect.position;

                var image = particleObject.AddComponent<Image>();
                image.sprite = tapParticleSprite;
                image.color = tapParticleColor;
                image.raycastTarget = false;

                var direction = Random.insideUnitCircle.normalized;
                var distance = Random.Range(tapParticleSpeed * 0.6f, tapParticleSpeed);
                var targetOffset = direction * distance;

                StartCoroutine(AnimateParticle(rect, image, targetOffset));
            }
        }

        private IEnumerator AnimateParticle(RectTransform rect, Image image, Vector2 offset)
        {
            var startPosition = rect.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < tapParticleLifetime)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / tapParticleLifetime);
                rect.anchoredPosition = startPosition + (offset * t);

                var color = image.color;
                color.a = Mathf.Lerp(1f, 0f, t);
                image.color = color;

                yield return null;
            }

            Destroy(rect.gameObject);
        }

        private void EnsureTapSprite()
        {
            if (tapParticleSprite != null)
            {
                return;
            }

            var texture = Texture2D.whiteTexture;
            tapParticleSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite LoadSprite(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                return null;
            }

            return Resources.Load<Sprite>(resourceName);
        }
    }
}
