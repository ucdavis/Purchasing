using System;
using System.Linq;
using OrAdmin.Entities.App;

namespace OrAdmin.Repositories.App
{
    public class ProfileRepository
    {
        private ProfileDataContext dc = new ProfileDataContext();

        public bool HasProfile(string userName)
        {
            return dc.Profiles.Where(p => p.UserName.Equals(userName)).Any();
        }

        public Profile GetProfile(string userName)
        {
            return dc.Profiles.Where(p => p.UserName.Equals(userName)).Single();
        }

        public Profile GetProfile(int profileId)
        {
            var profile = dc.Profiles.Where(p => p.Id.Equals(profileId)).SingleOrDefault();

            if (profile != null)
                return profile;
            else
                throw new Exception(String.Format("profileId {0} not found in repository", profileId));
        }

        public void InsertProfile(Profile profile)
        {
            dc.Profiles.InsertOnSubmit(profile);
        }

        public void Delete(Profile profile)
        {
            // Mark for deletion
            dc.Profiles.DeleteOnSubmit(profile);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
    }
}
