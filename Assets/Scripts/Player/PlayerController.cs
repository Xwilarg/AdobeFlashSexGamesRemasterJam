using FlashSexJam.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private float _xMov;

        [SerializeField]
        private GameObject _modelUp, _modelMid, _modelDown;

        private readonly Dictionary<BodyPartType, List<GameObject>> _clothes = new()
        {
            { BodyPartType.Head, new() },
            { BodyPartType.UpperBody, new() },
            { BodyPartType.LowerBody, new() }
        };

        private void Awake()
        {
            foreach (var bp in GetComponentsInChildren<BodyPart>())
            {
                bp.Owner = this;
            }
            foreach (var c in GetComponentsInChildren<Cloth>())
            {
                _clothes[c.Owner].Add(c.gameObject);
            }
            _modelUp.SetActive(false);
            _modelDown.SetActive(false);
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
                Destroy(c.gameObject);
            }
            _clothes[bodyPart].Clear();
            return true;
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            var mov = value.ReadValue<Vector2>();

            _modelUp.SetActive(false);
            _modelMid.SetActive(false);
            _modelDown.SetActive(false);

            if (mov.y > 0) _modelUp.SetActive(true);
            else if (mov.y < 0) _modelDown.SetActive(true);
            else _modelMid.SetActive(true);

            _xMov = mov.x;
        }
    }

}