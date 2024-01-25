﻿using System.Collections;
using UnityEngine;

namespace FlashSexJam.Enemy.Impl
{
    public class DogEnemy : EnemyController
    {
        private Animator _anim;

        private bool _didJump;
        private bool _isJumping;

        private float _baseY;
        private float _baseXSpeedOffset;

        private float _jumpXPos;

        private bool _shouldJump;

        public override (float Min, float Max) SpawnRange => (-3.5f, -3.5f);

        protected override void Awake()
        {
            base.Awake();

            _baseXSpeedOffset = _xSpeedOffset;
            _baseY = transform.position.y;
            _anim = GetComponent<Animator>();

            _jumpXPos = Random.Range(5f, 7f);
            _shouldJump = Random.Range(0, 3) > 0;
        }

        protected override void Update()
        {
            base.Update();

            if (_shouldJump)
            {
                if (!_didJump && transform.position.x < _jumpXPos)
                {
                    _didJump = true;
                    StartCoroutine(Jump());
                }
                if (_isJumping && transform.position.y < _baseY)
                {
                    _isJumping = false;
                    _rb.gravityScale = 0f;
                    transform.position = new(transform.position.x, _baseY);
                    _xSpeedOffset = _baseXSpeedOffset;
                    _doesMove = true;
                    _anim.SetInteger("JumpState", 0);
                }
            }


            if (_isJumping)
            {
                if (_rb.velocity.y < 0f)
                {
                    _anim.SetInteger("JumpState", 3);
                }
            }
        }

        private IEnumerator Jump()
        {
            _doesMove = false;
            _xSpeedOffset = 0f;
            _anim.SetInteger("JumpState", 1);

            yield return new WaitForSeconds(.2f);

            _rb.gravityScale = 1f;
            _rb.AddForce(new Vector2(-.5f, 1f).normalized * 10f, ForceMode2D.Impulse);
            _isJumping = true;
            _anim.SetInteger("JumpState", 2);
        }
    }
}
