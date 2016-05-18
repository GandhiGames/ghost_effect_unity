using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class SimplePlayerController : MonoBehaviour
{
	public float speed;
	public float jumpForce;
	public bool playerLookingRight = true;

	public Transform groundCheck;
	public LayerMask groundMask;

	private bool _leftButtonPressed = false;
	private bool _rightButtonPressed = false;
	private bool _jumpPressed = false;
	private bool _grounded;
	private bool _canDoubleJump = false;

	private Rigidbody2D _rigidBody2D;
	private SpriteRenderer _renderer;
	private Animator _animator;
	private Vector3 originalScale;

	private static readonly int SPEED_HASH = Animator.StringToHash ("speed");
	private static readonly int JUMP_HASH = Animator.StringToHash ("jumping");

	// Use this for initialization
	void Start ()
	{
		_rigidBody2D = GetComponent<Rigidbody2D> ();
		_renderer = GetComponent<SpriteRenderer> ();
		_animator = GetComponent<Animator> ();

		originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update ()
	{	
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) {
			_leftButtonPressed = true;
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.A)) {
			_leftButtonPressed = false;
		}
		
		if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D)) {
			_rightButtonPressed = true;
		}

		if (Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.D)) {
			_rightButtonPressed = false;
		}

		if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.Space)) {
			_jumpPressed = true;
		}

		if (Input.GetKeyUp (KeyCode.W) || Input.GetKeyUp (KeyCode.Space)) {
			_jumpPressed = false;
		}


	}

	void FixedUpdate ()
	{
		_grounded = CheckGrounded ();

		if (_jumpPressed) {
			if (_grounded) {
				_rigidBody2D.velocity = new Vector2 (_rigidBody2D.velocity.x, 0);
				_rigidBody2D.velocity = new Vector2 (_rigidBody2D.velocity.x, jumpForce);
				_canDoubleJump = true;
				_jumpPressed = false;
			} else if (_canDoubleJump) {
				_canDoubleJump = false;
				_rigidBody2D.velocity = new Vector2 (_rigidBody2D.velocity.x, 0);
				_rigidBody2D.velocity = new Vector2 (_rigidBody2D.velocity.x, jumpForce);

			}
		}

		if (_leftButtonPressed) {
			if (playerLookingRight) {
				playerLookingRight = false;
				_renderer.transform.localScale = new Vector3 (-originalScale.x, originalScale.y, originalScale.z);
			}
			
			_rigidBody2D.AddForce (new Vector2 (transform.right.x, transform.right.y) * -speed * Time.deltaTime);
		}
		
		if (_rightButtonPressed) {
			
			if (!playerLookingRight) {
				playerLookingRight = true;
				_renderer.transform.localScale = new Vector3 (originalScale.x, originalScale.y, originalScale.z);
			}
			_rigidBody2D.AddForce (new Vector2 (transform.right.x, transform.right.y) * speed * Time.deltaTime);
		}

		if (_grounded) {
			_animator.SetBool (JUMP_HASH, false);
			_animator.SetFloat (SPEED_HASH, Mathf.Abs (_rigidBody2D.velocity.x));
		} else {
			_animator.SetBool (JUMP_HASH, true);
			_animator.SetFloat (SPEED_HASH, 0);
		}

	}

	private bool CheckGrounded ()
	{
		return Physics2D.OverlapCircle (groundCheck.position, .1f, groundMask);
	}


}
