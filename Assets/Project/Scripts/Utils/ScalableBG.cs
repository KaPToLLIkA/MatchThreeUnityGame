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
    
    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
        _scale = CalculateScale();
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
        var currentCam = Camera.main;
        var camHeight = currentCam.orthographicSize * 2;

        var sHeight = _sprite.texture.height / pixelsPerUnit;

        return camHeight / sHeight;
    }
}
