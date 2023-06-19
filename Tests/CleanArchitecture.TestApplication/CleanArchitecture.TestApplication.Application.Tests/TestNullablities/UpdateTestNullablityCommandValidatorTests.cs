using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using CleanArchitecture.TestApplication.Application.TestNullablities.UpdateTestNullablity;
using CleanArchitecture.TestApplication.Domain.Nullability;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.TestNullablities
{
    public class UpdateTestNullablityCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateTestNullablityCommand testCommand)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            // Act
            var result = await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(Unit.Value));

            // Assert
            result.Should().Be(Unit.Value);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public static IEnumerable<object[]> GetFailedResultTestData()
        {
            var fixture = new Fixture();
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.MyEnum, () => default));
            var testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, "MyEnum", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.Str, () => default));
            testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, "Str", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            UpdateTestNullablityCommand testCommand,
            string expectedPropertyName,
            string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            // Act
            var act = async () => await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(Unit.Value));

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<UpdateTestNullablityCommand, Unit> GetValidationBehaviour()
        {
            return new ValidationBehaviour<UpdateTestNullablityCommand, Unit>(new[] { new UpdateTestNullablityCommandValidator() });
        }
    }
}