PS C:\Users\mccre\dev\boiler> dotnet build
Restore complete (0.5s)
  DTOs succeeded (0.2s) → src\shared\DTOs\bin\Debug\net9.0\DTOs.dll
  Contracts succeeded (0.1s) → src\shared\Contracts\bin\Debug\net9.0\Contracts.dll
  Common succeeded (0.1s) → src\shared\Common\bin\Debug\net9.0\Common.dll
  PermissionService.Tests succeeded (0.1s) → tests\unit\PermissionService.Tests\bin\Debug\net9.0\PermissionService.Tests.dll
  RoleService.Tests succeeded (0.1s) → tests\unit\RoleService.Tests\bin\Debug\net9.0\RoleService.Tests.dll
  AuthService succeeded (0.1s) → src\services\AuthService\bin\Debug\net9.0\AuthService.dll
  ApiGateway succeeded (0.1s) → src\services\ApiGateway\bin\Debug\net9.0\ApiGateway.dll
  UserService succeeded (0.2s) → src\services\UserService\bin\Debug\net9.0\UserService.dll
  AuthService.Tests succeeded (0.2s) → tests\unit\AuthService.Tests\bin\Debug\net9.0\AuthService.Tests.dll
  UserService.Tests succeeded (0.1s) → tests\unit\UserService.Tests\bin\Debug\net9.0\UserService.Tests.dll
  ApiGateway.Tests succeeded (0.2s) → tests\unit\ApiGateway.Tests\bin\Debug\net9.0\ApiGateway.Tests.dll
  UserService.IntegrationTests failed with 3 error(s) and 1 warning(s) (0.5s)
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\TestDataSeeder.cs(356,103): error CS1061: 'User' does not contain a definition for 'TenantId' and no accessible extension method 'TenantId' accepting a first argument of type 'User' could be found (are you missing a using directive or an assembly reference?)
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\TestDataSeeder.cs(951,55): error CS1061: 'User' does not contain a definition for 'TenantId' and no accessible extension method 'TenantId' accepting a first argument of type 'User' could be found (are you missing a using directive or an assembly reference?)
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\TestDataSeeder.cs(962,94): error CS1061: 'User' does not contain a definition for 'TenantId' and no accessible extension method 'TenantId' accepting a first argument of type 'User' could be found (are you missing a using directive or an assembly reference?)
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(150,43): warning CS8602: Dereference of a possibly null reference.

Build failed with 3 error(s) and 1 warning(s) in 1.9s
