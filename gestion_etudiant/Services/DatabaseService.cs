using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using gestion_etudiant.Models;

namespace gestion_etudiant.Services
{
    public class DatabaseService
    {
        private readonly string connectionString;

        public DatabaseService()
        {
            // Modifiez cette chaîne de connexion selon votre configuration
            connectionString = "Host=localhost;Database=etudiant_db;Username=localhost;Password=MOODkyle35";
        }

        public DatabaseService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        // Créer un étudiant
        public bool CreerEtudiant(Etudiant etudiant)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string query = @"INSERT INTO etudiants (nom, prenom, email, date_naissance, telephone, adresse) 
                                   VALUES (@nom, @prenom, @email, @date_naissance, @telephone, @adresse)";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nom", etudiant.Nom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@prenom", etudiant.Prenom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@email", etudiant.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@date_naissance", etudiant.DateNaissance ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@telephone", etudiant.Telephone ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@adresse", etudiant.Adresse ?? (object)DBNull.Value);

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de l'étudiant : {ex.Message}");
            }
        }

        // Lire tous les étudiants
        public List<Etudiant> ObtenirTousLesEtudiants()
        {
            List<Etudiant> etudiants = new List<Etudiant>();

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM etudiants ORDER BY nom, prenom";

                    using (var command = new NpgsqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            etudiants.Add(MapperEtudiant(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des étudiants : {ex.Message}");
            }

            return etudiants;
        }

        // Lire un étudiant par ID
        public Etudiant ObtenirEtudiantParId(int id)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM etudiants WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapperEtudiant(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'étudiant : {ex.Message}");
            }

            return null;
        }

        // Mettre à jour un étudiant
        public bool MettreAJourEtudiant(Etudiant etudiant)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string query = @"UPDATE etudiants SET 
                                   nom = @nom, 
                                   prenom = @prenom, 
                                   email = @email, 
                                   date_naissance = @date_naissance, 
                                   telephone = @telephone, 
                                   adresse = @adresse 
                                   WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", etudiant.Id);
                        command.Parameters.AddWithValue("@nom", etudiant.Nom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@prenom", etudiant.Prenom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@email", etudiant.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@date_naissance", etudiant.DateNaissance ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@telephone", etudiant.Telephone ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@adresse", etudiant.Adresse ?? (object)DBNull.Value);

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de l'étudiant : {ex.Message}");
            }
        }

        // Supprimer un étudiant
        public bool SupprimerEtudiant(int id)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM etudiants WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de l'étudiant : {ex.Message}");
            }
        }

        // Rechercher des étudiants
        public List<Etudiant> RechercherEtudiants(string terme)
        {
            List<Etudiant> etudiants = new List<Etudiant>();

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string query = @"SELECT * FROM etudiants 
                                   WHERE LOWER(nom) LIKE LOWER(@terme) 
                                   OR LOWER(prenom) LIKE LOWER(@terme) 
                                   OR LOWER(email) LIKE LOWER(@terme)
                                   ORDER BY nom, prenom";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@terme", $"%{terme}%");
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                etudiants.Add(MapperEtudiant(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la recherche d'étudiants : {ex.Message}");
            }

            return etudiants;
        }

        // Tester la connexion
        public bool TesterConnexion()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // Correction du mapping dans la méthode MapperEtudiant
        private Etudiant MapperEtudiant(NpgsqlDataReader reader)
        {
            return new Etudiant
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nom = reader.IsDBNull(reader.GetOrdinal("nom")) ? null : reader.GetString(reader.GetOrdinal("nom")),
                Prenom = reader.IsDBNull(reader.GetOrdinal("prenom")) ? null : reader.GetString(reader.GetOrdinal("prenom")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                DateNaissance = reader.IsDBNull(reader.GetOrdinal("date_naissance")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("date_naissance")),
                Telephone = reader.IsDBNull(reader.GetOrdinal("telephone")) ? null : reader.GetString(reader.GetOrdinal("telephone")),
                Adresse = reader.IsDBNull(reader.GetOrdinal("adresse")) ? null : reader.GetString(reader.GetOrdinal("adresse")),
                DateCreation = reader.GetDateTime(reader.GetOrdinal("date_creation"))
            };
        }
    }
}