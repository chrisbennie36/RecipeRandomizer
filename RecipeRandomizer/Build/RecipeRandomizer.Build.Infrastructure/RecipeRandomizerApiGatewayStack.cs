using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Constructs;

namespace RecipeRandomizer.Build.Infrastructure;

public class RecipeRandomizerApiGatewayStackProps : StackProps
{
    public required string BaseUrl { get; set; }
}

public class RecipeRandomizerApiGatewayStack : Stack
{
    public RecipeRandomizerApiGatewayStack(Construct scope, string id, RecipeRandomizerApiGatewayStackProps props) : base(scope, id)
    {
        RestApi restApi = new RestApi(this, "recipe-randomizer-api-gateway", new RestApiProps 
        {
            RestApiName = "RecipeRandomizerRestApi",
            Description = "API Gateway to route traffic to the Recipe Randomizer API EC2 instance"
        });

        _ = new CfnOutput(this, "recipe-randomizer-api-gateway-rest-api-id", new CfnOutputProps
        {
            Value = restApi.RestApiId,
            ExportName = "RecipeRandomizerApiGatewayRestApiId"
        });

        _ = new CfnOutput(this, "recipe-randomizer-api-gateway-root-resource-id", new CfnOutputProps
        {
            Value = restApi.RestApiRootResourceId,
            ExportName = "RecipeRandomizerApiGatewayRootResourceId"
        });

        AddRestApiResourceProxy(restApi, "Recipe", props.BaseUrl);
        AddRestApiResourceProxy(restApi, "RecipePreferences", props.BaseUrl);
        AddRestApiResourceProxy(restApi, "UserRecipePreferences", props.BaseUrl);
    }

    private IRestApi? GetExistingRestApi(Construct scope, string restApiIdImportKey, string rootResourceIdKey)
    {
        string restApiId = Fn.ImportValue(restApiIdImportKey);
        string rootResourceId = Fn.ImportValue(rootResourceIdKey);

        if(string.IsNullOrWhiteSpace(restApiId) || string.IsNullOrWhiteSpace(rootResourceId))
        {
            return null;
        }

        return RestApi.FromRestApiAttributes(scope, "recipe-randomizer-api-gateway", new RestApiAttributes 
        {
            RootResourceId = rootResourceId,
            RestApiId = restApiId
        });
    }

    private void AddRestApiResourceProxy(IRestApi restApi, string resourceName, string apiBaseUrl)
    {
        restApi.Root.AddResource(resourceName).AddProxy(new ProxyResourceOptions 
        {
            DefaultIntegration = new Integration(new IntegrationProps {
                Type = IntegrationType.HTTP_PROXY,
                Uri = $"{apiBaseUrl}/api/{resourceName}/",
                IntegrationHttpMethod = "ANY" 
            })
        });
    } 
}
