using FlashSexJam.Manager;
using FlashSexJam.SO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private float _xMov;

        public HSceneController HScene { set; get; }

        [SerializeField]
        private PositionData _modelUp, _modelMid, _modelDown;

        private readonly Dictionary<BodyPartType, List<GameObject>> _clothes = new()
        {
            { BodyPartType.Head, new() },
            { BodyPartType.UpperBody, new() },
            { BodyPartType.LowerBody, new() }
        };

        private void Awake()
        {
            var models = new[] { _modelUp, _modelMid, _modelDown };
            foreach (var m in models)
            {
                var (UpperCloth, LowerCloth) = m.Init(this);
                _clothes[BodyPartType.UpperBody].Add(UpperCloth);
                _clothes[BodyPartType.LowerBody].Add(LowerCloth);
            }
        }

        private void Update()
        {
            GameManager.Instance.IncreaseSpeed(_xMov * Time.deltaTime);
        }

        /// <returns>false if already naked</returns>
        public bool TryBreakCloth(BodyPartType bodyPart)
        {
            if (!_clothes[bodyPart].Any())
            {
                return false;
            }
            foreach (var c in _clothes[bodyPart])
            {
                Destroy(c);
            }
            _clothes[bodyPart].Clear();
            return true;
        }

        public bool IsTopBodyBroken => !_clothes[BodyPartType.UpperBody].Any();
        public bool IsLowerBodyBroken => !_clothes[BodyPartType.LowerBody].Any();

        public void OnMove(InputAction.CallbackContext value)
        {
            var mov = value.ReadValue<Vector2>();

            _modelUp.gameObject.SetActive(false);
            _modelMid.gameObject.SetActive(false);
            _modelDown.gameObject.SetActive(false);

            if (mov.y > 0) _modelUp.gameObject.SetActive(true);
            else if (mov.y < 0) _modelDown.gameObject.SetActive(true);
            else _modelMid.gameObject.SetActive(true);

            _xMov = mov.x;
        }
    }

}