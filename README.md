# DemoShop - Test assignment for employment

ASP.NET Core 2.0, C#, MS SQL Server, Suncfusion UI Controls (EJS1)

The Demo web application of the accounting system for the receipt / expenditure of goods in the store.

I was asked to perform this test task when applying for a job 8 months ago.

To start the application, you will need to create a configuration of secret variables in "Manage User Secrets" and insert this code there, substituting your values
~~~~
{
  "DefaultIdentityData": {
    "SuperAdministrator": {
      "Password": "P@ssw0rd"
    }
  },
  "Recaptcha": {
    "SiteKey": "HereisRecaptchaSiteKey",
    "SecretKey": "HereisRecaptchaSecretKey"
  },
  "SMTPSenderOptions": {
    "SMTPLogin": "no-reply@example.com",
    "SMTPPassword": "P@ssw0rd2"
  },
  "SparkPost": {
    "ApiKey": "HereisSparkPostApiKey"
  }
}
~~~~

## Developers

Alexey Chernyavskiy, [https://github.com/pprometey](https://github.com/pprometey)