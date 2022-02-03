using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    public static Scene sceneActuelle;

    static bool peutEtreConserver;
    static int nombreConservations;



    void Awake()
    {
        // Trouver la scène actuelle
        sceneActuelle = SceneManager.GetActiveScene();

        // Trouver les objets qui portent ce script
        var porteursScript = FindObjectsOfType<DontDestroyOnLoad>();

        // Si l'objet peut être conservé ET que le nom de la scène est celle du jeu,
        if (!peutEtreConserver && sceneActuelle.name == "Jeu")
        {
            // Conserver cet objet lors d'un changement de scène
            DontDestroyOnLoad(gameObject);

            // Incrémenter le nombre de fois où l'on a conservé un objet
            nombreConservations++;

            // Si tous les objets qui portent le script ont été conservés, Interdire de conserver d'avantage
            if (nombreConservations == porteursScript.Length) peutEtreConserver = true;
        }

        // Sinon,
        else
        {
            // Détruire cet objet
            Destroy(gameObject);
        }
    }
}