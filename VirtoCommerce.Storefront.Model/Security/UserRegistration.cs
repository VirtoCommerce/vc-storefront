using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserRegistration 
    {
        public User User { get; set; } = new User { UserType = "Customer" };

        public Contact Contact { get; set; } = new Contact();

        [FromForm(Name = "customer[photoUrl]")]
        public string PhotoUrl
        {
            get => Contact.PhotoUrl;
            set => Contact.PhotoUrl = value;
        }

        [FromForm(Name = "customer[first_name]")]
        public string FirstName
        {
            get => Contact.FirstName;
            set => Contact.FirstName = value;
        }

        [FromForm(Name = "customer[full_name]")]
        public string FullName
        {
            get => Contact.FullName;
            set => Contact.FullName = value;
        }

        [FromForm(Name = "customer[last_name]")]
        public string LastName
        {
            get => Contact.LastName;
            set => Contact.LastName = value;
        }

        [FromForm(Name = "customer[email]")]
        [EmailAddress]
        public string Email
        {
            get => User.Email;
            set => User.Email = value;
        }

        [FromForm(Name = "customer[user_name]")]
        [Required]
        public string UserName
        {
            get => User.UserName;
            set
            {
                User.UserName = value;
                if (string.IsNullOrEmpty(User.Email) && value.IsValidEmail())
                {
                    User.Email = value;
                }
            }
        }

        [FromForm(Name = "customer[password]")]
        [Required]
        public string Password
        {
            get => User.Password;
            set => User.Password = value;
        }

        [FromForm(Name = "customer[store_id]")]
        public string StoreId
        {
            get => User.StoreId;
            set => User.StoreId = value;
        }

        [FromForm(Name = "customer[name]")]
        public string Name
        {
            get => Contact.Name;
            set => Contact.Name = value;
        }

        [FromForm(Name = "customer[address]")]
        public virtual Address Address
        {
            get => Contact.Addresses.FirstOrDefault();
            set => Contact.Addresses.Add(value);
        }

        [FromForm(Name = "customer[salutation]")]
        public string Salutation
        {
            get => Contact.Salutation;
            set => Contact.Salutation = value;
        }

        [FromForm(Name = "customer[middleName]")]
        public string MiddleName
        {
            get => Contact.MiddleName;
            set => Contact.MiddleName = value;
        }

        [FromForm(Name = "customer[birthDate]")]
        public DateTime? BirthDate
        {
            get => Contact.BirthDate;
            set => Contact.BirthDate = value;
        }

        [FromForm(Name = "customer[timeZone]")]
        public string TimeZone
        {
            get => Contact.TimeZone;
            set => Contact.TimeZone = value;
        }
    }
}
