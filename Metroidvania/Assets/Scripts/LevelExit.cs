using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour, IInteractable
{
    [SerializeField] string nextScene = "NEXT";
    private ScreenFader sf;
    private Coroutines coroutines;
    //private GameManager gm;

    private void Start() {
        sf = FindObjectOfType<ScreenFader>();
        coroutines = Coroutines.GetInstance();
        //gm = FindObjectOfType<GameManager>();
    }

    private void NextScene() {

        sf.FadeIn();

        if (coroutines == null) {
            coroutines = FindObjectOfType<Coroutines>();
        }
        
        coroutines.StopAllCoroutines();
    
        if (nextScene == "NEXT") {
            int n = SceneManager.GetActiveScene().buildIndex;
            coroutines.WaitThenExecute(1.1f, () => {
                SceneManager.LoadScene(n+1); 
            });
        }
        else {
            coroutines.WaitThenExecute(1.1f, () => {
                SceneManager.LoadScene(nextScene); 
            });
        }
    }

    public void interact() {
        NextScene();
    }

}
