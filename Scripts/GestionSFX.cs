using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GestionSFX : MonoBehaviour
{
    /* ************************** */
    /* Gestion des effets sonores */
    /* ************************** */


    private AudioSource Audio;
    public GameObject RefPacman;

    public AudioClip P_Life;
    public AudioClip P_Food;
    public AudioClip P_Waka;
    public AudioClip P_Death;
    public AudioClip P_Music;
    public AudioClip P_Press;
    public AudioClip P_Start;
    public AudioClip P_Ghost_1;
    public AudioClip P_Ghost_2;
    public AudioClip P_Ghost_3;
    public AudioClip P_Ghost_4;
    public AudioClip P_Ghost_5;
    public AudioClip P_Ghost_Eat;
    public AudioClip P_Ghost_Return;
    public AudioClip P_Invincibility;




    void Start()
    {
        // Raccourci du AudioSource
        Audio = GetComponent<AudioSource>();
    }

    public void ArreterSons()
    {
        // Arrêter les sons
        Audio.Stop();
    }

    public void GererSons(string SFX)
    {
        // Selon l'effet sonore,
        switch (SFX)
        {
            /***********/
            /* Sons 2D */
            /***********/

            case "Life"          : Audio.PlayOneShot(P_Life,          1.00f);  break;
            case "Music"         : Audio.PlayOneShot(P_Music,         1.00f);  break;
            case "Press"         : Audio.PlayOneShot(P_Press,         1.00f);  break;
            case "Start"         : Audio.PlayOneShot(P_Start,         1.00f);  break;
            case "Ghost_1"       : Audio.PlayOneShot(P_Ghost_1,       0.75f);  break;
            case "Ghost_2"       : Audio.PlayOneShot(P_Ghost_2,       0.75f);  break;
            case "Ghost_3"       : Audio.PlayOneShot(P_Ghost_3,       0.75f);  break;
            case "Ghost_4"       : Audio.PlayOneShot(P_Ghost_4,       0.75f);  break;
            case "Ghost_5"       : Audio.PlayOneShot(P_Ghost_5,       0.75f);  break;
            case "Ghost_Eat"     : Audio.PlayOneShot(P_Ghost_Eat,     0.75f);  break;
            case "Ghost_Return"  : Audio.PlayOneShot(P_Ghost_Return,  0.75f);  break;
            case "Invincibility" : Audio.PlayOneShot(P_Invincibility, 0.50f);  break;


            /***********/
            /* Sons 3D */
            /***********/

            case "Food" : RefPacman.GetComponent<AudioSource>().PlayOneShot(P_Food);  break;
            case "Waka" : RefPacman.GetComponent<AudioSource>().PlayOneShot(P_Waka);  break;
            case "Death": RefPacman.GetComponent<AudioSource>().PlayOneShot(P_Death); break;
        }
    }

    public IEnumerator GhostSFX(int indexSFX)
    {
        // Tant que le jeu n'est pas terminé,
        while (!GestionScene.partieTerminee)
        {
            // Si Pacman est invulnérable,
            if (GestionCollisions.estInvulnerable)
            {
                // Mettre en pause la boucle
                yield return null;
            }

            // Sinon,
            else
            {
                // Jouer l'effet sonore du fantôme
                GererSons("Ghost_" + indexSFX);

                // Attendre la fin de l'effet sonore
                yield return new WaitForSeconds(9.85f);
            }
        }
    }
}