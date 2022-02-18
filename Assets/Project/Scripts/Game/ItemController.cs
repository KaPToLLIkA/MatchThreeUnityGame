using System;
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
        [Space(10)] 
        public AnimationCurve moveDownCurve;
        public float moveDownSpeed = 1f;
        
        public CellType Type { get; private set; }

        public bool Movable { get; private set; } = false;

        private Field _field;
        private int _x;
        private int _y;

        private SpriteRenderer _spriteRenderer;

        public void Init(int x, int y, Field field, CellType type)
        {
            _x = x;
            _y = y;
            _field = field;
            Type = type;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            var row = sprites.Find(item => item.cellType == Type);
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

        private void Update()
        {
            if (!Movable && _y - 1 >= 0 && _field[_x, _y - 1] == null)
            {
                StartCoroutine(AsyncMoveDown());
            }
        }

        private IEnumerator AsyncMoveDown()
        {
            Movable = true;
            var pos = _field.GetCellWorldPos(_x, _y - 1);

            yield return StartCoroutine(
                Animations.MoveAnimator(gameObject, pos, moveDownCurve, moveDownSpeed));

            _field[_x, _y] = null;
            _field[_x, _y - 1] = this;
            _y -= 1;
            Movable = false;
        }

        private IEnumerator AsyncExplode()
        {
            yield return StartCoroutine(
                Animations.ScaleAnimator(gameObject, explodeScaleTarget, explodeScaleCurve, explodeScaleSpeed));

            _field[_x, _y] = null;
            
            Destroy(gameObject);
        }
    }
}