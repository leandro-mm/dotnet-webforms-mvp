# Dotnet WebForms Application with MVP Pattern
> code examples using dotnet and C#

## The Context
The main issue with standard Web Forms is that the Ul logic, in code-behind files, is often tightly mixed with business logic, making it very difficult to write unit tests.

 ## The Solution: MVP Pattern 
For a .NET WebForms application, MVP is an excellent and well-suited architectural choice. Its primary value lies in making an existing, complex WebForms application testable and maintainable. The MVP pattern will enable the following features effectively:
- **Testability**: The core logic lives in the Presenter, which is a plain C# class that doesn't depend on the ASP.NET runtime. This allows you to write unit tests for it without spinning up a web server
- **Separation of Concerns**: MVP forces a clear separation. The View (the .aspx page) is just a "dumb" interface (IUserView e.g.). It only sets or displays data and raises events, while the Presenter does the heavy lifting, like calling the database and deciding what to show next.

 ## MVP Passive View 
- This is one of few ways to implement the MVP pattern.
- In the Passive View mode the View has zero logic, making it the most testable. 

 ## The Project
 - We are going to be building a Movie Manager application using the MVP pattern.
 - Whithin this project we are making use of validators and a gridView.
   
 |  | |
|---------|------------|
|![Presenter Image](WebForms-MVP/Assets/project_structure.png)|![Presenter Image](WebForms-MVP/Assets/test_structure.png)|

# Other stuffs
 ```csharp 
public interface MyInterface
{
    //to be implemented
}
```

| Jut | A | Table Focus |
|---------|------------|---------------|
|col1|col2|col3|


