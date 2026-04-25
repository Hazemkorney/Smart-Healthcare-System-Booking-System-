package com.clinic.ms03.records;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/medical-records")
public class MedicalRecordController {

    @GetMapping("/status")
    public ResponseEntity<String> status() {
        // US-027..US-028 records domain placeholder.
        return ResponseEntity.ok("Medical records module scaffold");
    }
}
