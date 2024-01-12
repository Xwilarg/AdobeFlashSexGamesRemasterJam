using System.Collections;
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
        public override (float Min, float Max) SpawnRange => (-3.5f, -3.5f);

        protected override void Awake()
        {
            base.Awake();

            _baseXSpeedOffset = _xSpeedOffset;
            _baseY = transform.position.y;
            _anim = GetComponent<Animator>();
        }

        protected override void Update()
        {
            base.Update();

            if (!_didJump && transform.position.x < 5f)
            {
                _didJump = true;
                StartCoroutine(Jump());
            }
            if (_isJumping && transform.position.y <= _baseY)
            {
                _isJumping = false;
                _rb.gravityScale = 0f;
                transform.position = new(transform.position.x, _baseY);
                _xSpeedOffset = _baseXSpeedOffset;
                _anim.SetInteger("JumpState", 0);
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
            _xSpeedOffset = 0f;
            _anim.SetInteger("JumpState", 1);

            yield return new WaitForSeconds(.2f);

            _rb.gravityScale = 1f;
            _rb.AddForce(new Vector2(-1f, 1f).normalized * 5f, ForceMode2D.Impulse);
            _isJumping = true;
            _anim.SetInteger("JumpState", 2);
        }
    }
}
