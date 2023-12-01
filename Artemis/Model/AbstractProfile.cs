using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Artemis.Model
{
    [BsonKnownTypes(typeof(CurrentUser))]
    public abstract class AbstractProfile
    {
        public abstract string ProfileId { get; set; }
        public abstract bool Admin { get; set; }

        [StringLength(50, ErrorMessage = "Name length cannot be more than 50.")]
        public abstract string Name { get; set; }

        [DataType(DataType.DateTime)]
        public abstract DateTime UpdatedOn { get; set; }

        [DataType(DataType.DateTime)]
        public abstract DateTime LastActive { get; set; }

        public abstract List<ImageModel> Images { get; set; }
    }
}
