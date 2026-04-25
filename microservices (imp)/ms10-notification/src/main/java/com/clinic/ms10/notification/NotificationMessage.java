package com.clinic.ms10.notification;

public record NotificationMessage(String recipient, String subject, String body, String dedupKey) {
}
