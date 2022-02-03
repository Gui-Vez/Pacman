using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GestionScene : MonoBehaviour
{
    /* ****************** */
    /* Gestion des scènes */
    /* ****************** */


    Scene sceneActuelle;
    public GameObject RefSons;
    public GameObject RefPacman;

    public GameObject[] listeIntroduction;
    public GameObject[] listePastilles;
    public GameObject[] listeFantomes;
    public GameObject[] listePacdots;
    public GameObject[] listeFruits;
    public GameObject[] listeVies;

    public List<GameObject> ListeObjets;
    public GameObject CanvasInstruction;
    public GameObject TextePreparation;
    public GameObject TexteFinPartie;
    public GameObject Felicitations;
    public GameObject Personnages;
    public GameObject Bordures;
    public GameObject Objets;
    public GameObject Scores;
    public GameObject Decor;
    public GameObject Vies;

    public Text pointageScore;
    public Text pointageHighScore;

    public static int score = 0;
    public static int highScore = 10000;
    public static int scoreVieExtra = 10000;
    public static int manche = 1;
    public static int nombreVies = 5;
    public static int nombreFruits = 5;
    public static int indexGhostSFX = 1;
    public static int nombrePastilles = 0;

    public static bool jeuCommence;
    public static bool partieTerminee;
    public static bool montrerInstructions;
    public static bool premierePartie = true;



    void Start()
    {
        // Trouver la scène actuelle
        sceneActuelle = SceneManager.GetActiveScene();

        // La partie n'est pas terminée
        partieTerminee = false;

        // Activer les vies
        Vies.SetActive(true);

        // Mettre à jour le score
        ActualiserScore();

        // Ajouter les sous-objets dans une liste:
        listeIntroduction = GameObject.FindGameObjectsWithTag("Menu Intro");
        listePastilles = GameObject.FindGameObjectsWithTag("Pastille");
        listePacdots = GameObject.FindGameObjectsWithTag("PacDot");
        listeFruits = GameObject.FindGameObjectsWithTag("Fruit");
        listeVies = GameObject.FindGameObjectsWithTag("Vie");

        // Ajouter les GameObjets dans une liste:
        ListeObjets.Add(Personnages);
        ListeObjets.Add(Bordures);
        ListeObjets.Add(Objets);
        ListeObjets.Add(Scores);

        ListeObjets.Add(Vies);

        // Donner des instructions aux scènes respectives
        switch (sceneActuelle.name)
        {
            case "Intro":

                // Pour tous les objets de la liste des GameObjets, Désactiver cet objet
                foreach (GameObject Objets in ListeObjets) Objets.SetActive(false);

                break;


            case "Jeu":

                // Pour tous les objets de la liste des GameObjets, Désactiver cet objet
                foreach (GameObject Objets in ListeObjets) Objets.SetActive(false);

                // Pour tous les sous-objets de la liste des objets, Affecter ce sous-objet
                foreach (GameObject Pastille in listePastilles) Pastille.SetActive(true);
                foreach (GameObject Pacdot in listePacdots) Pacdot.SetActive(true);
                foreach (GameObject Fruit in listeFruits) Fruit.SetActive(false);
                foreach (GameObject Vie in listeVies) Vie.SetActive(false);


                // Activer le texte de préparation
                TextePreparation.SetActive(true);

                // Désactiver le texte de félicitations
                Felicitations.SetActive(false);

                // Désactiver le texte de fin de partie
                TexteFinPartie.SetActive(false);

                // Jouer la partie après un certain délai
                StartCoroutine(JouerPartie(4.3f));

                break;
        }
    }

    public void AffichageMenu(string toggle)
    {
        // D'après l'action de l'utilisateur,
        switch (toggle)
        {
            case "Afficher":

                // Pour tous les éléments UI de la liste d'introduction, Désactiver cet objet
                foreach (GameObject element in listeIntroduction) element.SetActive(false);

                // Désactiver le décor
                Decor.SetActive(false);

                // Activer le menu d'instructions
                CanvasInstruction.SetActive(true);

                // Afficher les instructions
                montrerInstructions = true;

                break;


            case "Masquer":

                // Pour tous les éléments UI de la liste d'introduction, Activer cet objet
                foreach (GameObject element in listeIntroduction) element.SetActive(true);

                // Activer le décor
                Decor.SetActive(true);

                // Désactiver le menu d'instructions
                CanvasInstruction.SetActive(false);

                // Masquer les instructions
                montrerInstructions = false;

                break;
        }
    }


    public void CommencerPartie()
    {
        // Jouer l'effet sonore d'appui
        RefSons.GetComponent<GestionSFX>().GererSons("Press");

        // Charger la scène après un quart de seconde
        Invoke("ChargerScene", 0.25f);
    }

    void ChargerScene()
    {
        // Ouvrir la scène de jeu
        SceneManager.LoadScene("Jeu");
    }

    IEnumerator JouerPartie(float delai)
    {
        // Jouer la chanson d'intro
        RefSons.GetComponent<GestionSFX>().GererSons("Start");

        yield return new WaitForSeconds(delai);

        // Assigner un index d'effet sonore
        indexGhostSFX = manche % 5;

        // Si l'index est 0,
        if (indexGhostSFX == 0)
        {
            // Retourner l'index à 5
            indexGhostSFX = 5;
        }

        // Faire jouer l'effet sonore du fantôme
        StartCoroutine(RefSons.GetComponent<GestionSFX>().GhostSFX(indexGhostSFX));

        // Commencer le jeu
        jeuCommence = true;

        // Trouver le nombre de pastilles restantes
        nombrePastilles = listePastilles.Length;

        // Trouver le nombre de fruits
        nombreFruits = listeFruits.Length;

        // Désactiver le texte de préparation
        TextePreparation.SetActive(false);

        // Pour tous les objets de la liste des GameObjets, Activer cet objet
        foreach (GameObject Objets in ListeObjets) Objets.SetActive(true);

        // Mettre à jour la barre des vies
        ActualiserVies();

        // Ajouter un index de fruit (0 à 4)
        int indexFruit = (manche - 1) % nombreFruits;

        // Afficher le fruit
        StartCoroutine(AfficherFruit(indexFruit));
    }

    IEnumerator AfficherFruit(int fruit)
    {
        // Attendre 10 secondes
        yield return new WaitForSeconds(10f);

        // Activer un fruit choisi
        listeFruits[fruit].SetActive(true);
    }

    public void DonnerPointage(int pointage)
    {
        // S'il n'y a pas de score,
        if (score == 0)
        {
            // Donner une valeur à atteindre
            scoreVieExtra = 10000;
        }

        // Ajouter le pointage au score
        score += pointage;

        // Si le score atteint le pointage requis,
        if (score >= scoreVieExtra)
        {
            // Incrémenter le nombre de points à atteindre
            scoreVieExtra += 10000;

            // Gagner une vie
            GestionVie("Gagner Vie");
        }

        // Mettre à jour le score
        ActualiserScore();
    }

    void ActualiserScore()
    {
        // Si le score est suppérieur au highscore, Appliquer le score au highscore
        if (score > highScore) highScore = score;

        // Associer le texte du pointage au score actuel
        pointageScore.text = score.ToString();

        // Associer le texte du pointage au highscore actuel
        pointageHighScore.text = highScore.ToString();
    }

    void ActualiserVies()
    {
        // Pour chaque index du nombre de vie,
        for (int i = 0; i < nombreVies; i++)
        {
            // Activer le sprite de vie
            listeVies[i].SetActive(true);
        }

        // Pour chaque index à l'extérieur du nombre de vie,
        for (int i = listeVies.Length; i > nombreVies; i--)
        {
            // Désactiver le sprite de vie
            listeVies[i - 1].SetActive(false);
        }
    }

    public void GestionVie(string action)
    {
        // Selon l'action,
        switch (action)
        {
            case "Gagner Vie":

                // Si le nombre de vies est inférieur à 9, Incrémenter le nombre de vies
                if (nombreVies < 9) nombreVies++;

                // Jouer l'effet sonore de vie
                RefSons.GetComponent<GestionSFX>().GererSons("Life");

                break;


            case "Perdre Vie":

                // Si le nombre de vies est suppérieur à 0, Baisser le nombre de vies
                if (nombreVies > 0) nombreVies--;

                // Arrêter les effets sonores
                RefSons.GetComponent<GestionSFX>().ArreterSons();

                // Perdre une manche
                StartCoroutine(RecommencerManche("Perdue"));

                break;
        }

        // Mettre à jour la barre des vies
        ActualiserVies();
    }


    public IEnumerator RecommencerManche(string Manche)
    {
        // Si le jeu est commencé,
        if (jeuCommence)
        {
            // Le jeu n'est plus en cours
            jeuCommence = false;

            // Terminer la partie
            partieTerminee = true;

            // Dépendamment de la manche,
            switch (Manche)
            {
                case "Gagnée":

                    // Augmenter la numéro de manche
                    manche++;

                    // Désactiver les sous-objets
                    DesactiverSousObjets();

                    // Arrêter les effets sonores
                    RefSons.GetComponent<GestionSFX>().ArreterSons();

                    // Attendre deux seconde
                    yield return new WaitForSeconds(2f);

                    // Si le joueur a atteint la dernière manche, Gagner la partie
                    if (manche > 10) StartCoroutine(FinirPartie("Gagnée"));

                    // Sinon, Relancer la partie
                    else ChargerScene();

                    break;


                case "Perdue":

                    // Désactiver les sous-objets
                    DesactiverSousObjets();

                    // Attendre une seconde
                    yield return new WaitForSeconds(1f);

                    // Jouer l'animation de mort
                    RefPacman.GetComponent<Animator>().SetTrigger("Mort");

                    // Jouer l'effet sonore de mort
                    RefSons.GetComponent<GestionSFX>().GererSons("Death");

                    // Attendre deux secondes
                    yield return new WaitForSeconds(2f);

                    // S'il ne reste plus de vies, Perdre la partie
                    if (nombreVies == 0) StartCoroutine(FinirPartie("Perdue"));

                    // Sinon, Relancer la partie
                    else ChargerScene();

                    break;
            }
        }
    }

    void DesactiverSousObjets()
    {
        // Mettre les fantômes dans une liste
        listeFantomes = GameObject.FindGameObjectsWithTag("Fantome");

        // Pour tous les sous-objets de la liste des objets, Masquer ce sous-objet
        foreach (GameObject Pastille in listePastilles) Pastille.SetActive(false);
        foreach (GameObject Fantome in listeFantomes) Fantome.SetActive(false);
        foreach (GameObject Pacdot in listePacdots) Pacdot.SetActive(false);
        foreach (GameObject Fruit in listeFruits) Fruit.SetActive(false);
        foreach (GameObject Vie in listeVies) Vie.SetActive(false);
    }


    IEnumerator FinirPartie(string Partie)
    {
        // Pour tous les objets de la liste des GameObjets, Désactiver cet objet
        foreach (GameObject Objets in ListeObjets) Objets.SetActive(false);

        // Dépendamment de la partie,
        switch (Partie)
        {
            case "Gagnée":

                // Enlever le décor
                Decor.SetActive(false);

                // Activer le texte de félicitations
                Felicitations.SetActive(true);

                // Jouer la musique de fin
                RefSons.GetComponent<GestionSFX>().GererSons("Music");

                // Attendre quelques secondes
                yield return new WaitForSeconds(11f);

                break;


            case "Perdue":

                // Activer le texte de fin de partie
                TexteFinPartie.SetActive(true);

                // Attendre quelques secondes
                yield return new WaitForSeconds(3f);

                break;
        }

        // Recharger la scène d'intro
        SceneManager.LoadScene("Intro");

        // Remettre le score à 0
        score = 0;

        // Remettre la manche actuelle à 1
        manche = 1;

        // Remettre le nombre de vies à 5
        nombreVies = 5;
    }
}