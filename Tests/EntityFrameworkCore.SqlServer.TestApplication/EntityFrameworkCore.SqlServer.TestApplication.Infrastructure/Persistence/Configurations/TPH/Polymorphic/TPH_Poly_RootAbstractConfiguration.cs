using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPH.Polymorphic
{
    public class TPH_Poly_RootAbstractConfiguration : IEntityTypeConfiguration<TPH_Poly_RootAbstract>
    {
        public void Configure(EntityTypeBuilder<TPH_Poly_RootAbstract> builder)
        {
            builder.ToTable("TphPoly_RootAbstract");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AbstractField)
                .IsRequired();

            builder.HasOne(x => x.Poly_RootAbstract_Aggr)
                .WithMany()
                .HasForeignKey(x => x.Poly_RootAbstract_AggrId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Poly_RootAbstract_Comp, ConfigurePoly_RootAbstract_Comp);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigurePoly_RootAbstract_Comp(OwnedNavigationBuilder<TPH_Poly_RootAbstract, TPH_Poly_RootAbstract_Comp> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompField)
                .IsRequired();
        }
    }
}