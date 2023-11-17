using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Jump")]
    public class JumpState : State<PlayerController>
    {
        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;

        #endregion


        private float _xInput;
        private bool _jump;
        private bool _jumpHeld;
        private float _dash;
        public float inputTreshold = .15f;

        [SerializeField]
        private bool visualizer = true;

        [SerializeField]
        private float _jumpHeight = 40;
        [SerializeField]
        private float _verticalMovement = 50f;

        [SerializeField]
        private float _rayHeight = .1f;

        public float timeUntilCheckGround = .3f;
        private float lastOnGround;

 

        private bool isOnGround = false;




        public override void Init(PlayerController parent)
        {
            #region Get Components
            base.Init(parent);
            if (_col == null) _col = parent.GetComponentInChildren<PlatformerController2D>();
            if (_cc == null) _cc = parent.GetComponentInChildren<CapsuleCollider2D>();
            if (_rb == null) _rb = parent.GetComponentInChildren<Rigidbody2D>();
            if (_sr == null) _sr = parent.GetComponentInChildren<SpriteRenderer>();
            if (_anim == null) _anim = parent.PlayerAnimation;
            if (_data == null) _data = parent.PersistentPlayerData;

            #endregion



            Jump();




            if (visualizer)
            {
                _sr.color = Color.black;
                Debug.Log("<color=black>Started a jump state </color>");

            }
        }

        public override void CaptureInput() {
            FollowInputs();
        }

        public override void Update()
        {
  
            TimeUntilCheckGround();
            StartToIncreaseGravity();
            _anim.AdjustSpriteRotation(_xInput);

            if (visualizer)
                Debug.Log("Jump is held: " + _jumpHeld);

        }


        public override void FixedUpdate() {

            _col.VerticalRaycasts(_cc, _rayHeight);

            if (Mathf.Abs(_xInput) > inputTreshold)
                _rb.AddForce(new Vector2(_data.dir * _verticalMovement, _rb.velocity.y));
        }

        public override void ChangeState()
        {
            if (!isOnGround && _rb.velocity.y < 0)
                _runner.SetState(typeof(FallState));
            if (isOnGround && _rb.velocity.x < 1f)
                _runner.SetState(typeof(IdleState));
            if (isOnGround && _rb.velocity.x > 1f)
                _runner.SetState(typeof(WalkState));
            if (_data.jumpsLeft >= 1 && _jump)
                _runner.SetState(typeof(JumpState));
            if (_dash > 0)
                _runner.ActivateAbility(typeof(DashState), _data.dashCooldown);

        }

        public override void Exit() {
            isOnGround = false;
        }


        private void TimeUntilCheckGround()
        {
            if (isOnGround) return;

            lastOnGround += Time.deltaTime;

            if (lastOnGround > timeUntilCheckGround)
            {
                if (_col.collisions.VerticalBottom)
                {
                    isOnGround = true;

                }

            }
        }
        private void FollowInputs()
        {
            _xInput = Input.GetAxis("Horizontal");
            _jump = Input.GetButtonDown("Jump");
            _jumpHeld = !Input.GetButtonUp("Jump");
            _dash = Input.GetAxisRaw("Dash");
            _data.dir = (_xInput < inputTreshold) ? -1 : (_xInput > inputTreshold ? 1 : 0);
        }

        private void StartToIncreaseGravity()
        {
            if (_jumpHeld) return;

            _rb.gravityScale *= 3;
        }


        private void Jump()
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0); // resettaa aina hypyn alussa y nopeus nollaan
            _rb.gravityScale = 2;
            _jumpHeld = true;
            _data.jumpsLeft -= 1;
            lastOnGround = 0;

            float mass = _rb.mass;
            float distance = _jumpHeight;
            float forceMagnitude = (mass * 9.8f * _rb.gravityScale * distance) / 2;  // Dividing by 2 because of Impulse mode

            // Apply force using AddForce
            _rb.AddForce(new Vector2(0, forceMagnitude), ForceMode2D.Impulse);

            



            _anim.ChangeAnimationState("player-jump-beginning");
        }

    }
}