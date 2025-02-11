### Version 2.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).

### Version 1.1.1

- Fixed: Nullability related compiler warnings.

### Version 1.1.0

- Improvement : Upgraded to MediatR 12.

### Version 1.0.5

- Fixed: Software Factory executions on case-sensitive file systems would fail due to file not found errors.
- Fixed: Under certain circumstances the `EventHandlerController` file would not be generated.
- Fixed: `EventBusImplementation` was incorrectly being generated into the `Application` project instead of the `Infrastructure` project.

### Version 1.0.4

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.0.2

- Fixed: Wasn't automatically installing `Intent.Application.MediatR.CRUD.Eventing` and `Intent.Application.ServiceImplementations.CRUD.Eventing` when appropriate.
- Fixed: Updated dependencies.

### Version 1.0.1

- Fixed: Event handlers would be generated into a sub-folder with the same name as the message which depending on the message name would cause conflicts between the fully qualified message type and the event handler's namespace.
- Fixed: `IntentManaged` for the `Body` of event handlers classes was set to `Fully` instead of `Merge`.
- Fixed: Event's were being skipped over by `IUnitOfWork` implementations due to not implementing `ICommand`.
