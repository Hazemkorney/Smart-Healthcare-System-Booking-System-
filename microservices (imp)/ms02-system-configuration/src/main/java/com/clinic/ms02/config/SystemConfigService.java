package com.clinic.ms02.config;

import java.util.List;

import org.springframework.stereotype.Service;

@Service
public class SystemConfigService {

    public List<SystemSetting> listCoreSettings() {
        // US-007 placeholder settings.
        return List.of(
            new SystemSetting("clinic.workingHours", "08:00-20:00"),
            new SystemSetting("appointment.consultationDurationMin", "20"),
            new SystemSetting("notification.reminderHoursBefore", "24")
        );
    }
}
