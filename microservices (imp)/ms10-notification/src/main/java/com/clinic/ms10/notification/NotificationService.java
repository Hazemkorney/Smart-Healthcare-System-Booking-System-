package com.clinic.ms10.notification;

import org.springframework.stereotype.Service;

@Service
public class NotificationService {

    public String dispatch(NotificationMessage message) {
        // Placeholder for retry/backoff/idempotency pipeline.
        return "queued:" + message.dedupKey();
    }
}
