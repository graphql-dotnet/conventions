namespace GraphQLParser.Tests
{
    using GraphQLParser;
    using GraphQLParser.AST;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class GraphQLAstVisitorTests
    {
        private Parser parser;
        private List<GraphQLName> visitedAliases;
        private List<GraphQLArgument> visitedArguments;
        private List<ASTNode> visitedDefinitions;
        private List<GraphQLDirective> visitedDirectives;
        private List<GraphQLScalarValue> visitedEnumValues;
        private List<GraphQLFieldSelection> visitedFieldSelections;
        private List<GraphQLScalarValue> visitedFloatValues;
        private List<GraphQLFragmentDefinition> visitedFragmentDefinitions;
        private List<GraphQLFragmentSpread> visitedFragmentSpreads;
        private List<GraphQLNamedType> visitedFragmentTypeConditions;
        private List<GraphQLInlineFragment> visitedInlineFragments;
        private List<GraphQLScalarValue> visitedIntValues;
        private List<GraphQLName> visitedNames;
        private List<GraphQLSelectionSet> visitedSelectionSets;
        private List<GraphQLScalarValue> visitedStringValues;
        private List<GraphQLVariable> visitedVariables;
        private GraphQLAstVisitor visitor;
        public List<GraphQLScalarValue> visitedBooleanValues { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.parser = new Parser(new Lexer());
            this.visitor = Substitute.ForPartsOf<GraphQLAstVisitor>();

            this.visitedDefinitions = MockVisitMethod<ASTNode>((visitor) => visitor.BeginVisitOperationDefinition(null));
            this.visitedSelectionSets = MockVisitMethod<GraphQLSelectionSet>((visitor) => visitor.BeginVisitSelectionSet(null));
            this.visitedFieldSelections = MockVisitMethod<GraphQLFieldSelection>((visitor) => visitor.BeginVisitFieldSelection(null));
            this.visitedNames = MockVisitMethod<GraphQLName>((visitor) => visitor.BeginVisitName(null));
            this.visitedArguments = MockVisitMethod<GraphQLArgument>((visitor) => visitor.BeginVisitArgument(null));
            this.visitedAliases = MockVisitMethod<GraphQLName>((visitor) => visitor.BeginVisitAlias(null));
            this.visitedFragmentSpreads = MockVisitMethod<GraphQLFragmentSpread>((visitor) => visitor.BeginVisitFragmentSpread(null));
            this.visitedFragmentDefinitions = MockVisitMethod<GraphQLFragmentDefinition>((visitor) => visitor.BeginVisitFragmentDefinition(null));
            this.visitedFragmentTypeConditions = MockVisitMethod<GraphQLNamedType>((visitor) => visitor.BeginVisitNamedType(null));
            this.visitedInlineFragments = MockVisitMethod<GraphQLInlineFragment>((visitor) => visitor.BeginVisitInlineFragment(null));
            this.visitedDirectives = MockVisitMethod<GraphQLDirective>((visitor) => visitor.BeginVisitDirective(null));
            this.visitedVariables = MockVisitMethod<GraphQLVariable>((visitor) => visitor.BeginVisitVariable(null));
            this.visitedIntValues = MockVisitMethod<GraphQLScalarValue>((visitor) => visitor.BeginVisitIntValue(null));
            this.visitedFloatValues = MockVisitMethod<GraphQLScalarValue>((visitor) => visitor.BeginVisitFloatValue(null));
            this.visitedStringValues = MockVisitMethod<GraphQLScalarValue>((visitor) => visitor.BeginVisitStringValue(null));
            this.visitedBooleanValues = MockVisitMethod<GraphQLScalarValue>((visitor) => visitor.BeginVisitBooleanValue(null));
            this.visitedEnumValues = MockVisitMethod<GraphQLScalarValue>((visitor) => visitor.BeginVisitEnumValue(null));
        }

        [Test]
        public void Visit_BooleanValueArgument_VisitsOneBooleanValue()
        {
            this.visitor.Visit(this.Parse("{ stuff(id : true) }"));

            Assert.AreEqual(1, this.visitedBooleanValues.Count);
        }

        [Test]
        public void Visit_DefinitionWithSingleFragmentSpread_VisitsFragmentSpreadOneTime()
        {
            this.visitor.Visit(this.Parse("{ foo { ...fragment } }"));

            Assert.AreEqual(1, this.visitedFragmentSpreads.Count);
        }

        [Test]
        public void Visit_DefinitionWithSingleFragmentSpread_VisitsNameOfPropertyAndFragmentSpread()
        {
            this.visitor.Visit(this.Parse("{ foo { ...fragment } }"));

            Assert.AreEqual(2, this.visitedNames.Count);
        }

        [Test]
        public void Visit_DirectiveWithVariable_VisitsVariableOnce()
        {
            this.visitor.Visit(this.Parse("{ ... @include(if : $stuff) { field } }"));

            Assert.AreEqual(1, this.visitedVariables.Count);
        }

        [Test]
        public void Visit_EnumValueArgument_VisitsOneEnumValue()
        {
            this.visitor.Visit(this.Parse("{ stuff(id : TEST_ENUM) }"));

            Assert.AreEqual(1, this.visitedEnumValues.Count);
        }

        [Test]
        public void Visit_FloatValueArgument_VisitsOneFloatValue()
        {
            this.visitor.Visit(this.Parse("{ stuff(id : 1.2) }"));

            Assert.AreEqual(1, this.visitedFloatValues.Count);
        }

        [Test]
        public void Visit_FragmentWithTypeCondition_VisitsFragmentDefinitionOnce()
        {
            this.visitor.Visit(this.Parse("fragment testFragment on Stuff { field }"));

            Assert.AreEqual(1, this.visitedFragmentDefinitions.Count);
        }

        [Test]
        public void Visit_FragmentWithTypeCondition_VisitsTypeConditionOnce()
        {
            this.visitor.Visit(this.Parse("fragment testFragment on Stuff { field }"));

            Assert.AreEqual(1, this.visitedFragmentTypeConditions.Count);
        }

        [Test]
        public void Visit_InlineFragmentWithDirectiveAndArgument_VisitsArgumentsOnce()
        {
            this.visitor.Visit(this.Parse("{ ... @include(if : $stuff) { field } }"));

            Assert.AreEqual(1, this.visitedArguments.Count);
        }

        [Test]
        public void Visit_InlineFragmentWithDirectiveAndArgument_VisitsDirectiveOnce()
        {
            this.visitor.Visit(this.Parse("{ ... @include(if : $stuff) { field } }"));

            Assert.AreEqual(1, this.visitedDirectives.Count);
        }

        [Test]
        public void Visit_InlineFragmentWithDirectiveAndArgument_VisitsNameThreeTimes()
        {
            this.visitor.Visit(this.Parse("{ ... @include(if : $stuff) { field } }"));

            Assert.AreEqual(4, this.visitedNames.Count);
        }

        [Test]
        public void Visit_InlineFragmentWithOneField_VisitsOneField()
        {
            this.visitor.Visit(this.Parse("{ ... @include(if : $stuff) { field } }"));

            Assert.AreEqual(1, this.visitedFieldSelections.Count);
        }

        [Test]
        public void Visit_InlineFragmentWithTypeCondition_VisitsInlineFragmentOnce()
        {
            this.visitor.Visit(this.Parse("{ ... on Stuff { field } }"));

            Assert.AreEqual(1, this.visitedInlineFragments.Count);
        }

        [Test]
        public void Visit_InlineFragmentWithTypeCondition_VisitsTypeConditionOnce()
        {
            this.visitor.Visit(this.Parse("{ ... on Stuff { field } }"));

            Assert.AreEqual(1, this.visitedFragmentTypeConditions.Count);
        }

        [Test]
        public void Visit_IntValueArgument_VisitsOneIntValue()
        {
            this.visitor.Visit(this.Parse("{ stuff(id : 1) }"));

            Assert.AreEqual(1, this.visitedIntValues.Count);
        }

        [Test]
        public void Visit_OneDefinition_CallsVisitDefinitionOnce()
        {
            this.visitor.Visit(this.Parse("{ a }"));

            Assert.AreEqual(1, visitedDefinitions.Count);
        }

        [Test]
        public void Visit_OneDefinition_ProvidesCorrectDefinitionAsParameter()
        {
            var ast = this.Parse("{ a }");
            this.visitor.Visit(ast);

            Assert.AreEqual(ast.Definitions.Single(), visitedDefinitions.Single());
        }

        [Test]
        public void Visit_OneDefinition_VisitsOneSelectionSet()
        {
            this.visitor.Visit(this.Parse("{ a, b }"));

            Assert.AreEqual(1, this.visitedSelectionSets.Count);
        }

        [Test]
        public void Visit_OneDefinitionWithOneAliasedField_VisitsOneAlias()
        {
            this.visitor.Visit(this.Parse("{ foo, foo : bar }"));

            Assert.AreEqual(1, this.visitedAliases.Count);
        }

        [Test]
        public void Visit_OneDefinitionWithOneArgument_VisitsOneArgument()
        {
            this.visitor.Visit(this.Parse("{ foo(id : 1) { name } }"));

            Assert.AreEqual(1, this.visitedArguments.Count);
        }

        [Test]
        public void Visit_OneDefinitionWithOneNestedArgument_VisitsOneArgument()
        {
            this.visitor.Visit(this.Parse("{ foo{ names(size: 10) } }"));

            Assert.AreEqual(1, this.visitedArguments.Count);
        }

        [Test]
        public void Visit_StringValueArgument_VisitsOneStringValue()
        {
            this.visitor.Visit(this.Parse("{ stuff(id : \"abc\") }"));

            Assert.AreEqual(1, this.visitedStringValues.Count);
        }

        [Test]
        public void Visit_TwoDefinitions_CallsVisitDefinitionTwice()
        {
            this.visitor.Visit(this.Parse("{ a }\n{ b }"));

            Assert.AreEqual(2, visitedDefinitions.Count);
        }

        [Test]
        public void Visit_TwoFieldSelections_VisitsFieldSelectionTwice()
        {
            this.visitor.Visit(this.Parse("{ a, b }"));

            Assert.AreEqual(2, this.visitedFieldSelections.Count);
        }

        [Test]
        public void Visit_TwoFieldSelections_VisitsTwoFieldNames()
        {
            this.visitor.Visit(this.Parse("{ a, b }"));

            Assert.AreEqual(2, this.visitedNames.Count);
        }

        [Test]
        public void Visit_TwoFieldSelections_VisitsTwoFieldNamesAndDefinitionName()
        {
            this.visitor.Visit(this.Parse("query foo { a, b }"));

            Assert.AreEqual(3, this.visitedNames.Count);
        }

        [Test]
        public void Visit_TwoFieldSelectionsWithOneNested_VisitsFiveFieldSelections()
        {
            this.visitor.Visit(this.Parse("{a, nested { x,  y }, b}"));

            Assert.AreEqual(5, this.visitedFieldSelections.Count);
        }

        [Test]
        public void Visit_TwoFieldSelectionsWithOneNested_VisitsFiveNames()
        {
            this.visitor.Visit(this.Parse("{a, nested { x,  y }, b}"));

            Assert.AreEqual(5, this.visitedNames.Count);
        }

        private List<TEntity> MockVisitMethod<TEntity>(Action<GraphQLAstVisitor> visitorMethod)
        {
            var collection = new List<TEntity>();
            this.visitor.WhenForAnyArgs(visitorMethod)
                .Do(e => { collection.Add(e.Arg<TEntity>()); });

            return collection;
        }

        private GraphQLDocument Parse(string expression)
        {
            return this.parser.Parse(new Source(expression));
        }
    }
}