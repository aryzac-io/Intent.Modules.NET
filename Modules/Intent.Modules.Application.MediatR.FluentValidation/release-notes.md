### Version 4.5.0

- Improvement: Removed `// IntentMatch(...)` code management instructions from templates which are no longer needed since version `4.4.0` of the `Intent.OutputManager.RoslynWeaver` module.
- Improvement: Added CascadeMode option on Fluent Validations to specify this behaviour.
- Fixed: Text constraint and Index unique constraints will work again with advanced mapping scenarios.

### Version 4.4.3

- Fixed: Migration improvement using Roslyn instead of Regex to avoid updating incorrect IntentManaged attributes.

### Version 4.4.2

- Improvement: Added support for the [new Email Address](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.Application.FluentValidation/release-notes.md#version-383) option on the Validation stereotype.
- Fixed: A Software Factory crash around `DTO Unique Constraint validation` for DTO's mapping to constructors / operations.

### Version 4.4.0

- Improvement: `[IntentManaged(Mode.Fully)]` is no longer added to the `ConfigureValidationRules` method as it's redundant.
- Fixed: Nested DTO Validators introduced and will inject `IServiceProvider` to resolve Validators via DI.

### Version 4.3.2

- Improvement: Further consolidated/generalized logic with other FluentValidation modules.

### Version 4.3.0

- Update : Upgraded MediatR Package v12.
- Updated: All common logic for generating validations has been centralized into the `Intent.Application.FluentValidation` module version `3.7.2` to ensure consistency and parity between all `FluentValidation` modules.
- Updated: `Has Custom Validation` stereotype property has been replaced with, and its value will be automatically migrated to, a new `Must` property named in alignment with the `MustAsync()` method call being created.
- Updated: `Custom` stereotype property has been added which creates a `CustomAsync()` method call and corresponding method.

### Version 4.2.1

- Fixed spelling mistake in generated exception message.

### Version 4.2.0

- Updated: `FluentValidation` to version `11.6.0`.

### Version 4.1.2

- Upgrade: `CommandValidatorTemplate` moved to the the `CSharpFileBuilder` paradigm.

### Version 4.1.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Update: Refactored module based on new developments.
