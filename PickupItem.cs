using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class PickupItem : MonoBehaviour, IPickup
{
    public AudioClip pickupSound;
    public AudioClip dropSound;
    [Header("Camera will be auto-detected on spawn")]
    public Camera cam; // This will be automatically assigned

    private Transform oldParent;
    private Rigidbody rb;
    private AudioSource source;

    void Start()
    {
        oldParent = transform.parent;
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        
        // Auto-detect camera if not already assigned
        if (cam == null)
        {
            FindPlayerCamera();
        }
    }

    void FindPlayerCamera()
    {
        // Find camera tagged as "MainCamera"
        GameObject mainCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCameraObj != null)
        {
            cam = mainCameraObj.GetComponent<Camera>();
            if (cam != null)
            {
                Debug.Log($"PickupItem found MainCamera: {cam.name}");
            }
            else
            {
                Debug.LogError("MainCamera GameObject found but has no Camera component!");
            }
        }
        else
        {
            Debug.LogError("No GameObject with MainCamera tag found!");
        }
    }

    public void Pickup(Transform newParent)
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(newParent);

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        
        if (pickupSound != null && source != null)
            source.PlayOneShot(pickupSound);
    }

    public void Drop(float throwPower)
    {
        transform.SetParent(oldParent);
        rb.isKinematic = false;
        
        // Check if camera is available before using it
        if (cam != null)
        {
            rb.AddForce(cam.transform.forward * throwPower, ForceMode.VelocityChange);
        }
        else
        {
            // Fallback: throw forward relative to world
            rb.AddForce(Vector3.forward * throwPower, ForceMode.VelocityChange);
            Debug.LogWarning("PickupItem: No camera found, using world forward for drop direction");
        }
        
        if (dropSound != null && source != null)
            source.PlayOneShot(dropSound);
    }

    // Optional: Method to manually set camera (useful for spawners)
    public void SetCamera(Camera newCamera)
    {
        cam = newCamera;
        Debug.Log($"PickupItem camera manually set to: {cam.name}");
    }
}