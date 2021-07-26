using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Artemis.Model
{
    public class CurrentUser : AbstractProfile
    {
        #region special properties

        [BsonRepresentation(BsonType.String)]
        public SexualOrientationType SexualOrientation { get; set; }

        [BsonRepresentation(BsonType.String)]
        public GenderType Gender { get; set; }

        public List<string> Bookmarks { get; set; }
        public List<ChatMember> ChatMemberslist { get; set; }
        public ProfileFilter ProfileFilter { get; set; }
        public Dictionary<string, DateTime> Visited { get; set; }
        public Dictionary<string, DateTime> IsBookmarked { get; set; }
        public List<string> Likes { get; set; }
        #endregion

        internal override ObjectId _id { get; set; }
        public override string Auth0Id { internal get; set; }
        public override string ProfileId { get; set; }
        public override bool Admin { get; set; } = false;
        public override string Name { get; set; }
        public override DateTime CreatedOn { get; set; } = DateTime.Now;
        public override DateTime UpdatedOn { get; set; } = DateTime.Now;
        public override DateTime LastActive { get; set; } = DateTime.Now;
        public override int? Age { get; set; } = null;
        public override int? Height { get; set; }
        public override bool Contactable { get; set; } = true;
        public override string Description { get; set; }
        public override List<ImageModel> Images { get; set; }
        public override List<string> Tags { get; set; }
        public override BodyType Body { get; set; }
        public override SmokingHabitsType SmokingHabits { get; set; }
        public override HasChildrenType HasChildren { get; set; }
        public override WantChildrenType WantChildren { get; set; }
        public override HasPetsType HasPets { get; set; }
        public override LivesInType LivesIn { get; set; }
        public override EducationType Education { get; set; }
        public override EducationStatusType EducationStatus { get; set; }
        public override EmploymentStatusType EmploymentStatus { get; set; }
        public override SportsActivityType SportsActivity { get; set; }
        public override EatingHabitsType EatingHabits { get; set; }
        public override ClotheStyleType ClotheStyle { get; set; }
        public override BodyArtType BodyArt { get; set; }
    }
}