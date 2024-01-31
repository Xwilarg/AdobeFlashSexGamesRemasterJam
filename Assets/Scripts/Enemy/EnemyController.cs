﻿using FlashSexJam.Manager;
using FlashSexJam.Player;
using UnityEngine;

namespace FlashSexJam.Enemy
{
    public abstract class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hSceneHead, _hSceneUpperBody, _hSceneLowerBody;

        [SerializeField]
        protected float _xSpeedOffset;

        [SerializeField]
        private string _name;
        public string Name => _name;

        protected bool _doesMove = true;

        public int PlayerID { set; private get; }

        public bool OverridesSpawnPos { set; private get; } = true;

        public GameObject GetHScene(BodyPartType type)
        {
            return type switch
            {
                BodyPartType.Head => _hSceneHead,
                BodyPartType.UpperBody => _hSceneUpperBody,
                BodyPartType.LowerBody => _hSceneLowerBody,
                _ => null
            };
        }

        protected Rigidbody2D _rb;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (OverridesSpawnPos)
            {
                var (Min, Max) = SpawnRange;
                var yPos = Min == Max ? Min : Random.Range(Min, Max);
                transform.Translate(Vector2.up * yPos);
            }
        }

        protected virtual void Update()
        {
            if (_doesMove)
            {
                _rb.velocity = Vector3.left * (GameManager.Instance.GetSpeed(PlayerID) + _xSpeedOffset);
            }

            if (transform.position.x < -20f)
            {
                Destroy(gameObject);
            }
        }

        public abstract (float Min, float Max) SpawnRange { get; }
    }
}
