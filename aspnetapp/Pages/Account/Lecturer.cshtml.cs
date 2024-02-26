using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Security.Principal;

namespace aspnetapp.Pages.Account
{
    public class LecturerModel : LoggedInPage
    {
        public string Error { get; private set; }
        public Lecturer Lecturer { get; private set; }

        public ActionResult OnGet()
        {
            if (!tryGetAcount(out Models.Account account))
                return Redirect("/account/login");

            if (account.HasLecturer)
                Lecturer = Database.GetLecturer(account.LecturerGuid).Clone();
            else
                Lecturer = new Lecturer();

            if (Lecturer.contact is null)
                Lecturer.contact = new Lecturer.Contact();

            return Page();
        }

        public ActionResult OnPost()
        {
            if (!tryGetAcount(out Models.Account account))
                return Redirect("/account/login");

            if (account.HasLecturer)
                Lecturer = Database.GetLecturer(account.LecturerGuid).Clone();
            else
                Lecturer = new Lecturer();

            if (Lecturer.contact is null)
                Lecturer.contact = new Lecturer.Contact();

            IFormCollection form = Request.Form;

            if (form.TryGetValue("title_before", out StringValues title_before))
                Lecturer.title_before = title_before;
            if (form.TryGetValue("first_name", out StringValues first_name))
                Lecturer.first_name = first_name;
            if (form.TryGetValue("middle_name", out StringValues middle_name))
                Lecturer.middle_name = middle_name;
            if (form.TryGetValue("last_name", out StringValues last_name))
                Lecturer.last_name = last_name;
            if (form.TryGetValue("title_after", out StringValues title_after))
                Lecturer.title_after = title_after;

            if (form.TryGetValue("picture_url", out StringValues picture_url))
                Lecturer.picture_url = picture_url;
            if (form.TryGetValue("location", out StringValues location))
                Lecturer.location = location;
            if (form.TryGetValue("claim", out StringValues claim))
                Lecturer.claim = claim;
            if (form.TryGetValue("bio", out StringValues bio))
                Lecturer.bio = bio;
            if (form.TryGetValue("tags", out StringValues _tags))
                Lecturer.tags = _tags.ToString().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(_tag =>
                {
                    Lecturer.Tag tag = new Lecturer.Tag()
                    {
                        name = _tag,
                        uuid = _tag.GetHash()
                    };
                    if (!Database.ContainsTag(tag))
                        Database.AddTag(tag);

                    return tag;
                }).ToArray();
            if (form.TryGetValue("price_per_hour", out StringValues _price_per_hour) && uint.TryParse(_price_per_hour, out uint price_per_hour))
                Lecturer.price_per_hour = price_per_hour;

            if (form.TryGetValue("email", out StringValues emails))
                Lecturer.contact.emails = emails.Where(email => !string.IsNullOrWhiteSpace(email)).ToArray()!;
            else
                Lecturer.contact.emails = new string[0];
            if (form.TryGetValue("phone", out StringValues phones))
                Lecturer.contact.telephone_numbers = phones.Where(phone => !string.IsNullOrWhiteSpace(phone)).ToArray()!;
            else
                Lecturer.contact.telephone_numbers = new string[0];

            if (string.IsNullOrWhiteSpace(Lecturer.first_name))
                Error = "You must fill out your First name.";
            else if (string.IsNullOrWhiteSpace(Lecturer.last_name))
                Error = "You must fill out your Last name.";
            else if (Lecturer.contact.emails is null || Lecturer.contact.emails.Length < 1)
                Error = "You must have at least 1 email.";
            else if (Lecturer.contact.telephone_numbers is null || Lecturer.contact.telephone_numbers.Length < 1)
                Error = "You must have at least 1 telephone number.";

            if (!string.IsNullOrEmpty(Error))
                return Page();
            else
            {
                if (!account.HasLecturer)
                {
                    Lecturer.UUID = Guid.NewGuid();
                    account.LecturerGuid = Lecturer.UUID;
                    Database.AddLectuer(Lecturer);
                } else
                    Database.lectuerers[Lecturer.UUID] = Lecturer;

                return Redirect("/account");
            }
        }
    }
}
