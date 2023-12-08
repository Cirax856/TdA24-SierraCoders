using aspnetapp.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspnetapp
{
    public static class Database
    {
        public static readonly Dictionary<Guid, DbLecturer> lectuerers = new Dictionary<Guid, DbLecturer>();
        public static readonly List<Lecturer.Tag> tags = new List<Lecturer.Tag>();

        public static void Init()
        {
            runCommand(command =>
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Lecturers (
                        UUID TEXT PRIMARY KEY,
                        title_before TEXT,
                        first_name TEXT,
                        middle_name TEXT,
                        last_name TEXT,
                        title_after TEXT,
                        picture_url TEXT,
                        location TEXT,
                        claim TEXT,
                        bio TEXT,
                        tags TEXT,
                        price_per_hour INTEGER,
                        contact TEXT
                    );
                ";
                command.ExecuteNonQuery();
            });
            runCommand(command =>
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Tags (
                        UUID TEXT PRIMARY KEY,
                        name TEXT
                    );
                ";
                command.ExecuteNonQuery();
            });

            List<DbLecturer> _lecturers = new List<DbLecturer>();
            runCommand(command =>
            {
                command.CommandText = "SELECT * FROM Lecturers;";

                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        _lecturers.Add(new DbLecturer
                        {
                            UUID = Guid.Parse(reader["UUID"].ToString()),
                            title_before = reader["title_before"].ToString(),
                            first_name = reader["first_name"].ToString(),
                            middle_name = reader["middle_name"].ToString(),
                            last_name = reader["last_name"].ToString(),
                            title_after = reader["title_after"].ToString(),
                            picture_url = reader["picture_url"].ToString(),
                            location = reader["location"].ToString(),
                            claim = reader["claim"].ToString(),
                            bio = reader["bio"].ToString(),
                            tags = reader["tags"].ToString(),
                            price_per_hour = Convert.ToUInt32(reader["price_per_hour"]),
                            contact = reader["contact"].ToString(),
                        });
            });

            lectuerers.Clear();
            for (int i = 0; i < _lecturers.Count; i++)
                lectuerers.Add(_lecturers[i].UUID, _lecturers[i]);

            tags.Clear();
            runCommand(command =>
            {
                command.CommandText = "SELECT * FROM Tags;";

                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        tags.Add(new Lecturer.Tag
                        {
                            uuid = Guid.Parse(reader["UUID"].ToString()),
                            name = reader["name"].ToString(),
                        });
            });
        }

        private static SqliteConnection createConenction()
        {
            SqliteConnection connection = new SqliteConnection("Data Source=database.db");
            connection.Open();
            return connection;
        }
        private static void runCommand(Action<SqliteCommand> action)
        {
            SqliteConnection connection = createConenction();
            using (SqliteCommand command = connection.CreateCommand())
                action(command);
            closeConnection(connection);
        }
        private static void closeConnection(SqliteConnection connection)
        {
            connection.Close();
            connection.Dispose();
        }

        public static void AddLectuer(Lecturer lecturer)
        {
            runCommand(command =>
            {
                command.CommandText = @"
                    INSERT INTO Lecturers (
                        UUID, title_before, first_name, middle_name, last_name,
                        title_after, picture_url, location, claim, bio, tags,
                        price_per_hour, contact
                    ) VALUES (
                        @UUID, @title_before, @first_name, @middle_name, @last_name,
                        @title_after, @picture_url, @location, @claim, @bio, @tags,
                        @price_per_hour, @contact
                    );
                ";

                command.Parameters.AddWithValue("@UUID", lecturer.UUID.ToString());
                command.Parameters.AddWithValue("@title_before", lecturer.title_before == null ? DBNull.Value : lecturer.title_before);
                command.Parameters.AddWithValue("@first_name", lecturer.first_name);
                command.Parameters.AddWithValue("@middle_name", lecturer.middle_name == null ? DBNull.Value : lecturer.middle_name);
                command.Parameters.AddWithValue("@last_name", lecturer.last_name);
                command.Parameters.AddWithValue("@title_after", lecturer.title_after == null ? DBNull.Value : lecturer.title_after);
                command.Parameters.AddWithValue("@picture_url", lecturer.picture_url == null ? DBNull.Value : lecturer.picture_url);
                command.Parameters.AddWithValue("@location", lecturer.location == null ? DBNull.Value : lecturer.location);
                command.Parameters.AddWithValue("@claim", lecturer.claim == null ? DBNull.Value : lecturer.claim);
                command.Parameters.AddWithValue("@bio", lecturer.bio == null ? DBNull.Value : lecturer.bio);
                command.Parameters.AddWithValue("@tags", lecturer.tags);
                command.Parameters.AddWithValue("@price_per_hour", lecturer.price_per_hour.HasValue ? lecturer.price_per_hour : DBNull.Value);
                command.Parameters.AddWithValue("@contact", lecturer.contact);

                command.ExecuteNonQuery();
            });
            lectuerers.Add(lecturer.UUID, lecturer);
        }

        public static Lecturer GetLecturer(Guid uuid)
            => lectuerers[uuid];

        public static bool TryGetLecturer(Guid uuid, out Lecturer lecturer)
        {
            bool val = lectuerers.TryGetValue(uuid, out DbLecturer _lecturer);
            lecturer = _lecturer;
            return val;
        }

        public static bool ContainsKey(Guid uuid)
            => lectuerers.ContainsKey(uuid);

        public static bool Remove(Guid uuid)
        {
            runCommand(command =>
            {
                command.CommandText = "DELETE FROM Lecturers WHERE UUID = @UUID;";
                command.Parameters.AddWithValue("@UUID", uuid.ToString());

                command.ExecuteNonQuery();
            });
            return lectuerers.Remove(uuid);
        }

        public static void AddTag(Lecturer.Tag tag)
        {
            runCommand(command =>
            {
                command.CommandText = @"
                    INSERT INTO Tags (
                        UUID, name
                    ) VALUES (
                        @UUID, @name
                    );
                ";

                command.Parameters.AddWithValue("@UUID", tag.uuid.ToString());
                command.Parameters.AddWithValue("@name", tag.name);

                command.ExecuteNonQuery();
            });

            tags.Add(tag);
        }

        public static bool ContainsTag(Lecturer.Tag tag)
            => tags.Contains(tag);
    }
}
