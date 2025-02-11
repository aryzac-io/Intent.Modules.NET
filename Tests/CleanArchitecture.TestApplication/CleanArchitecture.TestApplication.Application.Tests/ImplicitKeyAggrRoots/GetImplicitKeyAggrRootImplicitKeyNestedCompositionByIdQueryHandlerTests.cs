using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositionById;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            var expectedEntity = existingOwnerEntity.ImplicitKeyNestedCompositions.First();
            fixture.Customize<GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery>(comp => comp
                .With(x => x.ImplicitKeyAggrRootId, existingOwnerEntity.Id)
                .With(x => x.Id, expectedEntity.Id));
            var testQuery = fixture.Create<GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery>();
            yield return new object[] { testQuery, existingOwnerEntity, expectedEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesImplicitKeyNestedComposition(
            GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery testQuery,
            ImplicitKeyAggrRoot existingOwnerEntity,
            ImplicitKeyNestedComposition existingEntity)
        {
            // Arrange
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            implicitKeyAggrRootRepository.FindByIdAsync(testQuery.ImplicitKeyAggrRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler(implicitKeyAggrRootRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ImplicitKeyAggrRootAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.With(p => p.ImplicitKeyNestedCompositions, new List<ImplicitKeyNestedComposition>()));
            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            fixture.Customize<GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery>(comp => comp
                .With(p => p.ImplicitKeyAggrRootId, existingOwnerEntity.Id));
            var testQuery = fixture.Create<GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery>();
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            implicitKeyAggrRootRepository.FindByIdAsync(testQuery.ImplicitKeyAggrRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));


            var sut = new GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler(implicitKeyAggrRootRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}