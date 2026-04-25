package com.clinic.ms10.notification;

public interface ChannelAdapter {
    String channel();
    void send(NotificationMessage message);
}
