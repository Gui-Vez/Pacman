using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class GestionObjets : MonoBehaviour
{
    /* ****************** */
    /* Gestion des objets */
    /* ****************** */


    float hauteur   = 0.000f;
    float frequence = 0.500f;
    float amplitude = 0.001f;

    float fruitAngleRot    = 0.00f;
    float fruitVitesseRot  = 0.25f;

    float camAngleRot      = 0.00f;
    float camVitesseRot    = 0.00f;
    float camVitesseRotMin = 0.01f;
    float camVitesseRotMax = 0.20f;

    public  Vector3[] TopLeft;
    public  Vector3[] TopRight;
    public  Vector3[] BottomLeft;
    public  Vector3[] BottomRight;
    private Vector3[] Pathfinding;

    private int indexPathfinding;



    void OnEnable()
    {
        // D'après le nom du parent de l'objet, exercer une fonction
        switch (transform.parent.name)
        {
            case "PacDots"  : StartCoroutine(ClignoterPacdot(0.5f)); break;
            case "Coins"    : StartCoroutine(BougerCoins(true));     break;
            case "Pastilles": StartCoroutine(BougerPastille());      break;
            case "Cameras"  : StartCoroutine(TournerCamera());       break;
            case "Fruits"   : StartCoroutine(TournerFruit());        break;
        }
    }


    IEnumerator BougerPastille()
    {
        // Tant que le jeu est en cours,
        while (GestionScene.jeuCommence)
        {
            // Appliquer une variante sinusoïdale pour la hauteur
            hauteur = Mathf.Sin(Time.time * frequence) * amplitude;

            // Modifier la hauteur de la pastille
            // transform.localPosition = new Vector3(transform.position.x, transform.position.y + hauteur, transform.position.z);

            yield return null;
        }
    }


    IEnumerator ClignoterPacdot(float delai)
    {
        // Tant que le jeu est en cours,
        while (GestionScene.jeuCommence)
        {
            // Activer le Mesh Renderer, puis donner un délai
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            yield return new WaitForSeconds(delai);

            // Désactiver le Mesh Renderer, puis donner un délai
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            yield return new WaitForSeconds(delai);

            // Mettre fin à la coroutine
            yield return null;
        }
    }


    IEnumerator TournerFruit()
    {
        // Tant que le jeu est en cours,
        while (GestionScene.jeuCommence)
        {
            // Si la rotation Y du fruit est inférieure à 360 degrés, incrémenter la rotation
            if (fruitAngleRot < 360) fruitAngleRot += fruitVitesseRot;

            // Sinon, soustraire la rotation de 360 degrés
            else fruitAngleRot = -360;

            // Si l'objet est actif,
            if (gameObject.activeSelf)
            {
                // Transformer la rotation du fruit
                transform.parent.rotation = Quaternion.Euler(transform.parent.rotation.x, fruitAngleRot, transform.parent.rotation.z);
            }

            // Mettre fin à la coroutine
            yield return null;
        }
    }


    IEnumerator TournerCamera()
    {
        // Tant que le jeu n'est pas commencé,
        while (!GestionScene.jeuCommence)
        {
            // Si le menu d'instruction est inactif,
            if (!GestionScene.montrerInstructions)
            {
                // Si la vitesse de rotation est inférieur au maximum, incrémenter celle-ci
                if (camVitesseRot < camVitesseRotMax) camVitesseRot += 0.0001f;

                // Si la rotation Y de la caméra est inférieure à 360 degrés, incrémenter la rotation
                if (camAngleRot < 360) camAngleRot += camVitesseRot;

                // Sinon, soustraire la rotation de 360 degrés
                else camAngleRot = -360;

                // Transformer la rotation de la caméra
                transform.parent.rotation = Quaternion.Euler(transform.parent.rotation.x, camAngleRot, transform.parent.rotation.z);
            }
            
            // Sinon,
            else
            {
                // Si la vitesse de rotation est suppérieur au minimum, réduire celle-ci
                if (camVitesseRot > camVitesseRotMin) camVitesseRot -= 0.0001f;
            }

            // Mettre fin à la coroutine
            yield return null;
        }
    }

    public IEnumerator BougerCoins(bool initialiser)
    {
        // S'il faut initialiser la coroutine,
        if (initialiser)
        {
            // Selon le nom du coin,
            switch (name)
            {
                // Assigner le Pathfinding à ce coin
                case "TopLeft"    : Pathfinding = TopLeft;     break;
                case "TopRight"   : Pathfinding = TopRight;    break;
                case "BottomLeft" : Pathfinding = BottomLeft;  break;
                case "BottomRight": Pathfinding = BottomRight; break;
            }
        }
        
        // Sinon,
        else
        {
            // Incrémenter l'index du Pathfinding
            indexPathfinding++;

            // Si l'index du Pathfinding touche à sa valeur maximale, Réinitialiser la valeur à 0
            if (indexPathfinding == Pathfinding.Length) indexPathfinding = 0;

            // Changer la position selon l'index du pathfinding
            transform.localPosition = Pathfinding[indexPathfinding];
        }

        // Mettre fin à la coroutine
        yield return null;
    }
}