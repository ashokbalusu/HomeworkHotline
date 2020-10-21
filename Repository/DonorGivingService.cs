using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;

namespace Repository
{
    public class DonorGivingService
    {
        private static bool UpdateDatabase = true;
        private HomeworkHotlineEntities entities;

        public DonorGivingService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public IList<DonorGivingModel> GetAll()
        {
            var result = HttpContext.Current.Session["DonorGivings"] as IList<DonorGivingModel>;

            if (result == null || UpdateDatabase)
            {
                result = entities.DonorGivings.Select(donorGiving => new DonorGivingModel
                {
                    DonorGivingID = donorGiving.DonorGivingID,
                    DonorID = donorGiving.DonorID,
                    AmountGiven = donorGiving.AmountGiven,
                    DateGiven = donorGiving.DateGiven,
                    InKind = donorGiving.InKind,
                    IsDeleted = donorGiving.IsDeleted
                }).Where(dg => dg.IsDeleted == false).ToList();

                HttpContext.Current.Session["DonorGivings"] = result;
            }

            return result;
        }

        public IList<DonorGivingModel> GetDonorGivingByDonor(int parentID)
        {
            var result = HttpContext.Current.Session["DonorGivings"] as IList<DonorGivingModel>;

            if (result == null || UpdateDatabase)
            {
                result = entities.DonorGivings.Select(donorGiving => new DonorGivingModel
                {
                    DonorGivingID = donorGiving.DonorGivingID,
                    DonorID = donorGiving.DonorID,
                    AmountGiven = donorGiving.AmountGiven,
                    DateGiven = donorGiving.DateGiven,
                    InKind = donorGiving.InKind,
                    IsDeleted = donorGiving.IsDeleted
                }).Where(dg => dg.DonorID == parentID && dg.IsDeleted == false).ToList();

                HttpContext.Current.Session["DonorGivings"] = result;
            }

            return result;
        }

        public IEnumerable<DonorGivingModel> Read(int parentID)
        {
            return GetDonorGivingByDonor(parentID);
        }

        public void Create(DonorGivingModel donorGiving)
        {
            if (!UpdateDatabase)
            {
                var first = GetAll().OrderByDescending(e => e.DonorGivingID).FirstOrDefault();
                var id = (first != null) ? first.DonorGivingID : 0;

                GetAll().Insert(0, donorGiving);
            }
            else
            {
                var entity = new DonorGiving();

                entity.DonorGivingID = donorGiving.DonorGivingID;
                entity.DonorID = donorGiving.DonorID;
                entity.AmountGiven = donorGiving.AmountGiven;
                entity.DateGiven = donorGiving.DateGiven;
                entity.InKind = donorGiving.InKind;
                entity.IsDeleted = donorGiving.IsDeleted;

                entities.DonorGivings.Add(entity);
                entities.SaveChanges();

                donorGiving.DonorGivingID = entity.DonorGivingID;
            }
        }

        public void Update(DonorGivingModel donorGiving)
        {
            if (!UpdateDatabase)
            {
                var target = One(e => e.DonorGivingID == donorGiving.DonorGivingID);

                if (target != null)
                {
                    target.DonorGivingID = donorGiving.DonorGivingID;
                    target.DonorID = donorGiving.DonorID;
                    target.AmountGiven = donorGiving.AmountGiven;
                    target.DateGiven = donorGiving.DateGiven;
                    target.InKind = donorGiving.InKind;
                    target.IsDeleted = donorGiving.IsDeleted;
                }
            }
            else
            {
                var entity = new DonorGiving();

                entity.DonorGivingID = donorGiving.DonorGivingID;
                entity.DonorID = donorGiving.DonorID;
                entity.AmountGiven = donorGiving.AmountGiven;
                entity.DateGiven = donorGiving.DateGiven;
                entity.InKind = donorGiving.InKind;
                entity.IsDeleted = donorGiving.IsDeleted;

                entities.DonorGivings.Attach(entity);
                entities.Entry(entity).State = EntityState.Modified;
                entities.SaveChanges();
            }
        }

        public void Destroy(DonorGivingModel donorGiving)
        {
            if (!UpdateDatabase)
            {
                var target = GetAll().FirstOrDefault(p => p.DonorGivingID == donorGiving.DonorGivingID);
                if (target != null)
                {
                    GetAll().Remove(target);
                }
            }
            else
            {
                var target = (from s in entities.DonorGivings
                              where s.DonorGivingID == donorGiving.DonorGivingID
                              select s).FirstOrDefault();

                if (target != null)
                {
                    target.IsDeleted = true;
                    entities.Entry(target).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public DonorGivingModel One(Func<DonorGivingModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}
