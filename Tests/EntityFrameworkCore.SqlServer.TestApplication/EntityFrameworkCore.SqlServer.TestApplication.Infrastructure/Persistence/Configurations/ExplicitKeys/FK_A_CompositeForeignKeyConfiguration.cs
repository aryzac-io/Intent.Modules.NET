using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.ExplicitKeys
{
    public class FK_A_CompositeForeignKeyConfiguration : IEntityTypeConfiguration<FK_A_CompositeForeignKey>
    {
        public void Configure(EntityTypeBuilder<FK_A_CompositeForeignKey> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.PK_A_CompositeKey)
                .WithMany()
                .HasForeignKey(x => new { x.PK_A_CompositeKeyCompositeKeyA, x.PK_A_CompositeKeyCompositeKeyB })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}