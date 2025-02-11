### Version 4.5.0

- Improvement: Moved `PagedList<T>` type into the `Intent.EntityFrameworkCore 4.6.0` module since pagination is an separate concert to the repository pattern.

### Version 4.3.0

- Improvement: Added several repository method overloads to bettere support more advanced queryOptions though LINQ.

### Version 4.2.10

- New Feature: Added a `Database Setting` namely `Add synchronous methods to repositories`, when this is on, the EF Repositories will contain synchronous versions of their operations.

### Version 4.2.7

- Fixed: In some cases, custom repositories weren't adding a using directive when `IReadOnlyCollection<T>` was used.

### Version 4.2.6

- Update: Support for Composite PrimaryKeys on repositories.
- Fixed: `User-Defined Table Type Settings` stereotype will no longer be automatically applied to `Data Contract` elements (it can still be applied manually).

### Version 4.2.5

- Stored procedures now support `OUTPUT` and `Table-Valued` parameters.

### Version 4.2.4

- FindByIdAsync indicates that return type could be null.

### Version 4.2.3

- Update: Removed various compiler warnings.
- Added new `IEfRepository` to contain methods with `Expression`s and `IQueryable`s which were removed from interfaces in `Intent.Entities.Repositories.Api` 4.0.5.
- Added `Update` method to repositories. With default EF Core configuration this is not needed, but is now available.

### Version 4.2.2

- Added support for executing Stored Procedures. To use a Stored Procedure:
	- Create a `Repository` in the Domain Designer (either in the package root or a folder).
	- You can optionally set the "type" of the repository to a `Class` which will extend the existing repository which is already generated for it, otherwise if no "type" is specified a new Repository is generated.
	- On a repository you can create `Stored Procedure`s.
	- At this time, the module supports a Stored Procedure returning: nothing, an existing `Class` or a `Data Contract` (`Domain Object`).
	- The Software Factory will generate methods on the Repositories for calling the Stored Procedures.
- Update: Removes some warnings from generated code.


### Version 4.2.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.0

- Update: Repositories will only be generated for Classes created inside a "Domain Package".

### Version 4.0.2

- Update: Converted RepositoryBase into Builder Pattern version.

### Version 4.0.1

- Update: Internal template changes.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.14

- Update: Added `IQueryable` option for `FindAsync` operation.
