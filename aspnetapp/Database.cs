using aspnetapp.Models;
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
        /*
        private static readonly string SavePath = Path.Combine(Environment.CurrentDirectory, "database.save");

        public static readonly Dictionary<Guid, DbLecturer> lectuerers = new Dictionary<Guid, DbLecturer>();
        public static readonly List<Lecturer.Tag> tags = new List<Lecturer.Tag>();

        public static void Init()
        {
            if (!File.Exists(SavePath) || File.ReadAllBytes(SavePath).Length < 1)
                save();

            using (SaveReader reader = new SaveReader(SavePath))
            {
                loadLecturers(reader);
                loadTags(reader);
            }
        }

        private static void save()
        {
            return; // fuck you
            using (SaveWriter writer = new SaveWriter(SavePath, true))
            {
                saveLecturers(writer);
                saveTags(writer);
                writer.Flush();
            }
        }

        private static void saveLecturers(SaveWriter writer)
        {
            writer.WriteInt32(lectuerers.Count);

            foreach (KeyValuePair<Guid, DbLecturer> item in lectuerers)
            {
                DbLecturer lec = item.Value;

                writer.WriteBytes(item.Key.ToByteArray());
                writer.WriteStringNullable(lec.title_before);
                writer.WriteString(lec.first_name);
                writer.WriteStringNullable(lec.middle_name);
                writer.WriteString(lec.last_name);
                writer.WriteStringNullable(lec.title_after);
                writer.WriteStringNullable(lec.picture_url);
                writer.WriteStringNullable(lec.location);
                writer.WriteStringNullable(lec.claim);
                writer.WriteStringNullable(lec.bio);
                writer.WriteString(lec.tags);
                writer.WriteUInt32Nullable(lec.price_per_hour);
                writer.WriteString(lec.contact);
            }
        }
        private static void saveTags(SaveWriter writer)
        {
            writer.WriteInt32(tags.Count);

            foreach (Lecturer.Tag tag in tags)
            {
                writer.WriteBytes(tag.uuid.ToByteArray());
                writer.WriteString(tag.name);
            }
        }
        private static void loadLecturers(SaveReader reader)
        {
            lectuerers.Clear();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                Guid uuid = new Guid(reader.ReadBytes(16));
                lectuerers.Add(uuid, new DbLecturer()
                {
                    UUID = uuid,
                    title_before = reader.ReadStringNullable(),
                    first_name = reader.ReadString(),
                    middle_name = reader.ReadStringNullable(),
                    last_name = reader.ReadString(),
                    title_after = reader.ReadStringNullable(),
                    picture_url = reader.ReadStringNullable(),
                    location = reader.ReadStringNullable(),
                    claim = reader.ReadStringNullable(),
                    bio = reader.ReadStringNullable(),
                    tags = reader.ReadString(),
                    price_per_hour = reader.ReadUInt32Nullable(),
                    contact = reader.ReadString(),
                });
            }
        }
        private static void loadTags(SaveReader reader)
        {
            tags.Clear();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                tags.Add(new Lecturer.Tag()
                {
                    uuid = new Guid(reader.ReadBytes(16)),
                    name = reader.ReadString(),
                });
        }

        public static void AddLectuer(DbLecturer lecturer)
        {
            lectuerers.Add(lecturer.UUID, lecturer);
            save();
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
            bool val = lectuerers.Remove(uuid);
            save();
            return val;
        }

        public static void AddTag(Lecturer.Tag tag)
        {
            tags.Add(tag);
            save();
        }

        public static bool ContainsTag(Lecturer.Tag tag)
            => tags.Contains(tag);
        */
        /*public static void Init()
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

        public static void AddLectuer(DbLecturer lecturer)
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
            => tags.Contains(tag);*/
    }
}
