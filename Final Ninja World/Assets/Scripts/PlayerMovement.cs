﻿
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public CharacterController controller;
    PlayerInput playerInput;
    //grappleTest
    public Vector3 grappleTarget;
    private bool isGrappled = false;
    private float grappleDistance;//grapple
    //jump+gravity
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.2f;

    public float jumpHeight = 20f;
    private float gravity = -9.81f;

    //1st person movement
    public float speed = 6f;
    
    Vector2 inputs;
    private bool isGrounded;
    bool isNextToWall;
    Vector3 wallNormal;
    Vector3 velocity = Vector3.zero;
    Vector3 move;
    Vector3 v_0 ;
    //Vector3 position_0;

    //////////////////////////////////GrappleTestCode//////////////////////////////////////////////
    void Start() {

        playerInput = GetComponent<PlayerInput>();

        //position_0 = transform.position;
        //StartGrapple(grappleTarget);
        v_0 = velocity;
    }


    public void StartGrapple(Vector3 grPos) {   
        //line     
        grappleTarget = grPos;
        grappleDistance = Vector3.Distance(transform.position, grappleTarget);
        isGrappled = true;
        //make sure distance from point stays the same 
        
    }


    
    public void StopGrapple() {
        isGrappled = false;
        grappleTarget = Vector3.zero;
        grappleDistance = 0;
    }
