using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace ExtraVoiceMod;

public class PlayerDataMod(Config config) : IScriptMod
{

    private readonly Config _config = config;
    public bool ShouldRun(string path) => path == "res://Scenes/Singletons/playerdata.gdc";

    // returns a list of tokens for the new script, with the input being the original script's tokens
    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        // wait for any newline after any reference to "_ready"
        var waiter = new MultiTokenWaiter([
            t => t.Type is TokenType.PrVar,
            t => t is IdentifierToken{Name:"guitar_shapes"},
            t => t.Type is TokenType.OpAssign,
            t => t.Type is TokenType.BracketOpen,
            t => t.Type is TokenType.BracketClose,
        ], allowPartialMatch: false);

        // loop through all tokens in the script
        foreach (var token in tokens)
        {
            if (waiter.Check(token))
            {
                var voiceOption = _config.VoiceOption;
                var peerVoiceOption = _config.ModlessVoiceOption;

                yield return token;
                yield return new Token(TokenType.Newline);
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("player_voicebank");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new StringVariant(voiceOption));
                yield return new Token(TokenType.Newline);
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("peer_voicebank");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new StringVariant(peerVoiceOption));
                yield return new Token(TokenType.Newline);
            }
            else
            {
                // return the original token
                yield return token;
            }
        }
    }
}
