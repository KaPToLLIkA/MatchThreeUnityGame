﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Game
{
    [Serializable]
    public class CellTypedSpriteRow
    {
        public CellType cellType;
        public Sprite sprite;
    }
    
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemController : MonoBehaviour, IFieldItem
    {
        public int pixelsPerUnit = 100;
        
        public List<CellTypedSpriteRow> sprites;

        [Header("Animations settings")] 
        public AnimationCurve explodeScaleCurve;
        public Vector3 explodeScaleTarget = new Vector3(0, 0, 0);
        public float explodeScaleSpeed = 1f;
        
        public CellType Type => _type;
        
        private CellType _type;
        private Field _field;
        private int _x;
        private int _y;
        
        private SpriteRenderer _spriteRenderer;

        public void Init(int x, int y, Field field, CellType type)
        {
            _x = x;
            _y = y;
            _field = field;
            _type = type;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            var row = sprites.Find(item => item.cellType == _type);
            _spriteRenderer.sprite = row.sprite;

            var maxDimension = (float)Mathf.Max(row.sprite.rect.width, row.sprite.rect.height);
            var curSize = maxDimension / pixelsPerUnit;

            var scale = field.CellSize / curSize;

            transform.localScale = new Vector3(scale, scale, 1);
        }

        public void Explode()
        {
            StartCoroutine(AsyncExplode());
        }

        private IEnumerator AsyncExplode()
        {
            yield return Animations.ScaleAnimator(gameObject, explodeScaleTarget, explodeScaleCurve, explodeScaleSpeed);

            _field[_x, _y] = null;
            
            Destroy(gameObject);
        }
    }
}