using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using FluentNHibernate.Mapping;
using Purchasing.Core.Helpers;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class DepartmentalAdminRequest : DomainObjectWithTypedId<string>
    {
        public DepartmentalAdminRequest()
        {
            
        }
        public DepartmentalAdminRequest(string id)
            : this()
        {
            Id = id;
            SharedOrCluster = false;
            Complete = false;
            DateCreated = DateTime.UtcNow.ToPacificTime();
            RequestCount = 0;
        }

        public virtual string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }
        
        public virtual string FullNameAndId { get { return string.Format("{0} ({1})", FullName, Id); } }
        
        public virtual string FullNameAndIdLastFirst { get { return string.Format("{0}, {1} ({2})", LastName, FirstName, Id); } }

       
        [Required]
        [StringLength(10)]
        [Display(Name = "KerberosID")]
        public override string Id { get; protected set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public virtual string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public virtual string LastName { get; set; }

        [StringLength(50)]
        [Display(Name = "Phone Number")]
        public virtual string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        [Email]
        public virtual string Email { get; set; }

        [Display(Name = "Department Size")]
        public virtual int DepartmentSize { get; set; }

        [Display(Name = "Shared Service Center Participant?")]
        public virtual bool SharedOrCluster { get; set; }

        [Display(Name = "Date Created")]
        public virtual DateTime DateCreated { get; set; }

        public virtual string Organizations { get; set; }


        public virtual bool Complete { get; set; }

        [Display(Name = "Request Count")]
        public virtual int RequestCount { get; set; }

        [Display(Name = "Attended Training")]
        public virtual bool AttendedTraining { get; set; }

       
    }

    public class DepartmentalAdminRequestMap : ClassMap<DepartmentalAdminRequest>
    {
        public DepartmentalAdminRequestMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Email);
            Map(x => x.PhoneNumber);
            Map(x => x.DepartmentSize);
            Map(x => x.SharedOrCluster);
            Map(x => x.Complete);
            Map(x => x.DateCreated);
            Map(x => x.Organizations);
            Map(x => x.RequestCount);
            Map(x => x.AttendedTraining);

        }
    }
}