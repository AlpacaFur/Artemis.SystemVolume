using Artemis.Core.DataModelExpansions;
using Artemis.SystemVolume.DataModels;
using NAudio.CoreAudioApi;
using System;

namespace Artemis.SystemVolume
{
    public class VolumeDataModelExpansion : DataModelExpansion<VolumeDataModel>
    {

        private MMDevice _playbackDevice;

        public override void Enable()
        {
            var enumerator = new MMDeviceEnumerator();
            _playbackDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        }

        public override void Disable()
        {
            _playbackDevice.Dispose();
        }

        public override void Update(double deltaTime)
        {
            int newVolume = (int)(_playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            if (newVolume != DataModel.volume) DataModel.volumeChanged.Trigger();
            DataModel.volume = newVolume;
        }
    }
}