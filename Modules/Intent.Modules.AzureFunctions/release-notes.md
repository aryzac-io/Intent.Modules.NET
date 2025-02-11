### Version 4.0.13

- New Feature: Azure Functions can now receive RabbitMQ triggers.
- Improvement: Removed `Intent.Application.DTOs` dependency since DTOs can be resolved using Role names.
- Improvement: Startup file is now generated using a File Builder Template.

### Version 4.0.11

- Fixed: Queue Trigger handler parameter name clashes with inline variable name.

### Version 4.0.9

- New Feature: Added support for CosmosDB Trigger Azure Functions.

### Version 4.0.8

- Fixed: Improved Http Trigger support collection based parameters.

### Version 4.0.7

- Improvement: Improved Http Trigger support for Header parameters.
- Improvement: Improved Queue Trigger support, including receiving messages as `QueueMessage` and output binding support for `Azure Functions`.
- Improvement: Removed compiler warning.
- Improvement: Services that produce Azure Functions will no longer dump its own functions in the root folder of the API project but in their own folders.
- Improvement: Moved away from Newtonsoft JSON and now using System.Text.Json instead.

### Version 4.0.5

- Improvement: Interop dependencies for App Services and MediatR dispatch.

### Version 4.0.3

- Improvement: Version bump of Nuget Package `Microsoft.Extensions.DependencyInjection` to 6.0.1

### Version 4.0.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Improvement: Support for multiple paradigms of expressing Azure Functions (i.e. through Stereotypes applied to Commands / Queries / Service Operations / etc.). The appropriate supporting modules will need to be installed to enable this.
- Improvement: Supports additional trigger types: Timer Trigger, EventHub Trigger and Manual Trigger for Azure Functions.

### Version 4.0.0

- Improvement: Changed fundamental paradigm for expressing Azure Functions to explicitly modeling the functions inside of the Services designer.

### Version 3.3.9

- Improvement: Azure Service Bus Queues and File Storage Queues are now better supported.
- Improvement: Done internal code cleanup.