using UnityEngine;

public enum SIDE
{
	LEFT = -1,
	MIDDLE,
	RIGHT
}

public enum JUMP_POSITION
{
	GROUND,
	JUMP,
	FALL
}

public enum HITX
{
	LEFT,
	MIDDLE,
	RIGHT,
	NONE
}

public enum HITY
{
	TOP,
	MIDDLE,
	BOTTOM,
	LOWBOTTOM,
	NONE
}

public enum HITZ
{
	FRONT,
	MIDDLE,
	BACK,
	NONE
}

public class Character : MonoBehaviour
{
	#region PublicVariables
	public float xOffset = 2.0f;
	public float sidesMoveSpeed = 10.0f;
	public float forwardMoveSpeed = 10.0f;
	public float jumpPower = 7.0f;
	public float rollDuration = 0.2f;
	#endregion

	#region Character
	private CharacterController m_CharacterController;
	private float m_ColliderHeight;
	private float m_ColliderCenterY;
	#endregion

	#region CharacterPosition
	private float m_PositionX;
	private float m_PositionY;
	private float m_StartPositionX;
	private float m_StartPositionY;
	private float m_StartPositionZ;

	private SIDE m_lineSide = SIDE.MIDDLE;
	private SIDE m_lastLineSide = SIDE.MIDDLE;
	private JUMP_POSITION m_JumpPosition = JUMP_POSITION.GROUND;
	private bool m_bInRoll = false;
	#endregion

	#region Controls
	private bool m_MoveLeft = false;
	private bool m_MoveRight = false;
	private bool m_MoveUp = false;
	private bool m_MoveDown = false;
	#endregion

	#region Collision
	private HITX m_HitX = HITX.NONE;
	private HITY m_HitY = HITY.NONE;
	private HITZ m_HitZ = HITZ.NONE;
	#endregion

	private void Start()
	{
		m_CharacterController = GetComponent<CharacterController>();
		if (m_CharacterController == null)
		{
			Debug.LogError("CharacterController is missing!");
		}

		m_JumpPosition = m_CharacterController.isGrounded ? JUMP_POSITION.GROUND : JUMP_POSITION.FALL;
		m_ColliderCenterY = m_CharacterController.center.y;
		m_ColliderHeight = m_CharacterController.height;

		m_PositionX = transform.position.x;
		m_PositionY = transform.position.y;
		m_StartPositionX = m_PositionX;
		m_StartPositionY = m_PositionY;
		m_StartPositionZ = transform.position.z;
	}

	private void Update()
	{
		if (!PlayerStateManager.Instance.HasBegunPlay() || PlayerStateManager.Instance.IsDead())
		{
			return;
		}

		m_MoveLeft = Input.GetKeyDown(KeyCode.LeftArrow);
		m_MoveRight = Input.GetKeyDown(KeyCode.RightArrow);
		m_MoveUp = Input.GetKeyDown(KeyCode.UpArrow);
		m_MoveDown = Input.GetKeyDown(KeyCode.DownArrow);

		if (m_MoveLeft)
		{
			if (m_lineSide != SIDE.LEFT)
			{
				m_lastLineSide = m_lineSide;
				m_lineSide--;
			}
		}
		else if (m_MoveRight)
		{
			if (m_lineSide != SIDE.RIGHT)
			{
				m_lastLineSide = m_lineSide;
				m_lineSide++;
			}
		}

		m_PositionX = Mathf.Lerp(m_PositionX, (int)m_lineSide * xOffset, Time.deltaTime * sidesMoveSpeed);
		Vector3 MoveVector = new Vector3(m_PositionX - transform.position.x, m_PositionY * Time.deltaTime, forwardMoveSpeed * Time.deltaTime);
		m_CharacterController.Move(MoveVector);

		Jump();
		Roll();
	}

	private void Jump()
	{
		if (m_bInRoll)
		{
			return;
		}

		if (m_CharacterController.isGrounded)
		{
			if (m_JumpPosition == JUMP_POSITION.FALL)
			{
				m_JumpPosition = JUMP_POSITION.GROUND;
			}

			if (m_MoveUp)
			{
				m_PositionY = jumpPower;
				m_JumpPosition = JUMP_POSITION.JUMP;
			}
		}
		else
		{
			m_PositionY -= jumpPower * 1.5f * Time.deltaTime;

			if (m_CharacterController.velocity.y < -0.1f && !m_bInRoll)
			{
				m_JumpPosition = JUMP_POSITION.FALL;
			}
		}
	}

