package com.clinic.ms03.queue;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/queue")
public class QueueController {

    @GetMapping("/status")
    public ResponseEntity<String> status() {
        // US-016..US-019 queue domain placeholder.
        return ResponseEntity.ok("Queue module scaffold");
    }
}
