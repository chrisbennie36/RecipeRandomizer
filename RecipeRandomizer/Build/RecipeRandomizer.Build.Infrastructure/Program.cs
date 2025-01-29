using Amazon.CDK;
using Utilities.RecipeRandomizer.Infrastructure.CDK.Helpers;

namespace RecipeRandomizer.Build.Infrastructure;

sealed class Program
{
    static string Region = "us-east-1";

    public static void Main(string[] args)
    {
        var app = new App();

        RecipeRandomizerElasticBeanstalkStack ebStack = new RecipeRandomizerElasticBeanstalkStack(app, "recipe-randomizer-elastic-beanstalk-stack", new RecipeRandomizerElasticBeanstalkStackProps 
        {
            ApplicationName = "RecipeRandomizer",
            Env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_ACCOUNT", EnvironmentVariableTarget.User),
                Region = Region
                //Region = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_REGION", EnvironmentVariableTarget.User)
            }
        });

        RecipeRandomizerApiGatewayStack gatewayStack = new RecipeRandomizerApiGatewayStack(app, "recipe-randomizer-api-gateway-stack", new RecipeRandomizerApiGatewayStackProps 
        {
            BaseUrl = CdkHelpers.GetElasticBeanstalkDomain(ebStack.RecipeRandomizerEbEnvironment.CnamePrefix ?? string.Empty, Region)
        });

        gatewayStack.AddDependency(ebStack);

        app.Synth();
    }
}
