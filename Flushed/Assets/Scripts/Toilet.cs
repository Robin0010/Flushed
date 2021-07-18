using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Toilet : MonoBehaviour
{
    
    [SerializeField] GameObject particlePosition;

    private bool switchScene;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InstantiateParticle(GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<ParticlePalette>().WaterDrops, particlePosition.transform.position);
            switchScene = true;
        }
    }

    private void InstantiateParticle(GameObject particle, Vector2 position)
    {
        Instantiate(particle, position, Quaternion.identity);
    }

    private void Update()
    {
        if (switchScene)
        {
            StartCoroutine(SwitchScene());
        }
    }

    private IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("GameScene");
    }


}
