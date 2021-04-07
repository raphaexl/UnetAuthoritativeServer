using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof (CharacterController))]
public class PlayerNetwork : NetworkPacketController
{
    [SerializeField]
    private float moveSpeed = .4f;
    [SerializeField]
    [Range(0.1f, 1.0f)]
    private float networkSendRate = 0.5f;
    [SerializeField]
    private bool isPredictionEnabled = default ;
    [SerializeField]
    private float correctionThreshold = 0.01f;
    
    CharacterController controller;
    List<ReceivedPackage> predictedPackages;
    Vector3 lastPosition;

    private float nextCorrectTime;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        PackageManager.SendSpeed = networkSendRate;
        ServerPackageManager.SendSpeed = networkSendRate;
        predictedPackages = new List<ReceivedPackage>();
    }

    // Update is called once per frame
    void Update()
    {
        LocalPlayerUpdate();
        ServerPlayerUpdate();
        RemoteClientUpdate();
    } 

    void LocalPlayerUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (!isLocalPlayer)
            return;
        if (inputX != 0 || inputY != 0)
        {
            float timestamp = Time.time;
            PackageManager.AddPackage(
                new Package
                {
                    Horizontal = inputX,
                    Vertical = inputY,
                    Timestamp = timestamp
                });
            if (isPredictionEnabled)
            {
                Move(inputX * moveSpeed, inputY * moveSpeed);
                predictedPackages.Add(new ReceivedPackage
                {
                    X = transform.position.x,
                    Y = transform.position.y,
                    Z = transform.position.z,
                    Timestamp = timestamp
                });
            }
        } 
      
    }

    void ServerPlayerUpdate()
    {
        if (!isServer || isLocalPlayer)
            return;
        Package packageData = PackageManager.GetNextDataReceived();
        if (packageData == null) { return; }
        Move(packageData.Horizontal * moveSpeed, packageData.Vertical * moveSpeed);
        if (transform.position == lastPosition)
            return;
        lastPosition = transform.position;

        ServerPackageManager.AddPackage(new ReceivedPackage
        {
            X = transform.position.x,
            Y = transform.position.y,
            Z = transform.position.z,
            Timestamp = packageData.Timestamp
        }); 
    }

    void    Move(float horizontal, float vertical)
    {
        controller.Move(new Vector3(horizontal, 0, vertical));
    }

    private void RemoteClientUpdate()
    {
        if (isServer) { return; }
        ReceivedPackage data = ServerPackageManager.GetNextDataReceived();
        if (data == null)
            return;
        if (isLocalPlayer && isPredictionEnabled)
        {
            var transmittedPackage = predictedPackages.Where(x => x.Timestamp == data.Timestamp).FirstOrDefault();
            if ( transmittedPackage == null)
            {
                //You should do something here :-) 
                return;
            }
            if (Vector3.Distance(new Vector3(transmittedPackage.X, transmittedPackage.Y, transmittedPackage.Z),
                new Vector3(data.X, data.Y, data.Z)) > correctionThreshold)
            {
                transform.position = new Vector3(data.X, data.Y, data.Z);
            }
            //Clear all pedicted
            predictedPackages.RemoveAll(x => x.Timestamp <= data.Timestamp); 
        }
        else { transform.position = new Vector3(data.X, data.Y, data.Z); }
        
    }
}
