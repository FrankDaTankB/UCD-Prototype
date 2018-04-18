
/*
 * Sonya Rivers-Medina
 * User Centered Design
 * Spring 2018
 * BAM! Movers Project
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//This class allows the user to swith between scenes
public class LevelManager : MonoBehaviour {

    
    //Sends user back to Welcome scene
	public void LoadWelcome()
    {
        SceneManager.LoadScene("Welcome");
    }

    //Sends user to Login scene
    public void LoadLogin()
    {
        SceneManager.LoadScene("Login");
    }

    //Sends user to SignUp scene
    public void LoadSignUp()
    {
        SceneManager.LoadScene("SignUp");
    }

}
