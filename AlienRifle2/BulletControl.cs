using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlienRifle2
{
    public class BulletControl : MonoBehaviour
    {
        public Rigidbody rb;

        public void Update()
        {
            rb.velocity = rb.velocity.normalized * 200f;
        }

        public void OnCollisionEnter(Collision col)
        {
            LiveMixin mixin = col.collider.GetComponent<LiveMixin>();
            if(mixin && mixin != Player.main.liveMixin)
            {
                mixin.TakeDamage(500f);
                transform.GetChild(0).parent = null;
                Destroy(gameObject);
                return;
            }
            BreakableResource res = col.collider.GetComponent<BreakableResource>();
            if (res)
            {
                res.BreakIntoResources();
                transform.GetChild(0).parent = null;
                Destroy(gameObject);
                return;
            }
        }
    }
}