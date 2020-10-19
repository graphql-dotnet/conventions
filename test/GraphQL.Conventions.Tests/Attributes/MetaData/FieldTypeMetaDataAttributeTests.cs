using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Language.AST;
using GraphQL.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class FieldTypeMetaDataAttributeTests : TestBase
    {
        [Test]
        public void TopLevelQuery_Should_Have_Correct_CustomAttributeValue()
        {
            var type = TypeInfo<CustomAttribute_Query>();
            var field = type.ShouldHaveFieldWithName("node");
            var customAttributes = field.AttributeProvider.GetCustomAttributes(false);
            var result = customAttributes.FirstOrDefault(x => x.GetType() == typeof(TestCustomAttribute)) as TestCustomAttribute;
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Permission == nameof(SomeTopLevelValidation));
        }

        [Test]
        public void ClassMethod_Should_Have_Correct_CustomAttributeValue()
        {
            var type = TypeInfo<TestType>();
            var field = type.ShouldHaveFieldWithName("testMethod");
            var customAttributes = field.AttributeProvider.GetCustomAttributes(false);
            var result = customAttributes.FirstOrDefault(x => x.GetType() == typeof(TestCustomAttribute)) as TestCustomAttribute;
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Permission == nameof(SomeMethodValidation));
        }

        [Test]
        public void ClassMethod_Without_CustomAttribute_ShouldBeNull()
        {
            var type = TypeInfo<TestType>();
            var field = type.ShouldHaveFieldWithName("noAttribute");
            var customAttributes = field.AttributeProvider.GetCustomAttributes(false);
            var result = customAttributes.FirstOrDefault(x => x.GetType() == typeof(TestCustomAttribute)) as TestCustomAttribute;
            Assert.IsTrue(result == null);
        }

        [Test]
        public async Task When_UserIsMissingAll_ValidationFieldTypeMetaData_ErrorsAreReturned()
        {
            var result = await Resolve_Query();
            result.ShouldHaveErrors(2);
            var expectedMessages = new[] 
            { 
                $"Required validation '{nameof(SomeTopLevelValidation)}' is not present. Query will not be executed.", 
                $"Required validation '{nameof(SomeMethodValidation)}' is not present. Query will not be executed." 
            }; 

            var messages = result.Errors.Select(e => e.Message);
            Assert.IsTrue(messages.All(x => expectedMessages.Contains(x)));
        }

        [Test]
        public async Task When_UserHas_Requested_ValidationFieldTypeMetaData_ThereAreNoErrors()
        {
            var result = await Resolve_Query(selectedFileds: "noAttribute name", accessPermissions: nameof(SomeTopLevelValidation));
            result.ShouldHaveNoErrors();
        }

        [Test]
        public async Task When_UserHasAll_Requested_ValidationFieldTypeMetaData_ThereAreNoErrors()
        {
            var result = await Resolve_Query(accessPermissions: new[] { nameof(SomeTopLevelValidation), nameof(SomeMethodValidation) });
            result.ShouldHaveNoErrors();
        }

        private async Task<ExecutionResult> Resolve_Query(string selectedFileds = "testMethod noAttribute name", params string[] accessPermissions)
        {
            var engine = GraphQLEngine
                .New<CustomAttribute_Query>();

            var user = new TestUserContext(accessPermissions);

            var result = await engine
                .NewExecutor()
                .WithQueryString("query { node { " + selectedFileds + " } }")
                .WithUserContext(user)
                .WithValidationRules(new[] { new TestValidation() })
                .Execute();

            return result;
        }
    }

    public class TestUserContext : IUserContext 
    {
        public TestUserContext(params string[] accessPermissions)
        {
            AccessPermissions = accessPermissions;
        }

        public string[] AccessPermissions { get; }
    }

    public class CustomAttribute_Query
    {
        [TestCustom(nameof(SomeTopLevelValidation))]
        public TestType Node() => new TestType();
    }

    public class TestType 
    {
        [TestCustom(nameof(SomeMethodValidation))]
        public string TestMethod() => "TestMethod";

        public string NoAttribute() => "NoAttribute";

        public string Name { get; set; } = "TestType";
    }

    public class SomeTopLevelValidation { }

    public class SomeMethodValidation { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)] 
    public class TestCustomAttribute : FieldTypeMetaDataAttribute
    {
        public TestCustomAttribute(string permission)
        {
            Permission = permission;
        }

        public string Permission { get; }

        public override string Key() => nameof(TestCustomAttribute);

        public override object Value() => Permission;
    }

    public class TestValidation : IValidationRule
    {
        public INodeVisitor Validate(ValidationContext context)
        {
            var userContext = context.UserContext as UserContextWrapper;
            var user = userContext.UserContext as TestUserContext;

            return new EnterLeaveListener(_ =>
            {
                _.Match<Field>(node =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();
                    if (fieldDef == null) return;
                    if (fieldDef.Metadata != null && fieldDef.HasMetadata(nameof(TestCustomAttribute)))
                    {
                        var permissionMetaData = fieldDef.Metadata.First(x => x.Key == nameof(TestCustomAttribute));
                        var requiredValidation = permissionMetaData.Value as string;

                        if(!user.AccessPermissions.Any(p => p == requiredValidation))
                            context.ReportError(new ValidationError( /* When reporting such errors no data would be returned use with cautious */
                                context.OriginalQuery,
                                "Authorization",
                                $"Required validation '{requiredValidation}' is not present. Query will not be executed.",
                                node));
                    }
                });
            });
        }
    }
}