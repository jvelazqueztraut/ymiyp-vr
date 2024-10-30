using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class MainMenu : MonoBehaviour
{
    private GameObject character;
    public GameObject menu, credits, about;
    public GameObject loadingInterface;
    public Image loadingProgressBar;
    //List of the scenes to load from Main Menu
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private void Start()
    {
        character = GameObject.FindGameObjectWithTag("MainCamera");
        RectTransform rect = GetComponent<RectTransform>();
        rect.position = new Vector3(rect.position.x, character.transform.position.y, rect.position.z);
    }

    public void StartGame()
    {
        HideMenu();
        HideCredits();
        ShowLoadingScreen();
        //Load the Scene asynchronously in the background
        scenesToLoad.Add(SceneManager.LoadSceneAsync("YMIYP Intro"));
        //Additive mode adds the Scene to the current loaded Scenes, in this case Gameplay scene
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Level01Part01", LoadSceneMode.Additive));
        StartCoroutine(LoadingScreen());
    }

    public void HideMenu()
    {
        menu.SetActive(false);
    }

    public void HideCredits()
    {
        credits.SetActive(false);
    }

    public void HideAbout()
    {
        about.SetActive(false);
    }

    public void ShowAbout()
    {
        about.SetActive(true);
    }

    public void ShowLoadingScreen()
    {
        loadingInterface.SetActive(true);
    }

    IEnumerator LoadingScreen()
    {
        float totalProgress = 0;
        //Iterate through all the scenes to load
        for (int i = 0; i < scenesToLoad.Count; ++i)
        {
            while (!scenesToLoad[i].isDone)
            {
                //Adding the scene progress to the total progress
                totalProgress += scenesToLoad[i].progress;
                //the fillAmount needs a value between 0 and 1, so we devide the progress by the number of scenes to load
                loadingProgressBar.fillAmount = totalProgress / scenesToLoad.Count;
                yield return null;
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}