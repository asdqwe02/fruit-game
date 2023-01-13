using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraEffectController : MonoBehaviour
{
    private Vector3 _startPosition;
    private Camera _camera;
    private float _originalSize;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _startPosition = transform.localPosition;
        _originalSize = _camera.orthographicSize;

    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = _startPosition;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = originalPos;
    }

    public IEnumerator ZoomIn(Transform target, float zoomDuration, float zoomMagnitude = 0.5f)
    {
        Vector3 originalPos = _startPosition;
        float elapsedTime = 0f;
        // zoom in
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            // transform.position = Vector3.MoveTowards(transform.position, target.position, 5f);
            transform.position = target.position;
            _camera.orthographicSize = _originalSize - _originalSize * zoomMagnitude * (elapsedTime / zoomDuration);
            yield return new WaitForEndOfFrame();
        }

        ResetCameraPosition();
    }

    public void ResetCameraPosition()
    {
        transform.position = _startPosition;
        _camera.orthographicSize = _originalSize;
    }
}