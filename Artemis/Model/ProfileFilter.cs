using System;
using System.Collections.Generic;

namespace Artemis.Model
{
    public class ProfileFilter
    {
        #region special properties
        //public string CurrentUserId { get; set; }

        #endregion

        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime LastActive { get; set; }
        public List<int?> Age { get; set; }
        public List<int?> Height { get; set; }
        public List<int?> Weight { get; set; }
        public string Description { get; set; }

        //public override string[] Tags { get; set; }

        //public override string JobTitle { get; set; } = string.Empty;

        public GenderType Gender { get; set; }
        public SexualOrientationType SexualOrientation { get; set; }
        public BodyType Body { get; set; }
        public SmokingHabitsType SmokingHabits { get; set; }

        //[BsonRepresentation(BsonType.String)]  // Maybe not
        //public abstract AllergiesType Allergies { get; set; }

        public HasChildrenType HasChildren { get; set; }

        public WantChildrenType WantChildren { get; set; }

        public HasPetsType HasPets { get; set; }

        public LocationType LivesIn { get; set; }

        public EducationType Education { get; set; }

        public EducationStatusType EducationStatus { get; set; }

        public EducationLevelType EducationLevel { get; set; }

        public EmploymentStatusType EmploymentStatus { get; set; }

        //[BsonRepresentation(BsonType.String)]
        //public abstract EmploymentAreaType EmploymentArea { get; set; }

        //[BsonRepresentation(BsonType.String)]
        //public abstract EmploymentLevelType EmploymentLevel { get; set; }

        //[BsonRepresentation(BsonType.String)] //Maybe not
        //public abstract PoliticalOrientationType PoliticalOrientation { get; set; }

        //[BsonRepresentation(BsonType.String)] //Maybe not
        //public abstract ReligiousOrientationType ReligiousOrientation { get; set; }

        public SportsActivityType SportsActivity { get; set; }

        public EatingHabitsType EatingHabits { get; set; }

        public ClotheStyleType ClotheStyle { get; set; }

        public BodyArtType BodyArt { get; set; }
    }
}
