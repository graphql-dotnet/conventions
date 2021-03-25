using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Execution;
using GraphQL.Language.AST;
using GraphQL.Validation;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Attributes.MetaData
{
    public class FieldTypeMetaDataAttributeTests : TestBase
    {
        [Test]
        public void TopLevelQuery_Should_Have_Correct_CustomAttributeValue()
        {
            var type = TypeInfo<CustomAttributeQuery>();
            var field = type.ShouldHaveFieldWithName("node");
            var customAttributes = field.AttributeProvider.GetCustomAttributes(false);
            var result = customAttributes.FirstOrDefault(x => x.GetType() == typeof(TestCustomAttribute)) as TestCustomAttribute;
            Assert.IsTrue(result != null);
            Assert.IsTrue(result?.Permission == nameof(SomeTopLevelValidation));
        }

        [Test]
        public void ClassMethod_Should_Have_Correct_CustomAttributeValue()
        {
            var type = TypeInfo<TestType>();
            var field = type.ShouldHaveFieldWithName("testMethod");
            var customAttributes = field.AttributeProvider.GetCustomAttributes(false);
            var result = customAttributes.FirstOrDefault(x => x.GetType() == typeof(TestCustomAttribute)) as TestCustomAttribute;
            Assert.IsTrue(result != null);
            Assert.IsTrue(result?.Permission == nameof(SomeMethodValidation));
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
            result.ShouldHaveErrors(4);
            var expectedMessages = new[]
            {
                $"Required validation '{nameof(SomeTopLevelValidation)}' is not present. Query will not be executed.",
                $"Required validation '{nameof(SomeMethodValidation)}' is not present. Query will not be executed."
            };

            var messages = result.Errors.Select(e => e.Message).Distinct();
            Assert.IsTrue(messages.All(x => expectedMessages.Contains(x)));
        }

        [Test]
        public async Task When_UserHas_Requested_ValidationFieldTypeMetaData_ThereAreNoErrors()
        {
            var result = await Resolve_Query(selectedFields: "noAttribute name", accessPermissions: nameof(SomeTopLevelValidation));
            result.ShouldHaveNoErrors();
        }

        [Test]
        public async Task When_UserHasAll_Requested_ValidationFieldTypeMetaData_ThereAreNoErrors()
        {
            var result = await Resolve_Query(accessPermissions: new[] { nameof(SomeTopLevelValidation), nameof(SomeMethodValidation) });
            result.ShouldHaveNoErrors();
        }

        private async Task<ExecutionResult> Resolve_Query(string selectedFields = "testMethod noAttribute name", params string[] accessPermissions)
        {
            var engine = GraphQLEngine
                .New<CustomAttributeQuery>();

            var user = new TestUserContext(accessPermissions);

            var result = await engine
                .NewExecutor()
                .WithQueryString("query { node { " + selectedFields + " } }")
                .WithUserContext(user)
                .WithValidationRules(new[] { new TestValidation() })
                .ExecuteAsync();

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

    public class CustomAttributeQuery
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
        public Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            return Task.FromResult<INodeVisitor>(new TestValidationNodeVisitor(context.GetUserContext() as TestUserContext));
        }
    }

    public class TestValidationNodeVisitor : INodeVisitor
    {
        private readonly TestUserContext _user;

        public TestValidationNodeVisitor(TestUserContext user)
        {
            _user = user;
        }

        public void Enter(INode node, ValidationContext context)
        {
            var fieldDef = context.TypeInfo.GetFieldDef();
            if (fieldDef == null) return;
            if (fieldDef.Metadata != null && fieldDef.HasMetadata(nameof(TestCustomAttribute)))
            {
                var permissionMetaData = fieldDef.Metadata.First(x => x.Key == nameof(TestCustomAttribute));
                var requiredValidation = permissionMetaData.Value as string;

                if (_user == null || _user.AccessPermissions.All(p => p != requiredValidation))
                    context.ReportError( new ValidationError( /* When reporting such errors no data would be returned use with cautious */
                        context.Document.OriginalQuery,
                        "Authorization",
                        $"Required validation '{requiredValidation}' is not present. Query will not be executed.",
                        node));
            }
        }

        public void Leave(INode node, ValidationContext context) { /* Noop */ }
    }
}
