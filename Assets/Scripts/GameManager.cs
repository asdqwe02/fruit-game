using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Image fadeImage;

    [SerializeField] private List<Blade> _blades;
    [SerializeField] Spawner spawner;
    [SerializeField] private float _countDownTime;
    private int score;
    public static GameManager Instance;
    private Camera _mainCamera;
    private CameraEffectController _cameraEffectController;
    [SerializeField] private Text _countDownText;
    [SerializeField] private Text _finalScoreText;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private GameObject _instructionScreen;
    private Coroutine countDownCoroutine;
    public bool Playing = false;
    [SerializeField] private GameObject _startFruit;
    public int StartFruitCount = 0;
    private int _inactiveBlades = 0;

    public int InactiveBlades
    {
        get { return _inactiveBlades; }
        set
        {
            _inactiveBlades = value;
            if (_inactiveBlades < 0)
                _inactiveBlades = 0;
            OnInactiveBlade();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _mainCamera = Camera.main;
        _cameraEffectController = _mainCamera.GetComponent<CameraEffectController>();
        // spawner = FindObjectOfType<Spawner>();
    }

    private void Start()
    {
        // NewGame();
        // Debug.Log(GetBladeStartPosition());
    }

    private void NewGame()
    {
        Time.timeScale = 1f;
        _startFruit.SetActive(false);
        _instructionScreen.SetActive(false);
        ClearScene();

        foreach (var blade in _blades)
        {
            blade.enabled = true;
        }

        spawner.enabled = true;
        Playing = true;

        score = 0;
        scoreText.text = score.ToString();
        _countDownText.text = _countDownTime.ToString();
        countDownCoroutine = StartCoroutine(CountDown());
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            if (!fruit.SliceToStart)
                Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void Explode(Transform bomb = null)
    {
        foreach (var blade in _blades)
        {
            blade.enabled = true;
        }
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence(bomb));
    }

    private void Update()
    {
        if (!Playing && StartFruitCount >= 2)
        {
            StartFruitCount = 0;
            ResetGame();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
            _startFruit.SetActive(true);
            _endScreen.SetActive(false);
        }
    }

    private IEnumerator ExplodeSequence(Transform bomb = null)
    {
        Time.timeScale = 0.25f;
        float elapsed = 0f;
        float duration = 0.5f;

        // StartCoroutine(_cameraEffectController.ZoomIn(bomb, 2f, 0.45f));
        yield return _cameraEffectController.Shake(2f, 1.25f);
        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        _cameraEffectController.ResetCameraPosition();
        // NewGame();
        EndGame();
        elapsed = 0f;

        // Fade back in
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }

    IEnumerator CountDown()
    {
        float elapsedTime = _countDownTime;
        while (elapsedTime > 0)
        {
            yield return new WaitForEndOfFrame();
            _countDownText.text = "Time: " + elapsedTime.ToString("0");
            elapsedTime -= Time.deltaTime;
        }

        EndGame();
    }

    public void ResetGame()
    {
        Debug.Log("Reset Game and show highscore");
        _endScreen.SetActive(false);
        NewGame();
    }

    public void EndGame()
    {
        Playing = false;
        spawner.enabled = false;
        StopCoroutine(countDownCoroutine);
        _endScreen.SetActive(true);
        _instructionScreen.SetActive(true);
        _finalScoreText.text = "Final Score: " + score;
        StartCoroutine(TurnOffEndScreen(5f));
        Time.timeScale = 0f;
    }

    public Vector3 GetBladeStartPosition()
    {
        return _mainCamera.ScreenToWorldPoint(Vector3.zero);
    }

    public Vector3 GetBottomRightBoundPos()
    {
        return _mainCamera.ScreenToWorldPoint(new Vector3(_mainCamera.pixelWidth, 0, 0));
    }

    public Vector3 GetTopLeftBoundPos()
    {
        return _mainCamera.ScreenToWorldPoint(new Vector3(0, _mainCamera.pixelHeight, 0));
    }

    public Vector3 GetUpperRightBoundaryPos()
    {
        return _mainCamera.ScreenToWorldPoint(new Vector3(_mainCamera.pixelWidth, _mainCamera.pixelHeight, 0));
    }

    public Vector2 GetBoundary()
    {
        Vector3 upperRightCorner = GetUpperRightBoundaryPos();
        Vector3 bladeStartPos = GetBladeStartPosition();
        return new Vector2(upperRightCorner.x - bladeStartPos.x, upperRightCorner.y - bladeStartPos.y);
    }

    IEnumerator TurnOffEndScreen(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1;
        _endScreen.SetActive(false);
        _startFruit.SetActive(true);
        ClearScene();
    }

    private void OnInactiveBlade()
    {
        if (_inactiveBlades >= 2)
        {
            EndGame();
        }
    }
}