using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] private int gunID = 1;

    public void SetGunID(int id)
    {
        gunID = id;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerShooting shooting = collision.GetComponent<PlayerShooting>();

            if (shooting != null)
            {
                shooting.ChangeWeapon(gunID);
            }

            Destroy(gameObject);
        }
    }
}