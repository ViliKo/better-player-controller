using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Idle")]
    public class IdleState : State<PlayerController>
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
        public float inputTreshold = .15f;

        [SerializeField]
        private bool visualizer = true;


        
        public float rayHeight = .1f;


        public bool blockinObstacle = false;



        public override void Init(PlayerController parent, CharacterMode characterMode)
        {
            #region Get Components
            base.Init(parent, characterMode);
            if (_col == null) _col = parent.GetComponentInChildren<PlatformerController2D>();
            if (_cc == null) _cc = parent.GetComponentInChildren<CapsuleCollider2D>();
            if (_rb == null) _rb = parent.GetComponentInChildren<Rigidbody2D>();
            if (_sr == null) _sr = parent.GetComponentInChildren<SpriteRenderer>();
            if (_anim == null) _anim = parent.PlayerAnimation;
            if (_data == null) _data = parent.PersistentPlayerData;

            #endregion

            if (visualizer)
            {
                _sr.color = Color.blue;
                Debug.Log("<color=blue>Started an idle animation</color>");
            }


            _data.jumpsLeft = _data.maxJumps;

            _rb.velocity = new Vector2(0, 0);

            _anim.ChangeAnimationState("player-idle");
        }

        public override void CaptureInput()
        {
            _xInput = Input.GetAxis("Horizontal");
            _jump = Input.GetButtonDown("Jump");
        }



        public override void FixedUpdate()
        {

        }

        public override void Update()
        {
            _col.VerticalRaycasts(_cc, rayHeight);

            if (visualizer)
                Debug.Log("does it add to persistent data" + _data.jumpsLeft);

            if (Mathf.Abs(_xInput) <= inputTreshold) return;
            CheckForWall();
            if (CheckInputDirectionIfCollided())
                blockinObstacle = false;
            else if (_col.collisions.HorizontalBottomUp)
                blockinObstacle = true;
                
        }

        private bool CheckInputDirectionIfCollided()
        {
            if (!_col.collisions.HorizontalBottomUp) return false;

            float direction = -_sr.transform.localScale.x; // pitaa kaantaa sprite direction koska alkuperainen spriten suunta on vasemmalle
            if (direction == 1) // jos suunta on oikealle
                return  _xInput + direction < direction;  // katso etta annettu syöte on eri suuntaan kun suunta

            if (direction == -1) // jos suunta on vasemmalle
                return _xInput + direction > direction; // katso etta annettu syöte on eri suuntaan kun suunta

            return false;
        }

        public override void ChangeState()
        {

            if (Mathf.Abs(_xInput) > inputTreshold && blockinObstacle == false)
                    _runner.SetState(typeof(WalkState));

            if (_jump && _col.collisions.VerticalBottom)
            {
                Debug.Log(_jump + ": why the hell would this be true");
                _runner.SetState(typeof(JumpState));
            }
                

            if (!_col.collisions.VerticalBottom && !_jump)
                _runner.SetState(typeof(FallState));
        }

        public override void Exit()
        {
            blockinObstacle = false;
        }

        private void CheckForWall() => _col.HorizontalRaycasts(-_sr.transform.localScale.x, _cc, .1f, false, true);


        

        

        
    }


}


