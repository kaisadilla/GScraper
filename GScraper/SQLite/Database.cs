using System;
using System.Data.SQLite;

namespace Kaisa.GScraper.SQLite {
    public static class Database {
        private static SQLiteConnection dbConn;

        public static void Initialize() {
            dbConn = new SQLiteConnection("Data Source=series.sqlite;Version=3");
        }

        public static void CreateDatabase() {
            SQLiteConnection.CreateFile("series.sqlite");
            Initialize();
            CreateTables();
        }

        private static void CreateTables() {
            dbConn.Open();
            var comm_movies = new SQLiteCommand(TableDefinitions.SQL_MOVIES, dbConn);
            var comm_series = new SQLiteCommand(TableDefinitions.SQL_SERIES, dbConn);
            var comm_episodes = new SQLiteCommand(TableDefinitions.SQL_EPISODES, dbConn);
            var comm_links = new SQLiteCommand(TableDefinitions.SQL_LINKS, dbConn);

            comm_movies.ExecuteNonQuery();
            comm_series.ExecuteNonQuery();
            comm_episodes.ExecuteNonQuery();
            comm_links.ExecuteNonQuery();

            dbConn.Close();
        }

        public static void InsertSeries(string internalName, string displayName, string imgPath) {
            string sql = $"insert into series values ('{internalName}', '{displayName}', '{imgPath}', '0', '-1');";
            QueryInsert(sql);
        }

        public static void InsertEpisode(string fullId, string series, int seasonIndex, int episodeIndex, string name, string date, string imgPath) {
            string sql = $"insert into episodes values ('{fullId}', '{series}', '{seasonIndex}', '{episodeIndex}', '{name}', '{date}', '{imgPath}', '-1');";
            QueryInsert(sql);
        }

        public static void InsertLink(string url, string episode, int linkType, string hostName, string quality, string language) {
            string sql = $"insert into links values ('{url}', '{episode}', '{linkType}', '{hostName}', '{quality}', '{language}', '0', '0');";
            QueryInsert(sql);
        }

        private static void QueryInsert(string query) {
            dbConn.Open();

            var comm = new SQLiteCommand(query, dbConn);
            try {
                comm.ExecuteNonQuery();
            }
            catch (SQLiteException ex) {
                Console.WriteLine($"Values not inserted in the database. Cause: {ex.Message}");
            }

            dbConn.Close();
        }
    }
}
