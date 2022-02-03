using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GestionCollisions : MonoBehaviour
{
    /* ********************** */
    /* Gestion des collisions */
    /* ********************** */


    public GameObject RefScene;
    public GameObject RefSons;
    public GameObject RefParticules;

    public ParticleSystem ParticulesMort;
    public ParticleSystem ParticulesFantome;

    public List<GameObject> listeTouches;
    private GameObject fantomeTouche;
    private GameObject fantomeVulnerable;

    public static bool estInvulnerable;
    private bool tempsReduit;
    private int tempsInvulnerabilite;
    private int combo;


    void Start()
    {
        // Donner un raccourci au script de gestion du son
        GestionSFX SFX = RefSons.GetComponent<GestionSFX>();

        // Réinitialiser le combo
        combo = 0;

        // Rendre Pacman vulnérable
        estInvulnerable = false;

        // Le temps d'invulnérabilité ne se réduit pas
        tempsReduit = false;
    }

    void OnTriggerEnter(Collider trigger)
    {
        // Si l'objet est une Pastille,
        if (trigger.tag == "Pastille")
        {
            // Désactiver la pastille
            trigger.gameObject.SetActive(false);

            // Donner 10 points
            RefScene.GetComponent<GestionScene>().DonnerPointage(10);

            // Jouer l'effet sonore de marche
            RefSons.GetComponent<GestionSFX>().GererSons("Waka");

            // Baisser le nombre de pastilles
            GestionScene.nombrePastilles--;

            // Sil ne reste plus de pastilles,
            if (GestionScene.nombrePastilles == 0)
            {
                // Trouver tous les fantomes bleus
                GameObject[] listeBleus = GameObject.FindGameObjectsWithTag("Fantome Bleu");

                // Pour chaque bleu de la liste des fantômes bleus, Ajouter ce bleu à la liste
                foreach (GameObject Bleu in listeBleus) listeTouches.Add(Bleu);

                // Désactiver les fantômes touchés
                AffecterFantomesTouches(false);

                // Enlever les éléments touchés de la liste
                listeTouches.Clear();

                // Gagner la manche
                StartCoroutine(RefScene.GetComponent<GestionScene>().RecommencerManche("Gagnée"));
            }
        }

        // Si l'objet est un 'PacDot',
        if (trigger.tag == "PacDot")
        {
            // Désactiver le Pacdot
            trigger.gameObject.SetActive(false);

            // Donner 50 points
            RefScene.GetComponent<GestionScene>().DonnerPointage(50);

            // Jouer l'effet sonore du PacDot
            RefSons.GetComponent<GestionSFX>().GererSons("Food");

            // Rendre Pacman invulnérable
            StartCoroutine(RendreInvulnerable(9));
        }

        // Si l'objet est un 'Fruit',
        if (trigger.tag == "Fruit")
        {
            // Appliquer un pointage selon le nom du fruit
            switch (trigger.name)
            {
                case "Banane": RefScene.GetComponent<GestionScene>().DonnerPointage(100); break;
                case "Cerise": RefScene.GetComponent<GestionScene>().DonnerPointage(300); break;
                case "Orange": RefScene.GetComponent<GestionScene>().DonnerPointage(500); break;
                case "Pomme" : RefScene.GetComponent<GestionScene>().DonnerPointage(700); break;
                case "Poire" : RefScene.GetComponent<GestionScene>().DonnerPointage(900); break;
            }

            // Jouer l'effet sonore du fruit
            RefSons.GetComponent<GestionSFX>().GererSons("Food");

            // Désactiver le fruit touché
            trigger.gameObject.SetActive(false);
        }

        // Si le trigger est un téléporteur,
        if (trigger.tag == "Teleporteur")
        {
            // Téléporter l'entité selon la direction du téléporteur
            switch (trigger.name)
            {
                case "Teleporteur Gauche": TeleporterEntite(11f); break;
                case "Teleporteur Droit": TeleporterEntite(-11f); break;
            }
        }

        // Si le trigger est un fantôme,
        if (trigger.tag == "Fantome")
        {
            // Si Pacman est invulnérable,
            if (estInvulnerable)
            {
                // Augmenter le combo
                combo++;

                // Modifier la position du clone sur le Fantôme
                RefParticules.transform.position = trigger.transform.position;

                // Assigner le trigger comme fantôme touché
                fantomeTouche = trigger.gameObject;

                // Trouver l'enveloppe bleue du fantôme touché
                fantomeVulnerable = GameObject.Find("Vulnerable(Clone)(" + trigger.name + ")");

                // Retourner le fantôme à la case principale
                fantomeTouche.GetComponent<DeplacementFantomes>().RetourPrincipal();

                // Retourner l'enveloppe bleue à la case principale
                fantomeVulnerable.transform.position = fantomeTouche.transform.position;

                // Ajouter le fantôme touché à la liste
                listeTouches.Add(fantomeTouche);

                // Ajouter le fantôme bleu à la liste
                listeTouches.Add(fantomeVulnerable);

                // Désactiver les fantômes touchés
                AffecterFantomesTouches(false);

                // Jouer le particule du fantôme
                RefParticules.GetComponent<ParticleSystem>().Play();

                // Jouer l'effet sonore du fantôme avalé
                RefSons.GetComponent<GestionSFX>().GererSons("Ghost_Eat");

                // Donner 200 points
                RefScene.GetComponent<GestionScene>().DonnerPointage(200 * combo);
            }

            // Sinon,
            else
            {
                // Donner un tag au fantôme touché
                trigger.tag = "Fantome Touche";

                // Trouver tous les fantomes bleus
                GameObject[] listeBleus = GameObject.FindGameObjectsWithTag("Fantome Bleu");

                // Pour chaque bleu de la liste des fantômes bleus, Ajouter ce bleu à la liste
                foreach (GameObject Bleu in listeBleus) listeTouches.Add(Bleu);

                // Désactiver les fantômes touchés
                AffecterFantomesTouches(false);

                // Jouer les particules de mort
                ParticulesMort.Play();

                // Perdre une vie
                RefScene.GetComponent<GestionScene>().GestionVie("Perdre Vie");
            }
        }
    }

    IEnumerator RendreInvulnerable(int tempsAjoute)
    {
        // Rendre la vulnérabilité à vrai
        estInvulnerable = true;

        // Attendre un certain temps
        yield return new WaitForSeconds(0.1f);

        // Arrêter les effets sonores
        RefSons.GetComponent<GestionSFX>().ArreterSons();

        // Jouer l'effet sonore de l'invincibilité
        RefSons.GetComponent<GestionSFX>().GererSons("Invincibility");

        // Ajouter du temps d'invulnérabilité
        tempsInvulnerabilite += tempsAjoute;

        // Si le temps n'est pas en train de se réduire,
        if (!tempsReduit)
        {
            // Le temps se réduit
            tempsReduit = true;

            // Réduire le temps après une seconde
            Invoke("ReduireTemps", 1);
        }

        // Sinon,
        else
        {
            // Ajuster le temps d'invulnérabilité
            tempsInvulnerabilite = tempsAjoute;
        }
    }

    void ReduireTemps()
    {
        // S'il reste du temps d'invulnérabilité,
        if (tempsInvulnerabilite > 0)
        {
            // Baisser le temps
            tempsInvulnerabilite--;

            // Réduire le temps après une seconde
            Invoke("ReduireTemps", 1);
        }

        // Sinon,
        else
        {
            // Le temps n'est plus réduit
            tempsReduit = false;

            // Enlever l'invulnérabilité
            EnleverInulnerabilite();
        }
    }

    void EnleverInulnerabilite()
    {
        // Réinitialiser le combo
        combo = 0;

        // Arrêter les effets sonores
        RefSons.GetComponent<GestionSFX>().ArreterSons();

        // Activer les fantomes touchés
        AffecterFantomesTouches(true);

        // Enlever les éléments touchés de la liste
        listeTouches.Clear();

        // Rendre la vulnérabilité à faux
        estInvulnerable = false;
    }

    void TeleporterEntite(float PosX)
    {
        // Donner une position au téléporteur
        Vector3 Teleporteur = new Vector3(PosX, transform.position.y, transform.position.z);

        // Téléporter l'entité à la position déterminée
        gameObject.transform.position = Teleporteur;
    }

    void AffecterFantomesTouches(bool active)
    {
        // Pour tous les fantômes touchés, Activer/Désactiver ce fantôme
        foreach (GameObject Touche in listeTouches) Touche.SetActive(active);
    }
}