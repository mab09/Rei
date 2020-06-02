using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public static bool reset = false;
    public Button restart;
    public Text finalresult;
    // Start is called before the first frame update
    void Start()
    {
        restart.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        reset = true;
    }

    // Update is called once per frame
    void Update()
    {
        string result = "Begin";
        int lchoice = LeftPlayer.strikeL;
        int rchoice = RightPlayer.strikeR;

        if (lchoice == 0 && rchoice != 0) { result = "Shogun loses"; } //Shogun didn't move
        else if (rchoice == 0 && lchoice != 0) { result = "Rei loses"; } //Rei didn't move
        else if (rchoice == 0 && lchoice == 0) { result = "Begin"; } //Initial
        else
        {
            if (rchoice != lchoice)
            {
                if (rchoice == 1)
                {
                    if (lchoice == 2)
                    {
                        result = "Shogun wins"; //Shogun: Paper x Rei: Rock
                    }
                    else if (lchoice == 3)
                    {
                        result = "Rei wins"; //Shogun: Scissor x Rei: Rock
                    }
                }
                else if (rchoice == 2)
                {
                    if (lchoice == 1)
                    {
                        result = "Rei wins"; //Shogun: Rock x Rei: Paper
                    }
                    else if (lchoice == 3)
                    {
                        result = "Shogun wins"; //Shogun: Scissor x Rei: Paper
                    }
                }
                else if (rchoice == 3)
                {
                    if (lchoice == 1)
                    {
                        result = "Shogun wins"; //Shogun: Rock x Rei: Scissor
                    }
                    else if (lchoice == 2)
                    {
                        result = "Rei wins"; //Shogun: Paper x Rei: Scissor
                    }
                }
            }
            else { result = "Again!"; } //Draw
        }
        if (reset == true)
        {
            if(SceneManager.GetActiveScene().name == "Amaterasu") SceneManager.LoadScene("Tsukiyomi");
            else SceneManager.LoadScene("Amaterasu");
            LeftPlayer.strikeL = 0;
            RightPlayer.strikeR = 0;
            reset = false;
        }
        finalresult.text = result;
    } 
}
