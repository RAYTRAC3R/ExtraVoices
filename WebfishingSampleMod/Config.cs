using System.Text.Json.Serialization;

namespace ExtraVoiceMod;

public class Config
{
    [JsonInclude] public string Options = "Options are: OldVoice, NewVoice, Animalese";
    [JsonInclude] public string VoiceOption = "NewVoice";
}