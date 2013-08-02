Final steps to start using FormsToTokenAccessAuthentication:

1. Configure your authentication realm:
   a. Open your project's Web.config file
   b. Set the value attribute of the /configuration/appSettings <add> element 
      (with the key "TokenAccessAuthentication-Realm") to be your desired 
      authentication realm.  Typically this is the URL of your API, but may be 
      different depending on the deployment environment

2. Register the FormsToTokenAccessAuthentication token error Web API message 
   handler:
   a. Open your project's App_Start\WebApiConfig.cs file
   b. Add the following code to the Register method:

        config.MessageHandlers.Add(
            new TokenErrorResponseMessageHandler(
                ConfigurationManager
                    .AppSettings["TokenAccessAuthentication-Realm"]));

3. Customize (as needed) the files added to the project to enable the 
   Authentication controller (POST /Authentication), which can be used by 
   clients to authenticate and obtain a token:
   * Controllers\AuthenticationController.cs
   * Models\Login.cs
   * Models\Authentication.cs
