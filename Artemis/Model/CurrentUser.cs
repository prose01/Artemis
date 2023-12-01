using System;
using System.Collections.Generic;

namespace Artemis.Model
{
    public class CurrentUser : AbstractProfile
    {
        public override string ProfileId { get; set; }
        public override bool Admin { get; set; } = false;
        public override string Name { get; set; }
        public override DateTime UpdatedOn { get; set; } = DateTime.Now;
        public override DateTime LastActive { get; set; } = DateTime.Now;
        public override List<ImageModel> Images { get; set; }
    }
}