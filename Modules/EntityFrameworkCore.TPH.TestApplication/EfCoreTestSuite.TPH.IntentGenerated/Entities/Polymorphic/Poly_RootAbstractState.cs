using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.Polymorphic
{

    public abstract partial class Poly_RootAbstract : IPoly_RootAbstract
    {
        public Poly_RootAbstract()
        {
        }

        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }

        private string _abstractField;

        public string AbstractField
        {
            get { return _abstractField; }
            set
            {
                _abstractField = value;
            }
        }


        public Guid? TopLevelId { get; set; }
    }
}
