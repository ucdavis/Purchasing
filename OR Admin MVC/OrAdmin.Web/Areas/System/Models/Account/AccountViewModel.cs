﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Security;
using Sys = System;


namespace OrAdmin.Web.Areas.System.Models.Account
{
    public class AccountViewModel
    {
        #region Models

        public class RegisterModel
        {
            [Required]
            [DisplayName("Kerberos")]
            public string Kerberos { get; set; }
        }
        #endregion

        #region Services
        // The FormsAuthentication type is sealed and contains static members, so it is difficult to
        // unit test code that calls its members. The interface and helper class below demonstrate
        // how to create an abstract wrapper around such a type in order to make the AccountController
        // code unit testable.

        public interface IMembershipService
        {
            int MinPasswordLength { get; }

            bool ValidateUser(string userName, string password);
            MembershipCreateStatus CreateUser(string userName, string password, string email);
            bool ChangePassword(string userName, string oldPassword, string newPassword);
        }

        public class AccountMembershipService : IMembershipService
        {
            private readonly MembershipProvider _provider;

            public AccountMembershipService()
                : this(null)
            {
            }

            public AccountMembershipService(MembershipProvider provider)
            {
                _provider = provider ?? Sys.Web.Security.Membership.Provider;
            }

            public int MinPasswordLength
            {
                get
                {
                    return _provider.MinRequiredPasswordLength;
                }
            }

            public bool ValidateUser(string userName, string password)
            {
                if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
                if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

                return _provider.ValidateUser(userName, password);
            }

            public MembershipCreateStatus CreateUser(string userName, string password, string email)
            {
                if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
                if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
                if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

                MembershipCreateStatus status;
                _provider.CreateUser(userName, password, email, null, null, true, null, out status);
                return status;
            }

            public bool ChangePassword(string userName, string oldPassword, string newPassword)
            {
                if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
                if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
                if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

                // The underlying ChangePassword() will throw an exception rather
                // than return false in certain failure scenarios.
                try
                {
                    MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                    return currentUser.ChangePassword(oldPassword, newPassword);
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (MembershipPasswordException)
                {
                    return false;
                }
            }
        }

        public interface IFormsAuthenticationService
        {
            void SignIn(string userName, bool createPersistentCookie);
            void SignOut();
        }

        public class FormsAuthenticationService : IFormsAuthenticationService
        {
            public void SignIn(string userName, bool createPersistentCookie)
            {
                if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

                FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            }

            public void SignOut()
            {
                FormsAuthentication.SignOut();
            }
        }
        #endregion

        #region Validation
        public static class AccountValidation
        {
            public static string ErrorCodeToString(MembershipCreateStatus createStatus)
            {
                // See http://go.microsoft.com/fwlink/?LinkID=177550 for
                // a full list of status codes.
                switch (createStatus)
                {
                    case MembershipCreateStatus.DuplicateUserName:
                        return "Username already exists. Please enter a different user name.";

                    case MembershipCreateStatus.DuplicateEmail:
                        return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                    case MembershipCreateStatus.InvalidPassword:
                        return "The password provided is invalid. Please enter a valid password value.";

                    case MembershipCreateStatus.InvalidEmail:
                        return "The e-mail address provided is invalid. Please check the value and try again.";

                    case MembershipCreateStatus.InvalidAnswer:
                        return "The password retrieval answer provided is invalid. Please check the value and try again.";

                    case MembershipCreateStatus.InvalidQuestion:
                        return "The password retrieval question provided is invalid. Please check the value and try again.";

                    case MembershipCreateStatus.InvalidUserName:
                        return "The user name provided is invalid. Please check the value and try again.";

                    case MembershipCreateStatus.ProviderError:
                        return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                    case MembershipCreateStatus.UserRejected:
                        return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                    default:
                        return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                }
            }
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
        public sealed class PropertiesMustMatchAttribute : ValidationAttribute
        {
            private const string _defaultErrorMessage = "'{0}' and '{1}' do not match.";
            private readonly object _typeId = new object();

            public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
                : base(_defaultErrorMessage)
            {
                OriginalProperty = originalProperty;
                ConfirmProperty = confirmProperty;
            }

            public string ConfirmProperty { get; private set; }
            public string OriginalProperty { get; private set; }

            public override object TypeId
            {
                get
                {
                    return _typeId;
                }
            }

            public override string FormatErrorMessage(string name)
            {
                return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                    OriginalProperty, ConfirmProperty);
            }

            public override bool IsValid(object value)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
                object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
                object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
                return Object.Equals(originalValue, confirmValue);
            }
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
        {
            private const string _defaultErrorMessage = "'{0}' must be at least {1} characters long.";
            private readonly int _minCharacters = Sys.Web.Security.Membership.Provider.MinRequiredPasswordLength;

            public ValidatePasswordLengthAttribute()
                : base(_defaultErrorMessage)
            {
            }

            public override string FormatErrorMessage(string name)
            {
                return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                    name, _minCharacters);
            }

            public override bool IsValid(object value)
            {
                string valueAsString = value as string;
                return (valueAsString != null && valueAsString.Length >= _minCharacters);
            }
        }
        #endregion
    }
}