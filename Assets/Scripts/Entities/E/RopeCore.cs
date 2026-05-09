using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCore : MonoBehaviour
{
    EntityGeneral ropeEntity;
    public EntityGeneral owner;
    public Joint2D joint;
    bool called = false;
    public float ropeDist = 1.8f;
    LineRenderer lineR;
    bool connected = false;

    void Start()
    {
        ropeEntity = this.GetComponent<EntityGeneral>();
        lineR = this.GetComponent<LineRenderer>();
        joint = GetComponent<Joint2D>();
        //joint.connectedBody = owner.rigid;
    }

    void Update()
    {
        if (called == false)
        {
            joint.connectedBody = owner.rigid;
            DistanceJoint2D Djoint = GetComponent<DistanceJoint2D>();
            Djoint.distance = Vector2.Distance(owner.transform.position,this.transform.position)+ropeDist;
            called = true;
        }
        Vector3 startPos = new Vector3(this.transform.position.x,this.transform.position.y,1.5f);
        Vector3 endPos = new Vector3(owner.transform.position.x,owner.transform.position.y,1.5f);
        lineR.SetPosition(0, startPos);
        lineR.SetPosition(1, endPos);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<EntityGeneral>() && !connected) //We hit an entity
        {
            EntityGeneral colEntity = col.gameObject.GetComponent<EntityGeneral>();
            if (colEntity.entityType != "player" && colEntity.entityType != "rope_core")
            {
                col.transform.parent = this.transform.parent;
                connected = true;
            }
        }
        else if (!connected) //We hit a block.... probably
        {
            ropeEntity.rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            connected = true;
        }
    }
}
