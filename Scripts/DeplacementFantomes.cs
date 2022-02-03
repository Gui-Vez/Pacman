using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeplacementFantomes : MonoBehaviour
{
    /* ************************ */
    /* Déplacement des fantômes */
    /* ************************ */


    public  GameObject Pacman;
    public  GameObject RefSons;
    public  GameObject prefabVulnerable;
    private GameObject cloneVulnerable;

    public  GameObject RefTopLeft;
    public  GameObject RefTopRight;
    public  GameObject RefBottomLeft;
    public  GameObject RefBottomRight;

    private NavMeshAgent navAgent;
    private Vector3 positionInitiale;
    private Vector3 visible;
    private Vector3 invisible;

    private Vector3 devant;
    private Vector3 dessus;
    private Vector3 derriere;
    private Vector3 aleatoire;

    private Vector3 TopLeft;
    private Vector3 TopRight;
    private Vector3 BottomLeft;
    private Vector3 BottomRight;

    private int indexPosition;
    private float difficulte;



    void Start()
    {
        // Donner un raccourci au script de gestion du son
        GestionSFX SFX = RefSons.GetComponent<GestionSFX>();

        // Appliquer la position initiale à l'endroit où se trouve l'objet
        positionInitiale = transform.position;

        // Créer un clône du fantôme vulnérable
        cloneVulnerable = Instantiate(prefabVulnerable, prefabVulnerable.transform.parent);

        // Ajouter le nom du fantôme au clône
        cloneVulnerable.name += "(" + gameObject.name + ")";

        // Activer le clône
        cloneVulnerable.SetActive(true);

        // Assigner des variables pour le scale
        visible   = new Vector3(1.0f, 1.0f, 1.0f);
        invisible = new Vector3(0.8f, 0.8f, 0.8f);

        // Raccourci du Nav Mesh Agent
        navAgent = GetComponent<NavMeshAgent>();

        // Ajuster une difficulté selon la manche actuelle
        difficulte = (float)GestionScene.manche / 5;

        // Donner une valeur aléatoire à la position
        indexPosition = Random.Range(1, 4);

        // Selon le type de fantôme, Donner une vitesse
        switch (gameObject.name)
        {
            // Assigner une vitesse au AI en additionnant la difficulté du jeu
            case "Blinky": navAgent.speed = 3.00f + difficulte; break;
            case "Inky"  : navAgent.speed = 2.50f + difficulte; break;
            case "Pinky" : navAgent.speed = 2.00f + difficulte; break;
            case "Clyde" : navAgent.speed = 1.50f + difficulte; break;
        }
    }

    void OnTriggerEnter(Collider coin)
    {
        // Si la collision est un coin, Faire bouger ce coin
        if (coin.transform.parent.name == "Coins") StartCoroutine(coin.GetComponent<GestionObjets>().BougerCoins(false));
    }

    void Update()
    {
        // Appliquer la position du clône sur le fantôme
        cloneVulnerable.transform.position = transform.position;

        // Assigner la rotation du clône sur le fantôme 
        cloneVulnerable.transform.rotation = transform.rotation;

        // Assigner les positions de poursuite
        devant    = Pacman.GetComponent<DeplacementPacman>().positionDevant;
        dessus    = Pacman.GetComponent<DeplacementPacman>().positionActuelle;
        derriere  = Pacman.GetComponent<DeplacementPacman>().positionDerriere;

        // Assigner les positions d'évasion
        TopLeft     = RefTopLeft.transform.position;
        TopRight    = RefTopRight.transform.position;
        BottomLeft  = RefBottomLeft.transform.position;
        BottomRight = RefBottomRight.transform.position;

        // Selon l'index de position,
        switch (indexPosition)
        {
            // Assigner une valeur aléatoire
            case 1: aleatoire = devant;   break;
            case 2: aleatoire = dessus;   break;
            case 3: aleatoire = derriere; break;
        }

        // Si la partie est terminée, Désactiver le navAgent
        if (GestionScene.partieTerminee) navAgent.enabled = false;

        // Sinon,
        else
        {
            // Activer le navAgent
            navAgent.enabled = true;

            // Si Pacman est invulnérable,
            if (GestionCollisions.estInvulnerable)
            {
                // Rendre le fantôme invisible
                gameObject.transform.localScale = invisible;

                // Rendre le clône visible
                cloneVulnerable.transform.localScale = visible;

                // Selon le nom du fantôme,
                switch (name)
                {
                    // Déplacer le AI sur l'un des coins du labyrinthe
                    case "Blinky": navAgent.SetDestination(TopRight);    break;
                    case "Inky"  : navAgent.SetDestination(BottomRight); break;
                    case "Pinky" : navAgent.SetDestination(TopLeft);     break;
                    case "Clyde" : navAgent.SetDestination(BottomLeft);  break;
                }
            }

            // Sinon,
            else
            {
                // Rendre le fantôme visible
                gameObject.transform.localScale = visible;

                // Rendre le clône invisible
                cloneVulnerable.transform.localScale = invisible;

                // Selon le nom du fantôme,
                switch (name)
                {
                    // Déplacer le AI devant/derrière/dessus Pacman
                    case "Blinky": navAgent.SetDestination(dessus);    break;
                    case "Inky"  : navAgent.SetDestination(devant);    break;
                    case "Pinky" : navAgent.SetDestination(derriere);  break;
                    case "Clyde" : navAgent.SetDestination(aleatoire); break;
                }
            }
        }
    }

    public void RetourPrincipal()
    {
        // Modifier la position du fantôme à sa position initiale
        transform.position = positionInitiale;
    }
}