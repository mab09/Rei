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
        finalresult.text = "Begin";
    }

    void TaskOnClick()
    {
        reset = true;
    }

    // Update is called once per frame
    void Update()
    {
        int lchoice = LeftPlayer.strikeL;
        int rchoice = RightPlayer.strikeR;

        if (lchoice == 0 && rchoice != 0) { finalresult.text = "Shogun loses"; } //Shogun didn't move
        else if (rchoice == 0 && lchoice != 0) { finalresult.text = "Rei loses"; } //Rei didn't move
        else if (rchoice == 0 && lchoice == 0) { /*finalresult.text = "Begin"; */ } //Initial
        else
        {
            if (rchoice != lchoice)
            {
                if (rchoice == 1)
                {
                    if (lchoice == 2)
                    {
                        finalresult.text = "Shogun wins"; //Shogun: Paper x Rei: Rock
                    }
                    else if (lchoice == 3)
                    {
                        finalresult.text = "Rei wins"; //Shogun: Scissor x Rei: Rock
                    }
                }
                else if (rchoice == 2)
                {
                    if (lchoice == 1)
                    {
                        finalresult.text = "Rei wins"; //Shogun: Rock x Rei: Paper
                    }
                    else if (lchoice == 3)
                    {
                        finalresult.text = "Shogun wins"; //Shogun: Scissor x Rei: Paper
                    }
                }
                else if (rchoice == 3)
                {
                    if (lchoice == 1)
                    {
                        finalresult.text = "Shogun wins"; //Shogun: Rock x Rei: Scissor
                    }
                    else if (lchoice == 2)
                    {
                        finalresult.text = "Rei wins"; //Shogun: Paper x Rei: Scissor
                    }
                }
            }
            else { finalresult.text = "Again!"; } //Draw
        }
        if (LeftPlayer.pos - RightPlayer.pos > 6)
        {
            LeftPlayer.strikeL = 0;
            RightPlayer.strikeR = 0;
        }
        if (reset == true)
        {
            if(SceneManager.GetActiveScene().name == "Amaterasu") SceneManager.LoadScene("Tsukiyomi");
            else SceneManager.LoadScene("Amaterasu");
            finalresult.text = "Begin";
            reset = false;
        }
    } 
}
