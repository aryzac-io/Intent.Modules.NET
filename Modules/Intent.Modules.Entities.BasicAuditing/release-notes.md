### Version 1.0.2

- Fixed: Any manually added attributes called `CreatedBy`, `CreatedDate`, `UpdatedBy` or `UpdatedDate` would be automatically removed on creation, regardless of whether or not auditing was ever applied to an entity.
- Improvement: The DbContext now ensures that its impossible to update the `CreatedBy` and `CreatedDate` column values through use of `entry.Property("<name>").IsModified = false` statements.

### Version 1.0.1

- Fixed: Small variable correction.

### Version 1.0.0

- New Feature: Initial release.
