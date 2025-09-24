using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private TMP_Text text_collectibles;
    private TMP_Text text_win;
    private GameObject start_menu;
    private GameObject game;
    private GameObject collectibles;
    void Start()
    {
        text_collectibles = transform.Find("text_collectibles").gameObject.GetComponent<TMP_Text>();
        text_win = transform.Find("text_win").gameObject.GetComponent <TMP_Text>();
        start_menu = transform.Find("StartMenu").gameObject;
        game = GameObject.Find("Game");
        collectibles = game.transform.Find("Collectibles").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        int collectibles_count = 0;
        foreach (Transform child in collectibles.transform)
        {
            if (child.gameObject.activeSelf)
            {
                collectibles_count++;
            }
        }

        text_collectibles.text = "Collectibles left: " + collectibles_count;

        if (collectibles_count == 0) {
            text_win.gameObject.SetActive(true);
            start_menu.SetActive(true);
            game.transform.Find("Player").gameObject.GetComponent<BallBehaviour>().enabled = false;
        }
    }
}