	internal float i_RollCounter;
	private void Roll()
	{
		i_RollCounter -= Time.deltaTime;
		if (i_RollCounter <= 0.0f)
		{
			m_bInRoll = false;
			m_CharacterController.center = new Vector3(0, m_ColliderCenterY, 0);
			m_CharacterController.height = m_ColliderHeight;
			i_RollCounter = 0.0f;
		}

		if (m_MoveDown)
		{
			i_RollCounter = rollDuration;
			m_bInRoll = true;
			m_PositionY -= 10.0f;
			m_CharacterController.height = m_ColliderHeight * 0.5f;
			m_CharacterController.center = new Vector3(0, m_ColliderCenterY * 0.5f, 0);
			if (m_JumpPosition == JUMP_POSITION.JUMP)
			{
				m_JumpPosition = JUMP_POSITION.FALL;
			}
		}
	}

	private HITX GetHitX(Collider col)
	{
		Bounds characterBounds = m_CharacterController.bounds;
		Bounds colliderBounds = col.bounds;

		float min_x = Mathf.Max(characterBounds.min.x, colliderBounds.min.x);
		float max_x = Mathf.Min(characterBounds.max.x, colliderBounds.max.x);

		float avg_x = (min_x + max_x) * 0.5f - colliderBounds.min.x;

		HITX hitX;
		if (avg_x > colliderBounds.size.x - 0.33f)
		{
			hitX = HITX.RIGHT;
		}
		else if (avg_x < 0.33f)
		{
			hitX = HITX.LEFT;
		}
		else
		{
			hitX = HITX.MIDDLE;
		}

		if (col.CompareTag("Obstacle"))
		{
			Debug.Log("HITX: " + hitX);
		}

		return hitX;
	}

	private HITY GetHitY(Collider col)
	{
		Bounds characterBounds = m_CharacterController.bounds;
		Bounds colliderBounds = col.bounds;

		float min_y = Mathf.Max(characterBounds.min.y, colliderBounds.min.y);
		float max_y = Mathf.Min(characterBounds.max.y, colliderBounds.max.y);

		float avg_y = ((min_y + max_y) * 0.5f - characterBounds.min.y) / characterBounds.size.y;

		HITY hitY;
		if (avg_y < 0.17f)
		{
			hitY = HITY.LOWBOTTOM;
		}
		else if (avg_y < 0.33f)
		{
			hitY = HITY.BOTTOM;
		}
		else if (avg_y < 0.66f)
		{
			hitY = HITY.MIDDLE;
		}
		else
		{
			hitY = HITY.TOP;
		}

		if (col.CompareTag("Obstacle"))
		{
			Debug.Log("HITY: " + hitY);
		}

		return hitY;
	}

	private HITZ GetHitZ(Collider col)
	{
		Bounds characterBounds = m_CharacterController.bounds;
		Bounds colliderBounds = col.bounds;

		float min_z = Mathf.Max(characterBounds.min.z, colliderBounds.min.z);
		float max_z = Mathf.Min(characterBounds.max.z, colliderBounds.max.z);

		float avg_z = ((min_z + max_z) * 0.5f - characterBounds.min.z) / characterBounds.size.z;

		HITZ hitZ;
		if (avg_z < 0.33f)
		{
			hitZ = HITZ.BACK;
		}
		else if (avg_z < 0.66f)
		{
			hitZ = HITZ.MIDDLE;
		}
		else
		{
			hitZ = HITZ.FRONT;
		}

		if (col.CompareTag("Obstacle"))
		{
			Debug.Log("HITZ: " + hitZ);
		}

		return hitZ;
	}

	private void ResetPosition()
	{
		m_lineSide = SIDE.MIDDLE;
		m_lastLineSide = SIDE.MIDDLE;
		m_bInRoll = false;
		m_JumpPosition = JUMP_POSITION.GROUND;
		m_PositionX = m_StartPositionX;
		m_PositionY = m_StartPositionY;
		transform.position = new Vector3(m_StartPositionX, m_StartPositionY, m_StartPositionZ);
	}

	private void ResetCollision()
	{
		m_HitX = HITX.NONE;
		m_HitY = HITY.NONE;
		m_HitZ = HITZ.NONE;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		m_HitX = GetHitX(hit.collider);
		m_HitY = GetHitY(hit.collider);
		m_HitZ = GetHitZ(hit.collider);

		if (m_HitZ == HITZ.FRONT && m_HitX == HITX.MIDDLE && m_HitY != HITY.LOWBOTTOM) //Death
		{
			if (!hit.collider.gameObject.CompareTag("Ramp"))
			{
				PlayerStateManager.Instance.Die();
				ResetPosition();
			}
		}
		else if (m_HitZ == HITZ.MIDDLE && m_HitY != HITY.LOWBOTTOM) //Side hit
		{
			if (m_HitX == HITX.LEFT)
			{
				m_lineSide = m_lastLineSide;
			}
			else if (m_HitX == HITX.RIGHT)
			{
				m_lineSide = m_lastLineSide;
			}
		}
		else
		{
			//Near miss
		}
	}
}
