package com.clinic.ms10.notification;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/notifications")
public class NotificationController {
    private final NotificationService notificationService;

    public NotificationController(NotificationService notificationService) {
        this.notificationService = notificationService;
    }

    @PostMapping("/send")
    public ResponseEntity<String> send() {
        // US-034..US-037 placeholder: dispatch pipeline + retries + dedup.
        NotificationMessage message =
            new NotificationMessage("patient@example.com", "Scaffold", "Notification scaffold", "SCAFFOLD:1");
        return ResponseEntity.ok(notificationService.dispatch(message));
    }
}
