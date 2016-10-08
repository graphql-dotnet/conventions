using BenchmarkDotNet.Attributes;

namespace GraphQLParser.Benchmarks {
    [Config(typeof(DefaultBenchmarkConfig))]
    public class LexerBenchmark {
        private const string KitchenSink = @"
query queryName($foo: ComplexType, $site: Site = MOBILE) {
  whoever123is: node(id: [123, 456]) {
    id ,
    ... on User @defer {
      field2 {
        id ,
        alias: field1(first:10, after:$foo,) @include(if: $foo) {
          id,
          ...frag
        }
      }
    }
    ... @skip(unless: $foo) {
      id
    }
    ... {
      id
    }
  }
}

mutation likeStory {
  like(story: 123) @defer {
    story {
      id
    }
  }
}

subscription StoryLikeSubscription($input: StoryLikeSubscribeInput) {
  storyLikeSubscribe(input: $input) {
    story {
      likers {
        count
      }
      likeSentence {
        text
      }
    }
  }
}

fragment frag on Friend {
  foo(size: $size, bar: $b, obj: {key: ""value""})
}

{
  unnamed(truthy: true, falsey: false),
  query
}";

        [Benchmark]
        public void LexKitchenSink() {
            var lexer = new Lexer();
            var source = new Source(KitchenSink);
            var resetPosition = 0;
            Token token;
            while((token = lexer.Lex(source, resetPosition)).Kind != TokenKind.EOF) {
                resetPosition = token.End;
            }
        }
    }
}
