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
        public static readonly Dictionary<Guid, Lecturer> lectuerers = new Dictionary<Guid, Lecturer>();
        public static readonly List<Lecturer.Tag> tags = new List<Lecturer.Tag>();

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
            => lectuerers.Add(lecturer.UUID, lecturer);

        public static Lecturer GetLecturer(Guid uuid)
            => lectuerers[uuid];

        public static bool TryGetLecturer(Guid uuid, out Lecturer lecturer)
            => lectuerers.TryGetValue(uuid, out lecturer);

        public static bool ContainsKey(Guid uuid)
            => lectuerers.ContainsKey(uuid);

        public static bool Remove(Guid uuid)
            => lectuerers.Remove(uuid);

        public static void AddTag(Lecturer.Tag tag)
            => tags.Add(tag);

        public static bool ContainsTag(Lecturer.Tag tag)
            => tags.Contains(tag);
    }
}
