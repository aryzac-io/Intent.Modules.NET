using System;
using System.Collections.Generic;
using System.Linq;
using Entities.PrivateSetters.EF.SqlServer.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Invoice
    {
        private List<Line> _lines = new List<Line>();
        private List<Tag> _tags = new List<Tag>();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public Invoice(DateTime date, IEnumerable<Tag> tags, IEnumerable<LineDataContract> lines)
        {
            Date = date;
            _tags = new List<Tag>(tags);
            _lines.AddRange(lines.Select(x => new Line(x.Description, x.Quantity)));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Invoice()
        {
        }

        public Guid Id { get; private set; }

        public DateTime Date { get; private set; }

        public virtual IReadOnlyCollection<Line> Lines
        {
            get => _lines.AsReadOnly();
            private set => _lines = new List<Line>(value);
        }

        public virtual IReadOnlyCollection<Tag> Tags
        {
            get => _tags.AsReadOnly();
            private set => _tags = new List<Tag>(value);
        }
    }
}