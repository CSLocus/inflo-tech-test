Hey! Thanks for taking the time to read this.

I'd just like to preface this by saying I've been a backend developer for the past 2-3 years, so a return to full-stack feels quite nice although my skills were definitely rusty. 
I've actually developed a product pretty much identical to this and can show this off in any next stage interview - it uses Identity Server to handle authorisation + authentication.

Unfortunately due to time constraints I'm unable to complete task 4 or rewrite the application in Blazor - but this is definitely doable. 

I've summarised my thoughts/ progress below, so if I'm successful in this stage we can definitely discuss the following if needed.

- Upgraded to .NET 8 for LTS, as well as primary constructor and Argument.ThrowIfNull functionality

- Could have implemented Bogus for fake data generation but seems a bit overkill for this app in particular

- Delete, view, edit user should all be the same screen - Delete is effectively editing a user, viewing a user should have the same data as an edit screen

- There is no need to implement a 'GetById' functionality in the data context layer, as we are still working with IQueryable in the service layer

- A datagrid component would have been a good shout for the user list, but it seemed a little heavyweight for something with so few records. If looking to scale definitely an option to look into

- I was going to implement Identity Server, Auth0 or Keycloak but unfortunately can't spend any longer on the project. I have demonstrable experience in this field and a proven track record of leading projects focused around these technologies.

- I've added CI that runs upon a PR being created, you can see this if you view the active PR

- I don't have the capacity to have a test azure instance set up for this to deploy to - but if I did I'd have secrets in KeyVault and deployment slots as well.

I've added the following libraries/ tools:

- .NET 8
- Boxicon (FontAwesome alternative)

This was a pretty fun one to be honest, so hopefully I'll hear from you guys soon :)
