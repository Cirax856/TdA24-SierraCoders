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