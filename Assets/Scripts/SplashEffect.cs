using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashEffect : MonoBehaviour
{
    [SerializeField] private float _fadeTime;
    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        // _defaultColor = _spriteRenderer.color;
    }

    private void Start()
    {
        StartCoroutine(FadeAndDestroy());
    }

    public void SetColor(Color color)
    {
        _defaultColor = color;
        _spriteRenderer.color = color;
    }

    IEnumerator FadeAndDestroy()
    {
        float elapsedTime = 0;
        while (elapsedTime < _fadeTime)
        {
            Color color = _defaultColor;
            color.a = _defaultColor.a * (_fadeTime - elapsedTime) / _fadeTime;
            _spriteRenderer.color = color;
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
        }

        Destroy(gameObject);
    }
}