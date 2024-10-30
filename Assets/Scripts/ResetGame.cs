using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    FadeMaster fadeMaster;
    private bool ending = false;

    private void Start()
    {
        fadeMaster = GameObject.FindGameObjectWithTag("Fade Master").GetComponent<FadeMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ending && fadeMaster.hasFinished())
        {
            EndGame();
            ending = false;
        }
    }

    public void Ending(GameObject obj)
    {
        fadeMaster.FadeOut(gameObject);
        ending = true;
    }

    private void EndGame()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}