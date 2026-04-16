using UnityEngine;
using UnityEngine.InputSystem;

public class ShipShoot : MonoBehaviour
{
    public GameObject projectileprefab;
    private PlayerInput playerInput;
    public InputAction shootAction;
    public Transform projectileposition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the input action was pressed during this frame
        if (shootAction != null && shootAction.WasPressedThisFrame())
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        AudioManager.PlayPlayerShoot();
        ProjectilePool.Instance.GetProjectile(projectileposition.position, projectileposition.rotation);
    }
}
