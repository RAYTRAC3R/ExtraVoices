using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace ExtraVoiceMod;

public class ExtraVoiceMod(Config config) : IScriptMod {

    private readonly Config _config = config;
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

    // returns a list of tokens for the new script, with the input being the original script's tokens
    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens) {
        //14683: IdentifierToken(Identifier, 75, PlayerData)
        //14684: Token(Period, )
        //14685: IdentifierToken(Identifier, 711, voice_pitch)
        //14686: Token(ParenthesisClose, )

        var starttalkwaiter = new MultiTokenWaiter([
            t => t.Type is TokenType.Newline & t.AssociatedData is 1,
            t => t is IdentifierToken{Name:"_talk"},
            t => t.Type is TokenType.ParenthesisOpen,
            t => t is IdentifierToken{Name:"letter"},
            t => t.Type is TokenType.Comma,
            t => t is IdentifierToken{Name:"PlayerData"},
            t => t.Type is TokenType.Period,
            t => t is IdentifierToken{Name:"voice_pitch"},
        ], allowPartialMatch: false);


        //14696: Token(BracketOpen, )
        //14697: IdentifierToken(Identifier, 708, letter)
        //14698: Token(Comma, )
        //14699: IdentifierToken(Identifier, 75, PlayerData)
        //14700: Token(Period, )
        //14701: IdentifierToken(Identifier, 711, voice_pitch)
        //14702: Token(BracketClose, )

        var synctalkwaiter = new MultiTokenWaiter([
            t => t.Type is TokenType.BracketOpen,
            t => t is IdentifierToken{Name:"letter"},
            t => t.Type is TokenType.Comma,
            t => t is IdentifierToken{Name:"PlayerData"},
            t => t.Type is TokenType.Period,
            t => t is IdentifierToken{Name:"voice_pitch"},
        ], allowPartialMatch: false);

        var dotalkwaiter = new MultiTokenWaiter([
            t => t.Type is TokenType.PrFunction,
            t => t is IdentifierToken{Name:"_talk"},
            t => t.Type is TokenType.ParenthesisOpen,
            t => t is IdentifierToken{Name:"letter"},
            t => t.Type is TokenType.Comma,
            t => t is IdentifierToken{ Name:"pitch"},
            t => t.Type == TokenType.OpAssign,
            t => t is ConstantToken{Value: RealVariant {Value: 1.5}},
        ], allowPartialMatch: false);

        var voicewaiter = new MultiTokenWaiter([
            t => t is ConstantToken{Value: StringVariant {Value: "NewVoice"}},
        ], allowPartialMatch: false);

        // loop through all tokens in the script
        foreach (var token in tokens) {
            if (starttalkwaiter.Check(token)) {
                yield return token;
                yield return new Token(TokenType.Comma);
                yield return new IdentifierToken("PlayerData");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("player_voicebank");
            }
            else if (synctalkwaiter.Check(token))
            {
                yield return token;
                yield return new Token(TokenType.Comma);
                yield return new IdentifierToken("PlayerData");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("player_voicebank");
            }
            else if (dotalkwaiter.Check(token))
            {
                yield return token;
                yield return new Token(TokenType.Comma);
                yield return new IdentifierToken("voicebank");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new StringVariant("NewVoice"));
                //yield return new IdentifierToken("PlayerData");
                //yield return new Token(TokenType.Period);
                //yield return new IdentifierToken("player_voicebank");

            }
            else if (voicewaiter.Check(token))
                {
                    //var voiceOption = _config.VoiceOption;
                    //var validOptions = new[] { "NewVoice", "OldVoice", "Animalese" };

                    //if (!validOptions.Contains(voiceOption))
                    //{
                    //    voiceOption = "NewVoice";
                    //}

                    // found our match, swap it out for the voice option
                    yield return new IdentifierToken("voicebank");
            }
                else {
                // return the original token
                yield return token;
            }
        }
    }
}
