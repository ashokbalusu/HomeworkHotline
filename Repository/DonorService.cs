using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;

namespace Repository
{
    public class DonorService
    {
        private static bool UpdateDatabase = true;
        private HomeworkHotlineEntities entities;

        public DonorService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public IList<DonorModel> GetAll()
        {
            var result = HttpContext.Current.Session["Donors"] as IList<DonorModel>;

            if (result == null || UpdateDatabase)
            {
                result = entities.Donors.Select(donor => new DonorModel
                {
                    DonorID = donor.DonorID,
                    DonorName = donor.DonorName,
                    Amount = donor.Amount,
                    Phone = donor.Phone,
                    CreateDate = donor.CreateDate,
                    Address1 = donor.Address1,
                    Address2 = donor.Address2,
                    City = donor.City,
                    DonorState = donor.State,
                    Zip = donor.Zip,
                    BusinessName = donor.BusinessName,
                    IsDeleted = donor.IsDeleted,
                    Title = donor.Title,
                    FirstName = donor.FirstName,
                    LastName = donor.LastName,
                    Email = donor.Email,
                    Notes = donor.Notes,
                    TotalDonations = (from c in entities.DonorGivings where c.IsDeleted == false && c.DonorID == donor.DonorID select c).Sum(td => td.AmountGiven)
                }).Where( e => e.IsDeleted == false).ToList();

                HttpContext.Current.Session["Donors"] = result;
            }

            return result;
        }

        public IEnumerable<DonorModel> Read()
        {
            return GetAll();
        }

        public void Create(DonorModel donor)
        {
            if (!UpdateDatabase)
            {
                var first = GetAll().OrderByDescending(e => e.DonorID).FirstOrDefault();
                var id = (first != null) ? first.DonorID : 0;

                GetAll().Insert(0, donor);
            }
            else
            {
                var entity = new Donor();

                entity.DonorID = donor.DonorID;
                entity.DonorName = donor.DonorName;
                entity.Amount = donor.Amount;
                entity.Phone = donor.Phone;
                entity.CreateDate = donor.CreateDate;
                entity.Address1 = donor.Address1;
                entity.Address2 = donor.Address2;
                entity.City = donor.City;
                entity.State = donor.DonorState;
                entity.Zip = donor.Zip;
                entity.BusinessName = donor.BusinessName;
                entity.FirstName = donor.FirstName;
                entity.LastName = donor.LastName;
                entity.Title = donor.Title;
                entity.Notes = donor.Notes;
                entity.Email = donor.Email;

                entities.Donors.Add(entity);
                entities.SaveChanges();

                donor.DonorID = entity.DonorID;
            }
        }

        public void Update(DonorModel donor)
        {
            if (!UpdateDatabase)
            {
                var target = One(e => e.DonorID == donor.DonorID);

                if (target != null)
                {
                    target.DonorID = donor.DonorID;
                    target.DonorName = donor.DonorName;
                    target.Amount = donor.Amount;
                    target.Phone = donor.Phone;
                    target.CreateDate = donor.CreateDate;
                    target.Address1 = donor.Address1;
                    target.Address2 = donor.Address2;
                    target.City = donor.City;
                    target.DonorState = donor.DonorState;
                    target.Zip = donor.Zip;
                    target.BusinessName = donor.BusinessName;
                    target.FirstName = donor.FirstName;
                    target.LastName = donor.LastName;
                    target.Title = donor.Title;
                    target.Notes = donor.Notes;
                    target.Email = donor.Email;
                }
            }
            else
            {
                var entity = new Donor();

                entity.DonorID = donor.DonorID;
                entity.DonorName = donor.DonorName;
                entity.Amount = donor.Amount;
                entity.Phone = donor.Phone;
                entity.CreateDate = donor.CreateDate;
                entity.Address1 = donor.Address1;
                entity.Address2 = donor.Address2;
                entity.City = donor.City;
                entity.State = donor.DonorState;
                entity.Zip = donor.Zip;
                entity.BusinessName = donor.BusinessName;
                entity.FirstName = donor.FirstName;
                entity.LastName = donor.LastName;
                entity.Title = donor.Title;
                entity.Notes = donor.Notes;
                entity.Email = donor.Email;


                entities.Donors.Attach(entity);
                entities.Entry(entity).State = EntityState.Modified;
                entities.SaveChanges();
            }
        }

        public void Destroy(DonorModel donor)
        {
            if (!UpdateDatabase)
            {
                var target = GetAll().FirstOrDefault(p => p.DonorID == donor.DonorID);
                if (target != null)
                {
                    GetAll().Remove(target);
                }
            }
            else
            {
                var target = (from s in entities.Donors
                              where s.DonorID == donor.DonorID
                              select s).FirstOrDefault();

                if (target != null)
                {
                    target.IsDeleted = true;
                    entities.Entry(target).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public DonorModel One(Func<DonorModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}
