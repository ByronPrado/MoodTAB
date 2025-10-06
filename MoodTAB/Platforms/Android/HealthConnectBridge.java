package com.example.healthbridge;

import android.content.Context;
import androidx.health.connect.client.HealthConnectClient;

public class HealthConnectBridge {
    public static HealthConnectClient getClient(Context context) {
        return HealthConnectClient.getOrCreate(context);
    }
}
