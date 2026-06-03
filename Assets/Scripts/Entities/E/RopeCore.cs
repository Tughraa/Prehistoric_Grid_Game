using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCore : MonoBehaviour
{
    EntityGeneral ropeEntity;
    public EntityGeneral owner;
    public DistanceJoint2D joint;
    public Vector2 initThrow;
    bool called = false;
    public float ropeDist = 1.8f;
    LineRenderer lineR;
    public bool connected = false;
    public DistanceJoint2D activeEntityJoint;

    void Start()
    {
        ropeEntity = this.GetComponent<EntityGeneral>();
        lineR = this.GetComponent<LineRenderer>();
        joint = GetComponent<DistanceJoint2D>();
        //joint.connectedBody = owner.rigid;
    }

    void Update()
    {
        if (called == false) //like LateStart()
        {
            InitConnect();
        }
        Vector3 startPos = new Vector3(this.transform.position.x,this.transform.position.y,1.5f);
        Vector3 endPos = new Vector3(owner.transform.position.x,owner.transform.position.y,1.5f);
        lineR.SetPosition(0, startPos);
        lineR.SetPosition(1, endPos);
    }
    void InitConnect()
    {
        ropeEntity.rigid.velocity = Vector2.zero;
        
        joint.connectedBody = owner.rigid;
        joint.distance = Vector2.Distance(owner.transform.position,this.transform.position)+ropeDist;
        called = true;

        ropeEntity.Knockback(initThrow,initThrow.magnitude);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (connected)
        {
            return;
        }
        if (col.gameObject.GetComponent<EntityGeneral>()) //We hit an entity
        {
            EntityGeneral colEntity = col.gameObject.GetComponent<EntityGeneral>();
            if (colEntity.entityType != "player" && colEntity.entityType != "rope_core")
            {
                this.GetComponent<Collider2D>().enabled = false;
                this.transform.SetParent(colEntity.transform);
                this.transform.localPosition = Vector3.zero;
                ropeEntity.rigid.velocity = Vector2.zero;
                ropeEntity.rigid.isKinematic = true;
                connected = true;
                ConnectToEntity(colEntity);

                joint.enabled = true;
                joint.connectedBody = owner.rigid;
                joint.distance = Vector2.Distance(owner.transform.position, transform.position) + ropeDist;
            }
        }
        else //We hit a block.... probably
        {
            ropeEntity.rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            connected = true;
        }
    }
    void OnDestroy()
    {
        if (activeEntityJoint != null)
            {GameObject.Destroy(activeEntityJoint);}
    }
    void ConnectToEntity(EntityGeneral entity)
    {
        //Constrain hit entity to owner
        DistanceJoint2D entityJoint = entity.gameObject.AddComponent<DistanceJoint2D>();
        entityJoint.connectedBody = owner.rigid;
        entityJoint.autoConfigureDistance = false;
        entityJoint.distance = joint.distance;
        entityJoint.maxDistanceOnly = true;

        activeEntityJoint = entityJoint;
    }
}
