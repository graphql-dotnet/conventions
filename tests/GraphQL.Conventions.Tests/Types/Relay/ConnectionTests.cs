using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Relay;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Types.Relay
{
    public class ConnectionTests : TestBase
    {
        [Test]
        public void Can_Create_Simple_Connection()
        {
            var connection = GenerateConnection(5);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(5));
            connection.Edges.Count().ShouldEqual(5);
            connection.Items.Count().ShouldEqual(5);

            for (var i = 1; i <= 5; i++)
            {
                connection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor(i));
                connection.Edges.ElementAt(i - 1).Cursor.ShouldNotEqual(Cursor<int>(i));
                connection.Edges.ElementAt(i - 1).Node.Value.ShouldEqual(i);
                connection.Items.ElementAt(i - 1).Value.ShouldEqual(i);
                connection.Edges.ElementAt(i - 1).Cursor.IntegerForCursor<Foo>().ShouldEqual(i);
            }
        }

        [Test]
        public void Can_Create_Connection_From_First_3()
        {
            var connection = GenerateConnection(5, first: 3);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(3));
            connection.Edges.Count().ShouldEqual(3);
            connection.Items.Count().ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor(i));
                connection.Edges.ElementAt(i - 1).Node.Value.ShouldEqual(i);
                connection.Items.ElementAt(i - 1).Value.ShouldEqual(i);
            }
        }

        [Test]
        public void Can_Create_Connection_From_Last_3()
        {
            var connection = GenerateConnection(5, last: 3);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(5));
            connection.Edges.Count().ShouldEqual(3);
            connection.Items.Count().ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor(i + 2));
                connection.Edges.ElementAt(i - 1).Node.Value.ShouldEqual(i + 2);
                connection.Items.ElementAt(i - 1).Value.ShouldEqual(i + 2);
            }
        }

        [Test]
        public void Can_Create_Connection_From_First_3_After_1()
        {
            var connection = GenerateConnection(5, after: Cursor(1), first: 3);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(2));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(4));
            connection.Edges.Count().ShouldEqual(3);
            connection.Items.Count().ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor(i + 1));
                connection.Edges.ElementAt(i - 1).Node.Value.ShouldEqual(i + 1);
                connection.Items.ElementAt(i - 1).Value.ShouldEqual(i + 1);
            }
        }

        [Test]
        public void Can_Create_Connection_From_Last_3_Before_5()
        {
            var connection = GenerateConnection(5, before: Cursor(5), last: 3);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(2));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(4));
            connection.Edges.Count().ShouldEqual(3);
            connection.Items.Count().ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor(i + 1));
                connection.Edges.ElementAt(i - 1).Node.Value.ShouldEqual(i + 1);
                connection.Items.ElementAt(i - 1).Value.ShouldEqual(i + 1);
            }
        }

        [Test]
        public void Can_Create_Connection_From_After_Bounds()
        {
            var connection = GenerateConnection(5, after: Cursor(5), first: 2);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(6));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(6));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Can_Create_Connection_From_Before_Bounds()
        {
            var connection = GenerateConnection(5, before: Cursor(1), last: 2);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(0));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(0));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Can_Create_Connection_From_First_0()
        {
            var connection = GenerateConnection(5, first: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(0));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(0));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Can_Create_Connection_From_First_0_After_2()
        {
            var connection = GenerateConnection(5, after: Cursor(2), first: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(3));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Can_Create_Connection_From_First_1_After_4()
        {
            var connection = GenerateConnection(5, after: Cursor(4), first: 1);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(5));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(5));
            connection.Edges.Count().ShouldEqual(1);
            connection.Items.Count().ShouldEqual(1);
            connection.Edges.ElementAt(0).Cursor.ShouldEqual(Cursor(5));
            connection.Edges.ElementAt(0).Node.Value.ShouldEqual(5);
            connection.Items.ElementAt(0).Value.ShouldEqual(5);
        }

        [Test]
        public void Can_Create_Connection_From_First_1_After_2()
        {
            var connection = GenerateConnection(5, after: Cursor(2), first: 1);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(3));
            connection.Edges.Count().ShouldEqual(1);
            connection.Items.Count().ShouldEqual(1);
            connection.Edges.ElementAt(0).Cursor.ShouldEqual(Cursor(3));
            connection.Edges.ElementAt(0).Node.Value.ShouldEqual(3);
            connection.Items.ElementAt(0).Value.ShouldEqual(3);
        }

        [Test]
        public void Can_Create_Connection_From_After_Last()
        {
            var connection = GenerateConnection(5, after: Cursor(5));

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(6));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(6));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Can_Create_Connection_From_Last_0()
        {
            var connection = GenerateConnection(5, last: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(5));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(5));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Can_Create_Connection_From_Last_0_Before_2()
        {
            var connection = GenerateConnection(5, before: Cursor(2), last: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(1));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Can_Create_Connection_From_Last_0_Before_3()
        {
            var connection = GenerateConnection(5, before: Cursor(3), last: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(2));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(2));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Can_Create_Connection_From_Before_First()
        {
            var connection = GenerateConnection(5, before: Cursor(1));

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(0));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(0));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
        }

        [Test]
        public void Cannot_Combine_Before_And_After()
        {
            ArgumentException argEx = null;
            try
            {
                GenerateConnection(5, before: Cursor(1), after: Cursor(1));
            }
            catch (ArgumentException ex)
            {
                argEx = ex;
            }
            argEx.ShouldNotBeNull();
            argEx?.Message.ShouldEqual("Cannot use `after` in conjunction with `before`.");
        }

        [Test]
        public void Cannot_Combine_First_And_Last()
        {
            ArgumentException argEx = null;
            try
            {
                GenerateConnection(5, first: 1, last: 1);
            }
            catch (ArgumentException ex)
            {
                argEx = ex;
            }
            argEx.ShouldNotBeNull();
            argEx?.Message.ShouldEqual("Cannot use `first` in conjunction with `last`.");
        }

        [Test]
        public void Cannot_Combine_First_And_Before()
        {
            ArgumentException argEx = null;
            try
            {
                GenerateConnection(5, first: 1, before: Cursor(3));
            }
            catch (ArgumentException ex)
            {
                argEx = ex;
            }
            argEx.ShouldNotBeNull();
            argEx?.Message.ShouldEqual("Cannot use `first` in conjunction with `before`.");
        }

        [Test]
        public void Cannot_Combine_Last_And_After()
        {
            ArgumentException argEx = null;
            try
            {
                GenerateConnection(5, last: 1, after: Cursor(1));
            }
            catch (ArgumentException ex)
            {
                argEx = ex;
            }
            argEx.ShouldNotBeNull();
            argEx?.Message.ShouldEqual("Cannot use `last` in conjunction with `after`.");
        }

        [Test]
        public void Can_Transform_Connection_And_Pick_First_3_After_1()
        {
            var connection = GenerateConnection(6, first: 3, after: Cursor(1));
            var transformedConnection = connection.Transform(foo => new Bar { Value = foo.Value.ToString("D4") });

            transformedConnection.TotalCount.ShouldEqual(6);
            transformedConnection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            transformedConnection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            transformedConnection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<Foo>(2));
            transformedConnection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<Foo>(4));
            transformedConnection.Edges.Count().ShouldEqual(3);
            transformedConnection.Items.Count().ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                transformedConnection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor<Foo>(i + 1));
                transformedConnection.Edges.ElementAt(i - 1).Node.Value.ShouldEqual($"{i + 1:D4}");
                transformedConnection.Items.ElementAt(i - 1).Value.ShouldEqual($"{i + 1:D4}");
            }
        }

        [Test]
        public void Can_Create_Connection_From_First_3O_In_Infinite_Collection()
        {
            var connection = GenerateInfiniteConnection(6, first: 3);

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(3));
            connection.Edges.Count().ShouldEqual(3);
            connection.Items.Count().ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor<int>(i));
                connection.Edges.ElementAt(i - 1).Node.ShouldEqual(i);
            }
        }

        [Test]
        public void Can_Create_Connection_From_First_3_After_2O_In_Infinite_Collection()
        {
            var connection = GenerateInfiniteConnection(6, first: 3, after: Cursor<int>(2));

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(5));
            connection.Edges.Count().ShouldEqual(3);
            connection.Items.Count().ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor<int>(i + 2));
                connection.Edges.ElementAt(i - 1).Node.ShouldEqual(i + 2);
            }
        }

        [Test]
        public void Cannot_Create_Connection_From_Last_3O_In_Infinite_Collection()
        {
            try
            {
                GenerateInfiniteConnection(6, last: 3);
                true.ShouldBeFalse("Getting the last entries of an infinite collection should fail.");
            }
            catch (Exception ex)
            {
                ex.Message.ShouldEqual("Enumerated too many entries");
            }
        }

        [Test]
        public void Cannot_Retrieve_Total_Count_Of_Infinite_Collection()
        {
            var connection = GenerateInfiniteConnection(6, first: 3);
            connection.TotalCount.ShouldBeNull();
        }

        [Test]
        public void Can_Create_Connection_From_Edges()
        {
            var edges = new List<Edge<int>>
            {
                new Edge<int> { Cursor = Cursor<int>(1), Node = 1 },
                new Edge<int> { Cursor = Cursor<int>(2), Node = 2 },
            };
            var connection = edges
                .ToConnection(true);

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(2));
            connection.Edges.Count().ShouldEqual(2);
            connection.Items.Count().ShouldEqual(2);
            connection.TotalCount.ShouldEqual(null);

            for (var i = 1; i <= 2; i++)
            {
                connection.Edges.ElementAt(i - 1).Cursor.ShouldEqual(Cursor<int>(i));
                connection.Edges.ElementAt(i - 1).Node.ShouldEqual(i);
            }
        }

        [Test]
        public void Can_Create_Connection_From_Edges_With_Count()
        {
            var edges = new List<Edge<int>>
            {
                new Edge<int> { Cursor = Cursor<int>(2), Node = 2 },
                new Edge<int> { Cursor = Cursor<int>(3), Node = 3 },
                new Edge<int> { Cursor = Cursor<int>(4), Node = 4 },
                new Edge<int> { Cursor = Cursor<int>(5), Node = 5 },
            };
            var connection = edges
                .ToConnection(true, true, 10);

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(2));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(5));
            connection.Edges.Count().ShouldEqual(4);
            connection.Items.Count().ShouldEqual(4);
            connection.TotalCount.ShouldEqual(10);
        }

        [Test]
        public void Can_Create_Connection_From_Zero_Edges()
        {
            var connection = new List<Edge<int>>()
                .ToConnection(true, false, null, Cursor<int>(4));

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(4));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(4));
            connection.Edges.Count().ShouldEqual(0);
            connection.Items.Count().ShouldEqual(0);
            connection.TotalCount.ShouldEqual(null);
        }

        [Test]
        public void Can_Create_Connection_From_Filtered_Enumerable()
        {
            var enumerable = Sequence(10)
                .Where(edge => (edge.Node / 2) % 2 == 1);

            var edges = enumerable.ToList();
            var connection = edges
                .ToConnection(false, totalCount: edges.Count());

            connection.TotalCount.ShouldEqual(5);

            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(9));

            connection.Edges.ElementAt(0).Cursor.ShouldEqual(Cursor<int>(1));
            connection.Edges.ElementAt(1).Cursor.ShouldEqual(Cursor<int>(3));
            connection.Edges.ElementAt(2).Cursor.ShouldEqual(Cursor<int>(5));
            connection.Edges.ElementAt(3).Cursor.ShouldEqual(Cursor<int>(7));
            connection.Edges.ElementAt(4).Cursor.ShouldEqual(Cursor<int>(9));

            connection.Edges.ElementAt(0).Node.ShouldEqual(1 * 2);
            connection.Edges.ElementAt(1).Node.ShouldEqual(3 * 2);
            connection.Edges.ElementAt(2).Node.ShouldEqual(5 * 2);
            connection.Edges.ElementAt(3).Node.ShouldEqual(7 * 2);
            connection.Edges.ElementAt(4).Node.ShouldEqual(9 * 2);
        }

        [Test]
        public void Can_Create_Connection_With_Single_Edge()
        {
            var items = new List<Edge<string>>
            {
                new Edge<string> { Cursor = Cursor<string>(1), Node = "onlyEntry" },
            };
            var connection = items
                .ToConnection(false);

            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(1));
        }

        [Test]
        public void Can_Create_Connection_With_Multiple_Edges()
        {
            var items = new List<Edge<string>>
            {
                new Edge<string> { Cursor = Cursor<string>(1), Node = "entry1" },
                new Edge<string> { Cursor = Cursor<string>(3), Node = "entry2" },
            };
            var connection = items
                .ToConnection(false);

            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(3));
        }

        [Test]
        public void Can_Create_Connection_From_Single_Edge_With_Offset()
        {
            var items = new List<Edge<string>>
            {
                new Edge<string> { Cursor = Cursor<string>(3), Node = "onlyEntry" },
            };
            var connection = items
                .ToConnection(false, totalCount: 1);

            connection.TotalCount.ShouldEqual(1);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(3));
        }

        [Test]
        public void Can_Create_Connection_From_Multiple_Edges_With_Offset()
        {
            var items = new List<Edge<string>>
            {
                new Edge<string> { Cursor = Cursor<string>(3), Node = "entry1" },
                new Edge<string> { Cursor = Cursor<string>(4), Node = "entry2" },
                new Edge<string> { Cursor = Cursor<string>(7), Node = "entry3" },
            };
            var connection = items
                .ToConnection(false, totalCount: items.Count);

            connection.TotalCount.ShouldEqual(3);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(7));
        }

        [Test]
        public void Can_Create_Connection_From_Empty_List_Of_Edges()
        {
            var connection = new List<Edge<string>>()
                .ToConnection(false, totalCount: 0);

            connection.TotalCount.ShouldEqual(0);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(0));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(0));
            connection.Items.Count().ShouldEqual(0);
            connection.Edges.Count().ShouldEqual(0);
        }

        private IEnumerable<Edge<int>> Sequence(int count)
        {
            for (var i = 1; i <= count; i++)
            {
                yield return new Edge<int>
                {
                    Cursor = Cursor<int>(i),
                    Node = i * 2,
                };
            }
        }

        private Cursor Cursor(int index)
        {
            return Cursor<Foo>(index);
        }

        private Cursor Cursor<T>(int index)
        {
            return GraphQL.Conventions.Relay.Cursor.New<T>(index);
        }

        private Connection<Foo> GenerateConnection(
            int count, int? first = null, int? last = null, Cursor? after = null, Cursor? before = null)
        {
            var items = new List<Foo>();
            for (var i = 1; i <= count; i++)
            {
                items.Add(new Foo { Value = i });
            }

            return items.ToConnection(first, after, last, before, count);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static IEnumerable<int> InfiniteCollection(int throwAboveThisNumber = -1)
        {
            for (var i = 1; ; i++)
            {
                if (throwAboveThisNumber >= 0 && i > throwAboveThisNumber)
                {
                    throw new Exception("Enumerated too many entries");
                }
                yield return i;
            }
        }

        private Connection<int> GenerateInfiniteConnection(int throwExceptionAboveThisNumber,
            int? first = null, int? last = null, Cursor? after = null, Cursor? before = null)
        {
            return InfiniteCollection(throwExceptionAboveThisNumber)
                .ToConnection(first, after, last, before);
        }

        private class Foo
        {
            public int Value { get; set; }
        }

        private class Bar
        {
            public string Value { get; set; }
        }
    }
}
