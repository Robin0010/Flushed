using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject menu;

    private bool active;

    private void Start()
    {
        if (menu != null)
        {
            menu.SetActive(false);
        } 
    }

    public void OnButtonClickLoadScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (menu != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                active = !active;

                menu.SetActive(active);
            }
        }
    }
}
