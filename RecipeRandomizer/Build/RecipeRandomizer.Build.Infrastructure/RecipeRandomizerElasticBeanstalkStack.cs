using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ElasticBeanstalk;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3.Assets;
using Constructs;

namespace RecipeRandomizer.Build.Infrastructure;

public class RecipeRandomizerElasticBeanstalkStackProps : StackProps
{
    public required string ApplicationName { get; set; }
}

public class RecipeRandomizerElasticBeanstalkStack : Stack
{
    public CfnApplication recipeRandomizerElasticBeanstalkStack { get; set; }
    public CfnEnvironment RecipeRandomizerEbEnvironment { get; set; }

    public RecipeRandomizerElasticBeanstalkStack(Construct scope, string id, RecipeRandomizerElasticBeanstalkStackProps props) : base(scope, id)
    {
        IRole role = Role.FromRoleName(this, "recipe-randomizer-eb-app-role", "aws-elasticbeanstalk-ec2-role");

        var instanceProfile = new InstanceProfile(this, "recipe-randomizer-eb-instance-profile", new InstanceProfileProps 
        {
            Role = role,
            InstanceProfileName = "RecipeRandomizerEBInstanceProfile"
        });

        _ = new CfnOutput(this, "recipe-randomizer-eb-iam-role", new CfnOutputProps 
        {
            Value = role.RoleArn
        });

        var archive = new Asset(this, "recipe-randomizer-app-zip-location", new AssetProps
        {
            Path = "../application.zip"
        });

        recipeRandomizerElasticBeanstalkStack = new CfnApplication(this, "recipe-randomizer-elb-app", new CfnApplicationProps
        {
            ApplicationName = props.ApplicationName,
        });

        CfnApplicationVersion applicationVersion = new CfnApplicationVersion(this, "recipe-randomizer-elb-app-version", new CfnApplicationVersionProps
        {
            ApplicationName = props.ApplicationName,
            SourceBundle = new CfnApplicationVersion.SourceBundleProperty
            {
                S3Bucket = archive.S3BucketName,
                S3Key = archive.S3ObjectKey
            }
        });

        CfnEnvironment recipeRandomizerElasticBeanstalkEnvironment = new CfnEnvironment(this, "recipe-randomizer-elb-environment", new CfnEnvironmentProps
        {
            ApplicationName = props.ApplicationName,
            CnamePrefix = "reciperandomizer",
            OptionSettings = new CfnEnvironment.OptionSettingProperty[] 
            {
                new CfnEnvironment.OptionSettingProperty{ Namespace = "aws:autoscaling:launchconfiguration", OptionName = "IamInstanceProfile", Value = instanceProfile.InstanceProfileArn },
                new CfnEnvironment.OptionSettingProperty {Namespace = "aws:autoscaling:launchconfiguration", OptionName = "RootVolumeType", Value = "gp3"},
                new CfnEnvironment.OptionSettingProperty{ Namespace = "aws:autoscaling:asg", OptionName = "MaxSize", Value = "1" },
                new CfnEnvironment.OptionSettingProperty{ Namespace = "aws:autoscaling:asg", OptionName = "MinSize", Value = "1" }
            },
            EnvironmentName = "RecipeRandomizer",   //Must be > 4 chars
            SolutionStackName = "64bit Amazon Linux 2023 v3.2.1 running .NET 8",
            VersionLabel = applicationVersion.Ref   //Critical apparently
        });

        RecipeRandomizerEbEnvironment = recipeRandomizerElasticBeanstalkEnvironment;

        recipeRandomizerElasticBeanstalkEnvironment.AddDependency(recipeRandomizerElasticBeanstalkStack);
        applicationVersion.AddDependency(recipeRandomizerElasticBeanstalkStack);

        string existingSecurityGroupId = Fn.ImportValue("RecipeRandomizerDbSecurityGroupId");

        ISecurityGroup securityGroup = SecurityGroup.FromSecurityGroupId(this, "recipe-randomizer-security-group", existingSecurityGroupId);

        securityGroup.AddIngressRule(Peer.Ipv4("34.205.203.205/32"), Port.AllTcp(), "RecipeRandomizerPublicIp");
        securityGroup.AddIngressRule(Peer.Ipv4($"172.31.4.22/32"), Port.AllTcp(), "RecipeRandomizerPrivateIp");
    }
}
