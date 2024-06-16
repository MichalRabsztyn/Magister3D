using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5.0f;

    private Vector3 m_Offset;
    private float m_PositionY;

    private void Start()
    {
        m_Offset = transform.position;
    }

    private void Update()
    {
        Vector3 followPosition = target.position + m_Offset;
        RaycastHit hit;
        if (Physics.Raycast(target.position, Vector3.down, out hit, 2.5f))
        {
            m_PositionY = Mathf.Lerp(m_PositionY, hit.point.y, followSpeed * Time.deltaTime);
        }
        else
        {
            m_PositionY = Mathf.Lerp(m_PositionY, target.position.y, followSpeed * Time.deltaTime);
        }

        followPosition.y = m_PositionY + m_Offset.y;
        transform.position = followPosition;
    }
}
