using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SchoolModel
    {
        public int SchoolID { get; set; }
        public string SchoolName { get; set; }
        public int? SchoolTypeID { get; set; }
        public int? QuadrantID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int? CountyID { get; set; }
        public string Phone { get; set; }
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public decimal? PEDS { get; set; }
        public int? Census { get; set; }
        public int? PredictedThirdGradeStudents { get; set; }
        public int? SchoolBoardDistrict { get; set; }
        public int? CityCouncilDistrict { get; set; }
        public bool? SonicPartner { get; set; }
        public bool? TitleOneSchool { get; set; }
        public bool? AppalachianRegion { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int? FlashCardQty { get; set; }
        public bool IsDeleted { get; set; }

        public string CountyName { get; set; }
        public string SchoolTypeName { get; set; }
        // School District
        public string DistrictName { get; set; }
        public string QuadrantName { get; set; }
        public string CreatedByFullName { get; set; }
        public string ModifiedByFullName { get; set; }

        public static implicit operator SchoolModel(School school)
        {
            return new SchoolModel
            {
                SchoolID = school.SchoolID,
                SchoolName = school.SchoolName,
                SchoolTypeID = school.SchoolTypeID,
                Address1 = school.Address1,
                Address2 = school.Address2,
                City = school.City,
                State = school.State,
                Zip = school.Zip,
                CountyID = school.CountyID,
                Phone = school.Phone,
                PrincipalName = school.PrincipalName,
                PrincipalEmail = school.PrincipalEmail,
                PEDS = school.PEDS,
                Census = school.Census,
                PredictedThirdGradeStudents = school.PredictedThirdGradeStudents,
                SchoolBoardDistrict = school.SchoolBoardDistrict,
                CityCouncilDistrict = school.CityCouncilDistrict,
                SonicPartner = school.SonicPartner,
                TitleOneSchool = school.TitleOneSchool,
                AppalachianRegion = school.AppalachianRegion,
                FlashCardQty = school.FlashCardQty,
                IsDeleted = school.IsDeleted,
                QuadrantID = school.QuadrantID,
                CreateDate = school.CreateDate,
                CreateBy = school.CreateBy,
                ModifiedDate = school.ModifiedDate,
                ModifiedBy = school.ModifiedBy,
                CountyName = school.County.CountyName,
                SchoolTypeName = school.SchoolType.SchoolTypeName,
                DistrictName = school.SchoolDistrict.DistrictName,
                QuadrantName = school.MnpsQuadrant.QuadrantName
            };
        }

        public static implicit operator School(SchoolModel schoolModel)
        {
            return new School
            {
                SchoolID = schoolModel.SchoolID,
                SchoolName = schoolModel.SchoolName,
                SchoolTypeID = schoolModel.SchoolTypeID,
                Address1 = schoolModel.Address1,
                Address2 = schoolModel.Address2,
                City = schoolModel.City,
                State = schoolModel.State,
                Zip = schoolModel.Zip,
                CountyID = schoolModel.CountyID,
                Phone = schoolModel.Phone,
                PrincipalName = schoolModel.PrincipalName,
                PrincipalEmail = schoolModel.PrincipalEmail,
                PEDS = schoolModel.PEDS,
                Census = schoolModel.Census,
                PredictedThirdGradeStudents = schoolModel.PredictedThirdGradeStudents,
                SchoolBoardDistrict = schoolModel.SchoolBoardDistrict,
                CityCouncilDistrict = schoolModel.CityCouncilDistrict,
                SonicPartner = schoolModel.SonicPartner,
                TitleOneSchool = schoolModel.TitleOneSchool,
                AppalachianRegion = schoolModel.AppalachianRegion,
                FlashCardQty = schoolModel.FlashCardQty,
                IsDeleted = schoolModel.IsDeleted,
                QuadrantID = schoolModel.QuadrantID,
                CreateDate = schoolModel.CreateDate,
                CreateBy = schoolModel.CreateBy,
                ModifiedDate = schoolModel.ModifiedDate,
                ModifiedBy = schoolModel.ModifiedBy

            };
        }





    }
}
