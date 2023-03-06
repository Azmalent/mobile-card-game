using Mirror;
using UnityEngine;

public class TestPlayer : NetworkBehaviour
{
    public float speed = 30;
    public Rigidbody rb;

    [SyncVar] Vector3 position;

    void FixedUpdate()
    {
        if (isLocalPlayer) 
        {
            var pos = rb.position + new Vector3(0, Input.GetAxisRaw("Vertical"), 0) * speed * Time.fixedDeltaTime;
            rb.MovePosition(pos);
            CmdSetPosition(pos);
        }
        else 
        {
            rb.MovePosition(position);
        }
    }

    [Command]
    void CmdSetPosition(Vector3 pos)
    {
        position = pos;
    }
}
