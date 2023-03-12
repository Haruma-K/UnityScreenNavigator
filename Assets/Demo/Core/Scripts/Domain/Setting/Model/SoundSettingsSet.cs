namespace Demo.Core.Scripts.Domain.Setting.Model
{
    public sealed class SoundSettingsSet
    {
        public SoundSettings Voice { get; } = new SoundSettings();
        public SoundSettings Bgm { get; } = new SoundSettings();
        public SoundSettings Se { get; } = new SoundSettings();
    }
}
