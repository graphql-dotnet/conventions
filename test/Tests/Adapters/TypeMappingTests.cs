using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Conventions.Adapters.Types;
using ConventionsTypes = GraphQL.Conventions.Adapters.Types;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Types;
using Extended = GraphQL.Conventions.Adapters.Types;
using UriGraphType = GraphQL.Conventions.Adapters.Types.UriGraphType;
using GuidGraphType = GraphQL.Conventions.Adapters.Types.GuidGraphType;

namespace GraphQL.Conventions.Tests.Adapters
{
    public class TypeMappingTests : ConstructionTestBase
    {
        [Test]
        public void Can_Derive_Nullable_Primitive_Types()
        {
            Type<string>().ShouldBeOfType<StringGraphType>();
            Type<bool?>().ShouldBeOfType<BooleanGraphType>();
            Type<sbyte?>().ShouldBeOfType<IntGraphType>();
            Type<byte?>().ShouldBeOfType<IntGraphType>();
            Type<short?>().ShouldBeOfType<IntGraphType>();
            Type<ushort?>().ShouldBeOfType<IntGraphType>();
            Type<int?>().ShouldBeOfType<IntGraphType>();
            Type<uint?>().ShouldBeOfType<IntGraphType>();
            Type<long?>().ShouldBeOfType<IntGraphType>();
            Type<ulong?>().ShouldBeOfType<IntGraphType>();
            Type<float?>().ShouldBeOfType<FloatGraphType>();
            Type<double?>().ShouldBeOfType<FloatGraphType>();
            Type<decimal?>().ShouldBeOfType<FloatGraphType>();
            Type<DateTime?>().ShouldBeOfType<DateTimeGraphType>();
            Type<DateTimeOffset?>().ShouldBeOfType<DateTimeOffsetGraphType>();
            Type<TimeSpan?>().ShouldBeOfType<TimeSpanGraphType>();
            Type<Id?>().ShouldBeOfType<Extended.IdGraphType>();
            Type<Url>().ShouldBeOfType<UrlGraphType>();
            Type<Uri>().ShouldBeOfType<UriGraphType>();
            Type<Cursor?>().ShouldBeOfType<Extended.Relay.CursorGraphType>();
            Type<Guid?>().ShouldBeOfType<GuidGraphType>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Primitive_Types()
        {
            Type<NonNull<string>>().ShouldBeOfNonNullableType<StringGraphType>();
            Type<bool>().ShouldBeOfNonNullableType<BooleanGraphType>();
            Type<sbyte>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<byte>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<short>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<ushort>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<int>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<uint>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<long>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<ulong>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<float>().ShouldBeOfNonNullableType<FloatGraphType>();
            Type<double>().ShouldBeOfNonNullableType<FloatGraphType>();
            Type<decimal>().ShouldBeOfNonNullableType<FloatGraphType>();
            Type<DateTime>().ShouldBeOfNonNullableType<DateTimeGraphType>();
            Type<DateTimeOffset>().ShouldBeOfNonNullableType<DateTimeOffsetGraphType>();
            Type<TimeSpan>().ShouldBeOfNonNullableType<TimeSpanGraphType>();
            Type<Id>().ShouldBeOfNonNullableType<Extended.IdGraphType>();
            Type<NonNull<Url>>().ShouldBeOfNonNullableType<UrlGraphType>();
            Type<NonNull<Uri>>().ShouldBeOfNonNullableType<UriGraphType>();
            Type<Cursor>().ShouldBeOfNonNullableType<Extended.Relay.CursorGraphType>();
            Type<Guid>().ShouldBeOfNonNullableType<GuidGraphType>();
        }

        [Test]
        public void Can_Derive_Nullable_Enum_Types()
        {
            Type<TestEnum?>().ShouldBeOfType<Extended.EnumerationGraphType<TestEnum>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Enum_Types()
        {
            Type<TestEnum>().ShouldBeOfNonNullableType<Extended.EnumerationGraphType<TestEnum>>();
        }

        [Test]
        public void Can_Derive_Nullable_Complex_Types()
        {
            Type<TestOutputObject>().ShouldBeOfType<OutputObjectGraphType<TestOutputObject>>();
            Type<TestInputObject>().ShouldBeOfType<ConventionsTypes.InputObjectGraphType<TestInputObject>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Complex_Types()
        {
            Type<NonNull<TestOutputObject>>().ShouldBeOfNonNullableType<OutputObjectGraphType<TestOutputObject>>();
            Type<NonNull<TestInputObject>>().ShouldBeOfNonNullableType<ConventionsTypes.InputObjectGraphType<TestInputObject>>();
        }

        [Test]
        public void Can_Derive_Nullable_Interfaces()
        {
            Type<ITestInterface>().ShouldBeOfType<Extended.InterfaceGraphType<ITestInterface>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Interfaces()
        {
            Type<NonNull<ITestInterface>>().ShouldBeOfNonNullableType<Extended.InterfaceGraphType<ITestInterface>>();
        }

        [Test]
        public void Can_Derive_Nullable_Union_Types()
        {
            Type<TestUnionType>().ShouldBeOfType<UnionGraphType<TestUnionType>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Union_Types()
        {
            Type<NonNull<TestUnionType>>().ShouldBeOfNonNullableType<UnionGraphType<TestUnionType>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Nullable_Primitive_Types()
        {
            Type<List<string>>().ShouldBeOfListType<StringGraphType>();
            Type<List<bool?>>().ShouldBeOfListType<BooleanGraphType>();
            Type<List<sbyte?>>().ShouldBeOfListType<IntGraphType>();
            Type<List<byte?>>().ShouldBeOfListType<IntGraphType>();
            Type<List<short?>>().ShouldBeOfListType<IntGraphType>();
            Type<List<ushort?>>().ShouldBeOfListType<IntGraphType>();
            Type<List<int?>>().ShouldBeOfListType<IntGraphType>();
            Type<List<uint?>>().ShouldBeOfListType<IntGraphType>();
            Type<List<long?>>().ShouldBeOfListType<IntGraphType>();
            Type<List<ulong?>>().ShouldBeOfListType<IntGraphType>();
            Type<List<float?>>().ShouldBeOfListType<FloatGraphType>();
            Type<List<double?>>().ShouldBeOfListType<FloatGraphType>();
            Type<List<decimal?>>().ShouldBeOfListType<FloatGraphType>();
            Type<List<DateTime?>>().ShouldBeOfListType<DateTimeGraphType>();
            Type<List<DateTimeOffset?>>().ShouldBeOfListType<DateTimeOffsetGraphType>();
            Type<List<TimeSpan?>>().ShouldBeOfListType<TimeSpanGraphType>();
            Type<List<Id?>>().ShouldBeOfListType<Extended.IdGraphType>();
            Type<List<Url>>().ShouldBeOfListType<UrlGraphType>();
            Type<List<Uri>>().ShouldBeOfListType<UriGraphType>();
            Type<List<Cursor?>>().ShouldBeOfListType<Extended.Relay.CursorGraphType>();
            Type<List<Guid?>>().ShouldBeOfListType<GuidGraphType>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Non_Nullable_Primitive_Types()
        {
            Type<List<NonNull<string>>>().ShouldBeOfListType<NonNullGraphType<StringGraphType>>();
            Type<List<bool>>().ShouldBeOfListType<NonNullGraphType<BooleanGraphType>>();
            Type<List<sbyte>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<List<byte>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<List<short>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<List<ushort>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<List<int>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<List<uint>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<List<long>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<List<ulong>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<List<float>>().ShouldBeOfListType<NonNullGraphType<FloatGraphType>>();
            Type<List<double>>().ShouldBeOfListType<NonNullGraphType<FloatGraphType>>();
            Type<List<decimal>>().ShouldBeOfListType<NonNullGraphType<FloatGraphType>>();
            Type<List<DateTime>>().ShouldBeOfListType<NonNullGraphType<DateTimeGraphType>>();
            Type<List<DateTimeOffset>>().ShouldBeOfListType<NonNullGraphType<DateTimeOffsetGraphType>>();
            Type<List<TimeSpan>>().ShouldBeOfListType<NonNullGraphType<TimeSpanGraphType>>();
            Type<List<Id>>().ShouldBeOfListType<NonNullGraphType<Extended.IdGraphType>>();
            Type<List<NonNull<Url>>>().ShouldBeOfListType<NonNullGraphType<UrlGraphType>>();
            Type<List<NonNull<Uri>>>().ShouldBeOfListType<NonNullGraphType<UriGraphType>>();
            Type<List<Cursor>>().ShouldBeOfListType<NonNullGraphType<Extended.Relay.CursorGraphType>>();
            Type<List<Guid>>().ShouldBeOfListType<NonNullGraphType<GuidGraphType>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Nullable_Enum_Types()
        {
            Type<List<TestEnum?>>().ShouldBeOfListType<Extended.EnumerationGraphType<TestEnum>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Non_Nullable_Enum_Types()
        {
            Type<List<TestEnum>>().ShouldBeOfListType<NonNullGraphType<Extended.EnumerationGraphType<TestEnum>>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Nullable_Complex_Types()
        {
            Type<List<TestOutputObject>>().ShouldBeOfListType<OutputObjectGraphType<TestOutputObject>>();
            Type<List<TestInputObject>>().ShouldBeOfListType<ConventionsTypes.InputObjectGraphType<TestInputObject>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Non_Nullable_Complex_Types()
        {
            Type<List<NonNull<TestOutputObject>>>().ShouldBeOfListType<NonNullGraphType<OutputObjectGraphType<TestOutputObject>>>();
            Type<List<NonNull<TestInputObject>>>().ShouldBeOfListType<NonNullGraphType<ConventionsTypes.InputObjectGraphType<TestInputObject>>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Nullable_Interfaces()
        {
            Type<List<ITestInterface>>().ShouldBeOfListType<Extended.InterfaceGraphType<ITestInterface>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Non_Nullable_Interfaces()
        {
            Type<List<NonNull<ITestInterface>>>().ShouldBeOfListType<NonNullGraphType<Extended.InterfaceGraphType<ITestInterface>>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Nullable_Union_Types()
        {
            Type<List<TestUnionType>>().ShouldBeOfListType<Extended.UnionGraphType<TestUnionType>>();
        }

        [Test]
        public void Can_Derive_Nullable_Lists_Of_Non_Nullable_Union_Types()
        {
            Type<List<NonNull<TestUnionType>>>().ShouldBeOfListType<NonNullGraphType<Extended.UnionGraphType<TestUnionType>>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Nullable_Primitive_Types()
        {
            Type<NonNull<List<string>>>().ShouldBeOfNonNullableListType<StringGraphType>();
            Type<NonNull<List<bool?>>>().ShouldBeOfNonNullableListType<BooleanGraphType>();
            Type<NonNull<List<sbyte?>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<NonNull<List<byte?>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<NonNull<List<short?>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<NonNull<List<ushort?>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<NonNull<List<int?>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<NonNull<List<uint?>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<NonNull<List<long?>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<NonNull<List<ulong?>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<NonNull<List<float?>>>().ShouldBeOfNonNullableListType<FloatGraphType>();
            Type<NonNull<List<double?>>>().ShouldBeOfNonNullableListType<FloatGraphType>();
            Type<NonNull<List<decimal?>>>().ShouldBeOfNonNullableListType<FloatGraphType>();
            Type<NonNull<List<DateTime?>>>().ShouldBeOfNonNullableListType<DateTimeGraphType>();
            Type<NonNull<List<DateTimeOffset?>>>().ShouldBeOfNonNullableListType<DateTimeOffsetGraphType>();
            Type<NonNull<List<TimeSpan?>>>().ShouldBeOfNonNullableListType<TimeSpanGraphType>();
            Type<NonNull<List<Id?>>>().ShouldBeOfNonNullableListType<Extended.IdGraphType>();
            Type<NonNull<List<Url>>>().ShouldBeOfNonNullableListType<UrlGraphType>();
            Type<NonNull<List<Uri>>>().ShouldBeOfNonNullableListType<UriGraphType>();
            Type<NonNull<List<Cursor?>>>().ShouldBeOfNonNullableListType<Extended.Relay.CursorGraphType>();
            Type<NonNull<List<Guid?>>>().ShouldBeOfNonNullableListType<GuidGraphType>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Non_Nullable_Primitive_Types()
        {
            Type<NonNull<List<NonNull<string>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<StringGraphType>>();
            Type<NonNull<List<bool>>>().ShouldBeOfNonNullableListType<NonNullGraphType<BooleanGraphType>>();
            Type<NonNull<List<sbyte>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<NonNull<List<byte>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<NonNull<List<short>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<NonNull<List<ushort>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<NonNull<List<int>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<NonNull<List<uint>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<NonNull<List<long>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<NonNull<List<ulong>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<NonNull<List<float>>>().ShouldBeOfNonNullableListType<NonNullGraphType<FloatGraphType>>();
            Type<NonNull<List<double>>>().ShouldBeOfNonNullableListType<NonNullGraphType<FloatGraphType>>();
            Type<NonNull<List<decimal>>>().ShouldBeOfNonNullableListType<NonNullGraphType<FloatGraphType>>();
            Type<NonNull<List<DateTime>>>().ShouldBeOfNonNullableListType<NonNullGraphType<DateTimeGraphType>>();
            Type<NonNull<List<DateTimeOffset>>>().ShouldBeOfNonNullableListType<NonNullGraphType<DateTimeOffsetGraphType>>();
            Type<NonNull<List<TimeSpan>>>().ShouldBeOfNonNullableListType<NonNullGraphType<TimeSpanGraphType>>();
            Type<NonNull<List<Id>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.IdGraphType>>();
            Type<NonNull<List<NonNull<Url>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<UrlGraphType>>();
            Type<NonNull<List<NonNull<Uri>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<UriGraphType>>();
            Type<NonNull<List<Cursor>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.Relay.CursorGraphType>>();
            Type<NonNull<List<Guid>>>().ShouldBeOfNonNullableListType<NonNullGraphType<GuidGraphType>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Nullable_Enum_Types()
        {
            Type<NonNull<List<TestEnum?>>>().ShouldBeOfNonNullableListType<Extended.EnumerationGraphType<TestEnum>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Non_Nullable_Enum_Types()
        {
            Type<NonNull<List<TestEnum>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.EnumerationGraphType<TestEnum>>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Nullable_Complex_Types()
        {
            Type<NonNull<List<TestOutputObject>>>().ShouldBeOfNonNullableListType<OutputObjectGraphType<TestOutputObject>>();
            Type<NonNull<List<TestInputObject>>>().ShouldBeOfNonNullableListType<ConventionsTypes.InputObjectGraphType<TestInputObject>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Non_Nullable_Complex_Types()
        {
            Type<NonNull<List<NonNull<TestOutputObject>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<OutputObjectGraphType<TestOutputObject>>>();
            Type<NonNull<List<NonNull<TestInputObject>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<ConventionsTypes.InputObjectGraphType<TestInputObject>>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Nullable_Interfaces()
        {
            Type<NonNull<List<ITestInterface>>>().ShouldBeOfNonNullableListType<Extended.InterfaceGraphType<ITestInterface>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Non_Nullable_Interfaces()
        {
            Type<NonNull<List<NonNull<ITestInterface>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.InterfaceGraphType<ITestInterface>>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Nullable_Union_Types()
        {
            Type<NonNull<List<TestUnionType>>>().ShouldBeOfNonNullableListType<Extended.UnionGraphType<TestUnionType>>();
        }

        [Test]
        public void Can_Derive_Non_Nullable_Lists_Of_Non_Nullable_Union_Types()
        {
            Type<NonNull<List<NonNull<TestUnionType>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.UnionGraphType<TestUnionType>>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Primitive_Types()
        {
            Type<Task<string>>().ShouldBeOfType<StringGraphType>();
            Type<Task<bool?>>().ShouldBeOfType<BooleanGraphType>();
            Type<Task<sbyte?>>().ShouldBeOfType<IntGraphType>();
            Type<Task<byte?>>().ShouldBeOfType<IntGraphType>();
            Type<Task<short?>>().ShouldBeOfType<IntGraphType>();
            Type<Task<ushort?>>().ShouldBeOfType<IntGraphType>();
            Type<Task<int?>>().ShouldBeOfType<IntGraphType>();
            Type<Task<uint?>>().ShouldBeOfType<IntGraphType>();
            Type<Task<long?>>().ShouldBeOfType<IntGraphType>();
            Type<Task<ulong?>>().ShouldBeOfType<IntGraphType>();
            Type<Task<float?>>().ShouldBeOfType<FloatGraphType>();
            Type<Task<double?>>().ShouldBeOfType<FloatGraphType>();
            Type<Task<decimal?>>().ShouldBeOfType<FloatGraphType>();
            Type<Task<DateTime?>>().ShouldBeOfType<DateTimeGraphType>();
            Type<Task<DateTimeOffset?>>().ShouldBeOfType<DateTimeOffsetGraphType>();
            Type<Task<TimeSpan?>>().ShouldBeOfType<TimeSpanGraphType>();
            Type<Task<Id?>>().ShouldBeOfType<Extended.IdGraphType>();
            Type<Task<Url>>().ShouldBeOfType<UrlGraphType>();
            Type<Task<Uri>>().ShouldBeOfType<UriGraphType>();
            Type<Task<Cursor?>>().ShouldBeOfType<Extended.Relay.CursorGraphType>();
            Type<Task<Guid?>>().ShouldBeOfType<GuidGraphType>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Primitive_Types()
        {
            Type<Task<NonNull<string>>>().ShouldBeOfNonNullableType<StringGraphType>();
            Type<Task<bool>>().ShouldBeOfNonNullableType<BooleanGraphType>();
            Type<Task<sbyte>>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<Task<byte>>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<Task<short>>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<Task<ushort>>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<Task<int>>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<Task<uint>>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<Task<long>>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<Task<ulong>>().ShouldBeOfNonNullableType<IntGraphType>();
            Type<Task<float>>().ShouldBeOfNonNullableType<FloatGraphType>();
            Type<Task<double>>().ShouldBeOfNonNullableType<FloatGraphType>();
            Type<Task<decimal>>().ShouldBeOfNonNullableType<FloatGraphType>();
            Type<Task<DateTime>>().ShouldBeOfNonNullableType<DateTimeGraphType>();
            Type<Task<DateTimeOffset>>().ShouldBeOfNonNullableType<DateTimeOffsetGraphType>();
            Type<Task<TimeSpan>>().ShouldBeOfNonNullableType<TimeSpanGraphType>();
            Type<Task<Id>>().ShouldBeOfNonNullableType<Extended.IdGraphType>();
            Type<Task<NonNull<Url>>>().ShouldBeOfNonNullableType<UrlGraphType>();
            Type<Task<NonNull<Uri>>>().ShouldBeOfNonNullableType<UriGraphType>();
            Type<Task<Cursor>>().ShouldBeOfNonNullableType<Extended.Relay.CursorGraphType>();
            Type<Task<Guid>>().ShouldBeOfNonNullableType<GuidGraphType>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Enum_Types()
        {
            Type<Task<TestEnum?>>().ShouldBeOfType<Extended.EnumerationGraphType<TestEnum>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Enum_Types()
        {
            Type<Task<TestEnum>>().ShouldBeOfNonNullableType<Extended.EnumerationGraphType<TestEnum>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Complex_Types()
        {
            Type<Task<TestOutputObject>>().ShouldBeOfType<OutputObjectGraphType<TestOutputObject>>();
            Type<Task<TestInputObject>>().ShouldBeOfType<ConventionsTypes.InputObjectGraphType<TestInputObject>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Complex_Types()
        {
            Type<Task<NonNull<TestOutputObject>>>().ShouldBeOfNonNullableType<OutputObjectGraphType<TestOutputObject>>();
            Type<Task<NonNull<TestInputObject>>>().ShouldBeOfNonNullableType<ConventionsTypes.InputObjectGraphType<TestInputObject>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Interfaces()
        {
            Type<Task<ITestInterface>>().ShouldBeOfType<Extended.InterfaceGraphType<ITestInterface>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Interfaces()
        {
            Type<Task<NonNull<ITestInterface>>>().ShouldBeOfNonNullableType<Extended.InterfaceGraphType<ITestInterface>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Union_Types()
        {
            Type<Task<TestUnionType>>().ShouldBeOfType<UnionGraphType<TestUnionType>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Union_Types()
        {
            Type<Task<NonNull<TestUnionType>>>().ShouldBeOfNonNullableType<UnionGraphType<TestUnionType>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Nullable_Primitive_Types()
        {
            Type<Task<List<string>>>().ShouldBeOfListType<StringGraphType>();
            Type<Task<List<bool?>>>().ShouldBeOfListType<BooleanGraphType>();
            Type<Task<List<sbyte?>>>().ShouldBeOfListType<IntGraphType>();
            Type<Task<List<byte?>>>().ShouldBeOfListType<IntGraphType>();
            Type<Task<List<short?>>>().ShouldBeOfListType<IntGraphType>();
            Type<Task<List<ushort?>>>().ShouldBeOfListType<IntGraphType>();
            Type<Task<List<int?>>>().ShouldBeOfListType<IntGraphType>();
            Type<Task<List<uint?>>>().ShouldBeOfListType<IntGraphType>();
            Type<Task<List<long?>>>().ShouldBeOfListType<IntGraphType>();
            Type<Task<List<ulong?>>>().ShouldBeOfListType<IntGraphType>();
            Type<Task<List<float?>>>().ShouldBeOfListType<FloatGraphType>();
            Type<Task<List<double?>>>().ShouldBeOfListType<FloatGraphType>();
            Type<Task<List<decimal?>>>().ShouldBeOfListType<FloatGraphType>();
            Type<Task<List<DateTime?>>>().ShouldBeOfListType<DateTimeGraphType>();
            Type<Task<List<DateTimeOffset?>>>().ShouldBeOfListType<DateTimeOffsetGraphType>();
            Type<Task<List<TimeSpan?>>>().ShouldBeOfListType<TimeSpanGraphType>();
            Type<Task<List<Id?>>>().ShouldBeOfListType<Extended.IdGraphType>();
            Type<Task<List<Url>>>().ShouldBeOfListType<UrlGraphType>();
            Type<Task<List<Uri>>>().ShouldBeOfListType<UriGraphType>();
            Type<Task<List<Cursor?>>>().ShouldBeOfListType<Extended.Relay.CursorGraphType>();
            Type<Task<List<Guid?>>>().ShouldBeOfListType<GuidGraphType>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Non_Nullable_Primitive_Types()
        {
            Type<Task<List<NonNull<string>>>>().ShouldBeOfListType<NonNullGraphType<StringGraphType>>();
            Type<Task<List<bool>>>().ShouldBeOfListType<NonNullGraphType<BooleanGraphType>>();
            Type<Task<List<sbyte>>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<Task<List<byte>>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<Task<List<short>>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<Task<List<ushort>>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<Task<List<int>>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<Task<List<uint>>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<Task<List<long>>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<Task<List<ulong>>>().ShouldBeOfListType<NonNullGraphType<IntGraphType>>();
            Type<Task<List<float>>>().ShouldBeOfListType<NonNullGraphType<FloatGraphType>>();
            Type<Task<List<double>>>().ShouldBeOfListType<NonNullGraphType<FloatGraphType>>();
            Type<Task<List<decimal>>>().ShouldBeOfListType<NonNullGraphType<FloatGraphType>>();
            Type<Task<List<DateTime>>>().ShouldBeOfListType<NonNullGraphType<DateTimeGraphType>>();
            Type<Task<List<DateTimeOffset>>>().ShouldBeOfListType<NonNullGraphType<DateTimeOffsetGraphType>>();
            Type<Task<List<TimeSpan>>>().ShouldBeOfListType<NonNullGraphType<TimeSpanGraphType>>();
            Type<Task<List<Id>>>().ShouldBeOfListType<NonNullGraphType<Extended.IdGraphType>>();
            Type<Task<List<NonNull<Url>>>>().ShouldBeOfListType<NonNullGraphType<UrlGraphType>>();
            Type<Task<List<NonNull<Uri>>>>().ShouldBeOfListType<NonNullGraphType<UriGraphType>>();
            Type<Task<List<Cursor>>>().ShouldBeOfListType<NonNullGraphType<Extended.Relay.CursorGraphType>>();
            Type<Task<List<Guid>>>().ShouldBeOfListType<NonNullGraphType<GuidGraphType>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Nullable_Enum_Types()
        {
            Type<Task<List<TestEnum?>>>().ShouldBeOfListType<Extended.EnumerationGraphType<TestEnum>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Non_Nullable_Enum_Types()
        {
            Type<Task<List<TestEnum>>>().ShouldBeOfListType<NonNullGraphType<Extended.EnumerationGraphType<TestEnum>>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Nullable_Complex_Types()
        {
            Type<Task<List<TestOutputObject>>>().ShouldBeOfListType<OutputObjectGraphType<TestOutputObject>>();
            Type<Task<List<TestInputObject>>>().ShouldBeOfListType<ConventionsTypes.InputObjectGraphType<TestInputObject>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Non_Nullable_Complex_Types()
        {
            Type<Task<List<NonNull<TestOutputObject>>>>().ShouldBeOfListType<NonNullGraphType<OutputObjectGraphType<TestOutputObject>>>();
            Type<Task<List<NonNull<TestInputObject>>>>().ShouldBeOfListType<NonNullGraphType<ConventionsTypes.InputObjectGraphType<TestInputObject>>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Nullable_Interfaces()
        {
            Type<Task<List<ITestInterface>>>().ShouldBeOfListType<Extended.InterfaceGraphType<ITestInterface>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Non_Nullable_Interfaces()
        {
            Type<Task<List<NonNull<ITestInterface>>>>().ShouldBeOfListType<NonNullGraphType<Extended.InterfaceGraphType<ITestInterface>>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Nullable_Union_Types()
        {
            Type<Task<List<TestUnionType>>>().ShouldBeOfListType<Extended.UnionGraphType<TestUnionType>>();
        }

        [Test]
        public void Can_Derive_Future_Nullable_Lists_Of_Non_Nullable_Union_Types()
        {
            Type<Task<List<NonNull<TestUnionType>>>>().ShouldBeOfListType<NonNullGraphType<Extended.UnionGraphType<TestUnionType>>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Nullable_Primitive_Types()
        {
            Type<Task<NonNull<List<string>>>>().ShouldBeOfNonNullableListType<StringGraphType>();
            Type<Task<NonNull<List<bool?>>>>().ShouldBeOfNonNullableListType<BooleanGraphType>();
            Type<Task<NonNull<List<sbyte?>>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<Task<NonNull<List<byte?>>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<Task<NonNull<List<short?>>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<Task<NonNull<List<ushort?>>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<Task<NonNull<List<int?>>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<Task<NonNull<List<uint?>>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<Task<NonNull<List<long?>>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<Task<NonNull<List<ulong?>>>>().ShouldBeOfNonNullableListType<IntGraphType>();
            Type<Task<NonNull<List<float?>>>>().ShouldBeOfNonNullableListType<FloatGraphType>();
            Type<Task<NonNull<List<double?>>>>().ShouldBeOfNonNullableListType<FloatGraphType>();
            Type<Task<NonNull<List<decimal?>>>>().ShouldBeOfNonNullableListType<FloatGraphType>();
            Type<Task<NonNull<List<DateTime?>>>>().ShouldBeOfNonNullableListType<DateTimeGraphType>();
            Type<Task<NonNull<List<DateTimeOffset?>>>>().ShouldBeOfNonNullableListType<DateTimeOffsetGraphType>();
            Type<Task<NonNull<List<TimeSpan?>>>>().ShouldBeOfNonNullableListType<TimeSpanGraphType>();
            Type<Task<NonNull<List<Id?>>>>().ShouldBeOfNonNullableListType<Extended.IdGraphType>();
            Type<Task<NonNull<List<Url>>>>().ShouldBeOfNonNullableListType<UrlGraphType>();
            Type<Task<NonNull<List<Uri>>>>().ShouldBeOfNonNullableListType<UriGraphType>();
            Type<Task<NonNull<List<Cursor?>>>>().ShouldBeOfNonNullableListType<Extended.Relay.CursorGraphType>();
            Type<Task<NonNull<List<Guid?>>>>().ShouldBeOfNonNullableListType<GuidGraphType>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Non_Nullable_Primitive_Types()
        {
            Type<Task<NonNull<List<NonNull<string>>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<StringGraphType>>();
            Type<Task<NonNull<List<bool>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<BooleanGraphType>>();
            Type<Task<NonNull<List<sbyte>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<Task<NonNull<List<byte>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<Task<NonNull<List<short>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<Task<NonNull<List<ushort>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<Task<NonNull<List<int>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<Task<NonNull<List<uint>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<Task<NonNull<List<long>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<Task<NonNull<List<ulong>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<IntGraphType>>();
            Type<Task<NonNull<List<float>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<FloatGraphType>>();
            Type<Task<NonNull<List<double>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<FloatGraphType>>();
            Type<Task<NonNull<List<decimal>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<FloatGraphType>>();
            Type<Task<NonNull<List<DateTime>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<DateTimeGraphType>>();
            Type<Task<NonNull<List<DateTimeOffset>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<DateTimeOffsetGraphType>>();
            Type<Task<NonNull<List<TimeSpan>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<TimeSpanGraphType>>();
            Type<Task<NonNull<List<Id>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.IdGraphType>>();
            Type<Task<NonNull<List<NonNull<Url>>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<UrlGraphType>>();
            Type<Task<NonNull<List<NonNull<Uri>>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<UriGraphType>>();
            Type<Task<NonNull<List<Cursor>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.Relay.CursorGraphType>>();
            Type<Task<NonNull<List<Guid>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<GuidGraphType>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Nullable_Enum_Types()
        {
            Type<Task<NonNull<List<TestEnum?>>>>().ShouldBeOfNonNullableListType<Extended.EnumerationGraphType<TestEnum>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Non_Nullable_Enum_Types()
        {
            Type<Task<NonNull<List<TestEnum>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.EnumerationGraphType<TestEnum>>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Nullable_Complex_Types()
        {
            Type<Task<NonNull<List<TestOutputObject>>>>().ShouldBeOfNonNullableListType<OutputObjectGraphType<TestOutputObject>>();
            Type<Task<NonNull<List<TestInputObject>>>>().ShouldBeOfNonNullableListType<ConventionsTypes.InputObjectGraphType<TestInputObject>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Non_Nullable_Complex_Types()
        {
            Type<Task<NonNull<List<NonNull<TestOutputObject>>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<OutputObjectGraphType<TestOutputObject>>>();
            Type<Task<NonNull<List<NonNull<TestInputObject>>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<ConventionsTypes.InputObjectGraphType<TestInputObject>>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Nullable_Interfaces()
        {
            Type<Task<NonNull<List<ITestInterface>>>>().ShouldBeOfNonNullableListType<Extended.InterfaceGraphType<ITestInterface>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Non_Nullable_Interfaces()
        {
            Type<Task<NonNull<List<NonNull<ITestInterface>>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.InterfaceGraphType<ITestInterface>>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Nullable_Union_Types()
        {
            Type<Task<NonNull<List<TestUnionType>>>>().ShouldBeOfNonNullableListType<Extended.UnionGraphType<TestUnionType>>();
        }

        [Test]
        public void Can_Derive_Future_Non_Nullable_Lists_Of_Non_Nullable_Union_Types()
        {
            Type<Task<NonNull<List<NonNull<TestUnionType>>>>>().ShouldBeOfNonNullableListType<NonNullGraphType<Extended.UnionGraphType<TestUnionType>>>();
        }

        enum TestEnum
        {
            MemberA,
            MemberB,
            MemberC,
        }

        class TestOutputObject
        {
            public string SomeField { get; set; }
        }

        [InputType]
        class TestInputObject
        {
            public string SomeField { get; set; }
        }

        interface ITestInterface
        {
            string SomeField { get; }
        }

        class AnotherTestOutputObject { }

        class TestUnionType : Union<TestOutputObject, AnotherTestOutputObject>
        {
        }
    }
}
