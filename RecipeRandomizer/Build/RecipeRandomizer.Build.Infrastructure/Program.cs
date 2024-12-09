using Amazon.CDK;

namespace RecipeRandomizer.Build.Infrastructure;

sealed class Program
{
    public static void Main(string[] args)
    {
        var app = new App();

        RecipeRandomizerElasticBeanstalkStack ebStack = new RecipeRandomizerElasticBeanstalkStack(app, "recipe-randomizer-elastic-beanstalk-stack", new RecipeRandomizerElasticBeanstalkStackProps 
        {
            ApplicationName = "RecipeRandomizer",
            Env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_ACCOUNT", EnvironmentVariableTarget.User),
                Region = "us-east-1"
                //Region = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_REGION", EnvironmentVariableTarget.User)
            }
        });

        //DatabaseMigrationLambdaStack dbMigrationLambdaStack = new DatabaseMigrationLambdaStack(app, "database-migration-lambda-stack");

        /*DatabaseStack dbStack = new DatabaseStack(app, "database-stack", new DatabaseStackProps
        {
            MigrationLambda = dbMigrationLambdaStack.DatabaseMigrationLambda,
            Env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_ACCOUNT", EnvironmentVariableTarget.User),
                Region = "us-east-1",
                //Region = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_REGION", EnvironmentVariableTarget.User)
            }
        });

        //dbMigrationLambdaStack.AddDependency(ebStack);*/

        app.Synth();
    }
}
