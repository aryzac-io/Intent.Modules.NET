using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    public class ManyToManySource
    {
        private List<ManyToManyDest> _manyToManyDests = new List<ManyToManyDest>();

        public ManyToManySource(string attribute, IEnumerable<ManyToManyDest> manyToManyDests)
        {
            Attribute = attribute;
            _manyToManyDests = new List<ManyToManyDest>(manyToManyDests);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ManyToManySource()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual IReadOnlyCollection<ManyToManyDest> ManyToManyDests
        {
            get => _manyToManyDests.AsReadOnly();
            private set => _manyToManyDests = new List<ManyToManyDest>(value);
        }

        public async Task OperationAsync(
            string attribute,
            IEnumerable<ManyToManyDest> manyToManyDests,
            CancellationToken cancellationToken = default)
        {
            Attribute = attribute;
            _manyToManyDests.Clear();
            _manyToManyDests.AddRange(manyToManyDests);
        }
    }
}