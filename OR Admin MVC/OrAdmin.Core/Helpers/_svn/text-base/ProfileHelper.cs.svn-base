using System;
using OrAdmin.Entities.App;
using OrAdmin.Repositories.App;

namespace OrAdmin.Core.Helpers
{
    public class ProfileHelper
    {
        public static string GetNameOrUsername(string userName, NameFormat format)
        {
            ProfileRepository repo = new ProfileRepository();
            if (repo.HasProfile(userName))
            {
                Profile profile = repo.GetProfile(userName);
                string nameStr = String.Format("{0} {1}", profile.FirstName, profile.LastName);
                switch (format)
                {
                    case NameFormat.First: nameStr = profile.FirstName; break;
                    case NameFormat.Last: nameStr = profile.LastName; break;
                    case NameFormat.LastCommaFirst: nameStr = String.Format("{0}, {1}", profile.LastName, profile.FirstName); break;
                    default: break;
                }

                return nameStr;
            }
            else
                return userName;
        }

        public enum NameFormat
        {
            FirstSpaceLast,
            LastCommaFirst,
            First,
            Last
        }
    }
}
