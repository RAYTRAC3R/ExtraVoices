using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace ExtraVoiceMod;

public class ExtraVoiceMod(Config config) : IScriptMod {

    private readonly Config _config = config;
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

    // returns a list of tokens for the new script, with the input being the original script's tokens
    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens) {
        // wait for any newline after any reference to "_ready"
        var waiter = new MultiTokenWaiter([
            t => t is ConstantToken{Value: StringVariant {Value: "NewVoice"}},
        ], allowPartialMatch: true);

        // loop through all tokens in the script
        foreach (var token in tokens) {
            if (waiter.Check(token)) {
                var voiceOption = _config.VoiceOption;
                var validOptions = new[] { "NewVoice", "OldVoice", "Animalese" };

                if (!validOptions.Contains(voiceOption))
                {
                    voiceOption = "NewVoice";
                }

                // found our match, swap it out for the voice option
                yield return new ConstantToken(new StringVariant(voiceOption));
            } else {
                // return the original token
                yield return token;
            }
        }
    }
}
