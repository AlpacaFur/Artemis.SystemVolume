using Artemis.Core.DataModelExpansions;
using Artemis.SystemVolume.DataModels;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;


namespace Artemis.SystemVolume
{
    public class VolumeDataModelExpansion : DataModelExpansion<VolumeDataModel>
    {

        private MMDevice _playbackDevice;
        private NaudioNotificationClient _notificationClient;
        private IMMNotificationClient _notifyClient;
        private MMDeviceEnumerator _enumerator;

        public override void Enable()
        {
            _notificationClient = new NaudioNotificationClient();
            _notificationClient.DefaultDeviceChanged += NotificationClient_DefaultDeviceChanged;
            _notifyClient = (IMMNotificationClient)_notificationClient;
            _enumerator = new MMDeviceEnumerator();
            _enumerator.RegisterEndpointNotificationCallback(_notifyClient);
            SetPlaybackDevice();
        }

        private void SetPlaybackDevice()
        {
            _playbackDevice = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        }
        private void NotificationClient_DefaultDeviceChanged()
        {
            SetPlaybackDevice();
        }

        public override void Disable()
        {
            _notificationClient.DefaultDeviceChanged -= NotificationClient_DefaultDeviceChanged;
            _enumerator.UnregisterEndpointNotificationCallback(_notifyClient);
            _playbackDevice.Dispose();
        }

        public override void Update(double deltaTime)
        {
            int newVolume = (int)(_playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            bool mute = _playbackDevice.AudioEndpointVolume.Mute;
            string deviceFriendlyName = _playbackDevice.DeviceFriendlyName;
            if (newVolume != DataModel.volume) DataModel.volumeChanged.Trigger();
            DataModel.volume = newVolume;
            DataModel.muted = mute;
            DataModel.DefaultDeviceName = deviceFriendlyName;
        }
    }
}