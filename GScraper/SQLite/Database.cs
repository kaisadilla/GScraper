using Kaisa.GScraper.SQLite.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Kaisa.GScraper.SQLite {
    public static class Database {
        private static SQLiteConnection dbConn;

        public static void Initialize () {
            dbConn = new SQLiteConnection("Data Source=series.sqlite;Version=3");
        }

        public static void CreateDatabase () {
            SQLiteConnection.CreateFile("series.sqlite");
            Initialize();
            CreateTables();
        }

        private static void CreateTables () {
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

        public static void InsertSeries (string internalName, string displayName, string imgPath) {
            string sql = $"insert into series values ('{internalName}', '{displayName}', '{imgPath}', '0', '-1');";
            QueryInsert(sql);
        }

        public static void InsertEpisode (string fullId, string series, int seasonIndex, int episodeIndex, string name, string date, string imgPath) {
            string sql = $"insert into episodes values ('{fullId}', '{series}', '{seasonIndex}', '{episodeIndex}', '{name}', '{date}', '{imgPath}', '-1');";
            QueryInsert(sql);
        }

        public static void InsertLink (string url, string episode, int linkType, string hostName, string quality, string language) {
            string sql = $"insert into links values ('{url}', '{episode}', '{linkType}', '{hostName}', '{quality}', '{language}', '0', '0');";
            QueryInsert(sql);
        }

        private static void QueryInsert (string query) {
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

        #region ParseReaderRows
        private static Series ParseReaderSeries(SQLiteDataReader reader) {
            string internalName = (string)reader["internalName"];
            string displayName = (string)reader["displayName"];
            string imgPath = (string)reader["imgPath"];
            bool favorite = (int)reader["favorite"] != 0;
            float rating = (float)(double)reader["rating"]; // you can't cast System.Double to float directly.
            return new Series(internalName, displayName, imgPath, favorite, rating);
        }
        private static Episode ParseReaderEpisode(SQLiteDataReader reader) {
            string fullId = (string)reader["fullId"];
            string series = (string)reader["series"];
            int seasonIndex = (int)reader["seasonIndex"];
            int episodeIndex = (int)reader["episodeIndex"];
            string name = (string)reader["name"];
            string date = (string)reader["date"];
            string imgPath = (string)reader["imgPath"];
            float rating = (float)(double)reader["rating"];

            return new Episode(fullId, series, seasonIndex, episodeIndex, name, date, imgPath, rating);
        }
        private static Link ParseReaderLink(SQLiteDataReader reader) {
            string url = (string)reader["url"];
            string episode = (string)reader["episode"];
            LinkType linkType = (LinkType)reader["linkType"];
            string hostName = (string)reader["hostName"];
            string quality = (string)reader["quality"];
            string language = (string)reader["language"];
            bool favorite = (int)reader["favorite"] != 0;
            bool badLink = (int)reader["badLink"] != 0;

            return new Link(url, episode, linkType, hostName, quality, language, favorite, badLink);
        }
        #endregion

        #region RetrieveTables
        public static Series[] RetrieveAllSeries () {
            dbConn.Open();
            string sql = $"select * from series";
            var comm = new SQLiteCommand(sql, dbConn);
            var reader = comm.ExecuteReader();
            var series = new List<Series>();

            while (reader.Read()) {
                series.Add(ParseReaderSeries(reader));
            }

            dbConn.Close();
            return series.ToArray();
        }

        #endregion

        #region RetrieveByQuery
        public static Series? RetrieveSeriesById (string id) {
            dbConn.Open();
            string sql = $"select * from series where internalName = '{id}'";
            var comm = new SQLiteCommand(sql, dbConn);
            var reader = comm.ExecuteReader();
            reader.Read();

            if (reader.HasRows) {
                var series = ParseReaderSeries(reader);
                dbConn.Close();
                return series;
            }
            else {
                dbConn.Close();
                return null;
            }
        }

        /// <summary>
        /// Retrieves all the series in the database where their display name contains any of the words (space separated) given in the query.
        /// </summary>
        /// <param name="query">The words to search for.</param>
        /// <returns></returns>
        public static Series[] RetrieveSeriesBySearch (string query) {
            if (string.IsNullOrEmpty(query)) return new Series[0];

            var series = new List<Series>();

            string[] keywords = query.Split(' ');
            string sql = $"select * from series where displayName like '%{keywords[0]}%'";
            for (int i = 1; i < keywords.Length; i++) {
                sql += $" or displayName like '%{keywords[1]}%'";
            }

            var comm = new SQLiteCommand(sql, dbConn);
            var reader = comm.ExecuteReader();
            reader.Read();


            while (reader.Read()) {
                series.Add(ParseReaderSeries(reader));
            }

            dbConn.Close();
            return series.ToArray();
        }

        public static Episode? RetrieveEpisodeById (string id) {
            dbConn.Open();
            string sql = $"select * from episodes where fullId = '{id}'";
            var comm = new SQLiteCommand(sql, dbConn);
            var reader = comm.ExecuteReader();
            reader.Read();

            if (reader.HasRows) {
                var episode = ParseReaderEpisode(reader);
                dbConn.Close();
                return episode;
            }
            else {
                dbConn.Close();
                return null;
            }
        }

        public static Episode[] RetrieveEpisodeBySeries (string seriesId) {
            dbConn.Open();
            string sql = $"select * from episodes where series = '{seriesId}'";
            var comm = new SQLiteCommand(sql, dbConn);
            var reader = comm.ExecuteReader();
            var episodes = new List<Episode>();
            while (reader.Read()) {
                episodes.Add(ParseReaderEpisode(reader));
            }

            dbConn.Close();
            return episodes.ToArray();
        }

        public static Link[] RetrieveLinksByEpisode (string episodeId) {
            dbConn.Open();
            string sql = $"select * from links where episode = '{episodeId}'";
            var comm = new SQLiteCommand(sql, dbConn);
            var reader = comm.ExecuteReader();
            var links = new List<Link>();

            while (reader.Read()) {
                links.Add(ParseReaderLink(reader));
            }

            dbConn.Close();
            return links.ToArray();
        }
        #endregion

        /// <summary>
        /// Builds the correct id for an episode given the internal name of the series it belongs to and its season and episode numbers.
        /// </summary>
        /// <param name="seriesId">The internal name of the series it belongs to.</param>
        /// <param name="seasonNumber">The number (index + 1) of the season.</param>
        /// <param name="episodeNumber">The number (index + 1) of the episode.</param>
        /// <returns></returns>
        public static string BuildEpisodeId(string seriesId, int seasonNumber, int episodeNumber) {
            return $"{seriesId}_{seasonNumber}_{episodeNumber}";
        }
    }
}
