package com.clinic.ms03.reporting;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/reports")
public class ReportingController {

    @GetMapping("/status")
    public ResponseEntity<String> status() {
        // US-038..US-041 reporting domain placeholder.
        return ResponseEntity.ok("Reporting module scaffold");
    }
}
