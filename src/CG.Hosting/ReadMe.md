### CG.Hosting

This package contains standardized hosting extensions that are typically used by CodeGator applications. Placing that code here reduces some of the boilerplate code required to start a new project.

#### Notes

This package optionally wires up user secrets. For that to happen, the local configuration must have an entry for the secrets id.

Here is what that entry should look like:

```json
{
  "UserSecrets": {
    "Id": "your secret id here"
  }
}
```

The actual secrets id is taken from your Visual Studio project. To find it, follow these steps:

(if you prefer to work at a command line)

1. at a .NET Core CLI, enter: dotnet user-secrets init

(if you prefer to work within Visual Studio)

1. Open the Solution Explorer window in Visual Studio.
2. Right click on the application project.
3. Choose Manage User Secrets from the context menu.
4. Add whatever secrets your projects needs in the JSON file that Visual Studio displays.

(the next step depends on whether your project is a web application, or not.)

(for a non-web application)

5. Select the application project in the Solution Explorer window.
6. When Visual Studio displays the project XML, copy the value of the <UserSecretsId> node.

(for a web application)

5. Open the Properties Explorer window in Visual Studio.
6. Select the application project in the Solution Explorer window.
7. Copy the value of the UserSecretsId property from the Properties Explorer window.

Once you have the secrets id for your project, enter that value in the appsettings, as shown above.

---

There are many other ways to use developer secrets in code. This [LINK](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) illustrates some of those ways.

Keep in mind, developer secrets are barely secret and are mostly just good for keeping sensitive information out of source control. For secure secret storage, use something like an Azure key vault instead.