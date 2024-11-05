using GDWeave;

namespace ExtraVoiceMod;

public class Mod : IMod {
    public Config Config;

    public Mod(IModInterface modInterface) {
        this.Config = modInterface.ReadConfig<Config>();
        modInterface.RegisterScriptMod(new ExtraVoiceMod(Config));
        modInterface.Logger.Information("ExtraVoice's DLL has loaded!");
    }

    public void Dispose() {
        // Cleanup anything you do here
    }
}
