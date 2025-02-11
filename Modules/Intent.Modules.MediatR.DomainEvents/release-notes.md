### Version 5.0.3

- Improvement: Added support for `Value Object` collections.
- Fixed: Domain Event handlers sometimes ended up with a `throw new NotImplementedException...` when there was an implementation.

### Version 5.0.2

- Fixed: Optional query filters not working with DbContext oriented data-access.

### Version 5.0.1

- Improvement: Support for optional query filters when querying lists (or paginated lists) of entities.
 
### Version 5.0.0

- Support for explicit modeling of Domain Event Handlers and the Advanced Mapping capabilities available in Intent Architect 4.1.0

### Version 4.2.1

- `Publish` method on the domain event service now accepts a `CancellationToken`.

### Version 4.2.0

- Converted Default Domain Event Handler to use the C# Builder implementation paradigm. These classes can now be easily extended.

### Version 4.1.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.6

- Fixed: `async` keyword added to methods where only `Task` was returned.

### Version 3.3.4

- Fixed: Event handlers not respecting folder paths from the designer.