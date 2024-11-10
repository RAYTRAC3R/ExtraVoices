using System.Text.Json.Serialization;

namespace ExtraVoiceMod;

public class Config
{
    [JsonInclude] public string Options = "Base Options are: OldVoice, NewVoice, Minty, Dhama";
    [JsonInclude] public string VoiceOption = "NewVoice";
    [JsonInclude] public string ModlessVoiceOption = "NewVoice";
}