using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class DeplacementPacman : MonoBehaviour
{
    /* ************************* */
    /* Déplacement du personnage */
    /* ************************* */


    public Vector3 positionDevant;
    public Vector3 positionDerriere;
    public Vector3 positionActuelle;

    public float vitesseTranslation;
    public float vitesseRotation;

    bool W;
    bool A;
    bool S;
    bool D;
    bool Haut;
    bool Gauche;
    bool Bas;
    bool Droite;

    private bool peutAvancer;
    private int indexRotation;
    private int degreRotation;
    Quaternion angleRotation;
    Vector3 translationHorizontale;
    Vector3 translationVerticale;
    KeyCode derniereTouche;



    void Start()
    {
        // Actualiser la position après chaque quart de seconde
        InvokeRepeating("ActualiserPosition", 0f, 0.25f);
    }

    void Update()
    {
        // Si la partie n'est pas terminée,
        if (!GestionScene.partieTerminee)
        {
            /* Raccourcis des touches du clavier */

            W = Input.GetKey(KeyCode.W);
            A = Input.GetKey(KeyCode.A);
            S = Input.GetKey(KeyCode.S);
            D = Input.GetKey(KeyCode.D);

            Haut   = Input.GetKey(KeyCode.UpArrow);
            Gauche = Input.GetKey(KeyCode.LeftArrow);
            Bas    = Input.GetKey(KeyCode.DownArrow);
            Droite = Input.GetKey(KeyCode.RightArrow);


            // Si le joueur appuie sur la touche '...',
            // La dernière touche sera associée à celle-ci

            if (W || Haut)   derniereTouche = KeyCode.W;
            if (D || Droite) derniereTouche = KeyCode.D;
            if (S || Bas)    derniereTouche = KeyCode.S;
            if (A || Gauche) derniereTouche = KeyCode.A;


            // Déplacer Pacman
            GererTranslation();

            // Orienter Pacman
            GererRotation();

            // Générer le Raycast
            GenererRaycast();
        }
    }
    
    void GererTranslation()
    {
        // Si Pacman est dans la même direction que son angle de rotation ET qu'il peut avancer,
        if (transform.rotation == angleRotation && peutAvancer)
        {
            // Appliquer une valeur pour la translation horizontale
            translationHorizontale = transform.TransformDirection(1f, 0f, 0f) * vitesseTranslation * Time.deltaTime;

            // Appliquer une valeur pour la translation verticale
            translationVerticale   = transform.TransformDirection(0f, 0f, 1f) * vitesseTranslation * Time.deltaTime;

            // Modifier la translation en fonction de sa direction
            switch (derniereTouche)
            {
                case KeyCode.W: transform.Translate(-translationVerticale);   break;
                case KeyCode.D: transform.Translate(translationHorizontale);  break;
                case KeyCode.S: transform.Translate(translationVerticale);    break;
                case KeyCode.A: transform.Translate(-translationHorizontale); break;
            }
        }
    }

    void GererRotation()
    {
        // Modifier l'index de l'angle en fonction de sa direction
        switch (derniereTouche)
        {
            case KeyCode.W: indexRotation = 1; break;  // 1 % 4 = 1
            case KeyCode.D: indexRotation = 2; break;  // 2 % 4 = 2
            case KeyCode.S: indexRotation = 3; break;  // 3 % 4 = 3
            case KeyCode.A: indexRotation = 0; break;  // 4 % 4 = 0
        }

        // Varier le degré de rotation selon l'index sélectionné
        degreRotation = (90 * indexRotation);

        // Appliquer un angle de rotation selon le degré actuel
        angleRotation = Quaternion.Euler(0, degreRotation, 0);

        // Transformer la rotation de Pacman selon l'angle choisi
        transform.rotation = Quaternion.RotateTowards(transform.rotation, angleRotation, vitesseRotation);
    }

    void GenererRaycast()
    {
        // Créer une variable qui contiendra l'information de type RaycastHit
        RaycastHit hit;

        // Créer une ligne de dubugage pour afficher le raycast
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left), Color.white);

        // Si le Raycast détecte qu'un objet est positionné devant Pacman, Interdire Pacman d'avancer plus loin
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 0.5f)) peutAvancer = false;

        // Sinon, Pacman peut avancer
        else peutAvancer = true;
    }

    void ActualiserPosition()
    {
        // Si Pacman peut avancer,
        if (peutAvancer)
        {
            // Deux cases devant Pacman
            positionDevant   = transform.position + transform.TransformDirection(Vector3.left * 2);

            // Deux cases derrière Pacman
            positionDerriere = transform.position - transform.TransformDirection(Vector3.left * 2);

            // Position actuelle de Pacman
            positionActuelle = transform.position;
        }
        
        // Sinon,
        else
        {
            // Assigner toutes les positions sur Pacman
            positionDevant = positionDerriere = positionActuelle;
        }
    }
}