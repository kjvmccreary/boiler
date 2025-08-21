Building Test Projects
========== Starting test run ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.04]   Starting:    UserService.Tests
?? Starting CreateUserAsync test with detailed debugging
?? TenantProvider returns: 1
?? About to call CreateUserAsync
?? PasswordService.HashPassword called
?? CreateUserAsync returned: Success=False, Message='An error occurred while creating the user'
?? EXCEPTION in CreateUserAsync test: Xunit.Sdk.XunitException: Expected result.Success to be true because CreateUser should succeed but failed with: An error occurred while creating the user, but found False.
   at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
   at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
   at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
   at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
   at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
   at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
   at FluentAssertions.Primitives.BooleanAssertions`1.BeTrue(String because, Object[] becauseArgs)
   at UserService.Tests.UserServiceTests.CreateUserAsync_WithValidData_ShouldCreateActiveUser() in C:\Users\mccre\dev\boiler\tests\unit\UserService.Tests\UserServiceTests.cs:line 190
[xUnit.net 00:00:00.55]     UserService.Tests.UserServiceTests.CreateUserAsync_WithValidData_ShouldCreateActiveUser [FAIL]
[xUnit.net 00:00:00.55]       Expected result.Success to be true because CreateUser should succeed but failed with: An error occurred while creating the user, but found False.
[xUnit.net 00:00:00.55]       Stack Trace:
[xUnit.net 00:00:00.55]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:00.55]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:00.55]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:00.55]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:00.55]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:00.55]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:00.55]            at FluentAssertions.Primitives.BooleanAssertions`1.BeTrue(String because, Object[] becauseArgs)
[xUnit.net 00:00:00.55]         C:\Users\mccre\dev\boiler\tests\unit\UserService.Tests\UserServiceTests.cs(190,0): at UserService.Tests.UserServiceTests.CreateUserAsync_WithValidData_ShouldCreateActiveUser()
[xUnit.net 00:00:00.55]         --- End of stack trace from previous location ---
[xUnit.net 00:00:00.55]   Finished:    UserService.Tests
========== Test run finished: 1 Tests (0 Passed, 1 Failed, 0 Skipped) run in 563 ms ==========
