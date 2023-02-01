using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.StateMachine{
    public class SightLineFSM : StateMachine, IParentingCollider {

        public LayerMask viewMask;

        public string targetTag = "Player";
        //bool canSeeTarget = false;
        bool targetsInViewCollider = false;
        HashSet<GameObject> targetsInView = new HashSet<GameObject>();


        ////////////////////////error: fix, check line of sight
        public void OnChildTriggerEnter(Collider collider){
            if(collider.tag == targetTag){
                targetsInView.Add(collider.gameObject);
                targetsInViewCollider = true;
            }
        }

        public void OnChildTriggerExit(Collider collider){
            if(collider.tag == targetTag){
                targetsInView.Remove(collider.gameObject);
                targetsInViewCollider = false;
            }


        }

        ///////////////////////////////

        bool CanSeeTarget(Transform target) {//todo, see if way to invert ray
            RaycastHit hitInfo;
            Vector3 vectResult;
            bool canSee = false;
            //if (in range)
                //if (in angle)
            Vector3 dirToPlayer = (target.position - transform.position);

            vectResult = Vector3.Cross(Vector3.up, dirToPlayer);
            if(!Physics.Linecast(transform.position, target.position, viewMask)){//if nothing is blocking the player (center)
                canSee= true;
                //Debug.DrawLine (transform.position, target.position, Color.red);
            }
            //Debug.DrawLine (target.position-vectResult, target.position, Color.yellow);
            
            else if(Physics.Linecast(target.position-vectResult, target.position, out hitInfo)){//if nothing is blocking the player (left)
                
                if(!Physics.Linecast(transform.position, hitInfo.point, viewMask)){
                    canSee = true;
                    //Debug.DrawLine(transform.position, hitInfo.point, Color.green);
                }
            }
            else if(Physics.Linecast(target.position+vectResult, target.position, out hitInfo)){//if nothing is blocking the player (right)
                
                if(!Physics.Linecast(transform.position, hitInfo.point, viewMask)){
                    canSee = true;
                    //Debug.DrawLine(transform.position, hitInfo.point, Color.green);
                }
            }
            return canSee;
            
        }

    } 
}