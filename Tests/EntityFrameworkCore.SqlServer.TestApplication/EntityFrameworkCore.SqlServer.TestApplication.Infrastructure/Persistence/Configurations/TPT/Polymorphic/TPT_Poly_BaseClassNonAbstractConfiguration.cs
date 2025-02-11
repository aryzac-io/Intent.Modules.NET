using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPT.Polymorphic
{
    public class TPT_Poly_BaseClassNonAbstractConfiguration : IEntityTypeConfiguration<TPT_Poly_BaseClassNonAbstract>
    {
        public void Configure(EntityTypeBuilder<TPT_Poly_BaseClassNonAbstract> builder)
        {
            builder.ToTable("TptPoly_BaseClassNonAbstract");

            builder.Property(x => x.BaseField)
                .IsRequired();
        }
    }
}