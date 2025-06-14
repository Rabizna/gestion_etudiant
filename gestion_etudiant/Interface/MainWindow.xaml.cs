using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using gestion_etudiant.Models;
using gestion_etudiant.Services;

namespace gestion_etudiant
{
    public partial class MainWindow : Window
    {
        private DatabaseService databaseService;
        private List<Etudiant> etudiants;
        private Etudiant etudiantSelectionne;
        private bool modeEdition = false;
        private bool modeCreation = false;

        public MainWindow()
        {
            InitializeComponent();
            InitialiserApplication();
        }

        private void InitialiserApplication()
        {
            try
            {
                // Initialiser le service de base de données
                databaseService = new DatabaseService();
                
                // Tester la connexion
                if (databaseService.TesterConnexion())
                {
                    lblConnexion.Text = "Connexion BD : ✅ Connecté";
                    ChargerEtudiants();
                }
                else
                {
                    lblConnexion.Text = "Connexion BD : ❌ Erreur";
                    MessageBox.Show("Impossible de se connecter à la base de données. Vérifiez vos paramètres de connexion.", 
                                  "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation : {ex.Message}", 
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChargerEtudiants()
        {
            try
            {
                etudiants = databaseService.ObtenirTousLesEtudiants();
                dgEtudiants.ItemsSource = etudiants;
                MettreAJourStatutNombre();
                lblStatut.Text = "Étudiants chargés avec succès";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des étudiants : {ex.Message}", 
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                lblStatut.Text = "Erreur lors du chargement";
            }
        }

        private void MettreAJourStatutNombre()
        {
            int nombre = etudiants?.Count ?? 0;
            lblNombreEtudiants.Text = $"{nombre} étudiant(s)";
        }

        private void dgEtudiants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            etudiantSelectionne = dgEtudiants.SelectedItem as Etudiant;
            
            if (etudiantSelectionne != null && !modeEdition)
            {
                AfficherEtudiant(etudiantSelectionne);
                btnModifier.IsEnabled = true;
                btnSupprimer.IsEnabled = true;
            }
            else if (!modeEdition)
            {
                ViderFormulaire();
                btnModifier.IsEnabled = false;
                btnSupprimer.IsEnabled = false;
            }
        }

        private void AfficherEtudiant(Etudiant etudiant)
        {
            txtNom.Text = etudiant.Nom;
            txtPrenom.Text = etudiant.Prenom;
            txtEmail.Text = etudiant.Email;
            dpDateNaissance.SelectedDate = etudiant.DateNaissance;
            txtTelephone.Text = etudiant.Telephone;
            txtAdresse.Text = etudiant.Adresse;
        }

        private void ViderFormulaire()
        {
            txtNom.Clear();
            txtPrenom.Clear();
            txtEmail.Clear();
            dpDateNaissance.SelectedDate = null;
            txtTelephone.Clear();
            txtAdresse.Clear();
        }

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            ViderFormulaire();
            ActiverModeEdition(true);
            modeCreation = true;
            lblStatut.Text = "Mode création - Saisissez les informations du nouvel étudiant";
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (etudiantSelectionne != null)
            {
                ActiverModeEdition(false);
                modeCreation = false;
                lblStatut.Text = "Mode modification - Modifiez les informations de l'étudiant";
            }
        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (etudiantSelectionne != null)
            {
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'étudiant {etudiantSelectionne.NomComplet} ?",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (databaseService.SupprimerEtudiant(etudiantSelectionne.Id))
                        {
                            ChargerEtudiants();
                            ViderFormulaire();
                            lblStatut.Text = "Étudiant supprimé avec succès";
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression de l'étudiant.", 
                                          "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", 
                                      "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (ValiderFormulaire())
            {
                try
                {
                    var etudiant = CreerEtudiantDepuisFormulaire();
                    bool succes = false;

                    if (modeCreation)
                    {
                        succes = databaseService.CreerEtudiant(etudiant);
                    }
                    else
                    {
                        etudiant.Id = etudiantSelectionne.Id;
                        succes = databaseService.MettreAJourEtudiant(etudiant);
                    }

                    if (succes)
                    {
                        ChargerEtudiants();
                        DesactiverModeEdition();
                        lblStatut.Text = modeCreation ? "Étudiant créé avec succès" : "Étudiant mis à jour avec succès";
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'enregistrement de l'étudiant.", 
                                      "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", 
                                  "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            DesactiverModeEdition();
            
            if (etudiantSelectionne != null && !modeCreation)
            {
                AfficherEtudiant(etudiantSelectionne);
            }
            else
            {
                ViderFormulaire();
            }
            
            lblStatut.Text = "Opération annulée";
        }

        private void btnRechercher_Click(object sender, RoutedEventArgs e)
        {
            EffectuerRecherche();
        }

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtRecherche.Text.Length == 0)
            {
                ChargerEtudiants();
            }
        }

        private void btnActualiser_Click(object sender, RoutedEventArgs e)
        {
            txtRecherche.Clear();
            ChargerEtudiants();
        }

        private void EffectuerRecherche()
        {
            try
            {
                string terme = txtRecherche.Text.Trim();
                
                if (string.IsNullOrEmpty(terme))
                {
                    ChargerEtudiants();
                    return;
                }

                var resultats = databaseService.RechercherEtudiants(terme);
                dgEtudiants.ItemsSource = resultats;
                etudiants = resultats;
                MettreAJourStatutNombre();
                lblStatut.Text = $"Recherche effectuée : {resultats.Count} résultat(s) trouvé(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche : {ex.Message}", 
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValiderFormulaire()
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text))
            {
                MessageBox.Show("Le nom est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNom.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                MessageBox.Show("Le prénom est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrenom.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("L'email est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validation basique de l'email
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Format d'email invalide.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private Etudiant CreerEtudiantDepuisFormulaire()
        {
            return new Etudiant
            {
                Nom = txtNom.Text.Trim(),
                Prenom = txtPrenom.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                DateNaissance = dpDateNaissance.SelectedDate,
                Telephone = string.IsNullOrWhiteSpace(txtTelephone.Text) ? null : txtTelephone.Text.Trim(),
                Adresse = string.IsNullOrWhiteSpace(txtAdresse.Text) ? null : txtAdresse.Text.Trim()
            };
        }

        private void ActiverModeEdition(bool creation)
        {
            modeEdition = true;
            modeCreation = creation;

            // Désactiver les boutons de navigation
            btnNouveau.IsEnabled = false;
            btnModifier.IsEnabled = false;
            btnSupprimer.IsEnabled = false;
            btnRechercher.IsEnabled = false;
            btnActualiser.IsEnabled = false;
            dgEtudiants.IsEnabled = false;
            txtRecherche.IsEnabled = false;

            // Activer les boutons de formulaire
            btnEnregistrer.IsEnabled = true;
            btnAnnuler.IsEnabled = true;

            // Activer les champs de saisie
            txtNom.IsEnabled = true;
            txtPrenom.IsEnabled = true;
            txtEmail.IsEnabled = true;
            dpDateNaissance.IsEnabled = true;
            txtTelephone.IsEnabled = true;
            txtAdresse.IsEnabled = true;

            // Focus sur le premier champ
            txtNom.Focus();
        }

        private void DesactiverModeEdition()
        {
            modeEdition = false;
            modeCreation = false;

            // Réactiver les boutons de navigation
            btnNouveau.IsEnabled = true;
            btnRechercher.IsEnabled = true;
            btnActualiser.IsEnabled = true;
            dgEtudiants.IsEnabled = true;
            txtRecherche.IsEnabled = true;

            // Désactiver les boutons de formulaire
            btnEnregistrer.IsEnabled = false;
            btnAnnuler.IsEnabled = false;

            // Désactiver les champs de saisie
            txtNom.IsEnabled = false;
            txtPrenom.IsEnabled = false;
            txtEmail.IsEnabled = false;
            dpDateNaissance.IsEnabled = false;
            txtTelephone.IsEnabled = false;
            txtAdresse.IsEnabled = false;

            // Réactiver les boutons selon la sélection
            if (etudiantSelectionne != null)
            {
                btnModifier.IsEnabled = true;
                btnSupprimer.IsEnabled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Désactiver les champs au démarrage
            DesactiverModeEdition();
        }
    }
}