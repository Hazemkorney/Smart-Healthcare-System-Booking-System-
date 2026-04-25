package com.clinic.ms03.consultation;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/consultations")
public class ConsultationController {

    @GetMapping("/status")
    public ResponseEntity<String> status() {
        // US-020..US-026 consultation domain placeholder.
        return ResponseEntity.ok("Consultation module scaffold");
    }
}
