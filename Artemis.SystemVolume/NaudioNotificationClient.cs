﻿using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;

public class NaudioNotificationClient : IMMNotificationClient
{
    public delegate void DefaultDeviceChangedHandler();
    public event DefaultDeviceChangedHandler DefaultDeviceChanged;

    public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
    {
        if (DefaultDeviceChanged != null)
        {
            DefaultDeviceChanged();
        }
    }

    public void OnDeviceAdded(string deviceId)
    {
    }

    public void OnDeviceRemoved(string deviceId)
    {
    }

    public void OnDeviceStateChanged(string deviceId, DeviceState newState)
    {
    }

    public NaudioNotificationClient()
    {
        if (System.Environment.OSVersion.Version.Major < 6)
        {
            //throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
        }
    }

    public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
    {
    }

}