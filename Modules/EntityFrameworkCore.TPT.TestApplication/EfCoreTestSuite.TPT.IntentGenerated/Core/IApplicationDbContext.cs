using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.TPT.IntentGenerated.Entities;
using EfCoreTestSuite.TPT.IntentGenerated.Entities.InheritanceAssociations;
using EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Core
{
    public interface IApplicationDbContext
    {
        DbSet<AbstractBaseClassAssociated> AbstractBaseClassAssociateds { get; set; }
        DbSet<ConcreteBaseClass> ConcreteBaseClasses { get; set; }
        DbSet<ConcreteBaseClassAssociated> ConcreteBaseClassAssociateds { get; set; }
        DbSet<DerivedClassForAbstract> DerivedClassForAbstracts { get; set; }
        DbSet<DerivedClassForAbstractAssociated> DerivedClassForAbstractAssociateds { get; set; }
        DbSet<DerivedClassForConcrete> DerivedClassForConcretes { get; set; }
        DbSet<DerivedClassForConcreteAssociated> DerivedClassForConcreteAssociateds { get; set; }
        DbSet<FkAssociatedClass> FkAssociatedClasses { get; set; }
        DbSet<FkBaseClass> FkBaseClasses { get; set; }
        DbSet<FkBaseClassAssociated> FkBaseClassAssociateds { get; set; }
        DbSet<FkDerivedClass> FkDerivedClasses { get; set; }
        DbSet<Poly_BaseClassNonAbstract> Poly_BaseClassNonAbstracts { get; set; }
        DbSet<Poly_ConcreteA> Poly_ConcreteAs { get; set; }
        DbSet<Poly_ConcreteB> Poly_ConcreteBs { get; set; }
        DbSet<Poly_SecondLevel> Poly_SecondLevels { get; set; }
        DbSet<Poly_TopLevel> Poly_TopLevels { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}