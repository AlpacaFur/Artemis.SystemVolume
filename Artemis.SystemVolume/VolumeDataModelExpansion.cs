using Artemis.Core.DataModelExpansions;
using Artemis.SystemVolume.DataModels;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace Artemis.SystemVolume
{
    public class VolumeDataModelExpansion : DataModelExpansion<VolumeDataModel>
    {
        private MMDevice _playbackDevice;
        private NaudioNotificationClient _notificationClient = new NaudioNotificationClient();
        private IMMNotificationClient _notifyClient;
        private MMDeviceEnumerator _enumerator = new MMDeviceEnumerator();
        private bool _playbackDeviceChanged = false;
        public override void Enable()
        {
            _notifyClient = (IMMNotificationClient)_notificationClient;
            _notificationClient.DefaultDeviceChanged += NotificationClient_DefaultDeviceChanged;

            _enumerator.RegisterEndpointNotificationCallback(_notifyClient);
            SetPlaybackDevice();
            UpdateDataModel();
        }

        private void UpdateDataModel()
        {
            int newVolume = (int)(_playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            DataModel.volumeChanged.Trigger();

            DataModel.volume = newVolume;
            DataModel.muted = _playbackDevice.AudioEndpointVolume.Mute;
            DataModel.DefaultDeviceName = _playbackDevice.DeviceFriendlyName;
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            UpdateDataModel();
        }

        private void NotificationClient_DefaultDeviceChanged()
        {
            _playbackDeviceChanged = true;
            // Workarround. NAudio MMDevice won't dispose if Dispose() is called from 
            // non parent thread and NaudioNotificationClient callbacks come from another thread.
        }

        private void UpdatePlaybackDevice()
        {
            FreePlaybackDevice();
            SetPlaybackDevice();
            _playbackDeviceChanged = false;
            UpdateDataModel();
        }

        public override void Disable()
        {
            _notificationClient.DefaultDeviceChanged -= NotificationClient_DefaultDeviceChanged;
            _enumerator.UnregisterEndpointNotificationCallback(_notifyClient);
            FreePlaybackDevice();
        }

        private void FreePlaybackDevice()
        {
            if (_playbackDevice != null)
            {
                _playbackDevice.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
                _playbackDevice.Dispose();
            }
        }

        private void SetPlaybackDevice()
        {
            _playbackDevice = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            _playbackDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        }

        public override void Update(double deltaTime)
        {
            if (_playbackDeviceChanged)
            {
                UpdatePlaybackDevice();
            }
        }
    }
}