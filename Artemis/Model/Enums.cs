namespace Artemis.Model
{
    public enum ClotheStyleType
    {
        NotChosen,
        Casual,
        Dressy,
        Dandy,
        Stylish,
        Formal,
        Other
    }

    public enum BodyType
    {
        NotChosen,
        Atlethic,
        Chubby,
        Normal,
        Robust,
        Slim,
        Other
    }

    public enum BodyArtType
    {
        NotChosen,
        Piercing,
        Tatoo,
        Other
    }

    public enum GenderType
    {
        Female,
        Male
    }

    public enum EatingHabitsType
    {
        NotChosen,
        Healthy,
        Gastronomic,
        Normal,
        Kosher,
        Organic,
        Traditional,
        Vegetarian,
        Other
    }

    public enum EducationStatusType
    {
        NotChosen,
        Graduated,
        Student,
        Other
    }

    public enum EducationType
    {
        NotChosen,
        School,
        Highschool,
        University,
        Other
    }

    public enum EmploymentStatusType
    {
        NotChosen,
        Unemployed,
        Employed,
        SelfEmployed,
        Other
    }

    public enum HasChildrenType
    {
        NotChosen,
        Yes,
        No,
        Other
    }

    public enum WantChildrenType
    {
        NotChosen,
        Yes,
        No,
        Other
    }

    public enum HasPetsType
    {
        NotChosen,
        Yes,
        No,
        Other
    }

    public enum LivesInType
    {
        NotChosen,
        City,
        Suburb,
        Countryside,
        Other
    }

    public enum SexualOrientationType
    {
        Heterosexual,
        Homosexual,
        Bisexual,
        Asexual
    }

    public enum SmokingHabitsType
    {
        NotChosen,
        NonSmoker,
        OccasionalSmoker,
        Smoker,
        Other
    }

    public enum SportsActivityType
    {
        NotChosen,
        Regularly,
        SomeRegularity,
        Seldom,
        Never,
        Other
    }

    public enum MaritalStatusType
    {
        NotChosen,
        Single,
        Married,
        Separated,
        Divorced,
        Other
    }


    // Enum types used for other stuff than Profiles.
    public enum OrderByType
    {
        CreatedOn,
        UpdatedOn,
        LastActive
    }
}