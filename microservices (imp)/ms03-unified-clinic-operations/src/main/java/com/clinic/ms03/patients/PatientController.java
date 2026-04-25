package com.clinic.ms03.patients;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/patients")
public class PatientController {

    @GetMapping("/status")
    public ResponseEntity<String> status() {
        // US-008..US-010 patient domain placeholder.
        return ResponseEntity.ok("Patient module scaffold");
    }
}
