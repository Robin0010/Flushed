using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    [SerializeField] bool usedByEnemy;
    [SerializeField] bool usedByPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (usedByPlayer)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);

                Destroy(gameObject);
            }
        }
        else if (usedByEnemy)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<CharacterController>().TakeDamage(damage);

                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