/////////////////////////////////////////////////////////////////////////////////////////////////////////


    // Update is called once per frame
    void Update()
    {
        

        //if regular moving
        /*
        controller.Move(move * speed * Time.deltaTime);
        */
        inputs = playerInput.input;
        //Gravity

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log(isGrounded);
        //CollisionFlags.Below

        //if can climb/slide
		isNextToWall = NextToWall(groundMask, out wallNormal);
        if(isNextToWall)
            print("wall" + wallNormal);
        
    }

    private void FixedUpdate() {
        //position
        //v_0 = (transform.position-position_0)/Time.deltaTime;

        //////////////////////////////GRAPPLE CODE//////////////////////////////////
        

        if(isGrappled){
            grapple(grappleTarget, grappleDistance, inputs);
        }
		/*else if(isNextToWall && !isGrounded){
			wallSlide();
		}*/
        else{
            normal(inputs);
        }

        ////////////////////////////////////////////////////////////////////////////
        
        /////////////////////////////Acceleration test/////////////////////////////
        /*float sqrAcc = Vector3.SqrMagnitude((velocity-v_0)/Time.deltaTime);
        if(sqrAcc>100 && isGrappled)
            Debug.Log(sqrAcc);
        */
        


        controller.Move(velocity* Time.deltaTime);
        

        v_0 = velocity;

        ///////////////////////Prev Velocity from Position///////////////////////// 
        //v_0 = (transform.position-position_0)/Time.deltaTime;
        //velocity = v_0+Vector3.up*gravity*Time.deltaTime;
        //position_0 = transform.position;
    }
    void OnDrawGizmos() {//groundcheck
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (groundCheck.position, groundDistance);

    }

    Vector3 quadFriction(Vector3 velocity, float coef = 0.5f){
        Vector3 frictVel = Vector3.zero;
        float frict = coef * Vector3.SqrMagnitude(velocity) * Time.deltaTime/2; 
        //F_r = c* v**2  
        frictVel = -frict*velocity/Vector3.Magnitude(velocity);

        return frictVel;
    }

    Vector3 linFriction(Vector3 velocity, float coef = 0.5f){
        Vector3 frictVel = Vector3.zero;
        //F_r = c* v   
        frictVel = -coef*velocity* Time.deltaTime;
        return frictVel;
    }

    Vector3 CrouchSlide(Vector3 velocity, float mu, float g, float time){ 
        Vector3 v_current;
        Vector3 v_0 = velocity;
        if( v_0 != Vector3.zero){ 

            v_current = Vector3.Lerp(v_0,Vector3.zero,mu*g*time); 
            return v_current;
        }
        else{
            return Vector3.zero;
        } 

    }

    void normal(Vector2 inputs){
        //1st person, character rotation with camera

        move = (transform.right * inputs.x + transform.forward * inputs.y)*speed;
        
        

        if(isGrounded && velocity.y<=0) {
           
            velocity.y = -1f;//-1f;
            if (Input.GetButtonDown("Jump")){
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.x = move.x;
            velocity.z = move.z;

        }
        else
        {
            velocity.y += gravity*Time.deltaTime; //gravity negative
            

            if(Vector2.SqrMagnitude(new Vector2(velocity.x,velocity.z))>(speed*speed + 0.5f)){
                

                if(move.x*velocity.x < 0){
                    velocity.x += move.x;
                }
                if(move.z*velocity.z < 0){
                    velocity.z += move.z;
                }
                
            }
            else{
                velocity.x = move.x;
                velocity.z = move.z;
            }


            //velocity += linFriction(new Vector3(velocity.x,0,velocity.z));

        }

        

    }

    void grapple(Vector3 target, float distance, Vector2 inputs){//move unused
        
        Vector3 ropeVect; 
        Vector3 sideVect;
        Vector3 pullDir;
        float pullSpeed = 0;//"attraction"
        float angle;
        float g = gravity;
        float l = distance;
        Vector3 correction;
        
        ropeVect = (target - transform.position);
        angle = Vector3.Angle(Vector3.up, ropeVect)* Mathf.Deg2Rad;


        if ((transform.position-target).sqrMagnitude < distance*distance ) {//not in tension
            normal(inputs);
        }
        /*else if (angle > 1.5708f){//in tension, in the air
			
			
            ropeVect = ropeVect.normalized;
          
			velocity.y += gravity*Time.deltaTime;

			//projection of ropevect on x and z
			
			rope2d = new Vector3(ropeVect.x, 0, ropeVect.z);//flat
            sideVect = Vector3.Cross(Vector3.up, ropeVect).normalized;//normalized, decide whether to optimize later

			
			if(rope2d*transform.forward*inputs.y<0){//inwards
				
				velocity +=  ( Vector3.Project(transform.forward,rope2d)+ Vector3.Project(transform.forward, sideVect))*inputs.y;
			}
			else{
				velocity +=  (Vector3.Project(transform.forward, sideVect))*inputs.y;
			}
			
			if(rope2d*transform.right*inputs.x<0){
				velocity +=  (Vector3.Project(transform.right,rope2d)+Vector3.Project(transform.right, sideVect))*inputs.x;
			}
			else{
				velocity +=  (Vector3.Project(transform.right, sideVect))*inputs.x;
			}
			
			
			//if(position+movement*Time.deltaTime < rope dist){
			//	velocity = movement
			//}

			
			//away from pulldir = 0
 			
			
        }*/
        else{//in tension
            isGrounded = true;


            ropeVect = ropeVect.normalized;
            //charcontroller overrides position transform
            /**/
            controller.enabled = false;
            transform.position = target-ropeVect*distance;
            controller.enabled = true;
  
            
            //Debug.DrawLine (transform.position, target, Color.yellow);          
            
            //theta' = -(g/l)sin(theta)*t (2D)
            //angle'' = -g/l * angle
            //angle'= omega = -g/l * angle * t (sketchy integration)
            //v_t = r*omega, r cancels out for pull speed


            if(angle<0.125){//small angle approx, sin theta => theta
                
                pullSpeed = -g*angle*Time.deltaTime;//*Time.deltaTime
            
            }
            else{
                pullSpeed = -g*Mathf.Sin(angle)*Time.deltaTime;//*Time.deltaTime       

                //print(pullSpeed);
                         
            }


            sideVect = Vector3.Cross(Vector3.up, ropeVect).normalized;//normalized, decide whether to optimize later
            pullDir =  Vector3.Cross(sideVect, ropeVect).normalized;//Quaternion.AngleAxis(-90, ropeVect)*sideVect;
          
            velocity +=pullDir*pullSpeed;
            
            //Debug.DrawLine (transform.position, transform.position+sideVect, Color.red);
           // Debug.DrawLine (transform.position, transform.position+velocity, Color.green);
            correction = (ropeVect*velocity.sqrMagnitude/distance)*Time.deltaTime;            //a_c=v**2/r, towards center
            //Debug.DrawLine (transform.position+velocity, transform.position+correction+velocity, Color.red);
            velocity +=correction;
            
            
            
            //player movement in relation to pulldir

            //basic slow down
            //velocity +=  (Vector3.Dot(transform.forward, pullDir)*pullDir/2 +Vector3.Dot(transform.forward, sideVect)*sideVect/4)*inputs.y;
            //velocity +=  (Vector3.Dot(transform.right, pullDir)*pullDir/2 +Vector3.Dot(transform.right, sideVect)*sideVect/4)*inputs.x;

            velocity +=  (Vector3.Project(transform.forward, pullDir)/2 +Vector3.Project(transform.forward, sideVect)/4)*inputs.y;
            velocity +=  (Vector3.Project(transform.right, pullDir)/2 +Vector3.Project(transform.right, sideVect)/4)*inputs.x;
                        

            
        }

    }


	
    
    void wallSlide( Vector3 wallNormal ,Vector2 inputs){//cst = density diff* volume
        float gVel = gravity*Time.deltaTime;
		Vector3 gravVelocity = Vector3.ProjectOnPlane(new Vector3(0,gVel,0), wallNormal);
        //float v_y = (cst*g/b)*(1-Mathf.Exp(-b*t/m));//lerp((0,(cst*g/b),-b*t/m) //check which is better
		
		float cst = Mathf.Lerp(0.75f, 0.125f, wallNormal.y);//Mathf.Max(wallNormal.y, 0.125f);
		
		velocity+= gravVelocity ;
		velocity+= quadFriction(velocity, cst);
		//inputs
		
        move = (transform.right * inputs.x + transform.forward * inputs.y)*speed;
		
		Vector3 sideDir = Vector3.Cross(wallNormal, gravVelocity);
		velocity += Vector3.Project(move, sideDir);
		//velocity +=inputs.x;
		
    }

    public bool NextToWall(LayerMask layer, out Vector3 wallNormal){ // stolen from colanderp
        float radius = 0.625f;//larger than player radius, around the player
        
		
		Vector3 startPos = transform.position-transform.up*0.4375f;//-height/4 //to start lower
        RaycastHit hit;
		float maxDist = 1f;
		
		if(Physics.SphereCast(startPos, radius, transform.up, out hit, maxDist, layer)){//throws sphere in direction indicated
			wallNormal = hit.normal;
			return true;
		}
		else{
            wallNormal = Vector3.zero;
			return false;
		}
		
        //return (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.right * dir, 0.05f, layer).Length >= 1);//throws capsule in direction indicated
    }

    /*
    void OnControllerColliderHit(ControllerColliderHit hit){
        Vector3 wallNorm = hit.normal;
		
		float projectWall = wallNorm.y;//normalized, hence angle directly
		
		if(projectWall <= 0.375f && projectWall >= -0.125f ){ 
			//wallslide
			//if(!isGrounded)
				//isNextToWall = true;//turn back false in update?

		}
		else if(projectWall > 0.375f){
			//slope
		}
		else{
			//fall
		}

    }
    */

}


//friction, assuming linear, F = -mu Fnorm
// F = m*a = -mu*m*g
// a = -mu*g
//v = v_0-mu*g*t

