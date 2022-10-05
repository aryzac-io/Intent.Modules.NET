using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.TPH.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Core
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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}