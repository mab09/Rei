using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------- */
                       //start move  //start count
    public static bool begin,        ready,        reset;

    public Button restart, readyB;
    public Text finalresult;

    public AudioSource IHBeat, FHBeat;

    private float time0, timeL, timeR, AISpeed, go;
    private int AIChoice;
    private bool end, ib1 = true, ib2 = true, fb = true;

    /* ------------------------------------------------------------------------------------------- */

    // Start is called before the first frame update
    void Start() 
    {
        restart.onClick.AddListener(ResetOnClick);
        readyB.onClick.AddListener(ReadyOnClick);
        //finalresult.text = "Begin";
       // end = false;
        reset = false;
        ready = false;
        begin = false;
        go = Random.Range(2.5f, 10f);
        AIChoice = Random.Range(1, 4);
        AISpeed = Random.Range(0.2f, 1.5f);
    }

    /* ------------------------------------------------------------------------------------------- */

    void ReadyOnClick()
    {
        time0 = Time.deltaTime;
        timeL = Time.deltaTime;
        timeR = Time.deltaTime;
        ready = true;
    }

    /* ------------------------------------------------------------------------------------------- */

    void ResetOnClick()
    {
        reset = true;
    }

    /* ------------------------------------------------------------------------------------------- */

    // Update is called once per frame
    void Update()
    {
        int lchoice = LeftPlayer.strikeL, rchoice = RightPlayer.strikeR, winner = 0;
        string result = "Begin", Left = "Shogun", Right = "Rei", lattack = "none", rattack = "none";


        /* ---Code to select attack animation--- */
        if(lchoice == 1) lattack = "TriggerUp";
        else if (lchoice == 2) lattack = "TriggerDown";
        else if (lchoice == 3) lattack = "TriggerFront";

        if (rchoice == 1) rattack = "TriggerUp";
        else if (rchoice == 2) rattack = "TriggerDown";
        else if (rchoice == 3) rattack = "TriggerFront";
        /* ---Code to select attack animation--- */


        if (ready == true)// && end == false)
        {
            readyB.gameObject.SetActive(false);

            /* --start counting delta time-- */
            time0 += Time.deltaTime;
            if (LeftPlayer.lcount == true) timeL += Time.deltaTime;
            if (RightPlayer.rcount == true) timeR += Time.deltaTime;
            /* --start counting delta time-- */


            if (time0 <= 2)
            {
                finalresult.text = "|";
                if (ib1)
                {
                    IHBeat.Play();
                    ib1 = false;
                }
                LeftPlayer.anim.SetTrigger("TriggerReady");
                RightPlayer.anim.SetTrigger("TriggerReady");
            }


            else if (time0 <= go)
            {
                finalresult.text = "| |";

                if (ib2)
                {
                    IHBeat.Play();
                    ib2 = false;
                }
            }


            else
            {
                begin = true;
                finalresult.text = "<b>| | |</b>";

                if (fb)
                {
                    FHBeat.Play();
                    fb = false;
                }


                /* ---Code for AI On--- */
                if (LeftPlayer.ai == true)
                {
                    if (time0 >= go + AISpeed)
                    {
                        LeftPlayer.strikeL = AIChoice;
                        lchoice = LeftPlayer.strikeL;
                        LeftPlayer.lcount = false;
                        LeftPlayer.anim.SetTrigger("TriggerRun");
                        LeftPlayer.katanaOff.SetActive(false);
                        LeftPlayer.katanaOn.SetActive(true);
                    }
                }
                /* ---Code for AI On--- */


                /* ---Main Code--- */
                if (rchoice == 0 && lchoice == 0) { /*result = "Begin"; */ } //Initial
                else if (rchoice == 0 && lchoice != 0) { result = Right + " loses"; winner = 1; } //Rei didn't move
                else if (lchoice == 0 && rchoice != 0) { result = Left + " loses"; winner = 2; } //Shogun didn't move
                else
                {


                    /* ---Code for quick win--- */
                    //Debug.Log(timeL + "|" + timeR);
                    if (timeL > timeR + 0.1f) //Rei is quick
                    {
                        result = Right + " Wins, speed"; winner = 2;
                    }
                    else if (timeR > timeL + 0.1f) //Shogun is quick;
                    {
                        result = Left + " Wins, speed"; winner = 1;
                    }
                    /* ---Code for quick win--- */


                    /* ---Code for Taka, Tora, Hebi (Rock, Paper, Scissors)--- */
                    else
                    {
                        if (rchoice != lchoice)
                        {
                            if (rchoice == 1)
                            {
                                if (lchoice == 2)
                                {
                                    result = Left + " wins"; //Shogun: Paper x Rei: Rock
                                    winner = 1;
                                }
                                else if (lchoice == 3)
                                {
                                    result = Right + " wins"; //Shogun: Scissor x Rei: Rock
                                    winner = 2;
                                }
                            }
                            else if (rchoice == 2)
                            {
                                if (lchoice == 1)
                                {
                                    result = Right + " wins"; //Shogun: Rock x Rei: Paper
                                    winner = 2;
                                }
                                else if (lchoice == 3)
                                {
                                    result = Left + " wins"; //Shogun: Scissor x Rei: Paper
                                    winner = 1;
                                }
                            }
                            else if (rchoice == 3)
                            {
                                if (lchoice == 1)
                                {
                                    result = Left + " wins"; //Shogun: Rock x Rei: Scissor
                                    winner = 1;
                                }
                                else if (lchoice == 2)
                                {
                                    result = Right + " wins"; //Shogun: Paper x Rei: Scissor
                                    winner = 2;
                                }
                            }
                        }
                        else { result = "Again!"; winner = 0; } //Draw
                    }
                    /* ---Code for Taka, Tora, Hebi (Rock, Paper, Scissors)--- */

                }
                /* ---Main Code--- */


                /* ---Code for attack animations---*/
                if (LeftPlayer.pos - RightPlayer.pos >= -10)
                {
                    LeftPlayer.anim.SetTrigger(lattack);
                    RightPlayer.anim.SetTrigger(rattack);
                }
                /* ---Code for attack animations---*/


                /* ---Post-Showdown--- */
                if (LeftPlayer.pos - RightPlayer.pos >= 5)
                {
                    LeftPlayer.strikeL = 0;
                    RightPlayer.strikeR = 0;
                    ready = false;
                    //begin = false;
                    //finalresult.text = result;
                    //end = true;
                    if (LeftPlayer.llose == true && RightPlayer.rlose == false)
                    {
                        LeftPlayer.anim.SetTrigger("TriggerDeath");
                        RightPlayer.anim.SetTrigger("TriggerWin");
                    }
                    else if (RightPlayer.rlose == true && LeftPlayer.llose == false)
                    {
                        RightPlayer.anim.SetTrigger("TriggerDeath");
                        LeftPlayer.anim.SetTrigger("TriggerWin");
                    }
                    else if (RightPlayer.rlose == true && LeftPlayer.llose == true)
                    {
                        LeftPlayer.anim.SetTrigger("TriggerDraw");
                        RightPlayer.anim.SetTrigger("TriggerDraw");
                    }
                    else
                    {
                        if (winner == 1)
                        {
                            RightPlayer.anim.SetTrigger("TriggerDeath");
                            LeftPlayer.anim.SetTrigger("TriggerWin");

                        }
                        else if (winner == 2)
                        {
                            LeftPlayer.anim.SetTrigger("TriggerDeath");
                            RightPlayer.anim.SetTrigger("TriggerWin");
                        }
                        else
                        {
                            LeftPlayer.anim.SetTrigger("TriggerDraw");
                            RightPlayer.anim.SetTrigger("TriggerDraw");
                        }
                    }
                }
                /* ---Post-Showdown--- */

            }
        }

        /* --Restart Game-- */
        if (reset == true)
        {
            SceneManager.LoadScene(Random.Range(0, SceneManager.sceneCount + 2));
           // finalresult.text = "Begin";
            LeftPlayer.llose = false;
            RightPlayer.rlose = false;
            LeftPlayer.lcount = true;
            RightPlayer.rcount = true;
            LeftPlayer.strikeL = 0;
            RightPlayer.strikeR = 0;
            readyB.gameObject.SetActive(true);
           // reset = false;
           // ready = false;
           // end = false;
           // ai = false;
        }
        /* --Restart Game-- */

    }
}
