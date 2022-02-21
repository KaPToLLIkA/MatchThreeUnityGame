using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Game
{
    [Serializable]
    public class CellTypedSpriteRow
    {
        public CellType cellType;
        public Sprite sprite;
    }
    
    [RequireComponent(typeof(BoxCollider2D))]
    public class ItemController : MonoBehaviour, IFieldItem, ISelectable
    {
        public int pixelsPerUnit = 100;
        
        public List<CellTypedSpriteRow> sprites;

        [Space(10)]
        public GameObject overlay;
        public GameObject image;
        
        [Header("Animations settings")] 
        public AnimationCurve explodeScaleCurve;
        public Vector3 explodeScaleTarget = new Vector3(0, 0, 0);
        public float explodeScaleSpeed = 1f;
        [Space(10)] 
        public AnimationCurve moveDownCurve;
        public float moveDownSpeed = 1f;
        [Space(10)] 
        public AnimationCurve swapCurve;
        public float swapSpeed = 1f;
        
        public CellType Type { get; private set; }

        public bool Movable { get; private set; } = false;

        public bool Explodable { get; private set; } = false;
        
        public Vector2Int Coords
        {
            get => new Vector2Int(_x, _y);
            set
            {
                _x = value.x;
                _y = value.y;
            }
        }

        private Field _field;
        private int _x;
        private int _y;

        private CellSelector _selector;
        
        private SpriteRenderer _overlayRenderer;
        private SpriteRenderer _imageRenderer;

        public void Init(int x, int y, Field field, CellType type, CellSelector selector)
        {
            _x = x;
            _y = y;
            _field = field;
            _selector = selector;
            Type = type;
            _imageRenderer = image.GetComponent<SpriteRenderer>();
            _overlayRenderer = overlay.GetComponent<SpriteRenderer>();
            var row = sprites.Find(item => item.cellType == Type);
            _imageRenderer.sprite = row.sprite;

            var maxDimension = (float)Mathf.Max(row.sprite.rect.width, row.sprite.rect.height);
            var curSize = maxDimension / pixelsPerUnit;

            var scale = field.CellSize / curSize;

            transform.localScale = new Vector3(scale, scale, 1);
        }

        public void Explode()
        {
            Explodable = true;
            StartCoroutine(AsyncExplode());
        }
        
        public void Select()
        {
            _overlayRenderer.enabled = true;
        }
        
        public void Deselect()
        {
            _overlayRenderer.enabled = false;
        }

        public void Swap(ItemController targetController)
        {
            StartCoroutine(AsyncDoubleSwap(targetController));
        }

        private void Update()
        {
            if (!Movable && _y - 1 >= 0 && _field[_x, _y - 1] == null)
            {
                StartCoroutine(AsyncMoveDown());
            }
        }
        
        private void OnMouseDown()
        {
            if (_field.ContainsAnyMovable()) return;
            if (_field.ContainsAnyEmpty()) return;

            _selector.Select(this);
        }

        private IEnumerator AsyncDoubleSwap(ItemController targetController)
        {
            yield return StartCoroutine(AsyncSwap(targetController));
            
            var f1 = _field.GetAnyFigureAt(Coords);
            var f2 = _field.GetAnyFigureAt(targetController.Coords);

            if (f1.Coords.Count < _field.MinFigureLength && f2.Coords.Count < _field.MinFigureLength)
            {
                yield return StartCoroutine(AsyncSwap(targetController));
            }
        } 
        
        private IEnumerator AsyncSwap(ItemController targetController)
        {
            Movable = true;
            
            var tx = targetController._x;
            var ty = targetController._y;
            var sx = _x;
            var sy = _y;
            var tPos = _field.GetCellWorldPos(tx, ty);
            var sPos = _field.GetCellWorldPos(sx, sy);

            var c1 = StartCoroutine(
                Animations.MoveAnimator(gameObject, tPos, swapCurve, swapSpeed));

            var c2 = StartCoroutine(
                Animations.MoveAnimator(targetController.gameObject, sPos, swapCurve, swapSpeed));

            yield return c1;
            yield return c2;

            var item1 = _field[Coords];
            var item2 = _field[targetController.Coords];
            
            (_field[Coords], _field[targetController.Coords]) = (_field[targetController.Coords], _field[Coords]);
            (item1.Coords, item2.Coords) = (item2.Coords, item1.Coords);
            
            Movable = false;
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