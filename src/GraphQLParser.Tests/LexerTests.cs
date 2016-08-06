namespace GraphQLParser.Tests
{
    using GraphQLParser;
    using NUnit.Framework;

    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void Lex_ATPunctuation_HasCorrectEnd()
        {
            var token = GetATPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_ATPunctuation_HasCorrectKind()
        {
            var token = GetATPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.AT, token.Kind);
        }

        [Test]
        public void Lex_ATPunctuation_HasCorrectStart()
        {
            var token = GetATPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_ATPunctuation_HasCorrectValue()
        {
            var token = GetATPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_BangPunctuation_HasCorrectEnd()
        {
            var token = GetBangPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_BangPunctuation_HasCorrectKind()
        {
            var token = GetBangPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.BANG, token.Kind);
        }

        [Test]
        public void Lex_BangPunctuation_HasCorrectStart()
        {
            var token = GetBangPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_BangPunctuation_HasCorrectValue()
        {
            var token = GetBangPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_ColonPunctuation_HasCorrectEnd()
        {
            var token = GetColonPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_ColonPunctuation_HasCorrectKind()
        {
            var token = GetColonPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.COLON, token.Kind);
        }

        [Test]
        public void Lex_ColonPunctuation_HasCorrectStart()
        {
            var token = GetColonPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_ColonPunctuation_HasCorrectValue()
        {
            var token = GetColonPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_DollarPunctuation_HasCorrectEnd()
        {
            var token = GetDollarPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_DollarPunctuation_HasCorrectKind()
        {
            var token = GetDollarPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.DOLLAR, token.Kind);
        }

        [Test]
        public void Lex_DollarPunctuation_HasCorrectStart()
        {
            var token = GetDollarPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_DollarPunctuation_HasCorrectValue()
        {
            var token = GetDollarPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_EmptySource_ReturnsEOF()
        {
            var token = new Lexer().Lex(new Source(""));

            Assert.AreEqual(TokenKind.EOF, token.Kind);
        }

        [Test]
        public void Lex_EqualsPunctuation_HasCorrectEnd()
        {
            var token = GetEqualsPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_EqualsPunctuation_HasCorrectKind()
        {
            var token = GetEqualsPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.EQUALS, token.Kind);
        }

        [Test]
        public void Lex_EqualsPunctuation_HasCorrectStart()
        {
            var token = GetEqualsPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_EqualsPunctuation_HasCorrectValue()
        {
            var token = GetEqualsPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_EscapedStringToken_HasCorrectEnd()
        {
            var token = GetEscapedStringTokenLexer();
            Assert.AreEqual(20, token.End);
        }

        [Test]
        public void Lex_EscapedStringToken_HasCorrectStart()
        {
            var token = GetEscapedStringTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_EscapedStringToken_HasCorrectValue()
        {
            var token = GetEscapedStringTokenLexer();
            Assert.AreEqual("escaped \n\r\b\t\f", token.Value);
        }

        [Test]
        public void Lex_EscapedStringToken_HasStringKind()
        {
            var token = GetEscapedStringTokenLexer();
            Assert.AreEqual(TokenKind.STRING, token.Kind);
        }

        [Test]
        public void Lex_LeftBracePunctuation_HasCorrectEnd()
        {
            var token = GetLeftBracePunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_LeftBracePunctuation_HasCorrectKind()
        {
            var token = GetLeftBracePunctuationTokenLexer();
            Assert.AreEqual(TokenKind.BRACE_L, token.Kind);
        }

        [Test]
        public void Lex_LeftBracePunctuation_HasCorrectStart()
        {
            var token = GetLeftBracePunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_LeftBracePunctuation_HasCorrectValue()
        {
            var token = GetLeftBracePunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_LeftBracketPunctuation_HasCorrectEnd()
        {
            var token = GetLeftBracketPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_LeftBracketPunctuation_HasCorrectKind()
        {
            var token = GetLeftBracketPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.BRACKET_L, token.Kind);
        }

        [Test]
        public void Lex_LeftBracketPunctuation_HasCorrectStart()
        {
            var token = GetLeftBracketPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_LeftBracketPunctuation_HasCorrectValue()
        {
            var token = GetLeftBracketPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_LeftParenthesisPunctuation_HasCorrectEnd()
        {
            var token = GetLeftParenthesisPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_LeftParenthesisPunctuation_HasCorrectKind()
        {
            var token = GetLeftParenthesisPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.PAREN_L, token.Kind);
        }

        [Test]
        public void Lex_LeftParenthesisPunctuation_HasCorrectStart()
        {
            var token = GetLeftParenthesisPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_LeftParenthesisPunctuation_HasCorrectValue()
        {
            var token = GetLeftParenthesisPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_MultipleDecimalsIntToken_HasCorrectEnd()
        {
            var token = GetMultipleDecimalsIntTokenLexer();
            Assert.AreEqual(3, token.End);
        }

        [Test]
        public void Lex_MultipleDecimalsIntToken_HasCorrectStart()
        {
            var token = GetMultipleDecimalsIntTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_MultipleDecimalsIntToken_HasCorrectValue()
        {
            var token = GetMultipleDecimalsIntTokenLexer();
            Assert.AreEqual("123", token.Value);
        }

        [Test]
        public void Lex_MultipleDecimalsIntToken_HasIntKind()
        {
            var token = GetMultipleDecimalsIntTokenLexer();
            Assert.AreEqual(TokenKind.INT, token.Kind);
        }

        [Test]
        public void Lex_NameTokenWithComments_HasCorrectEnd()
        {
            var token = GetSingleNameTokenLexerWithComments();
            Assert.AreEqual(13, token.End);
        }

        [Test]
        public void Lex_NameTokenWithComments_HasCorrectStart()
        {
            var token = GetSingleNameTokenLexerWithComments();
            Assert.AreEqual(10, token.Start);
        }

        [Test]
        public void Lex_NameTokenWithComments_HasCorrectValue()
        {
            var token = GetSingleNameTokenLexerWithComments();
            Assert.AreEqual("foo", token.Value);
        }

        [Test]
        public void Lex_NameTokenWithComments_HasNameKind()
        {
            var token = GetSingleNameTokenLexerWithComments();
            Assert.AreEqual(TokenKind.NAME, token.Kind);
        }

        [Test]
        public void Lex_NameTokenWithWhitespaces_HasCorrectEnd()
        {
            var token = GetSingleNameTokenLexerSurroundedWithWhitespaces();
            Assert.AreEqual(12, token.End);
        }

        [Test]
        public void Lex_NameTokenWithWhitespaces_HasCorrectStart()
        {
            var token = GetSingleNameTokenLexerSurroundedWithWhitespaces();
            Assert.AreEqual(9, token.Start);
        }

        [Test]
        public void Lex_NameTokenWithWhitespaces_HasCorrectValue()
        {
            var token = GetSingleNameTokenLexerSurroundedWithWhitespaces();
            Assert.AreEqual("foo", token.Value);
        }

        [Test]
        public void Lex_NameTokenWithWhitespaces_HasNameKind()
        {
            var token = GetSingleNameTokenLexerSurroundedWithWhitespaces();
            Assert.AreEqual(TokenKind.NAME, token.Kind);
        }

        [Test]
        public void Lex_NullInput_ReturnsEOF()
        {
            var token = new Lexer().Lex(new Source(null));

            Assert.AreEqual(TokenKind.EOF, token.Kind);
        }

        [Test]
        public void Lex_PipePunctuation_HasCorrectEnd()
        {
            var token = GetPipePunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_PipePunctuation_HasCorrectKind()
        {
            var token = GetPipePunctuationTokenLexer();
            Assert.AreEqual(TokenKind.PIPE, token.Kind);
        }

        [Test]
        public void Lex_PipePunctuation_HasCorrectStart()
        {
            var token = GetPipePunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_PipePunctuation_HasCorrectValue()
        {
            var token = GetPipePunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_QuoteStringToken_HasCorrectEnd()
        {
            var token = GetQuoteStringTokenLexer();
            Assert.AreEqual(10, token.End);
        }

        [Test]
        public void Lex_QuoteStringToken_HasCorrectStart()
        {
            var token = GetQuoteStringTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_QuoteStringToken_HasCorrectValue()
        {
            var token = GetQuoteStringTokenLexer();
            Assert.AreEqual("quote \"", token.Value);
        }

        [Test]
        public void Lex_QuoteStringToken_HasStringKind()
        {
            var token = GetQuoteStringTokenLexer();
            Assert.AreEqual(TokenKind.STRING, token.Kind);
        }

        [Test]
        public void Lex_RightBracePunctuation_HasCorrectEnd()
        {
            var token = GetRightBracePunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_RightBracePunctuation_HasCorrectKind()
        {
            var token = GetRightBracePunctuationTokenLexer();
            Assert.AreEqual(TokenKind.BRACE_R, token.Kind);
        }

        [Test]
        public void Lex_RightBracePunctuation_HasCorrectStart()
        {
            var token = GetRightBracePunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_RightBracePunctuation_HasCorrectValue()
        {
            var token = GetRightBracePunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_RightBracketPunctuation_HasCorrectEnd()
        {
            var token = GetRightBracketPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_RightBracketPunctuation_HasCorrectKind()
        {
            var token = GetRightBracketPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.BRACKET_R, token.Kind);
        }

        [Test]
        public void Lex_RightBracketPunctuation_HasCorrectStart()
        {
            var token = GetRightBracketPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_RightBracketPunctuation_HasCorrectValue()
        {
            var token = GetRightBracketPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_RightParenthesisPunctuation_HasCorrectEnd()
        {
            var token = GetRightParenthesisPunctuationTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_RightParenthesisPunctuation_HasCorrectKind()
        {
            var token = GetRightParenthesisPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.PAREN_R, token.Kind);
        }

        [Test]
        public void Lex_RightParenthesisPunctuation_HasCorrectStart()
        {
            var token = GetRightParenthesisPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_RightParenthesisPunctuation_HasCorrectValue()
        {
            var token = GetRightParenthesisPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_SimpleStringToken_HasCorrectEnd()
        {
            var token = GetSimpleStringTokenLexer();
            Assert.AreEqual(5, token.End);
        }

        [Test]
        public void Lex_SimpleStringToken_HasCorrectStart()
        {
            var token = GetSimpleStringTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SimpleStringToken_HasCorrectValue()
        {
            var token = GetSimpleStringTokenLexer();
            Assert.AreEqual("str", token.Value);
        }

        [Test]
        public void Lex_SimpleStringToken_HasStringKind()
        {
            var token = GetSimpleStringTokenLexer();
            Assert.AreEqual(TokenKind.STRING, token.Kind);
        }

        [Test]
        public void Lex_SingleDecimalIntToken_HasCorrectEnd()
        {
            var token = GetSingleDecimalIntTokenLexer();
            Assert.AreEqual(1, token.End);
        }

        [Test]
        public void Lex_SingleDecimalIntToken_HasCorrectStart()
        {
            var token = GetSingleDecimalIntTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleDecimalIntToken_HasCorrectValue()
        {
            var token = GetSingleDecimalIntTokenLexer();
            Assert.AreEqual("0", token.Value);
        }

        [Test]
        public void Lex_SingleDecimalIntToken_HasIntKind()
        {
            var token = GetSingleDecimalIntTokenLexer();
            Assert.AreEqual(TokenKind.INT, token.Kind);
        }

        [Test]
        public void Lex_SingleFloatTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleFloatTokenLexer();
            Assert.AreEqual(5, token.End);
        }

        [Test]
        public void Lex_SingleFloatTokenLexer_HasCorrectKind()
        {
            var token = GetSingleFloatTokenLexer();
            Assert.AreEqual(TokenKind.FLOAT, token.Kind);
        }

        [Test]
        public void Lex_SingleFloatTokenLexer_HasCorrectStart()
        {
            var token = GetSingleFloatTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleFloatTokenLexer_HasCorrectValue()
        {
            var token = GetSingleFloatTokenLexer();
            Assert.AreEqual("4.123", token.Value);
        }

        [Test]
        public void Lex_SingleFloatWithExplicitlyPositiveExponentTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleFloatWithExplicitlyPositiveExponentTokenLexer();
            Assert.AreEqual(6, token.End);
        }

        [Test]
        public void Lex_SingleFloatWithExplicitlyPositiveExponentTokenLexer_HasCorrectKind()
        {
            var token = GetSingleFloatWithExplicitlyPositiveExponentTokenLexer();
            Assert.AreEqual(TokenKind.FLOAT, token.Kind);
        }

        [Test]
        public void Lex_SingleFloatWithExplicitlyPositiveExponentTokenLexer_HasCorrectStart()
        {
            var token = GetSingleFloatWithExplicitlyPositiveExponentTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleFloatWithExplicitlyPositiveExponentTokenLexer_HasCorrectValue()
        {
            var token = GetSingleFloatWithExplicitlyPositiveExponentTokenLexer();
            Assert.AreEqual("123e+4", token.Value);
        }

        [Test]
        public void Lex_SingleFloatWithExponentCapitalLetterTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleFloatWithExponentCapitalLetterTokenLexer();
            Assert.AreEqual(5, token.End);
        }

        [Test]
        public void Lex_SingleFloatWithExponentCapitalLetterTokenLexer_HasCorrectKind()
        {
            var token = GetSingleFloatWithExponentCapitalLetterTokenLexer();
            Assert.AreEqual(TokenKind.FLOAT, token.Kind);
        }

        [Test]
        public void Lex_SingleFloatWithExponentCapitalLetterTokenLexer_HasCorrectStart()
        {
            var token = GetSingleFloatWithExponentCapitalLetterTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleFloatWithExponentCapitalLetterTokenLexer_HasCorrectValue()
        {
            var token = GetSingleFloatWithExponentCapitalLetterTokenLexer();
            Assert.AreEqual("123E4", token.Value);
        }

        [Test]
        public void Lex_SingleFloatWithExponentTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleFloatWithExponentTokenLexer();
            Assert.AreEqual(5, token.End);
        }

        [Test]
        public void Lex_SingleFloatWithExponentTokenLexer_HasCorrectKind()
        {
            var token = GetSingleFloatWithExponentTokenLexer();
            Assert.AreEqual(TokenKind.FLOAT, token.Kind);
        }

        [Test]
        public void Lex_SingleFloatWithExponentTokenLexer_HasCorrectStart()
        {
            var token = GetSingleFloatWithExponentTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleFloatWithExponentTokenLexer_HasCorrectValue()
        {
            var token = GetSingleFloatWithExponentTokenLexer();
            Assert.AreEqual("123e4", token.Value);
        }

        [Test]
        public void Lex_SingleFloatWithNegativeExponentTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleFloatWithNegativeExponentTokenLexer();
            Assert.AreEqual(6, token.End);
        }

        [Test]
        public void Lex_SingleFloatWithNegativeExponentTokenLexer_HasCorrectKind()
        {
            var token = GetSingleFloatWithNegativeExponentTokenLexer();
            Assert.AreEqual(TokenKind.FLOAT, token.Kind);
        }

        [Test]
        public void Lex_SingleFloatWithNegativeExponentTokenLexer_HasCorrectStart()
        {
            var token = GetSingleFloatWithNegativeExponentTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleFloatWithNegativeExponentTokenLexer_HasCorrectValue()
        {
            var token = GetSingleFloatWithNegativeExponentTokenLexer();
            Assert.AreEqual("123e-4", token.Value);
        }

        [Test]
        public void Lex_SingleNameSurroundedByCommasTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleNameSurroundedByCommasTokenLexer();
            Assert.AreEqual(6, token.End);
        }

        [Test]
        public void Lex_SingleNameSurroundedByCommasTokenLexer_HasCorrectKind()
        {
            var token = GetSingleNameSurroundedByCommasTokenLexer();
            Assert.AreEqual(TokenKind.NAME, token.Kind);
        }

        [Test]
        public void Lex_SingleNameSurroundedByCommasTokenLexer_HasCorrectStart()
        {
            var token = GetSingleNameSurroundedByCommasTokenLexer();
            Assert.AreEqual(3, token.Start);
        }

        [Test]
        public void Lex_SingleNameSurroundedByCommasTokenLexer_HasCorrectValue()
        {
            var token = GetSingleNameSurroundedByCommasTokenLexer();
            Assert.AreEqual("foo", token.Value);
        }

        [Test]
        public void Lex_SingleNameWithBOMHeaderTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleNameWithBOMHeaderTokenLexer();
            Assert.AreEqual(5, token.End);
        }

        [Test]
        public void Lex_SingleNameWithBOMHeaderTokenLexer_HasCorrectKind()
        {
            var token = GetSingleNameWithBOMHeaderTokenLexer();
            Assert.AreEqual(TokenKind.NAME, token.Kind);
        }

        [Test]
        public void Lex_SingleNameWithBOMHeaderTokenLexer_HasCorrectStart()
        {
            var token = GetSingleNameWithBOMHeaderTokenLexer();
            Assert.AreEqual(2, token.Start);
        }

        [Test]
        public void Lex_SingleNameWithBOMHeaderTokenLexer_HasCorrectValue()
        {
            var token = GetSingleNameWithBOMHeaderTokenLexer();
            Assert.AreEqual("foo", token.Value);
        }

        [Test]
        public void Lex_SingleNegativeFloatTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleNegativeFloatTokenLexer();
            Assert.AreEqual(6, token.End);
        }

        [Test]
        public void Lex_SingleNegativeFloatTokenLexer_HasCorrectKind()
        {
            var token = GetSingleNegativeFloatTokenLexer();
            Assert.AreEqual(TokenKind.FLOAT, token.Kind);
        }

        [Test]
        public void Lex_SingleNegativeFloatTokenLexer_HasCorrectStart()
        {
            var token = GetSingleNegativeFloatTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleNegativeFloatTokenLexer_HasCorrectValue()
        {
            var token = GetSingleNegativeFloatTokenLexer();
            Assert.AreEqual("-0.123", token.Value);
        }

        [Test]
        public void Lex_SingleNegativeFloatWithExponentTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleNegativeFloatWithExponentTokenLexer();
            Assert.AreEqual(6, token.End);
        }

        [Test]
        public void Lex_SingleNegativeFloatWithExponentTokenLexer_HasCorrectKind()
        {
            var token = GetSingleNegativeFloatWithExponentTokenLexer();
            Assert.AreEqual(TokenKind.FLOAT, token.Kind);
        }

        [Test]
        public void Lex_SingleNegativeFloatWithExponentTokenLexer_HasCorrectStart()
        {
            var token = GetSingleNegativeFloatWithExponentTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleNegativeFloatWithExponentTokenLexer_HasCorrectValue()
        {
            var token = GetSingleNegativeFloatWithExponentTokenLexer();
            Assert.AreEqual("-123e4", token.Value);
        }

        [Test]
        public void Lex_SingleNegativeIntTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleNegativeIntTokenLexer();
            Assert.AreEqual(2, token.End);
        }

        [Test]
        public void Lex_SingleNegativeIntTokenLexer_HasCorrectKind()
        {
            var token = GetSingleNegativeIntTokenLexer();
            Assert.AreEqual(TokenKind.INT, token.Kind);
        }

        [Test]
        public void Lex_SingleNegativeIntTokenLexer_HasCorrectStart()
        {
            var token = GetSingleNegativeIntTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleNegativeIntTokenLexer_HasCorrectValue()
        {
            var token = GetSingleNegativeIntTokenLexer();
            Assert.AreEqual("-3", token.Value);
        }

        [Test]
        public void Lex_SingleStringWithSlashesTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleStringWithSlashesTokenLexer();
            Assert.AreEqual(15, token.End);
        }

        [Test]
        public void Lex_SingleStringWithSlashesTokenLexer_HasCorrectKind()
        {
            var token = GetSingleStringWithSlashesTokenLexer();
            Assert.AreEqual(TokenKind.STRING, token.Kind);
        }

        [Test]
        public void Lex_SingleStringWithSlashesTokenLexer_HasCorrectStart()
        {
            var token = GetSingleStringWithSlashesTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleStringWithSlashesTokenLexer_HasCorrectValue()
        {
            var token = GetSingleStringWithSlashesTokenLexer();
            Assert.AreEqual("slashes \\ /", token.Value);
        }

        [Test]
        public void Lex_SingleStringWithUnicodeCharactersTokenLexer_HasCorrectEnd()
        {
            var token = GetSingleStringWithUnicodeCharactersTokenLexer();
            Assert.AreEqual(34, token.End);
        }

        [Test]
        public void Lex_SingleStringWithUnicodeCharactersTokenLexer_HasCorrectKind()
        {
            var token = GetSingleStringWithUnicodeCharactersTokenLexer();
            Assert.AreEqual(TokenKind.STRING, token.Kind);
        }

        [Test]
        public void Lex_SingleStringWithUnicodeCharactersTokenLexer_HasCorrectStart()
        {
            var token = GetSingleStringWithUnicodeCharactersTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SingleStringWithUnicodeCharactersTokenLexer_HasCorrectValue()
        {
            var token = GetSingleStringWithUnicodeCharactersTokenLexer();
            Assert.AreEqual("unicode \u1234\u5678\u90AB\uCDEF", token.Value);
        }

        [Test]
        public void Lex_SpreadPunctuation_HasCorrectEnd()
        {
            var token = GetSpreadPunctuationTokenLexer();
            Assert.AreEqual(3, token.End);
        }

        [Test]
        public void Lex_SpreadPunctuation_HasCorrectKind()
        {
            var token = GetSpreadPunctuationTokenLexer();
            Assert.AreEqual(TokenKind.SPREAD, token.Kind);
        }

        [Test]
        public void Lex_SpreadPunctuation_HasCorrectStart()
        {
            var token = GetSpreadPunctuationTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_SpreadPunctuation_HasCorrectValue()
        {
            var token = GetSpreadPunctuationTokenLexer();
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Lex_WhiteSpaceStringToken_HasCorrectEnd()
        {
            var token = GetWhiteSpaceStringTokenLexer();
            Assert.AreEqual(15, token.End);
        }

        [Test]
        public void Lex_WhiteSpaceStringToken_HasCorrectStart()
        {
            var token = GetWhiteSpaceStringTokenLexer();
            Assert.AreEqual(0, token.Start);
        }

        [Test]
        public void Lex_WhiteSpaceStringToken_HasCorrectValue()
        {
            var token = GetWhiteSpaceStringTokenLexer();
            Assert.AreEqual(" white space ", token.Value);
        }

        [Test]
        public void Lex_WhiteSpaceStringToken_HasStringKind()
        {
            var token = GetWhiteSpaceStringTokenLexer();
            Assert.AreEqual(TokenKind.STRING, token.Kind);
        }

        private static Token GetATPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("@"));
        }

        private static Token GetBangPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("!"));
        }

        private static Token GetColonPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source(":"));
        }

        private static Token GetDollarPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("$"));
        }

        private static Token GetEqualsPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("="));
        }

        private static Token GetEscapedStringTokenLexer()
        {
            return new Lexer().Lex(new Source("\"escaped \\n\\r\\b\\t\\f\""));
        }

        private static Token GetLeftBracePunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("{"));
        }

        private static Token GetLeftBracketPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("["));
        }

        private static Token GetLeftParenthesisPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("("));
        }

        private static Token GetMultipleDecimalsIntTokenLexer()
        {
            return new Lexer().Lex(new Source("123"));
        }

        private static Token GetPipePunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("|"));
        }

        private static Token GetQuoteStringTokenLexer()
        {
            return new Lexer().Lex(new Source("\"quote \\\"\""));
        }

        private static Token GetRightBracePunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("}"));
        }

        private static Token GetRightBracketPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("]"));
        }

        private static Token GetRightParenthesisPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source(")"));
        }

        private static Token GetSimpleStringTokenLexer()
        {
            return new Lexer().Lex(new Source("\"str\""));
        }

        private static Token GetSingleDecimalIntTokenLexer()
        {
            return new Lexer().Lex(new Source("0"));
        }

        private static Token GetSingleFloatTokenLexer()
        {
            return new Lexer().Lex(new Source("4.123"));
        }

        private static Token GetSingleFloatWithExplicitlyPositiveExponentTokenLexer()
        {
            return new Lexer().Lex(new Source("123e+4"));
        }

        private static Token GetSingleFloatWithExponentCapitalLetterTokenLexer()
        {
            return new Lexer().Lex(new Source("123E4"));
        }

        private static Token GetSingleFloatWithExponentTokenLexer()
        {
            return new Lexer().Lex(new Source("123e4"));
        }

        private static Token GetSingleFloatWithNegativeExponentTokenLexer()
        {
            return new Lexer().Lex(new Source("123e-4"));
        }

        private static Token GetSingleNameSurroundedByCommasTokenLexer()
        {
            return new Lexer().Lex(new Source(",,,foo,,,"));
        }

        private static Token GetSingleNameTokenLexerSurroundedWithWhitespaces()
        {
            return new Lexer().Lex(new Source("\r\n        foo\r\n\r\n    "));
        }

        private static Token GetSingleNameTokenLexerWithComments()
        {
            return new Lexer().Lex(new Source("\r\n#comment\r\nfoo#comment"));
        }

        private static Token GetSingleNameWithBOMHeaderTokenLexer()
        {
            return new Lexer().Lex(new Source("\uFEFF foo\\"));
        }

        private static Token GetSingleNegativeFloatTokenLexer()
        {
            return new Lexer().Lex(new Source("-0.123"));
        }

        private static Token GetSingleNegativeFloatWithExponentTokenLexer()
        {
            return new Lexer().Lex(new Source("-123e4"));
        }

        private static Token GetSingleNegativeIntTokenLexer()
        {
            return new Lexer().Lex(new Source("-3"));
        }

        private static Token GetSingleStringWithSlashesTokenLexer()
        {
            return new Lexer().Lex(new Source("\"slashes \\\\ \\/\""));
        }

        private static Token GetSingleStringWithUnicodeCharactersTokenLexer()
        {
            return new Lexer().Lex(new Source("\"unicode \\u1234\\u5678\\u90AB\\uCDEF\""));
        }

        private static Token GetSpreadPunctuationTokenLexer()
        {
            return new Lexer().Lex(new Source("..."));
        }

        private static Token GetWhiteSpaceStringTokenLexer()
        {
            return new Lexer().Lex(new Source("\" white space \""));
        }
    }
}