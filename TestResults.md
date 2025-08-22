 UserService.IntegrationTests.CacheInvalidationIntegrationTests.ConcurrentCacheInvalidation_ShouldNotCauseDataCorruption
   Source: CacheInvalidationIntegrationTests.cs line 194
   Duration: 222 ms

  Message: 
Expected finalPermissions to be a collection with 0 item(s) because Concurrent operations should not corrupt data, but {"users.view", "users.edit", "users.create", "users.delete", "users.view_all", "users.manage_roles", "roles.view", "roles.create", "roles.edit", "roles.delete", "roles.assign_users", "roles.manage_permissions", "tenants.view", "tenants.create", "tenants.edit", "tenants.delete", "tenants.initialize", "tenants.view_all", "tenants.manage_settings", "reports.view", "reports.create", "reports.export", "reports.schedule", "permissions.view", "permissions.create", "permissions.edit", "permissions.delete", "permissions.manage"}
contains 28 item(s).

With configuration:
- Use declared types and members
- Compare enums by value
- Compare tuples by their properties
- Compare anonymous types by their properties
- Compare records by their members
- Include non-browsable members
- Include all non-private properties
- Include all non-private fields
- Match member by name (or throw)
- Be strict about the order of items in byte arrays
- Without automatic conversion.


  Stack Trace: 
XUnit2TestFramework.Throw(String message)
TestFrameworkProvider.Throw(String message)
CollectingAssertionStrategy.ThrowIfAny(IDictionary`2 context)
AssertionScope.Dispose()
EquivalencyValidator.AssertEquality(Comparands comparands, EquivalencyValidationContext context)
EquivalencyValidator.AssertEquality(Comparands comparands, EquivalencyValidationContext context)
StringCollectionAssertions`2.BeEquivalentTo(IEnumerable`1 expectation, Func`2 config, String because, Object[] becauseArgs)
StringCollectionAssertions`2.BeEquivalentTo(IEnumerable`1 expectation, String because, Object[] becauseArgs)
CacheInvalidationIntegrationTests.ConcurrentCacheInvalidation_ShouldNotCauseDataCorruption() line 222
--- End of stack trace from previous location ---
