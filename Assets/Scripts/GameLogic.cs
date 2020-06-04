using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public static bool reset = false, ready = false, begin = false;
    public Button restart, readyB;
    public Text finalresult;
    private float time0, go;
    bool end;
    // Start is called before the first frame update
    void Start() 
    {
        restart.onClick.AddListener(TaskOnClick);
        readyB.onClick.AddListener(ReadyOnClick);
        finalresult.text = "Begin";
        end = false;
        go = Random.Range(2.5f, 10f);
    }

    void ReadyOnClick()
    {
        time0 = Time.deltaTime;
        ready = true;
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

        if (ready == true && end == false) {
            time0 += Time.deltaTime;
            LeftPlayer.timeL += Time.deltaTime;
            RightPlayer.timeR += Time.deltaTime;
            if (time0 <= 2) finalresult.text = "1";
            else if (time0 <= go) finalresult.text = "2";
            else
            {
                finalresult.text = "3";
                begin = true;
                if (rchoice == 0 && lchoice == 0) { /*result = "Begin"; */ } //Initial
                else if (rchoice == 0 && lchoice != 0) { result = "Rei loses"; } //Rei didn't move
                else if (lchoice == 0 && rchoice != 0) { result = "Shogun loses"; } //Shogun didn't move
                else
                {
                    if (LeftPlayer.timeL > RightPlayer.timeR + 0.5f) //Rei is patient
                        result = "Rei Wins";
                    else if (RightPlayer.timeR > LeftPlayer.timeL + 0.5f) //Shogun is patient; change time with speed
                        result = "Shogun Wins";
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
                }
                if (LeftPlayer.pos - RightPlayer.pos >= 6)
                {
                    LeftPlayer.strikeL = 0;
                    RightPlayer.strikeR = 0;
                    ready = false;
                    begin = false;
                    finalresult.text = result;
                    end = true;
                }
            }
        }
        if (LeftPlayer.llose == true && RightPlayer.rlose == true)
            finalresult.text = "Double Harakiri";
        else if(LeftPlayer.llose == true)
            finalresult.text = "Shogun does Harakiri";
        else if(RightPlayer.rlose == true)
            finalresult.text = "Rei does Harakiri";
        if (reset == true)
        {
            if (SceneManager.GetActiveScene().name == "Amaterasu") SceneManager.LoadScene("Tsukiyomi");
            else SceneManager.LoadScene("Amaterasu");
            finalresult.text = "Begin";
            LeftPlayer.llose = false;
            RightPlayer.rlose = false;
            reset = false;
            ready = false;
            end = false;
        }
    } 
}
