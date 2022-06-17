using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileWeapon : MonoBehaviour
{
    //bullet 
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    
    //bullet force
    public float speed;
    Vector3 direction;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools (statemachine)
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    

    //bug fixing :D
    public bool allowInvoke = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
         if(Input.GetKeyDown(KeyCode.Mouse0)){
           Shoot();
        }
    
    }

    private void MyInput(){
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot =0;

            Shoot();
        }
    }
    
    //replace instantiate (Instantiate(playerBullet, turret.transform.position, turret.transform.rotation);)
    void Shoot(){


        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable)) {}
        
        Vector3 bulletPos = projectileSpawn.position;//projectile.transform.position
        Quaternion bulletDirection= projectileSpawn.rotation;
        //Vector3 v = bulletDirection.eulerAngles;


        GameObject projectile = Instantiate(projectilePrefab, bulletPos, bulletDirection);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), projectileSpawn.parent.GetComponent<Collider>());

        /*
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        
        rb.AddForce(-speed*projectileSpawn.forward, ForceMode.Impulse);//speed on weapon insdead of projectile?
        rb.AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
        */
        projectile.GetComponent<Projectile>().Launch(speed);

    }

    private void ResetShot(){
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload(){
        reloading = true;
        Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
    }
    private void ReloadFinished(){
        //Fill magazine
        bulletsLeft = magazineSize;
        reloading = false;
    }
}


