using UnityEngine;

public class BackAndForthMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public Vector3 targetPosition = new Vector3(5f, 0f, 0f);
    public bool useWorldPosition = true; // If false, target is relative to start position
    
    private Vector3 startPosition;
    private Vector3 actualTargetPosition;
    private bool movingToTarget = true;
    
    void Start()
    {
        // Store the starting position
        startPosition = transform.position;
        
        // Set actual target position based on useWorldPosition setting
        if (useWorldPosition)
        {
            actualTargetPosition = targetPosition;
        }
        else
        {
            actualTargetPosition = startPosition + targetPosition;
        }
    }
    
    void Update()
    {
        // Move towards current destination
        Vector3 destination = movingToTarget ? actualTargetPosition : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        
        // Check if we've reached the destination
        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            // Switch direction
            movingToTarget = !movingToTarget;
        }
    }
    
    // Method to change target position at runtime
    public void SetTargetPosition(Vector3 newTarget, bool isWorldPosition = true)
    {
        targetPosition = newTarget;
        
        if (isWorldPosition)
        {
            actualTargetPosition = newTarget;
        }
        else
        {
            actualTargetPosition = startPosition + newTarget;
        }
    }
    
    // Method to set new start position (useful if you want to change the movement path)
    public void SetStartPosition(Vector3 newStart)
    {
        startPosition = newStart;
        
        if (!useWorldPosition)
        {
            actualTargetPosition = startPosition + targetPosition;
        }
    }
    
    // Visualize the movement path in the Scene view
    void OnDrawGizmos()
    {
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 target = Application.isPlaying ? actualTargetPosition : 
                        (useWorldPosition ? targetPosition : transform.position + targetPosition);
        
        // Draw line between start and target
        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, target);
        
        // Draw spheres at start and target positions
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(start, 0.2f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target, 0.2f);
    }
}