using aspnetapp.Auth;
using aspnetapp.Models;
using aspnetapp.Models.Schedule;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace aspnetapp
{
    public static class Database
    {
        internal static readonly string SavePath = Path.Combine(Environment.CurrentDirectory, "database.save");

        public static readonly Dictionary<Guid, DbLecturer> lectuerers = new Dictionary<Guid, DbLecturer>();
        public static readonly List<Lecturer.Tag> tags = new List<Lecturer.Tag>();

        internal static Dictionary<uint, Account> accounts = new Dictionary<uint, Account>();
        internal static List<LoginSession> sessions = new List<LoginSession>();
        internal static Dictionary<string, uint> emailVerifications = new Dictionary<string, uint>();

        internal static string emailPass;

        // lecturer id -> lecturer's subjects
        internal static Dictionary<Guid, List<Subject>> subjects = new Dictionary<Guid, List<Subject>>();

        public static void AddLectuer(Lecturer lecturer)
        {
            lock (lectuerers)
                lectuerers[lecturer.UUID] = lecturer;
        }

        public static Lecturer GetLecturer(Guid uuid)
            => lectuerers[uuid];

        public static bool TryGetLecturer(Guid uuid, out Lecturer lecturer)
        {
            bool val = lectuerers.TryGetValue(uuid, out DbLecturer _lecturer);
            if (val)
                lecturer = _lecturer;
            else
                lecturer = null;

            return val;
        }

        public static bool ContainsLecturer(Guid uuid)
            => lectuerers.ContainsKey(uuid);

        public static bool Remove(Guid uuid)
        {
            lock (lectuerers)
                return lectuerers.Remove(uuid);
        }

        public static void AddTag(Lecturer.Tag tag)
        {
            lock (tags)
                tags.Add(tag);
        }

        public static bool ContainsTag(Lecturer.Tag tag)
            => tags.Contains(tag);

        public static void Save()
        {
            try
            {
                using SaveWriter writer = new SaveWriter(SavePath, true);

                lock (lectuerers)
                    saveLecturers(writer);
                lock (tags)
                    saveTags(writer);
                lock (accounts)
                    saveAccounts(writer);
                lock (sessions)
                    saveSessions(writer);
                writer.WriteString(emailPass);

                lock (emailVerifications)
                    writer.WriteList(emailVerifications.ToList());

                writer.Flush();

                Log.Info("Saved database");
            }
            catch (Exception ex)
            {
                Log.Error("There was an error saving:");
                Log.Exception(ex);
                throw;
            }
        }
        public static void Load()
        {
            if (!File.Exists(SavePath))
            {
                lock (lectuerers)
                    lectuerers.Clear();
                lock (tags)
                    tags.Clear();
                lock (accounts)
                    accounts.Clear();
                lock (sessions)
                    sessions.Clear();
                emailPass = "";
                lock (emailVerifications)
                    emailVerifications.Clear();

                Save();
            }
            try
            {
                using SaveReader reader = new SaveReader(SavePath);

                lock (lectuerers)
                    loadLecturers(reader);
                lock (tags)
                    loadTags(reader);
                lock (accounts)
                    loadAccounts(reader);
                lock (sessions)
                    loadSessions(reader);
                emailPass = reader.ReadString();

                lock (emailVerifications)
                    emailVerifications = reader.ReadList<KeyValuePair<string, uint>>().ToDictionary(item => item.Key, item => item.Value);

                Log.Info("Loaded database");
            }
            catch (Exception ex)
            {
                Log.Error("There was an error loading:");
                Log.Exception(ex);
                throw;
            }
        }

        private static void saveLecturers(SaveWriter writer)
        {
            writer.WriteInt32(lectuerers.Count);

            foreach (KeyValuePair<Guid, DbLecturer> item in lectuerers)
            {
                DbLecturer lec = item.Value;

                writer.WriteBytes(item.Key.ToByteArray());
                writer.WriteString(lec.title_before);
                writer.WriteString(lec.first_name);
                writer.WriteString(lec.middle_name);
                writer.WriteString(lec.last_name);
                writer.WriteString(lec.title_after);
                writer.WriteString(lec.picture_url);
                writer.WriteString(lec.location);
                writer.WriteString(lec.claim);
                writer.WriteString(lec.bio);
                writer.WriteString(lec.tags);
                writer.WriteUInt32Null(lec.price_per_hour);
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
                    title_before = reader.ReadString(),
                    first_name = reader.ReadString(),
                    middle_name = reader.ReadString(),
                    last_name = reader.ReadString(),
                    title_after = reader.ReadString(),
                    picture_url = reader.ReadString(),
                    location = reader.ReadString(),
                    claim = reader.ReadString(),
                    bio = reader.ReadString(),
                    tags = reader.ReadString(),
                    price_per_hour = reader.ReadUInt32Null(),
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
        private static void saveAccounts(SaveWriter writer)
        {
            writer.WriteInt32(accounts.Count);
            foreach (var item in accounts)
            {
                writer.WriteUInt32(item.Key);
                item.Value.Save(writer);
            }
        }
        private static void loadAccounts(SaveReader reader)
        {
            accounts.Clear();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                accounts.Add(reader.ReadUInt32(), Account.Load(reader));
        }
        private static void saveSessions(SaveWriter writer)
        {
            writer.WriteInt32(sessions.Count);

            foreach (LoginSession session in sessions)
                session.Save(writer);
        }
        private static void loadSessions(SaveReader reader)
        {
            sessions.Clear();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                sessions.Add(LoginSession.Load(reader));
        }
    }
}
