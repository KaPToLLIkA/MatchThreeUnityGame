using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScalableBG : MonoBehaviour
{
    public int pixelsPerUnit = 100;

    private Sprite _sprite;
    private float _scale;
    private float _camHeight;
    
    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
        _scale = CalculateScale();
        var currentCam = Camera.main;
        _camHeight = currentCam.orthographicSize * 2;
        transform.localScale = new Vector3(_scale, _scale, 1);
    }

    private void FixedUpdate()
    {
        var newScale = CalculateScale();
        if (!(Mathf.Abs(newScale - _scale) >= 0.001)) return;
        _scale = newScale;
        transform.localScale = new Vector3(_scale, _scale, 1);
    }

    private float CalculateScale()
    {
        var sHeight = _sprite.texture.height / pixelsPerUnit;
        return _camHeight / sHeight;
    }
}
