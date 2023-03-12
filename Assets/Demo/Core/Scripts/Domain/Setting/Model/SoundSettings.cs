using System;
using UniRx;

namespace Demo.Core.Scripts.Domain.Setting.Model
{
    public sealed class SoundSettings
    {
        private readonly Subject<ValueChangedEvent> _valueChangedSubject = new Subject<ValueChangedEvent>();

        public float Volume { get; private set; }
        public bool Muted { get; private set; }
        public IObservable<ValueChangedEvent> ValueChanged => _valueChangedSubject;

        internal void SetValues(float volume, bool muted)
        {
            Volume = volume;
            Muted = muted;
            _valueChangedSubject.OnNext(new ValueChangedEvent(volume, muted));
        }

        public readonly struct ValueChangedEvent
        {
            public ValueChangedEvent(float volume, bool muted)
            {
                Volume = volume;
                Muted = muted;
            }

            public float Volume { get; }
            public bool Muted { get; }
        }
    }
}
