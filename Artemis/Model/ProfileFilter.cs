using System.Collections.Generic;

namespace Artemis.Model
{
    public class ProfileFilter
    {
        public string Name { get; set; }
        public List<int?> Age { get; set; }
        public List<int?> Height { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public GenderType? Gender { get; set; }
        public BodyType Body { get; set; }
        public SmokingHabitsType SmokingHabits { get; set; }
        public HasChildrenType HasChildren { get; set; }
        public WantChildrenType WantChildren { get; set; }
        public HasPetsType HasPets { get; set; }
        public LivesInType LivesIn { get; set; }
        public EducationType Education { get; set; }
        public EducationStatusType EducationStatus { get; set; }
        public EmploymentStatusType EmploymentStatus { get; set; }
        public SportsActivityType SportsActivity { get; set; }
        public EatingHabitsType EatingHabits { get; set; }
        public ClotheStyleType ClotheStyle { get; set; }
        public BodyArtType BodyArt { get; set; }
    }
}
