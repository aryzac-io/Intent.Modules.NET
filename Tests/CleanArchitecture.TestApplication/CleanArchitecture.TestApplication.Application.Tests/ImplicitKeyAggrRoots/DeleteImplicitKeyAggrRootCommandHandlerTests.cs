using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class DeleteImplicitKeyAggrRootCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<ImplicitKeyAggrRoot>();
            fixture.Customize<DeleteImplicitKeyAggrRootCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesImplicitKeyAggrRootFromRepository(
            DeleteImplicitKeyAggrRootCommand testCommand,
            ImplicitKeyAggrRoot existingEntity)
        {
            // Arrange
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            implicitKeyAggrRootRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteImplicitKeyAggrRootCommandHandler(implicitKeyAggrRootRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            implicitKeyAggrRootRepository.Received(1).Remove(Arg.Is<ImplicitKeyAggrRoot>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidImplicitKeyAggrRootId_ReturnsNotFound()
        {
            // Arrange
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootCommand>();
            implicitKeyAggrRootRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<ImplicitKeyAggrRoot>(default));


            var sut = new DeleteImplicitKeyAggrRootCommandHandler(implicitKeyAggrRootRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}