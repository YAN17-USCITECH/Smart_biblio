using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SmartLibrary.Models;

namespace SmartLibrary.DAO
{
    public class EmpruntDAO
    {
        private DatabaseConnection _dbConnection;

        public EmpruntDAO()
        {
            _dbConnection = DatabaseConnection.GetInstance();
        }

        // CREATE - Ajouter un emprunt
        public bool Ajouter(Emprunt emprunt)
        {
            try
            {
                using (MySqlConnection conn = _dbConnection.GetConnection())
                {
                    string query = "INSERT INTO emprunts (id_abonne, id_livre, date_emprunt, date_retour_prevu, date_retour_reel, retourne) " +
                                   "VALUES (@idAbonne, @idLivre, @dateEmprunt, @dateRetourPrevu, @dateRetourReel, @retourne)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idAbonne", emprunt.IdAbonne);
                        cmd.Parameters.AddWithValue("@idLivre", emprunt.IdLivre);
                        cmd.Parameters.AddWithValue("@dateEmprunt", emprunt.DateEmprunt);
                        cmd.Parameters.AddWithValue("@dateRetourPrevu", emprunt.DateRetourPrevu);
                        cmd.Parameters.AddWithValue("@dateRetourReel", emprunt.DateRetourReel ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@retourne", emprunt.Retourne ? 1 : 0);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'emprunt: {ex.Message}");
                return false;
            }
        }

        // READ - Récupérer tous les emprunts
        public List<Emprunt> ObtenirTous()
        {
            List<Emprunt> emprunts = new List<Emprunt>();
            try
            {
                using (MySqlConnection conn = _dbConnection.GetConnection())
                {
                    string query = "SELECT id, id_abonne, id_livre, date_emprunt, date_retour_prevu, date_retour_reel, retourne FROM emprunts";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Emprunt emprunt = new Emprunt(
                                    Convert.ToInt32(reader["id"]),
                                    Convert.ToInt32(reader["id_abonne"]),
                                    Convert.ToInt32(reader["id_livre"]),
                                    Convert.ToDateTime(reader["date_emprunt"]),
                                    Convert.ToDateTime(reader["date_retour_prevu"]),
                                    reader["date_retour_reel"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["date_retour_reel"]),
                                    Convert.ToBoolean(reader["retourne"])
                                );
                                emprunts.Add(emprunt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des emprunts: {ex.Message}");
            }
            return emprunts;
        }

        // READ - Récupérer un emprunt par ID
        public Emprunt ObtenirParId(int id)
        {
            try
            {
                using (MySqlConnection conn = _dbConnection.GetConnection())
                {
                    string query = "SELECT id, id_abonne, id_livre, date_emprunt, date_retour_prevu, date_retour_reel, retourne FROM emprunts WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Emprunt(
                                    Convert.ToInt32(reader["id")],
                                    Convert.ToInt32(reader["id_abonne"]),
                                    Convert.ToInt32(reader["id_livre"]),
                                    Convert.ToDateTime(reader["date_emprunt"]),
                                    Convert.ToDateTime(reader["date_retour_prevu"]),
                                    reader["date_retour_reel"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["date_retour_reel"]),
                                    Convert.ToBoolean(reader["retourne"])
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de l'emprunt: {ex.Message}");
            }
            return null;
        }

        // UPDATE - Modifier un emprunt
        public bool Modifier(Emprunt emprunt)
        {
            try
            {
                using (MySqlConnection conn = _dbConnection.GetConnection())
                {
                    string query = "UPDATE emprunts SET id_abonne = @idAbonne, id_livre = @idLivre, " +
                                   "date_emprunt = @dateEmprunt, date_retour_prevu = @dateRetourPrevu, " +
                                   "date_retour_reel = @dateRetourReel, retourne = @retourne WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", emprunt.Id);
                        cmd.Parameters.AddWithValue("@idAbonne", emprunt.IdAbonne);
                        cmd.Parameters.AddWithValue("@idLivre", emprunt.IdLivre);
                        cmd.Parameters.AddWithValue("@dateEmprunt", emprunt.DateEmprunt);
                        cmd.Parameters.AddWithValue("@dateRetourPrevu", emprunt.DateRetourPrevu);
                        cmd.Parameters.AddWithValue("@dateRetourReel", emprunt.DateRetourReel ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@retourne", emprunt.Retourne ? 1 : 0);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification de l'emprunt: {ex.Message}");
                return false;
            }
        }

        // READ - Récupérer les emprunts non retournés d'un abonné
        public List<Emprunt> ObtenirEmpruntNonRetournes(int idAbonne)
        {
            List<Emprunt> emprunts = new List<Emprunt>();
            try
            {
                using (MySqlConnection conn = _dbConnection.GetConnection())
                {
                    string query = "SELECT id, id_abonne, id_livre, date_emprunt, date_retour_prevu, date_retour_reel, retourne FROM emprunts WHERE id_abonne = @idAbonne AND retourne = 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idAbonne", idAbonne);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Emprunt emprunt = new Emprunt(
                                    Convert.ToInt32(reader["id"]),
                                    Convert.ToInt32(reader["id_abonne"]),
                                    Convert.ToInt32(reader["id_livre"]),
                                    Convert.ToDateTime(reader["date_emprunt"]),
                                    Convert.ToDateTime(reader["date_retour_prevu"]),
                                    reader["date_retour_reel"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["date_retour_reel"]),
                                    Convert.ToBoolean(reader["retourne"])
                                );
                                emprunts.Add(emprunt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des emprunts: {ex.Message}");
            }
            return emprunts;
        }

        // Décrémenter le nombre disponible du livre
        public bool DecrementerDisponibilite(int idLivre)
        {
            try
            {
                using (MySqlConnection conn = _dbConnection.GetConnection())
                {
                    string query = "UPDATE livres SET nombre_disponibles = nombre_disponibles - 1 WHERE id = @id AND nombre_disponibles > 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idLivre);
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la décrémentation: {ex.Message}");
                return false;
            }
        }
    }
}